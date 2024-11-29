using Foundation;
using LocalAuthentication;
using ObjCRuntime;
using Maui.Biometric.Abstractions;

// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

internal sealed class IosBiometricAuthentication : NotImplementedBiometricAuthentication, IDisposable
{
    private LAContext _context = new();

    protected override async Task<AuthenticationResult> NativeAuthenticateAsync(
        AuthenticationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (_context.RespondsToSelector(new Selector("localizedFallbackTitle")))
        {
            _context.LocalizedFallbackTitle = request.FallbackTitle;
        }
        if (_context.RespondsToSelector(new Selector("localizedCancelTitle")))
        {
            _context.LocalizedCancelTitle = request.CancelTitle;
        }
        
        await using var registration = cancellationToken.Register(CancelAuthentication).ConfigureAwait(true);
        var policy = GetPolicy(request.Strength);
        var (isSucceeded, error) = await _context.EvaluatePolicyAsync(policy, request.Reason).ConfigureAwait(true);

        var result = new AuthenticationResult();
        if (isSucceeded)
        {
            result.Status = AuthenticationResultStatus.Succeeded;
        }
        else
        {
            // #79 simulators return null for any reason
            if (error == null!)
            {
                result.Status = AuthenticationResultStatus.UnknownError;
                result.ErrorMessage = "";
            }
            else
            {
                result = GetResultFromError(error);
            }
        }

        CreateNewContext();
        return result;
    }

    public override Task<Availability> GetAvailabilityAsync(
        AuthenticationStrength strength = AuthenticationStrength.Weak)
    {
        var policy = GetPolicy(strength);
        if (_context.CanEvaluatePolicy(policy, out var error))
            return Task.FromResult(Availability.Available);

        return Task.FromResult((LAStatus)(int)error.Code switch
        {
            LAStatus.BiometryNotAvailable => IsDeniedError(error)
                ? Availability.Denied
                : Availability.NoSensor,
            LAStatus.BiometryNotEnrolled => Availability.NoBiometric,
            LAStatus.PasscodeNotSet => Availability.NoFallback,
            _ => Availability.Unknown,
        });
    }

    public override async Task<AuthenticationType> GetAuthenticationTypeAsync()
    {
        // we need to call this, because it will always return none, if you don't call CanEvaluatePolicy
        var availability = await GetAvailabilityAsync(
            strength: AuthenticationStrength.Weak).ConfigureAwait(false);

        // iOS 11+
        if (_context.RespondsToSelector(new Selector("biometryType")))
        {
            return _context.BiometryType switch
            {
                LABiometryType.None => AuthenticationType.None,
                LABiometryType.TouchId => AuthenticationType.Fingerprint,
                LABiometryType.FaceId => AuthenticationType.Face,
                _ => AuthenticationType.None,
            };
        }

        // iOS < 11
        if (availability is Availability.NoSensor or 
                            Availability.Unknown)
        {
            return AuthenticationType.None;
        }

        return AuthenticationType.Fingerprint;
    }

    private static LAPolicy GetPolicy(AuthenticationStrength strength)
    {
        return strength switch
        {
            AuthenticationStrength.Any => LAPolicy.DeviceOwnerAuthentication,
            _ => LAPolicy.DeviceOwnerAuthenticationWithBiometrics,
        };
    }

    private static AuthenticationResult GetResultFromError(NSError error)
    {
        var result = new AuthenticationResult();

        switch ((LAStatus)(int)error.Code)
        {
            case LAStatus.AuthenticationFailed:
                result.Status = error.Description.Contains("retry limit exceeded", StringComparison.OrdinalIgnoreCase)
                    ? AuthenticationResultStatus.TooManyAttempts
                    : AuthenticationResultStatus.Failed;
                break;

            case LAStatus.UserCancel:
            case LAStatus.AppCancel:
                result.Status = AuthenticationResultStatus.Canceled;
                break;

            case LAStatus.UserFallback:
                result.Status = AuthenticationResultStatus.FallbackRequested;
                break;

            case LAStatus.BiometryLockout: //TouchIDLockout
                result.Status = AuthenticationResultStatus.TooManyAttempts;
                break;

            case LAStatus.BiometryNotAvailable:
                // this can happen if it was available, but the user didn't allow face ID
                result.Status = IsDeniedError(error) ? 
                    AuthenticationResultStatus.Denied : 
                    AuthenticationResultStatus.NotAvailable;
                break;

            default:
                result.Status = AuthenticationResultStatus.UnknownError;
                break;
        }

        result.ErrorMessage = error.LocalizedDescription;

        return result;
    }

    private void CancelAuthentication()
    {
        CreateNewContext();
    }

    private void CreateNewContext()
    {
        if (_context.RespondsToSelector(new Selector("invalidate")))
        {
            _context.Invalidate();
        }
        _context.Dispose();
        _context = new LAContext();
    }

    private static bool IsDeniedError(NSError error)
    {
        if (string.IsNullOrEmpty(error.Description))
        {
            return false;
        }
        
        return error.Description.ToUpperInvariant().Contains("DENIED", StringComparison.OrdinalIgnoreCase);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
using System.Globalization;
using Foundation;
using LocalAuthentication;
using ObjCRuntime;
using Maui.Biometric.Abstractions;
using UIKit;

// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

internal sealed class IosBiometricAuthentication : NotImplementedBiometricAuthentication, IDisposable
{
    private LAContext? _context;

    public IosBiometricAuthentication()
    {
        CreateLaContext();
    }

    protected override async Task<AuthenticationResult> NativeAuthenticateAsync(
        AuthenticationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (_context == null)
        {
            return new AuthenticationResult
            {
                Status = AuthenticationResultStatus.NotAvailable,
                ErrorMessage = "Biometric authentication is not available on this device."
            };
        }
        
        var result = new AuthenticationResult();
        SetupContextProperties(request);

        Tuple<bool, NSError> resTuple;
        await using var registration = cancellationToken.Register(CancelAuthentication).ConfigureAwait(true);
        var policy = GetPolicy(request.Strength);
        resTuple = await _context.EvaluatePolicyAsync(policy, request.Reason).ConfigureAwait(true);

        if (resTuple.Item1)
        {
            result.Status = AuthenticationResultStatus.Succeeded;
        }
        else
        {
            // #79 simulators return null for any reason
            if (resTuple.Item2 == null!)
            {
                result.Status = AuthenticationResultStatus.UnknownError;
                result.ErrorMessage = "";
            }
            else
            {
                result = GetResultFromError(resTuple.Item2);
            }
        }

        CreateNewContext();
        return result;
    }

    public override Task<Availability> GetAvailabilityAsync(
        AuthenticationStrength strength = AuthenticationStrength.Weak)
    {
        if (_context == null)
            return Task.FromResult(Availability.NoApi);

        var policy = GetPolicy(strength);
        if (_context.CanEvaluatePolicy(policy, out var error))
            return Task.FromResult(Availability.Available);

        switch ((LAStatus)(int)error.Code)
        {
            case LAStatus.BiometryNotAvailable:
                return Task.FromResult(IsDeniedError(error) ? 
                    Availability.Denied :
                    Availability.NoSensor);
            case LAStatus.BiometryNotEnrolled:
                return Task.FromResult(Availability.NoBiometric);
            case LAStatus.PasscodeNotSet:
                return Task.FromResult(Availability.NoFallback);
            default:
                return Task.FromResult(Availability.Unknown);
        }
    }

    public override async Task<AuthenticationType> GetAuthenticationTypeAsync()
    {
        if (_context == null)
        {
            return AuthenticationType.None;
        }

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
        if (availability is Availability.NoApi or 
                            Availability.NoSensor or 
                            Availability.Unknown)
        {
            return AuthenticationType.None;
        }

        return AuthenticationType.Fingerprint;
    }

    private void SetupContextProperties(AuthenticationRequest configuration)
    {
        if (_context == null)
        {
            return;
        }
        
        if (_context.RespondsToSelector(new Selector("localizedFallbackTitle")))
        {
            _context.LocalizedFallbackTitle = configuration.FallbackTitle;
        }

        if (_context.RespondsToSelector(new Selector("localizedCancelTitle")))
        {
            _context.LocalizedCancelTitle = configuration.CancelTitle;
        }
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
        if (_context != null)
        {
            if (_context.RespondsToSelector(new Selector("invalidate")))
            {
                _context.Invalidate();
            }
            _context.Dispose();
        }

        CreateLaContext();
    }

    private void CreateLaContext()
    {
#if MACOS
        var info = new NSProcessInfo();
        var minVersion = new NSOperatingSystemVersion(10, 12, 0);
        if (!info.IsOperatingSystemAtLeastVersion(minVersion))
            return;
#else
        if (!UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            return;
#endif
        // Check LAContext is not available on iOS7 and below, so check LAContext after checking iOS version.
        if (Class.GetHandle(typeof(LAContext)) == IntPtr.Zero)
            return;

        _context = new LAContext();
    }

    private static bool IsDeniedError(NSError error)
    {
        if (!string.IsNullOrEmpty(error.Description))
        {
            // we might have some issues, if the error gets localized :/
#pragma warning disable CA1308
            return error.Description.ToLower(CultureInfo.InvariantCulture).Contains("denied", StringComparison.OrdinalIgnoreCase);
#pragma warning restore CA1308
        }

        return false;
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
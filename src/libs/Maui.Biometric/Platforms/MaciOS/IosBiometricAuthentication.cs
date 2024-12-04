using LocalAuthentication;

// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

/// <summary>
/// According to the Apple documentation:
/// https://developer.apple.com/documentation/LocalAuthentication/logging-a-user-into-your-app-with-face-id-or-touch-id
/// </summary>
internal sealed class IosBiometricAuthentication : IBiometricAuthentication
{
    public async Task<AuthenticationResult> AuthenticateAsync(
        AuthenticationRequest request,
        CancellationToken cancellationToken = default)
    {
        var context = new LAContext();
        var policy = request.Authenticators.MapToLaPolicy();
        if (!context.CanEvaluatePolicy(policy, out var canEvaluateError))
        {
            return new AuthenticationResult
            {
                // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                // # simulators return null for any reason
                Status = canEvaluateError?.ToStatus() ?? AuthenticationStatus.UnknownError,
                ErrorMessage = canEvaluateError?.LocalizedDescription ?? string.Empty,
            };
        }
        
        if (!string.IsNullOrEmpty(request.FallbackTitle))
        {
            context.LocalizedFallbackTitle = request.FallbackTitle;
        }
        if (!string.IsNullOrEmpty(request.CancelTitle))
        {
            context.LocalizedCancelTitle = request.CancelTitle;
        }

        await using var registration = cancellationToken.Register(() => context.Invalidate()).ConfigureAwait(true);
        
        // DeviceOwnerAuthentication(Strength: Any)
        // https://developer.apple.com/documentation/localauthentication/lapolicy/deviceownerauthentication
        //
        // DeviceOwnerAuthenticationWithBiometrics(Strength: Weak and Strong):
        // https://developer.apple.com/documentation/localauthentication/lapolicy/deviceownerauthenticationwithbiometrics
        var (isSucceeded, error) = await context.EvaluatePolicyAsync(
            policy: policy,
            localizedReason: request.Reason).ConfigureAwait(true);
        var result = new AuthenticationResult
        {
            Status = isSucceeded
                ? AuthenticationStatus.Success
                // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                // # simulators return null for any reason
                : error?.ToStatus() ?? AuthenticationStatus.UnknownError,
            ErrorMessage = error?.LocalizedDescription ?? string.Empty,
        };

        return result;
    }

    public Task<AuthenticationAvailability> GetAvailabilityAsync(
        Authenticator authenticators = AuthenticationRequest.DefaultAuthenticators,
        CancellationToken cancellationToken = default)
    {
        using var context = new LAContext();
        
        return Task.FromResult(
            context.CanEvaluatePolicy(authenticators.MapToLaPolicy(), out var error)
                ? AuthenticationAvailability.Available
                : error.ToAvailability());
    }

    public Task<AuthenticationType> GetAuthenticationTypeAsync(
        Authenticator authenticators = AuthenticationRequest.DefaultAuthenticators,
        CancellationToken cancellationToken = default)
    {
        using var context = new LAContext();
        
        // BiometryType: This property is set only after you call the canEvaluatePolicy(_:error:) method,
        // and is set no matter what the call returns.
        // https://developer.apple.com/documentation/localauthentication/lacontext/biometrytype
        _ = context.CanEvaluatePolicy(authenticators.MapToLaPolicy(), out _);
        
        // https://support.apple.com/en-ae/118483
        if (context.BiometryType == LABiometryType.OpticId)
        {
            return Task.FromResult(AuthenticationType.Iris);
        }

        return Task.FromResult(context.BiometryType switch
        {
            LABiometryType.None => AuthenticationType.None,
            LABiometryType.TouchId => AuthenticationType.Fingerprint,
            LABiometryType.FaceId => AuthenticationType.Face,
            _ => AuthenticationType.None,
        });
    }
}
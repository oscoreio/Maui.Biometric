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
        
        // Only set the fallback title if the device credential is enabled.
        if (request.Authenticators.HasFlag(Authenticator.DeviceCredential) &&
            !string.IsNullOrEmpty(request.FallbackTitle))
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

    public Task<AvailabilityResult> CheckAvailabilityAsync(
        Authenticator authenticators = AuthenticationRequest.DefaultAuthenticators,
        CancellationToken cancellationToken = default)
    {
        using var context = new LAContext();
        
        // BiometryType: This property is set only after you call the canEvaluatePolicy(_:error:) method,
        // and is set no matter what the call returns.
        // https://developer.apple.com/documentation/localauthentication/lacontext/biometrytype
        var availability = context.CanEvaluatePolicy(authenticators.MapToLaPolicy(), out var error)
            ? AuthenticationAvailability.Available
            : error.ToAvailability();
        
        return Task.FromResult(new AvailabilityResult
        {
            Availability = availability,
            Sensors = [context.BiometryType switch
            {
                LABiometryType.FaceId => BiometricSensor.Face,
                LABiometryType.OpticId => BiometricSensor.None,
                _ => BiometricSensor.None,
            }],
        });
    }
}

using Windows.Security.Credentials.UI;

// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

internal sealed class WindowsBiometricAuthentication : IBiometricAuthentication
{
    public async Task<AuthenticationResult> AuthenticateAsync(
        AuthenticationRequest configuration,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var operation = UserConsentVerifier.RequestVerificationAsync(message: configuration.Reason);
            await using var registration = cancellationToken.Register(() => operation.Cancel()).ConfigureAwait(true);
            
            var verificationResult = await operation;

            return new AuthenticationResult
            {
                Status = verificationResult switch
                {
                    UserConsentVerificationResult.Verified => AuthenticationStatus.Success,
                    UserConsentVerificationResult.DeviceBusy or UserConsentVerificationResult.DeviceNotPresent
                        or UserConsentVerificationResult.DisabledByPolicy
                        or UserConsentVerificationResult.NotConfiguredForUser => AuthenticationStatus.NotAvailable,
                    UserConsentVerificationResult.RetriesExhausted => AuthenticationStatus.TooManyAttempts,
                    UserConsentVerificationResult.Canceled => AuthenticationStatus.Canceled,
                    _ => AuthenticationStatus.Failed
                },
                ErrorMessage = verificationResult switch
                {
                    UserConsentVerificationResult.DeviceBusy => "The biometric verifier device is performing an operation and is unavailable.",
                    UserConsentVerificationResult.DeviceNotPresent => "There is no biometric verifier device available.",
                    UserConsentVerificationResult.DisabledByPolicy => "Group policy has disabled the biometric verifier device.",
                    UserConsentVerificationResult.NotConfiguredForUser => "A biometric verifier device is not configured for this user.",
                    UserConsentVerificationResult.RetriesExhausted => "After 10 attempts, the original verification request and all subsequent attempts at the same verification were not verified.",
                    UserConsentVerificationResult.Canceled => "The verification operation was canceled.",
                    _ => string.Empty,
                }
            };
        }
        catch (Exception ex)
        {
            return new AuthenticationResult
            {
                Status = AuthenticationStatus.UnknownError,
                ErrorMessage = ex.Message,
            };
        }
    }

    public async Task<AuthenticationAvailability> GetAvailabilityAsync(
        Authenticator authenticators = AuthenticationRequest.DefaultAuthenticators,
        CancellationToken cancellationToken = default)
    {
        var operation = UserConsentVerifier.CheckAvailabilityAsync();
        await using var registration = cancellationToken.Register(() => operation.Cancel()).ConfigureAwait(true);
        
        var availability = await operation;
        
        return availability switch
        {
            UserConsentVerifierAvailability.Available => AuthenticationAvailability.Available,
            UserConsentVerifierAvailability.DeviceNotPresent => AuthenticationAvailability.NoSensor,
            UserConsentVerifierAvailability.NotConfiguredForUser => AuthenticationAvailability.NoBiometric,
            UserConsentVerifierAvailability.DisabledByPolicy => AuthenticationAvailability.NoPermission,
            UserConsentVerifierAvailability.DeviceBusy => AuthenticationAvailability.TemporaryUnavailable,
            _ => AuthenticationAvailability.Unknown,
        };
    }

    public async Task<AuthenticationType> GetAuthenticationTypeAsync(
        Authenticator authenticators = AuthenticationRequest.DefaultAuthenticators,
        CancellationToken cancellationToken = default)
    {
        var availability = await GetAvailabilityAsync(authenticators, cancellationToken).ConfigureAwait(true);
        if (availability is AuthenticationAvailability.NoBiometric or
                            AuthenticationAvailability.NoPermission or
                            AuthenticationAvailability.Available)
        {
            return AuthenticationType.Fingerprint;
        }

        return AuthenticationType.None;
    }
}
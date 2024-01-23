using Windows.Security.Credentials.UI;
using Maui.Biometric.Abstractions;

namespace Maui.Biometric;

internal sealed class WindowsBiometricAuthentication : NotImplementedBiometricAuthentication
{
    protected override async Task<AuthenticationResult> NativeAuthenticateAsync(
        AuthenticationRequest configuration,
        CancellationToken cancellationToken = default)
    {
        var result = new AuthenticationResult();

        try
        {
            var verificationResult = await UserConsentVerifier.RequestVerificationAsync(configuration.Reason);

            switch (verificationResult)
            {
                case UserConsentVerificationResult.Verified:
                    result.Status = AuthenticationResultStatus.Succeeded;
                    break;

                case UserConsentVerificationResult.DeviceBusy:
                case UserConsentVerificationResult.DeviceNotPresent:
                case UserConsentVerificationResult.DisabledByPolicy:
                case UserConsentVerificationResult.NotConfiguredForUser:
                    result.Status = AuthenticationResultStatus.NotAvailable;
                    break;
                    
                case UserConsentVerificationResult.RetriesExhausted:
                    result.Status = AuthenticationResultStatus.TooManyAttempts;
                    break;
                case UserConsentVerificationResult.Canceled:
                    result.Status = AuthenticationResultStatus.Canceled;
                    break;
                default:
                    result.Status = AuthenticationResultStatus.Failed;
                    break;
            }
        }
        catch (Exception ex)
        {
            result.Status = AuthenticationResultStatus.UnknownError;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    public override async Task<Availability> GetAvailabilityAsync(
        AuthenticationStrength strength = AuthenticationStrength.Weak)
    {
        var availability = await UserConsentVerifier.CheckAvailabilityAsync();
        
        return availability switch
        {
            UserConsentVerifierAvailability.Available => Availability.Available,
            UserConsentVerifierAvailability.DeviceNotPresent => Availability.NoSensor,
            UserConsentVerifierAvailability.NotConfiguredForUser => Availability.NoBiometric,
            UserConsentVerifierAvailability.DisabledByPolicy => Availability.NoPermission,
            UserConsentVerifierAvailability.DeviceBusy => Availability.Unknown,
            _ => Availability.Unknown,
        };
    }

    public override async Task<AuthenticationType> GetAuthenticationTypeAsync()
    {
        var availability = await GetAvailabilityAsync(
            strength: AuthenticationStrength.Weak).ConfigureAwait(false);
        if (availability is Availability.NoBiometric or
                            Availability.NoPermission or
                            Availability.Available)
        {
            return AuthenticationType.Fingerprint;
        }

        return AuthenticationType.None;
    }
}
// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

internal sealed class NotImplementedBiometricAuthentication : IBiometricAuthentication
{
    public Task<AvailabilityResult> CheckAvailabilityAsync(
        Authenticator authenticators = Authenticator.Biometric | Authenticator.DeviceCredential,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new AvailabilityResult
        {
            Availability = AuthenticationAvailability.NoImplementation,
            Sensors = [],
        });
    }
    
    public async Task<AuthenticationResult> AuthenticateAsync(
        string title,
        string reason,
        Authenticator authenticators = Authenticator.Biometric | Authenticator.DeviceCredential,
        string cancelTitle = "",
        string fallbackTitle = "",
        bool confirmationRequired = true,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new AuthenticationResult
        {
            Status = AuthenticationStatus.NotAvailable,
            ErrorMessage = "Not implemented for the current platform.",
        });
    }
}

// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

internal class NotImplementedBiometricAuthentication : IBiometricAuthentication
{
    public Task<AvailabilityResult> CheckAvailabilityAsync(
        Authenticator authenticators = AuthenticationRequest.DefaultAuthenticators,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new AvailabilityResult
        {
            Availability = AuthenticationAvailability.NoImplementation,
            Sensors = [],
        });
    }
    
    public virtual Task<AuthenticationResult> AuthenticateAsync(
        AuthenticationRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new AuthenticationResult
        {
            Status = AuthenticationStatus.NotAvailable,
            ErrorMessage = "Not implemented for the current platform.",
        });
    }
}
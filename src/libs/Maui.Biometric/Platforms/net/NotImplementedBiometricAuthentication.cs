// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

/// <inheritdoc />
public class NotImplementedBiometricAuthentication : IBiometricAuthentication
{
    /// <inheritdoc />
    public Task<AuthenticationAvailability> GetAvailabilityAsync(
        Authenticator authenticators = AuthenticationRequest.DefaultAuthenticators,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(AuthenticationAvailability.NoImplementation);
    }

    /// <inheritdoc />
    public Task<AuthenticationType> GetAuthenticationTypeAsync(
        Authenticator authenticators = AuthenticationRequest.DefaultAuthenticators,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(AuthenticationType.None);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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
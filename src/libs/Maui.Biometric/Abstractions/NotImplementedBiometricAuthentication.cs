// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

/// <inheritdoc />
public class NotImplementedBiometricAuthentication : IBiometricAuthentication
{
    /// <inheritdoc />
    public async Task<AuthenticationResult> AuthenticateAsync(
        AuthenticationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var availability = await GetAvailabilityAsync(request.Authenticators).ConfigureAwait(false);
        if (availability != AuthenticationAvailability.Available)
        {
            var status = availability == AuthenticationAvailability.Denied ?
                AuthenticationStatus.Denied :
                AuthenticationStatus.NotAvailable;

            return new AuthenticationResult { Status = status, ErrorMessage = availability.ToString() };
        }

        return await NativeAuthenticateAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> IsAvailableAsync(
        Authenticator authenticators = AuthenticationRequest.DefaultAuthenticators)
    {
        var availability = await GetAvailabilityAsync(authenticators).ConfigureAwait(false);
        
        return availability == AuthenticationAvailability.Available;
    }

    /// <inheritdoc />
    public virtual Task<AuthenticationAvailability> GetAvailabilityAsync(
        Authenticator authenticators = AuthenticationRequest.DefaultAuthenticators)
    {
        return Task.FromResult(AuthenticationAvailability.NoImplementation);
    }

    /// <inheritdoc />
    public virtual Task<AuthenticationType> GetAuthenticationTypeAsync(
        Authenticator authenticators = AuthenticationRequest.DefaultAuthenticators)
    {
        return Task.FromResult(AuthenticationType.None);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual Task<AuthenticationResult> NativeAuthenticateAsync(
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
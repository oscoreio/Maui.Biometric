// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

/// <summary>
/// Biometric authentication availability.
/// </summary>
public static class BiometricAuthenticationExtensions
{
    /// <summary>
    /// Checks if <see cref="IBiometricAuthentication.GetAvailabilityAsync"/> returns <see cref="AuthenticationAvailability.Available"/>.
    /// </summary>
    /// <param name="biometricAuthentication"></param>
    /// <param name="authenticators"></param>
    /// <param name="cancellationToken"></param>
    /// <returns><c>true</c> if Available, else <c>false</c></returns>
    public static async Task<bool> IsAvailableAsync(
        this IBiometricAuthentication biometricAuthentication,
        Authenticator authenticators = AuthenticationRequest.DefaultAuthenticators,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(biometricAuthentication);
        
        var availability = await biometricAuthentication.GetAvailabilityAsync(authenticators, cancellationToken).ConfigureAwait(false);
        
        return availability == AuthenticationAvailability.Available;
    }

    /// <summary>
    /// Checks availability and requests the authentication.
    /// </summary>
    /// <param name="biometricAuthentication"></param>
    /// <param name="request">Configuration of the dialog that is displayed to the user.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>Authentication result</returns>
    public static async Task<AuthenticationResult> TryAuthenticateAsync(
        this IBiometricAuthentication biometricAuthentication,
        AuthenticationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(biometricAuthentication);
        ArgumentNullException.ThrowIfNull(request);

        var availability = await biometricAuthentication.GetAvailabilityAsync(request.Authenticators, cancellationToken).ConfigureAwait(false);
        if (availability != AuthenticationAvailability.Available)
        {
            var status = availability == AuthenticationAvailability.Denied ?
                AuthenticationStatus.Denied :
                AuthenticationStatus.NotAvailable;

            return new AuthenticationResult
            {
                Status = status,
                ErrorMessage = availability.ToString(),
            };
        }

        return await biometricAuthentication.AuthenticateAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
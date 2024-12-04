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
}
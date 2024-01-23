namespace Maui.Biometric.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IBiometricAuthentication
{
    /// <summary>
    /// Checks the availability of fingerprint authentication. <br/>
    /// Checks are performed in this order: <br/>
    /// 1. API supports accessing the fingerprint sensor <br/>
    /// 2. Permission for accessing the fingerprint sensor granted <br/>
    /// 3. Device has sensor <br/>
    /// 4. Fingerprint has been enrolled <br/>
    /// <see cref="Availability.Unknown"/> will be returned if the check failed 
    /// with some other platform specific reason.
    /// </summary>
    /// <param name="strength">
    /// En-/Disables the use of the PIN / Password as fallback.
    /// Supported Platforms: iOS, Mac
    /// Default: Weak
    /// </param>
    Task<Availability> GetAvailabilityAsync(
        AuthenticationStrength strength = AuthenticationStrength.Weak);

    /// <summary>
    /// Checks if <see cref="GetAvailabilityAsync"/> returns <see cref="Availability.Available"/>.
    /// </summary>
    /// <param name="strength">
    /// En-/Disables the use of the PIN / Password as fallback.
    /// Supported Platforms: iOS, Mac
    /// Default: falWeakse
    /// </param>
    /// <returns><c>true</c> if Available, else <c>false</c></returns>
    Task<bool> IsAvailableAsync(
        AuthenticationStrength strength = AuthenticationStrength.Weak);

    /// <summary>
    /// Requests the authentication.
    /// </summary>
    /// <param name="request">Configuration of the dialog that is displayed to the user.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>Authentication result</returns>
    Task<AuthenticationResult> AuthenticateAsync(
        AuthenticationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the available authentication type.
    /// </summary>
    /// <returns>Authentication type</returns>
    Task<AuthenticationType> GetAuthenticationTypeAsync();
}
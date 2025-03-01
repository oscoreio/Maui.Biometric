// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

/// <summary>
/// 
/// </summary>
public interface IBiometricAuthentication
{
    /// <summary>
    /// Checks the availability of biometric authentication. <br/>
    /// Checks are performed in this order: <br/>
    /// 1. Operating System API supports biometric authentication <br/>
    /// 2. Required permissions granted <br/>
    /// 3. Device has biometric sensor <br/>
    /// 4. Biometric enrollment exists <br/>
    /// <see cref="AuthenticationAvailability.Unknown"/> will be returned if the check failed 
    /// with some other platform specific reason.
    /// </summary>
    /// <param name="authenticators"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Authentication availability</returns>
    Task<AvailabilityResult> CheckAvailabilityAsync(
        Authenticator authenticators = Authenticator.Biometric | Authenticator.DeviceCredential,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks availability and requests the authentication if available.
    /// </summary>
    /// <param name="request">Configuration of the dialog that is displayed to the user.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>Authentication result</returns>
    Task<AuthenticationResult> AuthenticateAsync(
        string title,
        string reason,
        Authenticator authenticators = Authenticator.Biometric | Authenticator.DeviceCredential,
        string cancelTitle = "",
        string fallbackTitle = "",
        bool confirmationRequired = true,
        CancellationToken cancellationToken = default);
}

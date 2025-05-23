﻿// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

/// <summary>
/// 
/// </summary>
public interface IBiometricAuthentication
{
    /// <summary>
    /// Checks the availability of biometric authentication. <br/>
    /// Checks are performed in this order: <br/>
    /// 1. Operating System API supports accessing the fingerprint sensor <br/>
    /// 2. Permission for accessing the fingerprint sensor granted <br/>
    /// 3. Device has sensor <br/>
    /// 4. Fingerprint has been enrolled <br/>
    /// <see cref="AuthenticationAvailability.Unknown"/> will be returned if the check failed 
    /// with some other platform specific reason.
    /// </summary>
    /// <param name="authenticators"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Authentication availability</returns>
    Task<AvailabilityResult> CheckAvailabilityAsync(
        Authenticator authenticators = AuthenticationRequest.DefaultAuthenticators,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks availability and requests the authentication if available.
    /// </summary>
    /// <param name="request">Configuration of the dialog that is displayed to the user.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>Authentication result</returns>
    Task<AuthenticationResult> AuthenticateAsync(
        AuthenticationRequest request,
        CancellationToken cancellationToken = default);
}
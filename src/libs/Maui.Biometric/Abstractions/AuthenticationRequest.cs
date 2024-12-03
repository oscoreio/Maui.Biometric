// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

/// <summary>
/// Represents a request for authentication.
/// </summary>
/// <param name="title"></param>
/// <param name="reason"></param>
public class AuthenticationRequest(
    string title,
    string reason)
{
    /// <summary>
    /// Default authenticators that are used during authentication.
    /// </summary>
    public const Authenticator DefaultAuthenticators = Authenticator.Biometric | Authenticator.DeviceCredential;
    
    /// <summary>
    /// Title of the authentication request.
    /// </summary>
    public string Title { get; } = title;

    /// <summary>
    /// Reason for the authentication request.
    /// </summary>
    public string Reason { get; } = reason;

    /// <summary>
    /// Title of the cancel button.
    /// </summary>
    public string CancelTitle { get; set; } = string.Empty;

    /// <summary>
    /// Title of the fallback button.
    /// </summary>
    public string FallbackTitle { get; set; } = string.Empty;

    /// <summary>
    /// If set only allows certain authenticators to be used during authentication. <br/>
    /// Can be set to <see cref="Authenticator.BiometricStrong"/>  to use only fingerprint,
    /// if the face unlocking is configured to be <see cref="Authenticator.Biometric"/>,
    /// but this really depends on the phone manufacturers.
    /// </summary>
    public Authenticator Authenticators { get; set; } = DefaultAuthenticators;

    /// <summary>
    /// Sets a hint to the system for whether to require user confirmation after authentication. <br/>
    /// For example, implicit modalities like face and iris are passive,
    /// meaning they don't require an explicit user action to complete authentication. <br/>
    /// If set to true, these modalities should require the user
    /// to take some action (e.g. press a button) before AuthenticateAsync() returns. <br/>
    /// <br/>
    /// Supported Platforms: Android <br/>
    /// Default: true <br/>
    /// </summary>
    public bool ConfirmationRequired { get; set; } = true;
}
// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

/// <summary>
/// Represents a request for authentication.
/// </summary>
public class AuthenticationRequest
{
    /// <summary>
    /// Default authenticators that are used during authentication.
    /// </summary>
    public const Authenticator DefaultAuthenticators = Authenticator.Biometric | Authenticator.DeviceCredential;
    
    /// <summary>
    /// Title of the authentication request.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Reason for the authentication request.
    /// </summary>
    public string Reason { get; }

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

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationRequest"/> class.
    /// </summary>
    /// <param name="title">The title of the authentication request.</param>
    /// <param name="reason">The reason for the authentication request.</param>
    public AuthenticationRequest(string title, string reason)
    {
        Title = title;
        Reason = reason;
    }
}

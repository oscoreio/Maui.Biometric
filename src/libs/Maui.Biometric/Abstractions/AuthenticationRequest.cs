namespace Maui.Biometric.Abstractions;

/// <summary>
/// Configuration of the stuff presented to the user.
/// </summary>
/// <param name="title"></param>
/// <param name="reason"></param>
public class AuthenticationRequest(
    string title,
    string reason)
{
    /// <summary>
    /// Title of the authentication request.
    /// </summary>
    public string Title { get; } = title;

    /// <summary>
    /// Reason of the authentication request.
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
    /// Shown when a recoverable error has been encountered during authentication. 
    /// The help strings are provided to give the user guidance for what went wrong.
    /// If a string is null or empty, the string provided by Android is shown.
    /// 
    /// Supported Platforms: Android
    /// </summary>
    public AuthenticationHelpTexts HelpTexts { get; } = new();

    /// <summary>
    /// If set only allows certain authenticators to be used during authentication.
    /// Can be set to <see cref="AuthenticationStrength.Strong"/>  to use only fingerprint,
    /// if the face unlocking is configured to be <see cref="AuthenticationStrength.Weak"/>,
    /// but this really depends on the phone manufacturers.
    /// </summary>
    public AuthenticationStrength Strength { get; set; } = AuthenticationStrength.Weak;
    
    /// <summary>
    /// Sets a hint to the system for whether to require user confirmation after authentication.
    /// For example, implicit modalities like face and iris are passive, meaning they don't require an explicit user action to complete authentication.
    /// If set to true, these modalities should require the user to take some action (e.g. press a button) before AuthenticateAsync() returns.
    ///
    /// Supported Platforms: Android
    /// Default: true
    /// </summary>
    public bool ConfirmationRequired { get; set; } = true;
}
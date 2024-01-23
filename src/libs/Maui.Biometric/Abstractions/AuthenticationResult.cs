namespace Maui.Biometric.Abstractions;

/// <summary>
/// 
/// </summary>
public class AuthenticationResult
{
    /// <summary>
    /// Indicates whether the authentication was successful or not.
    /// </summary>
    public bool Authenticated => Status == AuthenticationResultStatus.Succeeded;

    /// <summary>
    /// Detailed information of the authentication.
    /// </summary>
    public AuthenticationResultStatus Status { get; set; }

    /// <summary>
    /// Reason for the unsuccessful authentication.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}
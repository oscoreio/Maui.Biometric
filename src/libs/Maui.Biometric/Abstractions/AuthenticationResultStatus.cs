namespace Maui.Biometric.Abstractions;

/// <summary>
/// Indicates the result status of an authentication.
/// </summary>
public enum AuthenticationResultStatus
{
    /// <summary>
    /// An unknown, platform specific error occurred. Authentication status could not be 
    /// mapped to a <see cref="AuthenticationResultStatus"/>.
    /// </summary>
    Unknown,
    
    /// <summary>
    /// Authentication was successful.
    /// </summary>
    Succeeded,
    
    /// <summary>
    /// The uer requested the fallback method.
    /// </summary>
    FallbackRequested,
    
    /// <summary>
    /// Authentication failed.
    /// </summary>
    Failed,
    
    /// <summary>
    /// Authentication was cancelled.
    /// </summary>
    Canceled,
    
    /// <summary>
    /// The user needed too many attempts to authenticate.
    /// </summary>
    TooManyAttempts,
    
    /// <summary>
    /// An unknown, platform specific error occurred. Authentication status could not be 
    /// mapped to a <see cref="AuthenticationResultStatus"/>.
    /// </summary>
    UnknownError,
    
    /// <summary>
    /// Authentication is not available.
    /// </summary>
    NotAvailable,
    
    /// <summary>
    /// User has denied the usage of the biometric authentication.
    /// </summary>
    Denied
}
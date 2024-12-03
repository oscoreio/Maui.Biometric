// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

/// <summary>
/// The status of the authentication.
/// </summary>
public enum AuthenticationStatus
{
    /// <summary>
    /// The authentication status is unknown.
    /// </summary>
    UnknownError,
    
    /// <summary>
    /// The authentication was successful.
    /// </summary>
    Success,
    
    /// <summary>
    /// The user requested to use a fallback method.
    /// </summary>
    FallbackRequested,
    
    /// <summary>
    /// The authentication failed.
    /// </summary>
    Failed,
    
    /// <summary>
    /// The authentication was canceled.
    /// </summary>
    Canceled,
    
    /// <summary>
    /// The user has tried too many times and is locked out.
    /// </summary>
    TooManyAttempts,
    
    /// <summary>
    /// The authentication is not available.
    /// </summary>
    NotAvailable,
    
    /// <summary>
    /// The user has denied the authentication.
    /// </summary>
    Denied,
}
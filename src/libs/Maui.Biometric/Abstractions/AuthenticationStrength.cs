namespace Maui.Biometric.Abstractions;

/// <summary>
/// Enumeration that optionally filters out authenticator methods to use.
/// You can filter by strong or weak authentication methods or can allow alternative methods by using Any.
/// </summary>
public enum AuthenticationStrength
{
    /// <summary>
    /// Any biometric (e.g. fingerprint, iris, or face) on the device
    /// that meets or exceeds the requirements for Class 3 (formerly Strong), as defined by the Android CDD.
    /// </summary>
    Strong,
    
    /// <summary>
    /// Any biometric (e.g. fingerprint, iris, or face) on the device
    /// that meets or exceeds the requirements for Class 2 (formerly Weak), as defined by the Android CDD.
    /// </summary>
    Weak,
    
    /// <summary>
    /// The non-biometric credential used to secure the device (i.e., PIN, pattern, or password).
    /// </summary>
    Any,
}
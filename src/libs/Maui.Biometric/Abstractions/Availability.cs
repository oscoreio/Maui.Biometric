namespace Maui.Biometric.Abstractions;

/// <summary>
/// Indicates if a biometric authentication can be performed.
/// </summary>
public enum Availability
{
    /// <summary>
    /// Biometric authentication can be used.
    /// </summary>
    Available,
    
    /// <summary>
    /// This plugin has no implementation for the current platform.
    /// </summary>
    NoImplementation,
    
    /// <summary>
    /// Operating system has no supported biometric API.
    /// </summary>
    NoApi,
    
    /// <summary>
    /// App is not allowed to access the biometric sensor.
    /// </summary>
    NoPermission,
    
    /// <summary>
    /// Device has no biometric sensor.
    /// </summary>
    NoSensor,
    
    /// <summary>
    /// Biometric has not been set up.
    /// </summary>
    NoBiometric,
    
    /// <summary>
    /// Fallback has not been set up.
    /// </summary>
    NoFallback,
    
    /// <summary>
    /// An unknown, platform specific error occurred. Availability status could not be 
    /// mapped to a <see cref="Availability"/>.
    /// </summary>
    Unknown,
    
    /// <summary>
    /// User has denied the usage of the biometric authentication.
    /// </summary>
    Denied,
}
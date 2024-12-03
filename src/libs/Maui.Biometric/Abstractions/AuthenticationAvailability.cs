// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

/// <summary>
/// Represents the availability of biometric authentication on the current platform.
/// </summary>
public enum AuthenticationAvailability
{
    /// <summary>
    /// The availability of biometric authentication is unknown.
    /// </summary>
    Unknown,

    /// <summary>
    /// The biometric authentication is available.
    /// </summary>
    Available,

    /// <summary>
    /// The biometric authentication is not implemented.
    /// </summary>
    NoImplementation,

    /// <summary>
    /// The biometric authentication is not supported.
    /// </summary>
    NotSupported,

    /// <summary>
    /// The biometric authentication is not permitted.
    /// </summary>
    NoPermission,

    /// <summary>
    /// The biometric authentication sensor is not available.
    /// </summary>
    NoSensor,

    /// <summary>
    /// The biometric authentication is not enrolled.
    /// </summary>
    NoBiometric,

    /// <summary>
    /// Fallback has not been set up.
    /// </summary>
    NoFallback,

    /// <summary>
    /// The biometric authentication is denied by the user.
    /// </summary>
    Denied,

    /// <summary>
    /// The biometric verifier device is performing an operation and is unavailable.
    /// </summary>
    TemporaryUnavailable,

    /// <summary>
    /// The companion device is not available.
    /// </summary>
    NoCompanionDeviceWasDiscovered,
}
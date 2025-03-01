// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

/// <summary>
/// The types of biometric authentication supported.
/// </summary>
public enum BiometricSensor
{
    /// <summary>
    /// None.
    /// </summary>
    None,

    /// <summary>
    /// Device supports fingerprint.
    /// </summary>
    Fingerprint,

    /// <summary>
    /// Device supports face detection.
    /// </summary>
    Face,

}

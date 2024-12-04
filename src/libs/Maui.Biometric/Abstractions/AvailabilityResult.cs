// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

/// <summary>
/// The result of the check for biometric authentication availability.
/// </summary>
public class AvailabilityResult
{
    /// <summary>
    /// Indicates if the authentication was successful.
    /// </summary>
    public bool IsAvailable => Availability == AuthenticationAvailability.Available;

    /// <summary>
    /// Detailed information of the authentication.
    /// </summary>
    public required AuthenticationAvailability Availability { get; init; }
    
    /// <summary>
    /// The biometric sensors available on the device.
    /// </summary>
    public required HashSet<BiometricSensor> Sensors { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static implicit operator bool(AvailabilityResult result)
    {
        return result?.ToBoolean() ?? false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool ToBoolean()
    {
        return IsAvailable;
    }
}
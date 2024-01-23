namespace Maui.Biometric.Abstractions;

/// <summary>
/// 
/// </summary>
public class AuthenticationHelpTexts
{
    /// <summary>
    /// The fingerprint image was incomplete due to quick motion.
    /// </summary>
    public string MovedTooFast { get; set; } = string.Empty;

    /// <summary>
    /// The fingerprint image was unreadable due to lack of motion.
    /// </summary>
    public string MovedTooSlow { get; set; } = string.Empty;

    /// <summary>
    /// Only a partial fingerprint image was detected.
    /// </summary>
    public string Partial { get; set; } = string.Empty;

    /// <summary>
    /// The fingerprint image was too noisy to process due to a detected condition.
    /// </summary>
    public string Insufficient { get; set; } = string.Empty;

    /// <summary>
    /// The fingerprint image was too noisy due to suspected or detected dirt on the sensor.
    /// </summary>
    public string Dirty { get; set; } = string.Empty;
}
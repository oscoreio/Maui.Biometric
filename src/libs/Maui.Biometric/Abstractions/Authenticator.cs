// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

/// <summary>
/// Types of authenticators. <br/>
/// Types may be combined via bitwise OR into a single integer representing multiple authenticators
/// (e.g. <see cref="Authenticator.DeviceCredential"/> | <see cref="Authenticator.Biometric"/>).
/// </summary>
[Flags]
public enum Authenticator
{
    /// <summary>
    /// No authenticator specified.
    /// </summary>
    None,
    
    /// <summary>
    /// The non-biometric credential used to secure the device (i.e., PIN, pattern, or password). <br/>
    /// <list type="table">
    /// <item>
    ///     <term>MacCatalyst</term>
    ///     <description>Uses deviceOwnerAuthentication.</description>
    /// </item>
    /// <item>
    ///     <term>iOS</term>
    ///     <description>Uses deviceOwnerAuthentication.</description>
    /// </item>
    /// <item>
    ///     <term>Android</term>
    ///     <description>Uses DEVICE_CREDENTIAL.</description>
    /// </item>
    /// <item>
    ///     <term>Windows</term>
    ///     <description>Does not allow specifying authenticator and always uses all available methods.</description>
    /// </item>
    /// </list>
    /// </summary>
    DeviceCredential = 1,
    
    /// <summary>
    /// Fingerprint-based authentication. <br/>
    /// <list type="table">
    /// <item>
    ///     <term>MacCatalyst</term>
    ///     <description>Uses deviceOwnerAuthentication or deviceOwnerAuthenticationWithBiometrics depending on other flags specified.</description>
    /// </item>
    /// <item>
    ///     <term>iOS</term>
    ///     <description>Uses deviceOwnerAuthentication or deviceOwnerAuthenticationWithBiometrics depending on other flags specified.</description>
    /// </item>
    /// <item>
    ///     <term>Android</term>
    ///     <description>Uses BIOMETRIC_WEAK | BIOMETRIC_STRONG.</description>
    /// </item>
    /// <item>
    ///     <term>Windows</term>
    ///     <description>Does not allow specifying authenticator and always uses all available methods.</description>
    /// </item>
    /// </list>
    /// </summary>
    Biometric = 2,
    
    /// <summary>
    /// Any biometric (e.g. fingerprint, iris, or face) on the device that meets or exceeds the requirements for Class 3 (formerly Strong),
    /// as defined by the Android CDD.
    /// <list type="table">
    /// <item>
    ///     <term>MacCatalyst</term>
    ///     <description>Behaves like a normal Biometric.</description>
    /// </item>
    /// <item>
    ///     <term>iOS</term>
    ///     <description>Behaves like a normal Biometric.</description>
    /// </item>
    /// <item>
    ///     <term>Android</term>
    ///     <description>Uses BIOMETRIC_STRONG.</description>
    /// </item>
    /// <item>
    ///     <term>Windows</term>
    ///     <description>Does not allow specifying authenticator and always uses all available methods.</description>
    /// </item>
    /// </list>
    /// </summary>
    BiometricStrong = 4,
    
    /// <summary>
    /// Companion authentication is required.
    /// If no nearby paired companion device can be found, <see cref="AuthenticationStatus.NotAvailable"/> is returned.
    /// Users should follow instructions on the companion device to authenticate. <br/>
    /// <list type="table">
    /// <item>
    ///     <term>MacCatalyst</term>
    ///     <description>Uses deviceOwnerAuthenticationWithBiometricsOrCompanion or deviceOwnerAuthenticationWithCompanion depending on other flags specified.</description>
    /// </item>
    /// <item>
    ///     <term>iOS</term>
    ///     <description>Uses deviceOwnerAuthenticationWithBiometricsOrCompanion or deviceOwnerAuthenticationWithCompanion depending on other flags specified.</description>
    /// </item>
    /// <item>
    ///     <term>Android</term>
    ///     <description>There is no analogue.</description>
    /// </item>
    /// <item>
    ///     <term>Windows</term>
    ///     <description>Does not allow specifying authenticator and always uses all available methods.</description>
    /// </item>
    /// </list>
    /// </summary>
    CompanionDevice = 8,
}

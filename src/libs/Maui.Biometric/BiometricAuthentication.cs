using Maui.Biometric.Abstractions;

namespace Maui.Biometric;

/// <summary>
/// This class provides access to the biometric authentication implementation.
/// </summary>
public static class BiometricAuthentication
{
    private static Lazy<IBiometricAuthentication> _implementation = new(() =>
    {
#if __IOS__
        return new IosBiometricAuthentication();
#elif __ANDROID__
        return new AndroidBiometricAuthentication();
#elif WINDOWS
        return new WindowsBiometricAuthentication();
#else
        return new NotImplementedBiometricAuthentication();
#endif
    });

    /// <summary>
    /// Current biometric authentication implementation to use.
    /// </summary>
    public static IBiometricAuthentication Current
    {
        get => _implementation.Value;
        set => _implementation = new Lazy<IBiometricAuthentication>(() => value);
    }
}
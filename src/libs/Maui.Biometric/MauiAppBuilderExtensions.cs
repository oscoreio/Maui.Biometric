namespace Maui.Biometric;

/// <summary>
/// This class adds biometric authentication support to the Maui application.
/// </summary>
public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// Adds biometric authentication support to the Maui application.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static MauiAppBuilder UseBiometricAuthentication(
        this MauiAppBuilder builder) 
    {
        builder = builder ?? throw new ArgumentNullException(nameof(builder));
        
        builder.Services.AddSingleton<IBiometricAuthentication>(static _ => BiometricAuthentication.Current);
        
        return builder;
    }
}

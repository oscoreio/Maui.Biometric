using Foundation;
using LocalAuthentication;

// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

internal static class TypeExtensions
{
    public static LAPolicy MapToLaPolicy(this Authenticator authenticator)
    {
        if (authenticator.HasFlag(Authenticator.CompanionDevice) &&
            (OperatingSystem.IsIOSVersionAtLeast(18) ||
            OperatingSystem.IsMacCatalystVersionAtLeast(18) ||
            OperatingSystem.IsMacOSVersionAtLeast(15)))
        {
            return authenticator.HasFlag(Authenticator.DeviceCredential)
                ? LAPolicy.DeviceOwnerAuthenticationWithCompanion
                : LAPolicy.DeviceOwnerAuthenticationWithBiometricsOrCompanion;
        }
        
        return authenticator.HasFlag(Authenticator.DeviceCredential)
            ? LAPolicy.DeviceOwnerAuthentication
            : LAPolicy.DeviceOwnerAuthenticationWithBiometrics;
    }

    public static bool IsDeniedError(this NSError error)
    {
        if (string.IsNullOrEmpty(error.Description))
        {
            return false;
        }
        
        return error.Description.Contains("denied", StringComparison.OrdinalIgnoreCase);
    }
    
    public static AuthenticationStatus ToStatus(this NSError error)
    {
        return (LAStatus)error.Code switch
        {
            LAStatus.AuthenticationFailed when error.Description.Contains("retry limit exceeded", StringComparison.OrdinalIgnoreCase)
                => AuthenticationStatus.TooManyAttempts,
            LAStatus.AuthenticationFailed => AuthenticationStatus.Failed,
            LAStatus.UserCancel or LAStatus.AppCancel => AuthenticationStatus.Canceled,
            LAStatus.UserFallback => AuthenticationStatus.FallbackRequested,
            LAStatus.BiometryLockout => AuthenticationStatus.TooManyAttempts,
            // this can happen if it was available, but the user didn't allow face ID
            LAStatus.BiometryNotAvailable when error.IsDeniedError() => AuthenticationStatus.Denied,
            LAStatus.BiometryNotAvailable => AuthenticationStatus.NotAvailable,
            LAStatus.CompanionNotAvailable => AuthenticationStatus.NotAvailable,
            _ => AuthenticationStatus.UnknownError
        };
    }
    
    public static AuthenticationAvailability ToAvailability(this NSError error)
    {
        return (LAStatus)error.Code switch
        {
            LAStatus.BiometryNotAvailable => error.IsDeniedError()
                ? AuthenticationAvailability.Denied
                : AuthenticationAvailability.NoSensor,
            LAStatus.BiometryNotEnrolled => AuthenticationAvailability.NoBiometric,
            LAStatus.PasscodeNotSet => AuthenticationAvailability.NoFallback,
            LAStatus.CompanionNotAvailable => AuthenticationAvailability.NoCompanionDeviceWasDiscovered,
            _ => AuthenticationAvailability.Unknown,
        };
    }
    
}
using Maui.Biometric.Abstractions;
using Android;
using Android.App;
using Android.Content.PM;
using AndroidX.Biometric;
using AndroidX.Fragment.App;
using Java.Util.Concurrent;
using Application = Android.App.Application;

// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

internal sealed class AndroidBiometricAuthentication : NotImplementedBiometricAuthentication
{
    private readonly BiometricManager _manager = BiometricManager.From(Application.Context);

    public override async Task<AuthenticationType> GetAuthenticationTypeAsync()
    {
        var availability = await GetAvailabilityAsync(
            strength: AuthenticationStrength.Weak).ConfigureAwait(false);
        if (availability is Availability.NoBiometric or 
                            Availability.NoPermission or 
                            Availability.Available)
        {
            return AuthenticationType.Fingerprint;
        }

        return AuthenticationType.None;
    }

    private static int GetAllowedAuthenticators(AuthenticationStrength strength)
    {
        return strength switch
        {
            AuthenticationStrength.Strong => BiometricManager.Authenticators.BiometricStrong,
            AuthenticationStrength.Weak => BiometricManager.Authenticators.BiometricStrong |
                                           BiometricManager.Authenticators.BiometricWeak,
            AuthenticationStrength.Any => BiometricManager.Authenticators.BiometricStrong |
                                          BiometricManager.Authenticators.BiometricWeak |
                                          BiometricManager.Authenticators.DeviceCredential,
            _ => throw new ArgumentOutOfRangeException(nameof(strength), strength, null)
        };

    }
    
    public override Task<Availability> GetAvailabilityAsync(
        AuthenticationStrength strength = AuthenticationStrength.Weak)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            return Task.FromResult(Availability.NoApi);
        }
        
        if (OperatingSystem.IsAndroidVersionAtLeast(23) &&
            !OperatingSystem.IsAndroidVersionAtLeast(28) &&
            Application.Context.CheckCallingOrSelfPermission(Manifest.Permission.UseFingerprint) != Permission.Granted)
        {
            return Task.FromResult(Availability.NoPermission);
        }

        if (OperatingSystem.IsAndroidVersionAtLeast(28) &&
            Application.Context.CheckCallingOrSelfPermission(Manifest.Permission.UseBiometric) != Permission.Granted)
        {
            return Task.FromResult(Availability.NoPermission);
        }

        var canAuthenticate = _manager.CanAuthenticate(GetAllowedAuthenticators(strength));
        var availability = canAuthenticate switch
        {
            BiometricManager.BiometricErrorNoHardware => Availability.NoSensor,
            BiometricManager.BiometricErrorHwUnavailable => Availability.Unknown,
            BiometricManager.BiometricErrorNoneEnrolled => Availability.NoBiometric,
            BiometricManager.BiometricSuccess => Availability.Available,
            _ => Availability.Unknown,
        };
        if (availability == Availability.Available ||
            strength is not AuthenticationStrength.Any)
        {
            return Task.FromResult(availability);
        }

        try
        {
            if (Application.Context.GetSystemService(
                    name: Android.Content.Context.KeyguardService) is not KeyguardManager manager)
            {
                return Task.FromResult(Availability.NoFallback);
            }

            return Task.FromResult(manager.IsDeviceSecure
                ? Availability.Available
                : Availability.NoFallback);
        }
        catch
        {
            return Task.FromResult(Availability.NoFallback);
        }
    }

    protected override async Task<AuthenticationResult> NativeAuthenticateAsync(
        AuthenticationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Title, nameof(request));

        if (Platform.CurrentActivity is not FragmentActivity)
        {
            throw new InvalidOperationException(
                $"Expected current activity to be '{typeof(FragmentActivity).FullName}' but " +
                $"was '{Platform.CurrentActivity?.GetType().FullName}'. " +
                "You need to use AndroidX. Have you installed Xamarin.AndroidX.Migration in your Android App project!?");
        }

        try
        {
            var cancel = string.IsNullOrWhiteSpace(request.CancelTitle) ?
                Application.Context.GetString(Android.Resource.String.Cancel) :
                request.CancelTitle;

            var handler = new AuthenticationHandler();
            var builder = new BiometricPrompt.PromptInfo.Builder()
                .SetTitle(request.Title)
                .SetConfirmationRequired(request.ConfirmationRequired)
                .SetDescription(request.Reason);

            // It's not allowed to allow alternative auth & set the negative button
            builder = request.Strength is AuthenticationStrength.Any
                ? builder.SetAllowedAuthenticators(GetAllowedAuthenticators(request.Strength))
                : builder.SetNegativeButtonText(cancel);
            
            var info = builder.Build();
            var executor = Executors.NewSingleThreadExecutor();
            if (executor is null)
            {
                return new AuthenticationResult
                {
                    Status = AuthenticationResultStatus.UnknownError,
                    ErrorMessage = "Failed to create executor.",
                };
            }

            var activity = (FragmentActivity)Platform.CurrentActivity;
            using var dialog = new BiometricPrompt(activity, executor, handler);
            await using var registration = cancellationToken.Register(
                // ReSharper disable once AccessToDisposedClosure
                () => dialog.CancelAuthentication()).ConfigureAwait(false);
            
            dialog.Authenticate(info);
            var result = await handler.GetTask().ConfigureAwait(true);

            return result;
        }
        catch (Exception e)
        {
            return new AuthenticationResult
            {
                Status = AuthenticationResultStatus.UnknownError,
                ErrorMessage = e.Message
            };
        }
    }
}
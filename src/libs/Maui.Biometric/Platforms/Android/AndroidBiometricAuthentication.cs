using Android;
using Android.App;
using Android.Content.PM;
using AndroidX.Biometric;
using AndroidX.Fragment.App;
using Java.Util.Concurrent;
using Application = Android.App.Application;

// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

internal sealed class AndroidBiometricAuthentication : IBiometricAuthentication
{
    private readonly BiometricManager _manager = BiometricManager.From(Application.Context);

    public async Task<AuthenticationType> GetAuthenticationTypeAsync(
        Authenticator authenticators = AuthenticationRequest.DefaultAuthenticators,
        CancellationToken cancellationToken = default)
    {
        var availability = await GetAvailabilityAsync(
            authenticators, cancellationToken).ConfigureAwait(true);
        if (availability is AuthenticationAvailability.NoBiometric or 
                            AuthenticationAvailability.NoPermission or 
                            AuthenticationAvailability.Available)
        {
            return AuthenticationType.Fingerprint;
        }

        return AuthenticationType.None;
    }

    private static int GetAllowedAuthenticators(
        Authenticator authenticators = AuthenticationRequest.DefaultAuthenticators)
    {
        var androidAuthenticators = 0;
        if (authenticators.HasFlag(Authenticator.Biometric))
        {
            androidAuthenticators |= BiometricManager.Authenticators.BiometricWeak;
        }
        if (authenticators.HasFlag(Authenticator.DeviceCredential))
        {
            androidAuthenticators |= BiometricManager.Authenticators.DeviceCredential;
        }
        if (authenticators.HasFlag(Authenticator.BiometricStrong))
        {
            androidAuthenticators |= BiometricManager.Authenticators.BiometricStrong;
        }
        
        return androidAuthenticators;

    }
    
    public Task<AuthenticationAvailability> GetAvailabilityAsync(
        Authenticator authenticators = AuthenticationRequest.DefaultAuthenticators,
        CancellationToken cancellationToken = default)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            return Task.FromResult(AuthenticationAvailability.NotSupported);
        }
        
        if (OperatingSystem.IsAndroidVersionAtLeast(23) &&
            !OperatingSystem.IsAndroidVersionAtLeast(28) &&
            Application.Context.CheckCallingOrSelfPermission(Manifest.Permission.UseFingerprint) != Permission.Granted)
        {
            return Task.FromResult(AuthenticationAvailability.NoPermission);
        }

        if (OperatingSystem.IsAndroidVersionAtLeast(28) &&
            Application.Context.CheckCallingOrSelfPermission(Manifest.Permission.UseBiometric) != Permission.Granted)
        {
            return Task.FromResult(AuthenticationAvailability.NoPermission);
        }

        var canAuthenticate = _manager.CanAuthenticate(GetAllowedAuthenticators(authenticators));
        var availability = canAuthenticate switch
        {
            BiometricManager.BiometricErrorNoHardware => AuthenticationAvailability.NoSensor,
            BiometricManager.BiometricErrorHwUnavailable => AuthenticationAvailability.Unknown,
            BiometricManager.BiometricErrorNoneEnrolled => AuthenticationAvailability.NoBiometric,
            BiometricManager.BiometricSuccess => AuthenticationAvailability.Available,
            _ => AuthenticationAvailability.Unknown,
        };
        if (availability == AuthenticationAvailability.Available ||
            !authenticators.HasFlag(Authenticator.DeviceCredential))
        {
            return Task.FromResult(availability);
        }

        try
        {
            if (Application.Context.GetSystemService(
                    name: Android.Content.Context.KeyguardService) is not KeyguardManager manager)
            {
                return Task.FromResult(AuthenticationAvailability.NoFallback);
            }

            return Task.FromResult(manager.IsDeviceSecure
                ? AuthenticationAvailability.Available
                : AuthenticationAvailability.NoFallback);
        }
        catch
        {
            return Task.FromResult(AuthenticationAvailability.NoFallback);
        }
    }

    public async Task<AuthenticationResult> AuthenticateAsync(
        AuthenticationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Title, nameof(request));

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
            builder = request.Authenticators.HasFlag(Authenticator.DeviceCredential)
                ? builder.SetAllowedAuthenticators(GetAllowedAuthenticators(request.Authenticators))
                : builder.SetNegativeButtonText(cancel);
            
            var info = builder.Build();
            //var executor = ContextCompat.GetMainExecutor(Platform.CurrentActivity);
            var executor = Executors.NewSingleThreadExecutor();
            if (executor is null)
            {
                return new AuthenticationResult
                {
                    Status = AuthenticationStatus.UnknownError,
                    ErrorMessage = "Failed to create executor.",
                };
            }

            if (Platform.CurrentActivity is not FragmentActivity fragmentActivity)
            {
                return new AuthenticationResult
                {
                    Status = AuthenticationStatus.UnknownError,
                    ErrorMessage = "Current activity is not a FragmentActivity.",
                };
            }
            
            using var dialog = new BiometricPrompt(fragmentActivity, executor, handler);
            await using var registration = cancellationToken.Register(
                // ReSharper disable once AccessToDisposedClosure
                () => dialog.CancelAuthentication()).ConfigureAwait(true);
            
            dialog.Authenticate(info);
            
            var result = await handler.GetTask().ConfigureAwait(true);

            return result;
        }
        catch (Exception e)
        {
            return new AuthenticationResult
            {
                Status = AuthenticationStatus.UnknownError,
                ErrorMessage = e.Message,
            };
        }
    }
}
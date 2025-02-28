using Android;
using Android.Content.PM;
using AndroidX.Biometric;
using AndroidX.Fragment.App;
using Java.Util.Concurrent;
using Application = Android.App.Application;

// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

internal sealed class AndroidBiometricAuthentication : IBiometricAuthentication
{
    public Task<AvailabilityResult> CheckAvailabilityAsync(
        Authenticator authenticators = AuthenticationRequest.DefaultAuthenticators,
        CancellationToken cancellationToken = default)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            return Task.FromResult(new AvailabilityResult
            {
                Availability = AuthenticationAvailability.NotSupported,
                Sensors = [],
            });
        }
        

        if (OperatingSystem.IsAndroidVersionAtLeast(28) &&
            Platform.AppContext.CheckCallingOrSelfPermission(Manifest.Permission.UseBiometric) != Permission.Granted)
        {
            return Task.FromResult(new AvailabilityResult
            {
                Availability = AuthenticationAvailability.NoPermission,
                Sensors = [],
            });
        }
        
        var biometricManager = BiometricManager.From(Platform.AppContext);
        var canAuthenticate = biometricManager.CanAuthenticate(GetAllowedAuthenticators(authenticators));

        return Task.FromResult(new AvailabilityResult
        {
            Availability = canAuthenticate switch
            {
                BiometricManager.BiometricErrorNoHardware => AuthenticationAvailability.NoSensor,
                BiometricManager.BiometricErrorHwUnavailable => AuthenticationAvailability.Unknown,
                BiometricManager.BiometricErrorNoneEnrolled => AuthenticationAvailability.NoBiometric,
                BiometricManager.BiometricSuccess => AuthenticationAvailability.Available,
                _ => AuthenticationAvailability.Unknown,
            },
            Sensors = sensors,
        });
    }

    public async Task<AuthenticationResult> AuthenticateAsync(
        AuthenticationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Title, nameof(request));

        var availability = await CheckAvailabilityAsync(request.Authenticators, cancellationToken).ConfigureAwait(true);
        if (availability.Availability != AuthenticationAvailability.Available)
        {
            var status = availability.Availability == AuthenticationAvailability.Denied ?
                AuthenticationStatus.Denied :
                AuthenticationStatus.NotAvailable;

            return new AuthenticationResult
            {
                Status = status,
                ErrorMessage = availability.Availability.ToString(),
            };
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
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Maui.Biometric.SampleApp;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _status = string.Empty;

    [ObservableProperty]
    private bool _autoCancel;

    [ObservableProperty]
    private bool _confirmationRequired;

    [ObservableProperty]
    private AvailabilityResult _availabilityResult = new()
    {
        Availability = AuthenticationAvailability.Unknown,
        Sensors = [],
    };

    [ObservableProperty]
    private bool _isAuthenticationAvailable;

    [ObservableProperty]
    private bool _deviceCredentialAuthenticator = AuthenticationRequest.DefaultAuthenticators.HasFlag(Authenticator.DeviceCredential);

    [ObservableProperty]
    private bool _biometricAuthenticator = AuthenticationRequest.DefaultAuthenticators.HasFlag(Authenticator.Biometric);

    [ObservableProperty]
    private bool _biometricStrongAuthenticator = AuthenticationRequest.DefaultAuthenticators.HasFlag(Authenticator.BiometricStrong);

    [ObservableProperty]
    private bool _companionDeviceAuthenticator = AuthenticationRequest.DefaultAuthenticators.HasFlag(Authenticator.CompanionDevice);
    
    private Authenticator Authenticators => 
        (DeviceCredentialAuthenticator ? Authenticator.DeviceCredential : 0) |
        (BiometricAuthenticator ? Authenticator.Biometric : 0) |
        (BiometricStrongAuthenticator ? Authenticator.BiometricStrong : 0) |
        (CompanionDeviceAuthenticator ? Authenticator.CompanionDevice : 0);

    async partial void OnDeviceCredentialAuthenticatorChanged(bool value)
    {
        await Appearing();
    }
    
    async partial void OnBiometricAuthenticatorChanged(bool value)
    {
        await Appearing();
    }
    
    async partial void OnBiometricStrongAuthenticatorChanged(bool value)
    {
        await Appearing();
    }
    
    async partial void OnCompanionDeviceAuthenticatorChanged(bool value)
    {
        await Appearing();
    }

    [RelayCommand]
    private async Task Appearing()
    {
        try
        {
            AvailabilityResult = await BiometricAuthentication.Current.CheckAvailabilityAsync(Authenticators);
            IsAuthenticationAvailable = await BiometricAuthentication.Current.IsAvailableAsync(Authenticators);
        }
        catch (Exception ex)
        {
            Status = ex.Message;
        }
    }

    [RelayCommand]
    private async Task Authenticate(CancellationToken cancellationToken = default)
    {
        try
        {
            await AuthenticateAsync(
                reason: "Prove you have fingers!",
                cancellationToken: cancellationToken);
        }
        catch (TaskCanceledException)
        {
            Status = "Authentication was canceled!";
        }
        catch (Exception ex)
        {
            Status = ex.Message;
        }
    }

    [RelayCommand]
    private async Task AuthenticateLocalized(CancellationToken cancellationToken = default)
    {
        try
        {
            await AuthenticateAsync(
                reason: "Beweise, dass du Finger hast!",
                cancel: "Abbrechen",
                fallback: "Anders!",
                cancellationToken: cancellationToken);
        }
        catch (TaskCanceledException)
        {
            Status = "Authentication was canceled!";
        }
        catch (Exception ex)
        {
            Status = ex.Message;
        }
    }

    private async Task AuthenticateAsync(
        string reason,
        string? cancel = null,
        string? fallback = null,
        CancellationToken cancellationToken = default)
    {
        using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        if (AutoCancel)
        {
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));
        }

        Status = string.Empty;

        var result = await BiometricAuthentication.Current.AuthenticateAsync(
                title: "My App",
                reason: reason,
                authenticators: Authenticators,
                cancelTitle: cancel ?? string.Empty,
                fallbackTitle: fallback ?? string.Empty,
                confirmationRequired: ConfirmationRequired,
                cancellationToken: cancellationTokenSource.Token);
        if (!result.IsSuccessful)
        {
            Status = $"{result.Status}: {result.ErrorMessage}";
            return;
        }
        
        await Application.Current!.Windows[0].Page!.DisplayAlert("Success", "You are authenticated!", "OK");
    }
}

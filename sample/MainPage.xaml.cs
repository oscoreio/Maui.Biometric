using Maui.Biometric.Abstractions;

namespace Maui.Biometric.SampleApp;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        AuthenticationTypeLabel.Text = "Auth Type: " + await BiometricAuthentication.Current.GetAuthenticationTypeAsync();
    }

    private async void OnAuthenticate(object sender, EventArgs e)
    {
        await AuthenticateAsync(
            reason: "Prove you have fingers!");
    }

    private async void OnAuthenticateLocalized(object sender, EventArgs e)
    {
        await AuthenticateAsync(
            reason: "Beweise, dass du Finger hast!",
            cancel: "Abbrechen",
            fallback: "Anders!",
            tooFast: "Viel zu schnell!");
    }

    private async Task AuthenticateAsync(
        string reason,
        string? cancel = null,
        string? fallback = null,
        string? tooFast = null)
    {
        using var cancellationTokenSource = AutoCancelSwitch.IsToggled
            ? new CancellationTokenSource(TimeSpan.FromSeconds(10))
            : new CancellationTokenSource();
        StatusLabel.Text = "";

        var result = await BiometricAuthentication.Current.AuthenticateAsync(
            request: new AuthenticationRequest("My App", reason)
            {
                // all optional
                CancelTitle = cancel ?? string.Empty,
                FallbackTitle = fallback ?? string.Empty,
                Strength = AllowAlternativeSwitch.IsToggled
                    ? AuthenticationStrength.Any
                    : AuthenticationStrength.Weak,
                ConfirmationRequired = ConfirmationRequiredSwitch.IsToggled,
                HelpTexts =
                {
                    // optional
                    MovedTooFast = tooFast ?? string.Empty,
                }
            },
            cancellationToken: cancellationTokenSource.Token);
        if (!result.Authenticated)
        {
            StatusLabel.Text = $"{result.Status}: {result.ErrorMessage}";
            return;
        }
        
        await Navigation.PushAsync(new SecretPage());
    }
}


namespace Maui.Biometric.SampleApp;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();

        BindingContext = new MainViewModel();
	}
}
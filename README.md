# Maui.Biometric

[![Nuget package](https://img.shields.io/nuget/vpre/Oscore.Maui.Biometric)](https://www.nuget.org/packages/Oscore.Maui.Biometric/)
[![CI/CD](https://github.com/oscoreio/Maui.Biometric/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/oscoreio/Maui.Biometric/actions/workflows/dotnet.yml)
[![License: MIT](https://img.shields.io/github/license/oscoreio/Maui.Biometric)](https://github.com/oscoreio/Maui.Biometric/blob/main/LICENSE)

Provides a cross-platform implementation of biometric authentication.  
Supports iOS, macOS, Android and Windows.  
Continuation of the abandoned Plugin.Fingerprint in the MAUI ecosystem.  

## Usage
- Add NuGet package to your project:
```xml
<PackageReference Include="Oscore.Maui.Biometric" Version="1.0.0" />
```

- iOS - Add `NSFaceIDUsageDescription` to your Info.plist to describe the reason your app uses Face ID. 
(see [Documentation](https://developer.apple.com/library/content/documentation/General/Reference/InfoPlistKeyReference/Articles/CocoaKeys.html#//apple_ref/doc/uid/TP40009251-SW75)). 
Otherwise the App will crash when you start a Face ID authentication on iOS 11.3+.
```xml
<key>NSFaceIDUsageDescription</key>
<string>Need your face to unlock secrets!</string>
```

- Android - Request the permission in `AndroidManifest.xml`
```xml
<uses-permission android:name="android.permission.USE_FINGERPRINT" android:maxSdkVersion="27" />
<uses-permission android:name="android.permission.USE_BIOMETRIC" android:minSdkVersion="28" />
```

- Add the following to your `MauiProgram.cs` `CreateMauiApp` method:
```diff
builder
    .UseMauiApp<App>()
+   .UseBiometricAuthentication()
    .ConfigureFonts(fonts =>
    {
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        fonts.AddFont("OpenSans-Sed:nammibold.ttf", "OpenSansSemibold");
    });
```

- Use through `BiometricAuthentication.Current` or using `IBiometricAuthentication` from DI:
```csharp
var result = await BiometricAuthentication.Current.AuthenticateAsync(
    new AuthenticationRequest(
        title: "Prove you have fingers!",
        reason: "Because without it you can't have access"));
if (result.Authenticated)
{
    // do secret stuff :)
}
else
{
    // not allowed to do secret stuff :(
}
```

## Testing on Simulators

### iOS

![Controlling the sensor on the iOS Simulator](assets/ios_simulator.png "Controlling the sensor on the iOS Simulator")

With the Hardware menu you can

- Toggle the enrollment status
- Trigger valid (<kbd>⌘</kbd> <kbd>⌥</kbd> <kbd>M</kbd>) and invalid (<kbd>⌘</kbd> <kbd>⌥</kbd> <kbd>N</kbd>) fingerprint sensor events

### Android

- start the emulator (Android >= 6.0)
- open the settings app
- go to Security > Fingerprint, then follow the enrollment instructions
- when it asks for touch
- open command prompt
- `telnet 127.0.0.1 <emulator-id>` (`adb devices` prints "emulator-&lt;emulator-id&gt;")
- `finger touch 1`
- `finger touch 1`

Sending fingerprint sensor events for testing the plugin can be done with the telnet commands, too.

**Note for Windows users:**
You have to enable telnet: Programs and Features > Add Windows Feature > Telnet Client

## Links
- https://github.com/smstuebe/xamarin-fingerprint
- https://stackoverflow.com/questions/633132/is-ms-pl-microsoft-public-license-code-allowed-in-commercial-product

## Legal information and credits

It was forked from the [xamarin-fingerprint](https://github.com/smstuebe/xamarin-fingerprint) project.  
xamarin-fingerprint is a project by [Sven-Michael Stübe](https://github.com/smstuebe).  
It was licensed under the [MS-PL license](https://github.com/smstuebe/xamarin-fingerprint/blob/master/LICENSE).  
This fork changes the license to MIT with attribution to the original license.  
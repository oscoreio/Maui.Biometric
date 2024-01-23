using Android.Content;
using Java.Lang;
using Maui.Biometric.Abstractions;
using AndroidX.Biometric;

// ReSharper disable once CheckNamespace

namespace Maui.Biometric;

internal sealed class AuthenticationHandler : BiometricPrompt.AuthenticationCallback, IDialogInterfaceOnClickListener
{
    private readonly TaskCompletionSource<AuthenticationResult> _taskCompletionSource = new();

    public Task<AuthenticationResult> GetTask()
    {
        return _taskCompletionSource.Task;
    }

    public override void OnAuthenticationSucceeded(BiometricPrompt.AuthenticationResult result)
    {
        base.OnAuthenticationSucceeded(result);
        
        _taskCompletionSource.TrySetResult(new AuthenticationResult
        {
            Status = AuthenticationResultStatus.Succeeded,
        });
    }

    public override void OnAuthenticationError(int errorCode, ICharSequence errString)
    {
        base.OnAuthenticationError(errorCode, errString);

        _taskCompletionSource.TrySetResult(new AuthenticationResult
        {
            ErrorMessage = errString != null!
                ? errString.ToString()
                : string.Empty,
            Status = errorCode switch
            {
                BiometricPrompt.ErrorLockout => AuthenticationResultStatus.TooManyAttempts,
                BiometricPrompt.ErrorUserCanceled => AuthenticationResultStatus.Canceled,
                BiometricPrompt.ErrorNegativeButton => AuthenticationResultStatus.Canceled,
                _ => AuthenticationResultStatus.Failed
            }
        });
    }

    public void OnClick(IDialogInterface? dialog, int which)
    {
        _taskCompletionSource.TrySetResult(new AuthenticationResult
        {
            Status = AuthenticationResultStatus.Canceled,
        });
    }

    //public override void OnAuthenticationHelp(BiometricAcquiredStatus helpCode, ICharSequence helpString)
    //{
    //    base.OnAuthenticationHelp(helpCode, helpString);
    //    _listener?.OnHelp(FingerprintAuthenticationHelp.MovedTooFast, helpString?.ToString());
    //}
}
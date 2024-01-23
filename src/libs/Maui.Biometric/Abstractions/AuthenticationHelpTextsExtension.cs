namespace Maui.Biometric.Abstractions;

/// <summary>
/// 
/// </summary>
public static class AuthenticationHelpTextsExtension
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="texts"></param>
    /// <param name="helpText"></param>
    /// <param name="nativeText"></param>
    /// <returns></returns>
    public static string GetText(
        this AuthenticationHelpTexts texts,
        AuthenticationHelpText helpText,
        string nativeText)
    {
        ArgumentNullException.ThrowIfNull(texts);
        
        return helpText switch
        {
            AuthenticationHelpText.MovedTooFast
                when !string.IsNullOrEmpty(texts.MovedTooFast) => texts.MovedTooFast,
            AuthenticationHelpText.MovedTooSlow
                when !string.IsNullOrEmpty(texts.MovedTooSlow) => texts.MovedTooSlow,
            AuthenticationHelpText.Partial
                when !string.IsNullOrEmpty(texts.Partial) => texts.Partial,
            AuthenticationHelpText.Insufficient
                when !string.IsNullOrEmpty(texts.Insufficient) => texts.Insufficient,
            AuthenticationHelpText.Dirty
                when !string.IsNullOrEmpty(texts.Dirty) => texts.Dirty,
            _ => nativeText
        };
    }
}
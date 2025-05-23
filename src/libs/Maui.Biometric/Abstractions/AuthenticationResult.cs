﻿// ReSharper disable once CheckNamespace
namespace Maui.Biometric;

/// <summary>
/// The result of the authentication.
/// </summary>
public class AuthenticationResult
{
    /// <summary>
    /// Indicates if the authentication was successful.
    /// </summary>
    public bool IsSuccessful => Status == AuthenticationStatus.Success;

    /// <summary>
    /// Detailed information of the authentication.
    /// </summary>
    public required AuthenticationStatus Status { get; init; }

    /// <summary>
    /// Reason for the unsuccessful authentication.
    /// </summary>
    public string ErrorMessage { get; init; } = string.Empty;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static implicit operator bool(AuthenticationResult result)
    {
        return result?.ToBoolean() ?? false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool ToBoolean()
    {
        return IsSuccessful;
    }
}
namespace Maui.Biometric.Abstractions;

/// <inheritdoc />
public class NotImplementedBiometricAuthentication : IBiometricAuthentication
{
    /// <inheritdoc />
    public async Task<AuthenticationResult> AuthenticateAsync(
        AuthenticationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var availability = await GetAvailabilityAsync(request.Strength).ConfigureAwait(false);
        if (availability != Availability.Available)
        {
            var status = availability == Availability.Denied ?
                AuthenticationResultStatus.Denied :
                AuthenticationResultStatus.NotAvailable;

            return new AuthenticationResult { Status = status, ErrorMessage = availability.ToString() };
        }

        return await NativeAuthenticateAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> IsAvailableAsync(
        AuthenticationStrength strength = AuthenticationStrength.Weak)
    {
        return await GetAvailabilityAsync(strength).ConfigureAwait(false) == Availability.Available;
    }

    /// <inheritdoc />
    public virtual Task<Availability> GetAvailabilityAsync(
        AuthenticationStrength strength = AuthenticationStrength.Weak)
    {
        return Task.FromResult(Availability.NoImplementation);
    }

    /// <inheritdoc />
    public virtual Task<AuthenticationType> GetAuthenticationTypeAsync()
    {
        return Task.FromResult(AuthenticationType.None);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual Task<AuthenticationResult> NativeAuthenticateAsync(
        AuthenticationRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new AuthenticationResult
        {
            Status = AuthenticationResultStatus.NotAvailable,
            ErrorMessage = "Not implemented for the current platform.",
        });
    }
}
namespace MediClaim.Application
    .Features.Auth.Common;

public class AuthResponseDto
{
    public string AccessToken { get; set; }
        = default!;

    public string RefreshToken
    {
        get; set;
    } = default!;
}
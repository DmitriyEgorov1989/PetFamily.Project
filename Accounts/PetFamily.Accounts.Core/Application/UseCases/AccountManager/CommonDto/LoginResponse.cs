namespace PetFamily.Accounts.Core.Application.UseCases.AccountManager.CommonDto
{
    public record class LoginResponse(string AccessToken, string RefreshToken);
}
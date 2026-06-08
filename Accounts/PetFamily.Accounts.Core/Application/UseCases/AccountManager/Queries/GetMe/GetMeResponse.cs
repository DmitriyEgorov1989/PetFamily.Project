namespace PetFamily.Accounts.Core.Application.UseCases.AccountManager.Queries.GetMe
{
    public record GetMeResponse(UserInfo UserInfo);

    public record UserInfo(Guid Id, string Email, string Name, string RoleName);
}
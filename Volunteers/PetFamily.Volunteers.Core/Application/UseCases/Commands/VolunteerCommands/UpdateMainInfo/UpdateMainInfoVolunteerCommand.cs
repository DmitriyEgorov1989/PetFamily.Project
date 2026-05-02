using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.SharedKernel.Errors;
using PetFamily.Volunteers.Core.Application.UseCases.CommonDto;

namespace PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.UpdateMainInfo
{
    public record UpdateMainInfoVolunteerCommand(
        Guid VolunteerId,
        UpdateMainInfoVolunteerDto UpdateMainInfo) : IRequest<UnitResult<Error.ErrorList>>;

    public record UpdateMainInfoVolunteerDto(
        FullNameDto FullName,
        string Email,
        string Description,
        int? Experience,
        string PhoneNumber
    );
}
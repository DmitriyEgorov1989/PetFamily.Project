using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Core.Application.UseCases.CommonDto;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Core.Application.UseCases.Comands.Volunteer.UpdateMainInfo
{
    public record UpdateMainInfoVolunteerCommand(
        Guid VolunteerId,
        UpdateMainInfoVolunteerDto UpdateMainInfo) : IRequest<UnitResult<ErrorList>>;

    public record UpdateMainInfoVolunteerDto(
    FullNameDto FullName,
    string Email,
    string Description,
    int? Experience,
    string PhoneNumber
    );
}
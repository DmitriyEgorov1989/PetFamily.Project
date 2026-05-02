using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Volunteers.Core.Application.UseCases.CommonDto;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.UpdatePet;

public record UpdatePetCommand(
    Guid PetId,
    Guid VolunteerId,
    string Name,
    string Description,
    string Color,
    string HealthInfo,
    AddressDto Address,
    decimal? Weight,
    int? Height,
    string PhoneNumber,
    bool? IsSterilized,
    DateTime? BirthDate,
    bool? IsVaccined) : IRequest<UnitResult<ErrorList>>;
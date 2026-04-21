using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Core.Application.UseCases.CommonDto;
using static Primitives.Error;

namespace PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.AddPet;

public record AddPetCommand(
    Guid VolunteerId,
    PetDto Pet) : IRequest<Result<Guid, ErrorList>>;

public record PetDto(
    string Name,
    string Description,
    PetSpeciesInfoDto SpeciesInfo,
    string Color,
    string HealthInfo,
    AddressDto Address,
    decimal Weight,
    int Height,
    string PhoneNumber,
    bool IsSterilized,
    DateTime BirthDate,
    bool IsVaccined,
    int PetHelpStatus,
    HelpRequisiteDto[] HelpRequisites);
using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Core.Application.UseCases.CommonDto;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.AddPet;

public record AddPetCommand(
    Guid VolunteerId,
    PetWriteDto PetWrite) : IRequest<Result<Guid, ErrorList>>;

public record PetWriteDto(
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
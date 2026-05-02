using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Volunteers.Core.Application.UseCases.CommonDto;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Volunteers.Core.Application.UseCases.Commands.VolunteerCommands.UpdateHelpRequisites
{
    public record UpdateHelpRequisitesCommand(Guid VolunteerId, List<HelpRequisiteDto> HelpRequisites)
        : IRequest<UnitResult<ErrorList>>;
}
using CSharpFunctionalExtensions;
using MediatR;
using PetFamily.Core.Application.UseCases.CommonDto;
using static Primitives.Error;

namespace PetFamily.Core.Application.UseCases.Comands.VolunteerComands.UpdateHelpRequisites
{
    public record UpdateHelpRequisitesCommand(Guid VolunteerId, List<HelpRequisiteDto> HelpRequisites)
        : IRequest<UnitResult<ErrorList>>;
}
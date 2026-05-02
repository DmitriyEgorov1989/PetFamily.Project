using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using PetFamily.Core.Application.UseCases.Commands.VolunteerCommands.UpdateHelpRequisites;
using PetFamily.Core.Application.UseCases.CommonDto;
using PetFamily.Core.Ports;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.DomainModels.VO;
using PetFamily.Volunteers.Core.Domain.Models.VolunteerAggregate;
using PetFamily.Volunteers.Core.Ports;
using Serilog;
using Xunit;
using Email = PetFamily.Core.Domain.Models.SharedKernel.VO.Email;

namespace PetFamily.UnitTests.Core.Application.UseCases.Commands.VolunteerCommands.UpdateHelpRequisites;

public class UpdateHelpRequisitesShould
{
    private readonly ILogger _logger = Substitute.For<ILogger>();

    private readonly IUnitOfWork _unitOfWork =
        Substitute.For<IUnitOfWork>();

    private readonly IValidator<UpdateHelpRequisitesCommand> _validator =
        Substitute.For<IValidator<UpdateHelpRequisitesCommand>>();

    private readonly IVolunteerRepository _volunteerRepository =
        Substitute.For<IVolunteerRepository>();

    [Fact]
    public async Task BeUpdateHelpRequisitesReturnIsSuccess()
    {
        //arrange
        var volunteer = ExistedVolunteer();
        var newHelpRequisiteDto =
            new HelpRequisiteDto("newName", "newDescription");
        List<HelpRequisiteDto> newHelpRequisites = [newHelpRequisiteDto];

        _volunteerRepository
            .GetByIdAsync(volunteer.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(volunteer));

        _validator.ValidateAsync(Arg.Any<UpdateHelpRequisitesCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        var command =
            new UpdateHelpRequisitesCommand((Guid)volunteer.Id, newHelpRequisites);
        var handler =
            new UpdateHelpRequisitesHandler(_volunteerRepository, _logger, _validator, _unitOfWork);

        //act
        var result = await handler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        volunteer.HelpRequisites.ToList()[0]
            .Name.Should().Be(newHelpRequisiteDto.Name);
        volunteer.HelpRequisites.ToList()[0]
            .Description.Should().Be(newHelpRequisiteDto.Description);
        await _validator.Received(1).ValidateAsync(Arg.Any<UpdateHelpRequisitesCommand>(),
            Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private Volunteer ExistedVolunteer()
    {
        return Volunteer.Create(
            VolunteerId.NewId(),
            FullName.Create("ivan", "ivanivich", "ivanov").Value,
            Email.Create("diman@mail.ru").Value,
            "description",
            Experience.Create(5).Value,
            PhoneNumber.Create("89258761315").Value,
            HelpRequisites.Create(null),
            SocialNetworks.Create(null)).Value;
    }
}
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using PetFamily.Core.Application.UseCases.Comands.VolunteerComands.DeleteVolunteer;
using PetFamily.Core.Domain.Models.SharedKernel.VO;
using PetFamily.Core.Domain.Models.VolunteerAggregate;
using PetFamily.Core.Domain.Models.VolunteerAggregate.VO;
using PetFamily.Core.Ports;
using Serilog;
using Xunit;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace PetFamily.UnitTests.Core.Application.UseCases.Commands.VolunteerCommands.DeleteVolunteer;

public class DeleteVolunteerHandlerShould
{
    private readonly ILogger _logger =
        Substitute.For<ILogger>();

    private readonly IValidator<DeleteVolunteerCommand> _validator =
        Substitute.For<IValidator<DeleteVolunteerCommand>>();

    private readonly IVolunteerRepository _volunteerRepository =
        Substitute.For<IVolunteerRepository>();

    [Fact]
    public async Task BeDeleteVolunteerReturnSuccessVolunteer()
    {
        //arrange
        var volunteer = ExistedVolunteer();
        _volunteerRepository
            .GetByIdAsync(volunteer.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(volunteer));

        _validator.ValidateAsync(Arg.Any<DeleteVolunteerCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        var command = new DeleteVolunteerCommand((Guid)volunteer.Id);
        var handler = new DeleteVolunteerHandler(_logger, _volunteerRepository, _validator);
        //act
        var result = await handler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        volunteer.IsDelete.Should().Be(true);
        await _validator
            .Received(1)
            .ValidateAsync(
                Arg.Any<DeleteVolunteerCommand>()
                , Arg.Any<CancellationToken>());
        await _volunteerRepository
            .SaveAsync(Arg.Any<CancellationToken>());
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
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using PetFamily.Core.Application.UseCases.Comands.Volunteer.UpdateSocialNetwork;
using PetFamily.Core.Application.UseCases.CommonDto;
using PetFamily.Core.Domain.Models.VolunteerAggregate;
using PetFamily.Core.Ports;
using PetFamily.SharedKernel.DomainModels.Ids;
using PetFamily.SharedKernel.DomainModels.VO;
using Serilog;
using Xunit;
using Email = PetFamily.Core.Domain.Models.SharedKernel.VO.Email;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace PetFamily.UnitTests.Core.Application.UseCases.Commands.VolunteerCommands.UpdateMainInfo;

public class UpdateMainInfoShould
{
    private readonly ILogger _logger = Substitute.For<ILogger>();

    private readonly IUnitOfWork _unitOfWork =
        Substitute.For<IUnitOfWork>();

    private readonly IValidator<UpdateSocialNetworkCommand> _validator =
        Substitute.For<IValidator<UpdateSocialNetworkCommand>>();

    private readonly IVolunteerRepository _volunteerRepository =
        Substitute.For<IVolunteerRepository>();

    [Fact]
    public async Task BeUpdateSocialNetworkReturnIsSuccess()
    {
        //arrange
        var volunteer = ExistedVolunteer();
        var newSocialNetworkDto =
            new SocialNetworkDto("newName", "newDescription");
        List<SocialNetworkDto> newSocialNetworkDtos = [newSocialNetworkDto];

        _volunteerRepository
            .GetByIdAsync(volunteer.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(volunteer));

        _validator.ValidateAsync(Arg.Any<UpdateSocialNetworkCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        var command =
            new UpdateSocialNetworkCommand((Guid)volunteer.Id, newSocialNetworkDtos);
        var handler =
            new UpdateSocialNetworkHandler(_volunteerRepository, _logger, _validator, _unitOfWork);

        //act
        var result = await handler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        volunteer.SocialNetworks.ToList()[0]
            .Name.Should().Be(newSocialNetworkDto.Name);
        volunteer.SocialNetworks.ToList()[0]
            .Link.Should().Be(newSocialNetworkDto.Link);
        await _validator.Received(1).ValidateAsync(Arg.Any<UpdateSocialNetworkCommand>(),
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
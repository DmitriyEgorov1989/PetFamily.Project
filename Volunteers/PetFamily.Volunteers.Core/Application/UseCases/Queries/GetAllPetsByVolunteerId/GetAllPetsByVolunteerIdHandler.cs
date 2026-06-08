using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using PetFamily.SharedKernel.Extensions.Validations;
using PetFamily.Volunteers.Core.Ports.DataBaseForRead;
using Serilog;
using static PetFamily.SharedKernel.Errors.Error;

namespace PetFamily.Volunteers.Core.Application.UseCases.Queries.GetAllPetsByVolunteerId
{
    public class GetAllPetsByVolunteerIdHandler :
        IRequestHandler<GetAllPetsByVolunteerIdQuery,
            Result<GetAllPetsByVolunteerIdResponse, ErrorList>>
    {
        private readonly ILogger _logger;
        private readonly IValidator<GetAllPetsByVolunteerIdQuery> _validator;
        private readonly IPetsReadRepository _petsReadRepository;

        public GetAllPetsByVolunteerIdHandler(ILogger logger,
            IValidator<GetAllPetsByVolunteerIdQuery> validator,
            IPetsReadRepository petsReadRepository)
        {
            _logger = logger;
            _validator = validator;
            _petsReadRepository = petsReadRepository;
        }

        public async Task<Result<GetAllPetsByVolunteerIdResponse, ErrorList>> Handle(
            GetAllPetsByVolunteerIdQuery query,
            CancellationToken cancellationToken)
        {
            var resultValidation =
                await _validator.ValidateAsync(query, cancellationToken);

            if (!resultValidation.IsValid)
                return resultValidation.ToValidationErrorResponse(query);

            var resultGetPets =
                await _petsReadRepository.GetAllByVolunteerIdAsync(query.VolunteerId, cancellationToken);

            if (resultGetPets.IsFailure)
            {
                _logger.Error(
                    "Error occurred while getting all pets by volunteer id {VolunteerId}. Errors: {Errors}",
                    query.VolunteerId, resultGetPets.Error);

                return (ErrorList)resultGetPets.Error;
            }

            var pets = resultGetPets.Value;

            return new GetAllPetsByVolunteerIdResponse(pets);
        }
    }
}

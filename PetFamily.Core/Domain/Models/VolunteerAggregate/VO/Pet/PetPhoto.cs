using CSharpFunctionalExtensions;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace PetFamily.Core.Domain.Models.VolunteerAggregate.VO.Pet
{
    public record PetPhoto
    {
        private const long MAX_SIZE = 5 * 1024 * 1024;

        [ExcludeFromCodeCoverage]
        private PetPhoto() { }
        private PetPhoto(string pathStorage)
        {
            PathStorage = pathStorage;
        }
        public string PathStorage { get; }
        public static Result<PetPhoto, Error> Create(long size, string pathStorage)
        {
            if (size > MAX_SIZE)
                return GeneralErrors.InvalidSize(nameof(size));
            if (pathStorage == null)
                return GeneralErrors.ValueIsInvalid(nameof(pathStorage));

            return new PetPhoto(pathStorage);
        }
        public static Result<PetPhoto, Error> Create(string pathStorage)
        {

            if (pathStorage == null)
                return GeneralErrors.ValueIsInvalid(nameof(pathStorage));

            return new PetPhoto(pathStorage);
        }
    }
}
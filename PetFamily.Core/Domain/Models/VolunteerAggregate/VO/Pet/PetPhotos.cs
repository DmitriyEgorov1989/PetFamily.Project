using PetFamily.Core.Domain.Models.VolunteerAggregate.VO.Pet;

public class PetPhotos
{
    private readonly List<PetPhoto> _listPetPhotos = [];

    private PetPhotos() { }

    public IReadOnlyList<PetPhoto> ListPetPhotos => _listPetPhotos;

    public void Add(PetPhoto photo) => _listPetPhotos.Add(photo);
    public void Remove(PetPhoto photo) => _listPetPhotos.Remove(photo);
    public static PetPhotos Create(IEnumerable<PetPhoto>? list)
    {
        var photos = new PetPhotos();

        if (list is not null)
            photos._listPetPhotos.AddRange(list);

        return photos;
    }
}
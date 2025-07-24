using CAS.Core.Models;
using CAS.Data.Entities;
using CAS.Data.Repositories;

namespace CAS.Core.Services;

public interface IAnimalService
{
    Task<Animal> GetAsync(string id);
}

public class AnimalService(IAnimalRepository animalRepository) : IAnimalService
{
    public async Task<Animal> GetAsync(string id)
    {
        return Map(await animalRepository.GetAsync(id));
    }

    private static Animal Map(AnimalEntity animalEntity) => new()
    {
        Id = animalEntity.Id,
    };
}
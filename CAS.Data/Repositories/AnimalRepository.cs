using CAS.Data.Entities;

namespace CAS.Data.Repositories;

public interface IAnimalRepository
{
    Task<AnimalEntity> GetAsync(string id);
}

public class AnimalRepository : IAnimalRepository
{
    public Task<AnimalEntity> GetAsync(string id)
    {
        return Task.FromResult(new AnimalEntity { Id = "123" });
    }
}

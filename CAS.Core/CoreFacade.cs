using CAS.Core.Services;
using CAS.Data.Repositories;

namespace CAS.Core;

public interface ICoreFacade
{
    IAnimalService Animals { get; }
}

public class CoreFacade(IAnimalService animals) : ICoreFacade
{
    public IAnimalService Animals { get; } = animals;
}
using MongoDB.Driver;

namespace CentralAnimalDataStore.Utils.Mongo;

public interface IMongoDbClientFactory
{
    IMongoClient GetClient();

    IMongoCollection<T> GetCollection<T>(string collection);
}
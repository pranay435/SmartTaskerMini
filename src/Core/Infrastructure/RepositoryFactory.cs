using SmartTaskerMini.Core.Application;

namespace SmartTaskerMini.Core.Infrastructure;

public static class RepositoryFactory
{
    public static ITaskRepository CreateTaskRepository()
    {
        return Configuration.StorageType.ToUpper() switch
        {
            "JSON" => new JsonTaskRepository(Configuration.JsonFilePath),
            "XML" => new XmlTaskRepository(Configuration.XmlFilePath),
            _ => new AdoNetRepo(Configuration.ProductionConnectionString)
        };
    }
}
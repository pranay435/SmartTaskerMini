namespace SmartTaskerMini.Core.Application;

public static class Configuration
{
    public const string ProductionConnectionString = "Server=localhost;Database=SmartTaskerMini;User Id=myuser;Password=MyStrongPassword123!;TrustServerCertificate=True;";
    public const string TestConnectionString = "Server=localhost;Database=TestDb;User Id=myuser;Password=MyStrongPassword123!;TrustServerCertificate=True;";
    
    public static string StorageType => ConfigurationManager.Settings.StorageType;
    public static string JsonFilePath => ConfigurationManager.Settings.JsonFilePath;
    public static string XmlFilePath => ConfigurationManager.Settings.XmlFilePath;
}     
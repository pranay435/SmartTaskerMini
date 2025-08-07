using SmartTaskerMini.Core.Application;

namespace SmartTaskerMini.Tests.Application;

public class ConfigurationTests
{
    [Fact]
    public void ProductionConnectionString_IsNotEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(Configuration.ProductionConnectionString));
    }

    [Fact]
    public void TestConnectionString_IsNotEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(Configuration.TestConnectionString));
    }

    [Fact]
    public void ConnectionStrings_AreDifferent()
    {
        Assert.NotEqual(Configuration.ProductionConnectionString, Configuration.TestConnectionString);
    }

    [Fact]
    public void ConnectionStrings_ContainExpectedDatabases()
    {
        Assert.Contains("SmartTaskerMini", Configuration.ProductionConnectionString);
        Assert.Contains("TestDb", Configuration.TestConnectionString);
    }
}
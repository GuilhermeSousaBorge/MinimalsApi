using MinimalApi.Domain.Entities;
namespace Test.Domain.Entities;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void AdministratorTest()
    {
        var admin = new Administrator();
        //Act
        admin.Id = 1;
        admin.Email = "admin@teste.com";
        admin.Password = "123456";
        admin.Role = "Admin";
        //Assert
        Assert.AreEqual(1, admin.Id);
        Assert.AreEqual("admin@teste.com", admin.Email);
        Assert.AreEqual("123456", admin.Password);
        Assert.AreEqual("Admin", admin.Role);
    }
}


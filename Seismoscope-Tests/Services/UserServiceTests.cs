using Moq;
using Seismoscope.Enums;
using Seismoscope.Model.Interfaces;
using Seismoscope.Utils.Services;

namespace Seismoscope_Tests;

public class UserServiceTests
{
    private Mock<IUserDAL> _userDalMock;
    private UserService _userService;

    [SetUp]
    public void Setup()
    {
        _userDalMock = new Mock<IUserDAL>();
        _userService = new UserService(_userDalMock.Object);
    }

    [Test]
    public void Authentifier_WithValidCredentials_ReturnsUser()
    {

        var user = new User
        {
            Id = 1,
            Prenom = "Alice",
            Nom = "Bob",
            NomUtilisateur = "alice",
            MotDePasse = "hashé",
            Role = Role.Administrateur
        };

        _userDalMock.Setup(dal => dal.Authentifier("alice", "1234")).Returns(user);

        var result = _userService.Authentifier("alice", "1234");


        Assert.That(result, Is.Not.Null);
        Assert.That(result!.NomUtilisateur, Is.EqualTo("alice"));
    }

    [Test]
    public void Authentifier_WithInvalidCredentials_ReturnsNull()
    {

        _userDalMock.Setup(dal => dal.Authentifier("bob", "wrong")).Returns((User?)null);

        var result = _userService.Authentifier("bob", "wrong");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void Authentifier_Calls_IUserDAL_Once()
    {
        _userService.Authentifier("alice", "1234");

        _userDalMock.Verify(dal => dal.Authentifier("alice", "1234"), Times.Once);
    }
}

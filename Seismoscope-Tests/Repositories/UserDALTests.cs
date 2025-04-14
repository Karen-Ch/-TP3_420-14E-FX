using Microsoft.EntityFrameworkCore;
using Seismoscope.Enums;
using Seismoscope.Model.DAL;
using Seismoscope.Utils;

namespace Seismoscope_Tests;

public class UserDALTests
{
    private ApplicationDbContext _context;
    private UserDAL _userDAL;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_User")
            .EnableSensitiveDataLogging()
            .Options;

        _context = new ApplicationDbContext(options);
        _userDAL = new UserDAL(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public void Authentifier_WithCorrectCredentials_ReturnsUser()
    {
        
        var motDePasse = "123456";
        var user = new User
        {
            Prenom = "Alice",
            Nom = "Dupont",
            NomUtilisateur = "alice",
            MotDePasse = Securite.HasherMotDePasse(motDePasse),
            Role = Role.Administrateur,
            StationId = null
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        var result = _userDAL.Authentifier("alice", "123456");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Prenom, Is.EqualTo("Alice"));
    }

    [Test]
    public void Authentifier_WithIncorrectPassword_ReturnsNull()
    {
        var user = new User
        {
            Prenom = "Bob",
            Nom = "Martin",
            NomUtilisateur = "bob",
            MotDePasse = Securite.HasherMotDePasse("correct-password"),
            Role = Role.Employe,
            StationId = null
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        var result = _userDAL.Authentifier("bob", "wrong-password");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void Authentifier_WithUnknownUser_ReturnsNull()
    {
        var result = _userDAL.Authentifier("unknown", "any-password");

        Assert.That(result, Is.Null);
    }
}

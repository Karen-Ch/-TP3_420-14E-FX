using Microsoft.EntityFrameworkCore;
using Seismoscope.Enums;
using Seismoscope.Model.DAL;
using Seismoscope.Model;

namespace Seismoscope_Tests;

public class CapteurDALTests
{
    private ApplicationDbContext _context;
    private CapteurDAL _capteurDAL;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_Capteur")
            .EnableSensitiveDataLogging()
            .Options;

        _context = new ApplicationDbContext(options);
        _capteurDAL = new CapteurDAL(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public void CreateCapteur_WithEstLivreTrue_AddsCapteur()
    {
        var capteur = new Capteur
        {
            Nom = "Capteur1",
            Type = "Sismographe",
            Statut = Etat.Actif,
            FrequenceCollecte = 1.0,
            SeuilAlerte = 10.0,
            DateInstallation = DateTime.Now,
            EstDesactive = false,
            EstLivre = true,
            StationId = 1
        };

        _capteurDAL.CreateCapteur(capteur);

        var result = _context.Capteurs.FirstOrDefault(c => c.Nom == "Capteur1");
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Type, Is.EqualTo("Sismographe"));
    }

    [Test]
    public void DeleteCapteur_WithEstDesactiveTrue_RemovesCapteur()
    {
        var capteur = new Capteur
        {
            Nom = "ToDelete",
            Type = "Radar",
            Statut = Etat.HorsService,
            FrequenceCollecte = 0.5,
            SeuilAlerte = 5.0,
            DateInstallation = DateTime.Now,
            EstDesactive = true,
            EstLivre = true,
            StationId = 1
        };

        _context.Capteurs.Add(capteur);
        _context.SaveChanges();

        _capteurDAL.DeleteCapteur(capteur);

        var result = _context.Capteurs.FirstOrDefault(c => c.Nom == "ToDelete");
        Assert.That(result, Is.Null);
    }

    [Test]
    public void UpdateCapteur_WithValidCapteur_UpdatesSuccessfully()
    {
        var capteur = new Capteur
        {
            Nom = "CapteurUpdate",
            Type = "Radar",
            Statut = Etat.Actif,
            FrequenceCollecte = 2.0,
            SeuilAlerte = 20.0,
            DateInstallation = DateTime.Now,
            EstDesactive = false,
            EstLivre = true,
            StationId = 1
        };

        _context.Capteurs.Add(capteur);
        _context.SaveChanges();

        capteur.Type = "Thermique";
        _capteurDAL.UpdateCapteur(capteur);

        var result = _context.Capteurs.First(c => c.Nom == "CapteurUpdate");
        Assert.That(result.Type, Is.EqualTo("Thermique"));
    }

    [Test]
    public void FindById_ReturnsCorrectCapteur()
    {
        var capteur = new Capteur
        {
            Nom = "CapteurX",
            Type = "Radar",
            Statut = Etat.Actif,
            FrequenceCollecte = 1.0,
            SeuilAlerte = 5.0,
            DateInstallation = DateTime.Now,
            EstDesactive = false,
            EstLivre = true,
            StationId = 99
        };

        _context.Capteurs.Add(capteur);
        _context.SaveChanges();

        var result = _capteurDAL.FindById(capteur.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Nom, Is.EqualTo("CapteurX"));
    }

    [Test]
    public void FindAll_ReturnsCapteursForGivenStation()
    {
        var station = new Station
        {
            Nom = "Station1",
            Localisation = "Montreal",
            Code = "ST123",
            Responsable = "Jean",
            Capteurs = new List<Capteur>()
        };

        _context.Stations.Add(station);
        _context.SaveChanges();

        var capteurs = new List<Capteur>
            {
                new Capteur { Nom = "C1", Type = "Sismographe", EstDesactive = false, EstLivre = true, DateInstallation = DateTime.Now, FrequenceCollecte = 1, SeuilAlerte = 5, Statut = Etat.Actif, StationId = station.Id },
                new Capteur { Nom = "C2", Type = "Radar", EstDesactive = false, EstLivre = true, DateInstallation = DateTime.Now, FrequenceCollecte = 2, SeuilAlerte = 10, Statut = Etat.Actif, StationId = station.Id }
            };

        _context.Capteurs.AddRange(capteurs);
        _context.SaveChanges();

        var result = _capteurDAL.FindAll(station);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Any(c => c.Nom == "C1"), Is.True);
        Assert.That(result.Any(c => c.Nom == "C2"), Is.True);
    }
}

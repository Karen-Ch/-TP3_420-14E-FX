using Microsoft.EntityFrameworkCore;
using Seismoscope.Model.DAL;
using Seismoscope.Model;
using Moq;
using Seismoscope.Utils.Services.Interfaces;

namespace Seismoscope_Tests;

public class StationDALTests
{
    private ApplicationDbContext _context;
    private StationDAL _stationDAL;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Station")
            .EnableSensitiveDataLogging() 
            .Options;

        var ConfigService = new Mock<IConfigurationService>();
        ConfigService.Setup(c => c.GetDbPath()).Returns("chemin/test.db");
        _context = new ApplicationDbContext(options, ConfigService.Object);
        _stationDAL = new StationDAL(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public void Add_ValidStation_SavesToDatabase()
    {
        var station = new Station
        {
            Nom = "ST-Test",
            Localisation = "Testville",
            Code = "CODE001",
            Responsable = "Alice Dupont"
        };

        _stationDAL.Add(station);

        var result = _context.Stations.FirstOrDefault(s => s.Nom == "ST-Test");
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Localisation, Is.EqualTo("Testville"));
    }

    [Test]
    public void GetAll_ReturnsAllStations()
    {
        var stations = new List<Station>
            {
                new Station { Nom = "ST1", Localisation = "Ville 1", Code = "CODE002", Responsable = "Bob" },
                new Station { Nom = "ST2", Localisation = "Ville 2", Code = "CODE003", Responsable = "Charlie" }
            };

        _context.Stations.AddRange(stations);
        _context.SaveChanges();

        var result = _stationDAL.GetAll();

        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public void FindAll_ReturnsStationsWithCapteurs()
    {
        var station = new Station
        {
            Nom = "ST-Inclusive",
            Localisation = "Incluville",
            Code = "CODE004",
            Responsable = "Diane",
            Capteurs = new List<Capteur>
                {
                    new Capteur { Nom = "C1", Type = "Sismographe" }
                }
        };

        _context.Stations.Add(station);
        _context.SaveChanges();

        var result = _stationDAL.FindAll();

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Capteurs.Count, Is.EqualTo(1));
        Assert.That(result[0].Capteurs.First().Nom, Is.EqualTo("C1"));
    }

    [Test]
    public void FindById_ReturnsCorrectStation()
    {
        var station = new Station
        {
            Nom = "ST-Unique",
            Localisation = "Solo",
            Code = "CODE005",
            Responsable = "Émilie",
            Capteurs = new List<Capteur>
                {
                    new Capteur { Nom = "UniqueCapteur", Type = "Radar" }
                }
        };

        _context.Stations.Add(station);
        _context.SaveChanges();

        var result = _stationDAL.FindById(station.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Nom, Is.EqualTo("ST-Unique"));
        Assert.That(result.Capteurs.Count, Is.EqualTo(1));
        Assert.That(result.Capteurs.First().Nom, Is.EqualTo("UniqueCapteur"));
    }
}
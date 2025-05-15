using Microsoft.EntityFrameworkCore;
using Seismoscope.Model.DAL;
using Seismoscope.Model;
using Seismoscope.Enums;
using System;
using Moq;
using Seismoscope.Utils.Services.Interfaces;

namespace Seismoscope_Tests;

public class EvenementDALTests
{
    private ApplicationDbContext _context;
    private EvenementDAL _evenementDal;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb_Evenement")
            .EnableSensitiveDataLogging()
            .Options;

        var ConfigService = new Mock<IConfigurationService>();
        ConfigService.Setup(c => c.GetDbPath()).Returns("chemin/test.db");
        _context = new ApplicationDbContext(options, ConfigService.Object);
        _evenementDal = new EvenementDAL(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public void AjouterEvenement_AjouteCorrectement()
    {
        var evenement = new EvenementSismique
        {
            DateEvenement = DateTime.Now,
            TypeOnde = TypeOnde.P,
            Amplitude = 12.5,
            SeuilAtteint = 10.0,
            StationId = 1
        };

        _evenementDal.AjouterEvenement(evenement);

        var result = _context.Evenements.FirstOrDefault(e => e.StationId == 1);
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Amplitude, Is.EqualTo(12.5));
    }

    [Test]
    public void ObtenirEvenementsParStation_RetourneEvenements()
    {
        var evenements = new List<EvenementSismique>
            {
            new EvenementSismique
                {
                    DateEvenement = DateTime.Now,
                    TypeOnde = TypeOnde.S,
                    Amplitude = 9.2,
                    SeuilAtteint = 8.0,
                    StationId = 2
                },
            new EvenementSismique
            {
                    DateEvenement = DateTime.Now,
                    TypeOnde = TypeOnde.Surface,
                    Amplitude = 11.7,
                    SeuilAtteint = 9.0,
                    StationId = 2
                }
            };

        _context.Evenements.AddRange(evenements);
        _context.SaveChanges();

        var result = _evenementDal.ObtenirEvenementsParStation(2);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.All(e => e.StationId == 2), Is.True);
    }
}

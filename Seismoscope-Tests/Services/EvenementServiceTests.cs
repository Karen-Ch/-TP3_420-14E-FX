using Moq;
using Seismoscope.Model.Interfaces;
using Seismoscope.Model;
using Seismoscope.Utils.Services;

namespace Seismoscope_Tests;

public class EvenementServiceTests
{
    private Mock<IEvenementDAL> _evenementDalMock;
    private EvenementService _evenementService;

    [SetUp]
    public void Setup()
    {
        _evenementDalMock = new Mock<IEvenementDAL>();
        _evenementService = new EvenementService(_evenementDalMock.Object);
    }

    [Test]
    public void ObtenirParStation_RetourneResultats()
    {
        var evenements = new List<EvenementSismique>
            {
                new EvenementSismique { Id = 1, Amplitude = 5.5 },
                new EvenementSismique { Id = 2, Amplitude = 6.2 }
            };
        _evenementDalMock.Setup(dal => dal.ObtenirEvenementsParStation(1)).Returns(evenements);

        var result = _evenementService.ObtenirParStation(1);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].Amplitude, Is.EqualTo(5.5));
    }

    [Test]
    public void AjouterEvenement_AppelleDAL()
    {
        var evenement = new EvenementSismique { Amplitude = 7.1 };

        _evenementService.AjouterEvenement(evenement);

        _evenementDalMock.Verify(dal => dal.AjouterEvenement(evenement), Times.Once);
    }
}

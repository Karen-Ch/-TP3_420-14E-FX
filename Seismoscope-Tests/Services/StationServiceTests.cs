using Moq;
using Seismoscope.Model.Interfaces;
using Seismoscope.Model;
using Seismoscope.Utils.Services;

namespace Seismoscope_Tests;

public class StationServiceTests
{
    private Mock<IStationDAL> _stationDalMock;
    private StationService _stationService;

    [SetUp]
    public void Setup()
    {
        _stationDalMock = new Mock<IStationDAL>();
        _stationService = new StationService(_stationDalMock.Object);
    }

    [Test]
    public void ObtenirParId_WithValidId_ReturnsStation()
    {
        var station = new Station { Id = 1, Nom = "ST-Test", Localisation = "Ville" };

        _stationDalMock.Setup(dal => dal.FindById(1)).Returns(station);

        var result = _stationService.ObtenirParId(1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Nom, Is.EqualTo("ST-Test"));
    }

    [Test]
    public void ObtenirParId_WithInvalidId_ReturnsNull()
    {
        _stationDalMock.Setup(dal => dal.FindById(99)).Returns((Station?)null);

        var result = _stationService.ObtenirParId(99);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ObtenirToutesAvecCapteurs_ReturnsList()
    {
        var stations = new List<Station>
            {
                new Station { Id = 1, Nom = "ST1", Localisation = "A" },
                new Station { Id = 2, Nom = "ST2", Localisation = "B" }
            };

        _stationDalMock.Setup(dal => dal.FindAll()).Returns(stations);

        var result = _stationService.ObtenirToutesAvecCapteurs();

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].Nom, Is.EqualTo("ST1"));
    }
}

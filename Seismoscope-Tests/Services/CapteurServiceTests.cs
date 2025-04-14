using Moq;
using Seismoscope.Model.Interfaces;
using Seismoscope.Model;
using Seismoscope.Utils.Services;

namespace Seismoscope_Tests;

public class CapteurServiceTests
{
    private Mock<ICapteurDAL> _capteurDalMock;
    private CapteurService _capteurService;

    [SetUp]
    public void Setup()
    {
        _capteurDalMock = new Mock<ICapteurDAL>();
        _capteurService = new CapteurService(_capteurDalMock.Object);
    }

    [Test]
    public void ObtenirParId_WithValidId_ReturnsCapteur()
    {
        var capteur = new Capteur { Id = 1, Nom = "C1", Type = "Radar" };
        _capteurDalMock.Setup(dal => dal.FindById(1)).Returns(capteur);

        var result = _capteurService.ObtenirParId(1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Nom, Is.EqualTo("C1"));
    }

    [Test]
    public void ObtenirTous_ReturnsList()
    {
        var capteurs = new List<Capteur>
            {
                new Capteur { Id = 1, Nom = "C1", Type = "Radar" },
                new Capteur { Id = 2, Nom = "C2", Type = "Thermique" }
            };
        _capteurDalMock.Setup(dal => dal.GetAll()).Returns(capteurs);

        var result = _capteurService.ObtenirTous();

        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public void ObtenirParStation_ReturnsCapteurs()
    {
        var station = new Station { Id = 1, Nom = "Station A" };
        var capteurs = new List<Capteur> { new Capteur { Nom = "C1", StationId = 1 } };

        _capteurDalMock.Setup(dal => dal.FindAll(station)).Returns(capteurs);

        var result = _capteurService.ObtenirParStation(station);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Nom, Is.EqualTo("C1"));
    }

    [Test]
    public void AjouterCapteur_CallsCreateCapteur()
    {
        var capteur = new Capteur { Nom = "Nouveau", EstLivre = true };
        _capteurService.AjouterCapteur(capteur);

        _capteurDalMock.Verify(dal => dal.CreateCapteur(capteur), Times.Once);
    }

    [Test]
    public void SupprimerCapteur_CallsDeleteCapteur()
    {
        var capteur = new Capteur { Nom = "Obsolete", EstDesactive = true };
        _capteurService.SupprimerCapteur(capteur);

        _capteurDalMock.Verify(dal => dal.DeleteCapteur(capteur), Times.Once);
    }

    [Test]
    public void ModifierCapteur_CallsUpdateCapteur()
    {
        var capteur = new Capteur { Nom = "Modifié" };
        _capteurService.ModifierCapteur(capteur);

        _capteurDalMock.Verify(dal => dal.UpdateCapteur(capteur), Times.Once);
    }
}

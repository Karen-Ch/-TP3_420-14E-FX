using Moq;
using Seismoscope.Model;
using Seismoscope.Utils.Services.Interfaces.Seismoscope.Services.Interfaces;
using Seismoscope.Utils.Services.Interfaces;
using Seismoscope.Utils.Services;

namespace Seismoscope_Tests;

public class AjustementServiceTests
{
    private Mock<ICapteurService> _capteurServiceMock;
    private Mock<IJournalService> _journalServiceMock;
    private AjustementService _ajustementService;
    private Capteur _capteur;

    [SetUp]
    public void Setup()
    {
        _capteurServiceMock = new Mock<ICapteurService>();
        _journalServiceMock = new Mock<IJournalService>();
        _ajustementService = new AjustementService(_capteurServiceMock.Object, _journalServiceMock.Object);

        _capteur = new Capteur
        {
            Id = 1,
            SeuilAlerte = 50,
            SeuilMin = 30,
            SeuilMax = 100,
            FrequenceCollecte = 30,
            FrequenceMinimale = 5,
            FrequenceParDefaut = 30
        };
    }

    [Test]
    public void HausseSeuil_AmplitudeSuperieure130Pourcent_SeuilAugmente()
    {
        var lectures = new List<double> { 70 }; 

        _ajustementService.AppliquerRegles(_capteur, lectures);

        Assert.That(_capteur.SeuilAlerte, Is.EqualTo(55).Within(0.01));
        _journalServiceMock.Verify(j => j.Enregistrer(1, It.Is<string>(s => s.Contains("Hausse")), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void ReductionSeuil_CinqLecturesProches_Success()
    {
        var lectures = new List<double> { 45, 46, 47, 48, 49 };

        _ajustementService.AppliquerRegles(_capteur, lectures);

        Assert.That(_capteur.SeuilAlerte, Is.EqualTo(45)); 
        _journalServiceMock.Verify(j => j.Enregistrer(1, It.Is<string>(s => s.Contains("Réduction")), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void AugmenterFrequence_AmplitudeSuperieure160Pourcent_FrequenceDiminue()
    {
        var capteur = new Capteur
        {
            Id = 1,
            SeuilAlerte = 50,
            FrequenceCollecte = 30,
            FrequenceMinimale = 5,
            FrequenceParDefaut = 30
        };

        var lectures = new List<double> { 85 };

        var capteurServiceMock = new Mock<ICapteurService>();
        var journalServiceMock = new Mock<IJournalService>();
        var service = new AjustementService(capteurServiceMock.Object, journalServiceMock.Object);

        service.AppliquerRegles(capteur, lectures);

        Assert.That(capteur.FrequenceCollecte, Is.EqualTo(27).Within(0.01));
        journalServiceMock.Verify(j => j.Enregistrer(1,
            It.Is<string>(s => s.Contains("fréquence")), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void ReinitialiserFrequence_AucuneActiviteDepuis2Min_FrequenceParDefaut()
    {
        _capteur.FrequenceCollecte = 10;
        _capteur.DernierEvenement = DateTime.Now.AddMinutes(-3);

        var lectures = new List<double> { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };

        _ajustementService.AppliquerRegles(_capteur, lectures);

        Assert.That(_capteur.FrequenceCollecte, Is.EqualTo(_capteur.FrequenceParDefaut));
        _journalServiceMock.Verify(j => j.Enregistrer(1, It.Is<string>(s => s.Contains("Réinitialisation")), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void AucuneRegleDeclenchee_AucuneModification()
    {
        var lectures = new List<double> { 20, 25, 30 };

        _ajustementService.AppliquerRegles(_capteur, lectures);

        Assert.That(_capteur.SeuilAlerte, Is.EqualTo(50));
        Assert.That(_capteur.FrequenceCollecte, Is.EqualTo(30));
        _journalServiceMock.Verify(j => j.Enregistrer(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}

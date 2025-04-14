using Moq;
using Seismoscope.Enums;
using Seismoscope.Model;
using Seismoscope.Utils.Services.Interfaces;
using Seismoscope.ViewModel;

namespace Seismoscope_Tests;

public class CapteurViewModelTests
{
    private Mock<IUserSessionService> _userSessionMock;
    private Mock<ICapteurService> _capteurServiceMock;
    private Mock<INavigationService> _navigationServiceMock;
    private Mock<IDialogService> _dialogServiceMock;

    private CapteurViewModel _viewModel;
    private Mock<Capteur> _capteurMock;

    [SetUp]
    public void Setup()
    {
        _userSessionMock = new Mock<IUserSessionService>();
        _capteurServiceMock = new Mock<ICapteurService>();
        _navigationServiceMock = new Mock<INavigationService>();
        _dialogServiceMock = new Mock<IDialogService>();

        _viewModel = new CapteurViewModel(
            _userSessionMock.Object,
            _capteurServiceMock.Object,
            _navigationServiceMock.Object,
            _dialogServiceMock.Object
        );

        _capteurMock = new Mock<Capteur>();
        _capteurMock.SetupAllProperties();
        _capteurMock.Object.Id = 1;
        _capteurMock.Object.Nom = "C1";
        _capteurMock.Object.Type = "Radar";
        _capteurMock.Object.Statut = Etat.Actif;
        _capteurMock.Object.FrequenceCollecte = 10;
        _capteurMock.Object.SeuilAlerte = 5;
        _capteurMock.Object.DateInstallation = DateTime.Today;
        _capteurMock.Object.EstDesactive = false;
        _capteurMock.Object.EstLivre = true;
        _capteurMock.Object.StationId = 42;
    }

    [Test]
    public void Receive_CapteurExiste_PopuleLesProprietes()
    {
        _capteurServiceMock.Setup(s => s.ObtenirParId(1)).Returns(_capteurMock.Object);

        _viewModel.Receive(1);

        Assert.That(_viewModel.Id, Is.EqualTo(1));
        Assert.That(_viewModel.Nom, Is.EqualTo("C1"));
        Assert.That(_viewModel.Type, Is.EqualTo("Radar"));
        Assert.That(_viewModel.Statut, Is.EqualTo(Etat.Actif));
        Assert.That(_viewModel.FrequenceCollecte, Is.EqualTo(10));
        Assert.That(_viewModel.SeuilAlerte, Is.EqualTo(5));
        Assert.That(_viewModel.EstDesactive, Is.False);
        Assert.That(_viewModel.EstLivre, Is.True);
        Assert.That(_viewModel.StationId, Is.EqualTo(42));
    }

    [Test]
    public void ToggleActivation_InverseEtatEtEnregistre()
    {
        _capteurServiceMock.Setup(s => s.ObtenirParId(1)).Returns(_capteurMock.Object);
        _viewModel.Id = 1;
        _viewModel.EstDesactive = false;

        _viewModel.ToggleActivationCommand.Execute(null);

        Assert.That(_viewModel.EstDesactive, Is.True);
        _capteurServiceMock.Verify(s => s.ModifierCapteur(It.Is<Capteur>(c => c.EstDesactive == true)), Times.Once);
        _dialogServiceMock.Verify(d => d.ShowMessage("Statut du capteur mis à jour.", "Succès"), Times.Once);
        _navigationServiceMock.Verify(n => n.NavigateTo<StationViewModel>(), Times.Once);
    }

    [Test]
    public void SauvegarderModifications_FrequenceInvalide_AfficheErreur()
    {
        _viewModel.FrequenceCollecte = 0;
        _viewModel.SeuilAlerte = 10;

        _viewModel.SauvegarderModificationsCommand.Execute(null);

        _dialogServiceMock.Verify(d => d.ShowMessage("La fréquence de collecte doit être un nombre positif.", "Valeur invalide"), Times.Once);
        _capteurServiceMock.Verify(s => s.ModifierCapteur(It.IsAny<Capteur>()), Times.Never);
    }

    [Test]
    public void SauvegarderModifications_SeuilInvalide_AfficheErreur()
    {
        _viewModel.FrequenceCollecte = 10;
        _viewModel.SeuilAlerte = 0;

        _viewModel.SauvegarderModificationsCommand.Execute(null);

        _dialogServiceMock.Verify(d => d.ShowMessage("Le seuil d’alerte doit être un nombre positif.", "Valeur invalide"), Times.Once);
        _capteurServiceMock.Verify(s => s.ModifierCapteur(It.IsAny<Capteur>()), Times.Never);
    }

    [Test]
    public void SauvegarderModifications_Valide_ModifieCapteurEtNavigue()
    {
        _viewModel.Id = 1;
        _viewModel.FrequenceCollecte = 25;
        _viewModel.SeuilAlerte = 6;
        _capteurServiceMock.Setup(s => s.ObtenirParId(1)).Returns(_capteurMock.Object);

        _viewModel.SauvegarderModificationsCommand.Execute(null);

        _capteurServiceMock.Verify(s => s.ModifierCapteur(It.Is<Capteur>(
            c => c.FrequenceCollecte == 25 && c.SeuilAlerte == 6
        )), Times.Once);

        _dialogServiceMock.Verify(d => d.ShowMessage("Paramètres mis à jour avec succès.", "Succès"), Times.Once);
        _navigationServiceMock.Verify(n => n.NavigateTo<StationViewModel>(), Times.Once);
    }
}

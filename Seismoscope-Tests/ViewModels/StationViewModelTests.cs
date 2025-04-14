using Moq;
using Seismoscope.Enums;
using Seismoscope.Model;
using Seismoscope.Utils.Services.Interfaces;
using Seismoscope.ViewModel;

namespace Seismoscope_Tests;

public class StationViewModelTests
{
    private Mock<IUserSessionService> _userSessionMock;
    private Mock<IStationService> _stationServiceMock;
    private Mock<ICapteurService> _capteurServiceMock;
    private Mock<INavigationService> _navigationServiceMock;
    private Mock<IDialogService> _dialogServiceMock;

    private StationViewModel _viewModel;
    private Mock<User> _userMock;
    private Mock<Station> _stationMock;
    private Mock<Capteur> _capteurMock;

    [SetUp]
    public void Setup()
    {
        _userMock = new Mock<User>();
        _userMock.SetupAllProperties();
        _userMock.Object.Prenom = "Test";
        _userMock.Object.Nom = "User";
        _userMock.Object.StationId = 1;

        _stationMock = new Mock<Station>();
        _stationMock.SetupAllProperties();
        _stationMock.Object.Id = 1;
        _stationMock.Object.Nom = "Station1";
        _stationMock.Object.Localisation = "Ici";
        _stationMock.Object.Etat = Etat.Actif;
        _stationMock.Object.Capteurs = new List<Capteur>();

        _capteurMock = new Mock<Capteur>();
        _capteurMock.SetupAllProperties();
        _capteurMock.Object.Nom = "CapteurTest";
        _capteurMock.Object.EstLivre = true;
        _capteurMock.Object.DateInstallation = default;
        _capteurMock.Object.StationId = null;

        _userSessionMock = new Mock<IUserSessionService>();
        _userSessionMock.Setup(u => u.ConnectedUser).Returns(_userMock.Object);

        _stationServiceMock = new Mock<IStationService>();
        _stationServiceMock.Setup(s => s.ObtenirParId(It.IsAny<int>())).Returns(_stationMock.Object);

        _capteurServiceMock = new Mock<ICapteurService>();
        _capteurServiceMock.Setup(c => c.ObtenirTous()).Returns(new List<Capteur> { _capteurMock.Object });

        _navigationServiceMock = new Mock<INavigationService>();
        _dialogServiceMock = new Mock<IDialogService>();

        _viewModel = new StationViewModel(
            _userSessionMock.Object,
            _stationServiceMock.Object,
            _navigationServiceMock.Object,
            _capteurServiceMock.Object,
            _dialogServiceMock.Object
        );
    }

    [Test]
    public void AjouterCapteurCommand_CapteurValide_AjouteEtAfficheMessage()
    {
        _viewModel.NouvelleStation = _stationMock.Object;
        _viewModel.CapteurSelectionne = _capteurMock.Object;

        _viewModel.AjouterCapteurCommand.Execute(null);

        _capteurServiceMock.Verify(s => s.ModifierCapteur(_capteurMock.Object), Times.Once);
        _dialogServiceMock.Verify(d => d.ShowMessage("Capteur ajouté !", "Succès"), Times.Once);
        Assert.That(_viewModel.Capteurs.Count, Is.EqualTo(1));
    }

    [Test]
    public void AjouterCapteurCommand_CapteurNull_AfficheErreur()
    {
        _viewModel.CapteurSelectionne = null;

        _viewModel.AjouterCapteurCommand.Execute(null);

        _dialogServiceMock.Verify(d => d.ShowMessage("Veuillez sélectionner un capteur.", "Erreur"), Times.Once);
    }

    [Test]
    public void AjouterCapteurCommand_CapteurNonLivre_AfficheErreur()
    {
        _capteurMock.Object.EstLivre = false;
        _viewModel.CapteurSelectionne = _capteurMock.Object;

        _viewModel.AjouterCapteurCommand.Execute(null);

        _dialogServiceMock.Verify(d => d.ShowMessage("Ce capteur n'est pas livrable.", "Erreur"), Times.Once);
    }

    [Test]
    public void AjouterCapteurCommand_StationCibleNull_AfficheErreur()
    {
        _viewModel.CapteurSelectionne = _capteurMock.Object;
        _viewModel.NouvelleStation = null;

        _viewModel.AjouterCapteurCommand.Execute(null);

        _dialogServiceMock.Verify(d => d.ShowMessage("Capteur ajouté !", "Succès"), Times.Once); 
    }

    [Test]
    public void EffacerCapteurCommand_Confirmation_OkEtCapteurSupprimé()
    {
        _capteurMock.Object.EstDesactive = true;
        _dialogServiceMock.Setup(d => d.ShowConfirmation(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        _viewModel.Capteurs.Add(_capteurMock.Object);
        _viewModel.TousCapteurs.Add(_capteurMock.Object);

        _viewModel.EffacerCapteurCommand.Execute(_capteurMock.Object);

        _capteurServiceMock.Verify(s => s.SupprimerCapteur(_capteurMock.Object), Times.Once);
        Assert.That(_viewModel.Capteurs.Contains(_capteurMock.Object), Is.False);
    }

    [Test]
    public void EffacerCapteurCommand_CapteurActif_MessageErreur()
    {
        _capteurMock.Object.EstDesactive = false;
        _dialogServiceMock.Setup(d => d.ShowConfirmation(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        _viewModel.EffacerCapteurCommand.Execute(_capteurMock.Object);

        _dialogServiceMock.Verify(d => d.ShowMessage(It.Is<string>(msg => msg.Contains("ne peut pas être supprimé")), "Information"), Times.Once);
    }
}
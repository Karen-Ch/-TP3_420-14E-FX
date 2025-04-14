using Moq;
using Seismoscope.Enums;
using Seismoscope.Model;
using Seismoscope.Utils.Services.Interfaces;
using Seismoscope.ViewModel;

namespace Seismoscope_Tests;

public class AjouterCapteurViewModelTests
{
    private Mock<IUserSessionService> _userSessionMock;
    private Mock<IStationService> _stationServiceMock;
    private Mock<ICapteurService> _capteurServiceMock;
    private Mock<INavigationService> _navigationMock;
    private Mock<IDialogService> _dialogServiceMock;

    private AjouterCapteurViewModel _viewModel;

    [SetUp]
    public void Setup()
    {
        var capteurMock = new Mock<Capteur>();
        capteurMock.SetupAllProperties();
        capteurMock.Object.EstLivre = true;

        var stationMock = new Mock<Station>();
        stationMock.SetupAllProperties();
        stationMock.Object.Id = 99;

        var userMock = new Mock<User>();
        userMock.SetupAllProperties();
        userMock.Object.StationId = 99;

        _userSessionMock = new Mock<IUserSessionService>();
        _userSessionMock.SetupGet(s => s.ConnectedUser).Returns(userMock.Object);

        _stationServiceMock = new Mock<IStationService>();
        _stationServiceMock.Setup(s => s.ObtenirParId(99)).Returns(stationMock.Object);

        _capteurServiceMock = new Mock<ICapteurService>();
        _capteurServiceMock.Setup(s => s.ObtenirTous()).Returns(new List<Capteur> { capteurMock.Object });

        _navigationMock = new Mock<INavigationService>();
        _dialogServiceMock = new Mock<IDialogService>();

        _viewModel = new AjouterCapteurViewModel(
            _userSessionMock.Object,
            _navigationMock.Object,
            _capteurServiceMock.Object,
            _stationServiceMock.Object,
            _dialogServiceMock.Object
        );
    }

    [Test]
    public void ValiderCapteurCommand_AvecCapteurLivre_AppelleModifierCapteurEtAfficheMessage()
    {
        var capteurMock = new Mock<Capteur>();
        capteurMock.SetupAllProperties();
        capteurMock.Object.EstLivre = true;

        _viewModel.CapteurSelectionne = capteurMock.Object;

        _viewModel.ValiderCapteurCommand.Execute(null);

        _capteurServiceMock.Verify(s => s.ModifierCapteur(capteurMock.Object), Times.Once);
        _dialogServiceMock.Verify(d => d.ShowMessage("Capteur ajouté !", ""), Times.Once);
        Assert.That(capteurMock.Object.StationId, Is.EqualTo(99));
    }


    [Test]
    public void ValiderCapteurCommand_SansSelection_AfficheMessageCapteurNonLivre()
    {
        _viewModel.CapteurSelectionne = null;

        _viewModel.ValiderCapteurCommand.Execute(null);

        _capteurServiceMock.Verify(s => s.ModifierCapteur(It.IsAny<Capteur>()), Times.Never);
        _dialogServiceMock.Verify(d => d.ShowMessage("Veuillez sélectionner un capteur livré.", ""), Times.Once);
    }



    [Test]
    public void ValiderCapteurCommand_CapteurNonLivre_AfficheMessageErreur()
    {
        var capteurMock = new Mock<Capteur>();
        capteurMock.SetupAllProperties();
        capteurMock.Object.EstLivre = false;

        _viewModel.CapteurSelectionne = capteurMock.Object;

        _viewModel.ValiderCapteurCommand.Execute(null);

        _capteurServiceMock.Verify(s => s.ModifierCapteur(It.IsAny<Capteur>()), Times.Never);
        _dialogServiceMock.Verify(d => d.ShowMessage("Veuillez sélectionner un capteur livré.", ""), Times.Once);
    }



}

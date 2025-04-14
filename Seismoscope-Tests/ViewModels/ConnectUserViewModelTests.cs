using Moq;
using Seismoscope.Enums;
using Seismoscope.Utils.Services.Interfaces;
using Seismoscope.ViewModel;

namespace Seismoscope_Tests;

public class ConnectUserViewModelTests
{
    private Mock<IUserService> _userServiceMock;
    private Mock<IUserSessionService> _sessionMock;
    private Mock<INavigationService> _navigationMock;

    private ConnectUserViewModel _viewModel;
    private Mock<User> _userMock;

    [SetUp]
    public void Setup()
    {
        _userMock = new Mock<User>();
        _userMock.SetupAllProperties();
        _userMock.Object.Role = Role.Employe;
        _userMock.Object.StationId = 42;

        _userServiceMock = new Mock<IUserService>();
        _sessionMock = new Mock<IUserSessionService>();
        _navigationMock = new Mock<INavigationService>();

        _viewModel = new ConnectUserViewModel(
            _userServiceMock.Object,
            _navigationMock.Object,
            _sessionMock.Object
        );
    }

    [Test]
    public void ConnectCommand_UtilisateurValideNavigueVersStation()
    {
        _viewModel.NomUtilisateur = "test";
        _viewModel.MotDePasse = "password";
        _userServiceMock.Setup(s => s.Authentifier("test", "password")).Returns(_userMock.Object);

        _viewModel.ConnectCommand.Execute(null);

        _sessionMock.VerifySet(s => s.ConnectedUser = _userMock.Object, Times.Once);
        _navigationMock.Verify(n => n.NavigateTo<StationViewModel>(42), Times.Once);
    }

    [Test]
    public void ConnectCommand_AdministrateurNavigueVersCarte()
    {
        _viewModel.NomUtilisateur = "admin";
        _viewModel.MotDePasse = "secure";
        _userMock.Object.Role = Role.Administrateur;

        _userServiceMock.Setup(s => s.Authentifier("admin", "secure")).Returns(_userMock.Object);

        _viewModel.ConnectCommand.Execute(null);

        _navigationMock.Verify(n => n.NavigateTo<CarteViewModel>(_userMock.Object), Times.Once);
    }

    [Test]
    public void ConnectCommand_UtilisateurInvalideAjouteErreur()
    {
        _viewModel.NomUtilisateur = "fake";
        _viewModel.MotDePasse = "wrong";
        _userServiceMock.Setup(s => s.Authentifier(It.IsAny<string>(), It.IsAny<string>())).Returns((User?)null);

        _viewModel.ConnectCommand.Execute(null);

        var erreurs = _viewModel.GetErrors(nameof(_viewModel.MotDePasse))?.Cast<string>().ToList();
        Assert.That(erreurs, Is.Not.Null);
        Assert.That(erreurs.First(), Is.EqualTo("Nom d'utilisateur ou mot de passe invalide."));
    }

    [Test]
    public void ValidateProperty_ChampsInvalidesAjouteErreurs()
    {
        _viewModel.NomUtilisateur = "";
        _viewModel.MotDePasse = "";

        var erreursNom = _viewModel.GetErrors(nameof(_viewModel.NomUtilisateur))?.Cast<string>().ToList();
        var erreursMdp = _viewModel.GetErrors(nameof(_viewModel.MotDePasse))?.Cast<string>().ToList();

        Assert.Multiple(() =>
        {
            Assert.That(erreursNom, Is.Not.Null);
            Assert.That(erreursNom.Any(), Is.True);
            Assert.That(erreursNom.First(), Is.EqualTo("Le nom d'utilisateur est requis."));

            Assert.That(erreursMdp, Is.Not.Null);
            Assert.That(erreursMdp.Any(), Is.True);
            Assert.That(erreursMdp.First(), Is.EqualTo("Le mot de passe est requis."));
        });
    }

    [Test]
    public void CanConnect_RetourneFalseSiErreurOuChampsVides()
    {
        _viewModel.NomUtilisateur = "";
        _viewModel.MotDePasse = "";

        Assert.That(_viewModel.ConnectCommand.CanExecute(null), Is.False);
    }
}
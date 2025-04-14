using Moq;
using Seismoscope.Enums;
using Seismoscope.Model;
using Seismoscope.Utils.Services.Interfaces;
using Seismoscope.ViewModel;
using System.Windows.Media;

namespace Seismoscope_Tests;

public class CarteViewModelTests
{
    private Mock<IUserSessionService> _userSessionMock;
    private Mock<IStationService> _stationServiceMock;
    private Mock<INavigationService> _navigationServiceMock;

    private CarteViewModel _viewModel;

    private Mock<User> _userMock;
    private Mock<Station> _station1Mock;
    private Mock<Station> _station2Mock;
    private Mock<Station> _station3Mock;

    [SetUp]
    public void Setup()
    {
        _userSessionMock = new Mock<IUserSessionService>();
        _stationServiceMock = new Mock<IStationService>();
        _navigationServiceMock = new Mock<INavigationService>();

        // Mock User
        _userMock = new Mock<User>();
        _userMock.SetupAllProperties();
        _userMock.Object.Prenom = "Alice";
        _userMock.Object.Nom = "Bob";
        _userSessionMock.Setup(s => s.ConnectedUser).Returns(_userMock.Object);

        // Mock Stations
        _station1Mock = new Mock<Station>();
        _station1Mock.SetupAllProperties();
        _station1Mock.Object.Id = 1;
        _station1Mock.Object.Etat = Etat.Actif;

        _station2Mock = new Mock<Station>();
        _station2Mock.SetupAllProperties();
        _station2Mock.Object.Id = 2;
        _station2Mock.Object.Etat = Etat.Maintenance;

        _station3Mock = new Mock<Station>();
        _station3Mock.SetupAllProperties();
        _station3Mock.Object.Id = 3;
        _station3Mock.Object.Etat = Etat.HorsService;

        var stations = new List<Station>
            {
                _station1Mock.Object,
                _station2Mock.Object,
                _station3Mock.Object
            };

        _stationServiceMock.Setup(s => s.ObtenirToutesAvecCapteurs()).Returns(stations);

        // Init ViewModel
        _viewModel = new CarteViewModel(
            _userSessionMock.Object,
            _navigationServiceMock.Object,
            _stationServiceMock.Object
        );
    }

    [Test]
    public void MessageDeBienvenue_EstCorrect()
    {
        Assert.That(_viewModel.Message, Is.EqualTo("Bienvenue Alice Bob sur la carte interactive !"));
    }

    [Test]
    public void ChargerStations_RemplieDictionnaireAvecBonneTailleEtClignotement()
    {
        var result = _viewModel.StationsInfo;

        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result[1].EtatMessage, Is.EqualTo("Station active"));
        Assert.That(result[2].EtatMessage, Is.EqualTo("En maintenance"));
        Assert.That(result[3].Clignote, Is.True);
    }

    [Test]
    public void StationSelectedCommand_NavigateToStationViewModel()
    {
        _viewModel.StationSelectedCommand.Execute(42);

        _navigationServiceMock.Verify(n => n.NavigateTo<StationViewModel>(42), Times.Once);
    }

    [Test]
    public void CouleurEtatCorrespondAEtatStation()
    {
        var affichage = _viewModel.StationsInfo[1];
        var brush = affichage.CouleurEtat as SolidColorBrush;
        Assert.That(brush.Color, Is.EqualTo((Color)ColorConverter.ConvertFromString("#27ae60")));

        affichage = _viewModel.StationsInfo[2];
        brush = affichage.CouleurEtat as SolidColorBrush;
        Assert.That(brush.Color, Is.EqualTo((Color)ColorConverter.ConvertFromString("#f39c12")));

        affichage = _viewModel.StationsInfo[3];
        brush = affichage.CouleurEtat as SolidColorBrush;
        Assert.That(brush.Color, Is.EqualTo((Color)ColorConverter.ConvertFromString("#c0392b")));
    }
}
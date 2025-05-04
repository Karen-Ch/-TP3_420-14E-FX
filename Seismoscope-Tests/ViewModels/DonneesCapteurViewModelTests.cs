using Moq;
using Seismoscope.Enums;
using Seismoscope.Model;
using Seismoscope.Utils.Services.Interfaces;
using Seismoscope.ViewModel;
using System.Collections.ObjectModel;

namespace Seismoscope_Tests;
[Apartment(System.Threading.ApartmentState.STA)]
public class DonnesCapteurViewModelTests
{
    private Mock<IUserSessionService> _userSessionMock;
    private Mock<INavigationService> _navigationMock;
    private Mock<ICapteurService> _capteurServiceMock;
    private Mock<IStationService> _stationServiceMock;
    private Mock<IDialogService> _dialogServiceMock;
    private Mock<IEvenementService> _evenementServiceMock;

    private DonneesCapteurViewModel _viewModel;
    private Capteur _capteurTest;

    [SetUp]
    public void Setup()
    {
        _capteurTest = new Capteur
        {
            Id = 1,
            Nom = "CapteurTest",
            FrequenceCollecte = 1,
            SeuilAlerte = 10,
            StationId = 2
        };

        _userSessionMock = new Mock<IUserSessionService>();
        _navigationMock = new Mock<INavigationService>();
        _capteurServiceMock = new Mock<ICapteurService>();
        _stationServiceMock = new Mock<IStationService>();
        _dialogServiceMock = new Mock<IDialogService>();
        _evenementServiceMock = new Mock<IEvenementService>();
        _capteurServiceMock.Setup(c => c.ObtenirTous()).Returns(new List<Capteur> { _capteurTest });

        _viewModel = new DonneesCapteurViewModel(
            _userSessionMock.Object,
            _navigationMock.Object,
            _capteurServiceMock.Object,
            _stationServiceMock.Object,
            _dialogServiceMock.Object,
            _evenementServiceMock.Object
        );
    }

    [Test]
    public void ArreterLecture_Sets_EstLectureEnCours_To_False()
    {
        _viewModel.EstLectureEnCours = true;

        _viewModel.ArreterLecture();

        Assert.IsFalse(_viewModel.EstLectureEnCours);
    }

    [Test]
    public void PeutCommencerLecture_True_When_CsvDonnees_Exists_And_NotEmpty()
    {
        _viewModel.CsvDonnees = new ObservableCollection<Tuple<string, double>>
        {
            Tuple.Create("P", 1.0)
        };

        Assert.That(_viewModel.PeutCommencerLecture, Is.True);
    }

    [Test]
    public void PeutCommencerLecture_False_When_CsvDonnees_Null_Or_Empty()
    {
        _viewModel.CsvDonnees = null;
        Assert.That(_viewModel.PeutCommencerLecture, Is.False);

        _viewModel.CsvDonnees = new ObservableCollection<Tuple<string, double>>();
        Assert.That(_viewModel.PeutCommencerLecture, Is.False);
    }

    [Test]
    public async Task LancerLectureAsync_Sets_EstLectureEnCours_False_After_Run()
    {
        _viewModel.CsvDonnees = new ObservableCollection<Tuple<string, double>>
        {
            Tuple.Create("P", 5.0),
            Tuple.Create("S", 15.0)
        };
        _viewModel.Receive(_capteurTest.Id);

        await _viewModel.LancerLectureAsync();

        Assert.That(_viewModel.EstLectureEnCours, Is.False);
        Assert.That(_viewModel.ValeursAmplitude.Count, Is.EqualTo(2));
        Assert.That(_viewModel.LabelsTemps.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task LancerLectureAsync_DeclencheEvenement_When_AmplitudeDepasseSeuil()
    {
        var capteurTest = new Capteur
        {
            Nom = "Capteur1",
            SeuilAlerte = 5.0,
            StationId = 1
        };

        _viewModel.CapteurSelectionne = capteurTest;
        _viewModel.CsvDonnees = new ObservableCollection<Tuple<string, double>>
        {
            Tuple.Create("P", 10.0) 
        };

        await _viewModel.LancerLectureAsync();

        _dialogServiceMock.Verify(d => d.ShowMessage(
            It.Is<string>(msg => msg.Contains("événement sismique")),
            It.IsAny<string>()), Times.Once);
    }


    [Test]
    public void Receive_Assigns_Properties_Correctly()
    {
        _capteurServiceMock.Setup(c => c.ObtenirParId(_capteurTest.Id)).Returns(_capteurTest);

        _viewModel.Receive(_capteurTest.Id);

        Assert.That(_viewModel.Nom, Is.EqualTo(_capteurTest.Nom));
        Assert.That(_viewModel.NouveauStationId, Is.EqualTo(_capteurTest.StationId));

    }
}

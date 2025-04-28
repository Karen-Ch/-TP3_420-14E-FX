using Moq;
using Seismoscope.Enums;
using Seismoscope.Model;
using Seismoscope.Utils.Services.Interfaces;
using Seismoscope.ViewModel;

namespace Seismoscope_Tests;

public class HistoriqueEvenementsViewModelTests
{
    private Mock<IUserSessionService> _userSessionMock;
    private Mock<IEvenementService> _evenementServiceMock;
    private Mock<INavigationService> _navigationServiceMock;
    private Mock<IDialogService> _dialogServiceMock;

    private HistoriqueEvenementsViewModel _viewModel;
    private Mock<User> _userMock;
    private List<EvenementSismique> _evenementsMock;

    [SetUp]
    public void Setup()
    {
        _userMock = new Mock<User>();
        _userMock.SetupAllProperties();
        _userMock.Object.StationId = 1;

        _evenementsMock = new List<EvenementSismique>
            {
                new EvenementSismique { Amplitude = 5.2, TypeOnde = TypeOnde.P },
                new EvenementSismique { Amplitude = 6.8, TypeOnde = TypeOnde.S },
                new EvenementSismique { Amplitude = 7.1, TypeOnde = TypeOnde.Surface }
            };

        _userSessionMock = new Mock<IUserSessionService>();
        _userSessionMock.Setup(u => u.ConnectedUser).Returns(_userMock.Object);

        _evenementServiceMock = new Mock<IEvenementService>();
        _evenementServiceMock.Setup(s => s.ObtenirParStation(It.IsAny<int>())).Returns(_evenementsMock);

        _navigationServiceMock = new Mock<INavigationService>();
        _dialogServiceMock = new Mock<IDialogService>();

        _viewModel = new HistoriqueEvenementsViewModel(
            _userSessionMock.Object,
            _evenementServiceMock.Object,
            _navigationServiceMock.Object,
            _dialogServiceMock.Object
        );
    }

    [Test]
    public void Receive_ChargeEvenements()
    {
        _viewModel.Receive(null);

        Assert.That(_viewModel.Evenements.Count, Is.EqualTo(3));
        Assert.That(_viewModel.TousEvenements.Count, Is.EqualTo(3));
    }

    [Test]
    public void FiltrerEvenements_AppliqueFiltrage()
    {
        _viewModel.Receive(null);

        _viewModel.TypeOndeSelectionne = TypeOnde.S;
        _viewModel.FiltrerCommand.Execute(null);

        Assert.That(_viewModel.Evenements.Count, Is.EqualTo(1));
        Assert.That(_viewModel.Evenements[0].TypeOnde, Is.EqualTo(TypeOnde.S));
    }

    [Test]
    public void ReinitialiserFiltre_AfficheTous()
    {
        _viewModel.Receive(null);

        _viewModel.TypeOndeSelectionne = TypeOnde.S;
        _viewModel.FiltrerCommand.Execute(null);
        _viewModel.ReinitialiserFiltreCommand.Execute(null);

        Assert.That(_viewModel.Evenements.Count, Is.EqualTo(3));
    }
}

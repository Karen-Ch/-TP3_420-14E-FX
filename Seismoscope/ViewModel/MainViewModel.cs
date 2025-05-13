using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Seismoscope.Model;
using Seismoscope.Model.Interfaces;
using Seismoscope.Utils;
using Seismoscope.Utils.Commands;
using Seismoscope.Utils.Services;
using Seismoscope.Utils.Services.Interfaces;
using Seismoscope.Enums;
using Microsoft.Extensions.Logging;

namespace Seismoscope.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IUserSessionService _userSessionService;
        private readonly ILogger<MainViewModel> _logger;

        public INavigationService NavigationService
        {
            get => _navigationService;
        }

        public IUserSessionService UserSessionService
        {
            get => _userSessionService;
        }

        public ICommand NavigateToConnectUserViewCommand { get; set; }
        public ICommand NavigateToHomeViewCommand { get; set; }

        public ICommand DisconnectCommand { get; }
        private void NaviguerVersAccueil()
        {
            var user = _userSessionService.ConnectedUser;

            if (user == null)
            {
                _navigationService.NavigateTo<ConnectUserViewModel>();
            }
            else if (user.Role == Role.Administrateur)
            {
                _navigationService.NavigateTo<CarteViewModel>();
            }
            else
            {
                _navigationService.NavigateTo<StationViewModel>();
            }
        }


        private void Disconnect()
        {
            _userSessionService.ConnectedUser = null;
            OnPropertyChanged(nameof(UserSessionService.IsUserConnected));
            _navigationService.NavigateTo<ConnectUserViewModel>();
        }

        public MainViewModel(INavigationService navigationService, IUserSessionService userSessionService, ILogger<MainViewModel>logger)
        {
            _navigationService = navigationService;
            _userSessionService = userSessionService;
            _logger = logger;

            NavigateToConnectUserViewCommand = new RelayCommand(() => NavigationService.NavigateTo<ConnectUserViewModel>());
            NavigateToHomeViewCommand = new RelayCommand(NaviguerVersAccueil);
            DisconnectCommand = new RelayCommand(Disconnect, () => UserSessionService.IsUserConnected);

            NavigationService.NavigateTo<HomeViewModel>();
            TesterLogger();
        }

        private void TesterLogger()
        {
            var user = _userSessionService.ConnectedUser;
            _logger.LogInformation("Test log : opération normale");
            _logger.LogTrace("Test log : utilisateur {User}", user);
            _logger.LogWarning("Test log : comportament inattendu");
            _logger.LogError("Test log : erreur critique");

        }
    }
}

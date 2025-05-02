using Seismoscope.Model.Interfaces;
using Seismoscope.Utils;
using Seismoscope.Utils.Commands;
using Seismoscope.Utils.Services.Interfaces;
using Seismoscope.Utils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Seismoscope.ViewModel
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly IUserDAL _userDAL;
        private readonly INavigationService _navigationService;
        private readonly IUserSessionService _userSessionService;

        public User? ConnectedUser
        {
            get => _userSessionService.ConnectedUser;
        }

        public string WelcomeMessage
        {
            get => ConnectedUser == null ? "Bienvenue. Merci de vous connecter." : $"Bienvenue {ConnectedUser.Prenom}!";
        }

        public HomeViewModel(IUserDAL userDAL, INavigationService navigationService, IUserSessionService userSessionService)
        {
            _userDAL = userDAL;
            _navigationService = navigationService;
            _userSessionService = userSessionService;
            LogoutCommand = new RelayCommand(Logout, CanLogout);
        }

   
        public ICommand LogoutCommand { get; set; }


        private void Logout()
        {
            _userSessionService.ConnectedUser = null;
            _navigationService.NavigateTo<ConnectUserViewModel>();
        }


        private bool CanLogout()
        {
            return _userSessionService.IsUserConnected;
        }
    }
}

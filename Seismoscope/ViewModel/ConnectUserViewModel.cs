using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Seismoscope.Model;
using Seismoscope.Model.DAL;
using Seismoscope.Model.Interfaces;
using Seismoscope.Utils;
using Seismoscope.Utils.Commands;
using Seismoscope.Utils.Services.Interfaces;
using Seismoscope.Utils.Services;
using Seismoscope.Enums;

namespace Seismoscope.ViewModel
{
    public class ConnectUserViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly INavigationService _navigationService;
        private readonly IUserSessionService _userSessionService;

        private string _nomUtilisateur;
        public string NomUtilisateur
        {
            get => _nomUtilisateur;
            set
            {
                if (_nomUtilisateur != value)
                {
                    _nomUtilisateur = value;
                    OnPropertyChanged(nameof(NomUtilisateur));
                    ValidateProperty(nameof(NomUtilisateur), value);
                }
            }
        }

        private string _motDePasse;
        public string MotDePasse
        {
            get => _motDePasse;
            set
            {
                if (_motDePasse != value)
                {
                    _motDePasse = value;
                    OnPropertyChanged(nameof(MotDePasse));
                    ValidateProperty(nameof(MotDePasse), value);
                }
            }
        }

        public ICommand ConnectCommand { get; }

        public ConnectUserViewModel(
            IUserService userService,
            INavigationService navigationService,
            IUserSessionService userSessionService)
        {
            _userService = userService;
            _navigationService = navigationService;
            _userSessionService = userSessionService;

            ConnectCommand = new RelayCommand(Connect, CanConnect);
        }

        private void Connect()
        {
            User? user = _userService.Authentifier(NomUtilisateur, MotDePasse);
            if (user != null)
            {
                _userSessionService.ConnectedUser = user;

                if (user.Role == Role.Administrateur)
                    _navigationService.NavigateTo<CarteViewModel>(user);
                else
                    _navigationService.NavigateTo<StationViewModel>(parameter: user.StationId);
            }
            else
            {
                AddError(nameof(MotDePasse), "Nom d'utilisateur ou mot de passe invalide.");
                OnPropertyChanged(nameof(ErrorMessages));
            }
        }

        private bool CanConnect()
        {
            return !HasErrors && NomUtilisateur.NotEmpty() && MotDePasse.NotEmpty();
        }

        private void ValidateProperty(string propertyName, string value)
        {
            ClearErrors(propertyName);

            switch (propertyName)
            {
                case nameof(NomUtilisateur):
                    if (value.Empty())
                        AddError(propertyName, "Le nom d'utilisateur est requis.");
                    else if (value.Length < 2)
                        AddError(propertyName, "Le nom d'utilisateur doit contenir au moins 2 caractères.");
                    break;

                case nameof(MotDePasse):
                    if (value.Empty())
                        AddError(propertyName, "Le mot de passe est requis.");
                    break;
            }

            OnPropertyChanged(nameof(ErrorMessages));
        }
    }
}

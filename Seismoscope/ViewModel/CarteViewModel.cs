using Seismoscope.Enums;
using Seismoscope.Model.Interfaces;
using Seismoscope.Model;
using Seismoscope.Utils;
using Seismoscope.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Seismoscope.Utils.Commands;

namespace Seismoscope.ViewModel
{
    public class CarteViewModel : BaseViewModel
    {
        private readonly IUserSessionService _userSession;
        private readonly INavigationService _navigationService;
        private readonly IStationService _stationService;

        public string Message { get; private set; }
        public ICommand StationSelectedCommand { get; }

        public Dictionary<int, StationAffichage> StationsInfo { get; } = new();

        public CarteViewModel(
            IUserSessionService userSession,
            INavigationService navigationService,
            IStationService stationService)
        {
            _userSession = userSession;
            _navigationService = navigationService;
            _stationService = stationService;

            var user = _userSession.ConnectedUser;
            Message = $"Bienvenue {user.Prenom} {user.Nom} sur la carte interactive !";

            StationSelectedCommand = new RelayCommand<int>(AfficherStation);

            ChargerStations();
        }

        private void AfficherStation(int stationId)
        {
            _navigationService.NavigateTo<StationViewModel>(stationId);
        }

        private void ChargerStations()
        {
            var stations = _stationService.ObtenirToutesAvecCapteurs();
            StationsInfo.Clear();

            foreach (var station in stations)
            {
                var affichage = new StationAffichage
                {
                    Id = station.Id,
                    CouleurEtat = new SolidColorBrush((Color)ColorConverter.ConvertFromString(
                        station.Etat switch
                        {
                            Etat.Actif => "#27ae60",
                            Etat.Maintenance => "#f39c12",
                            Etat.HorsService => "#c0392b",
                            _ => "Gray"
                        })),
                    EtatMessage = station.Etat switch
                    {
                        Etat.Actif => "Station active",
                        Etat.Maintenance => "En maintenance",
                        Etat.HorsService => "Hors service",
                        _ => "État inconnu"
                    },
                    Clignote = station.Etat == Etat.HorsService
                };

                StationsInfo[station.Id] = affichage;
            }

            OnPropertyChanged(nameof(StationsInfo));
        }
    }
}

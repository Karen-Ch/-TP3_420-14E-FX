using Seismoscope.Model;
using Seismoscope.Model.Interfaces;
using Seismoscope.Utils;
using Seismoscope.Utils.Commands;
using Seismoscope.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Seismoscope.ViewModel
{
    public class AjouterCapteurViewModel : BaseViewModel
    {
        private readonly IUserSessionService _userSession;
        private readonly INavigationService _navigationService;
        private readonly ICapteurService _capteurService;
        private readonly IStationService _stationService;
        private readonly IDialogService _dialogService;

        public ObservableCollection<Capteur> Capteurs { get; set; }
        public Capteur CapteurSelectionne { get; set; }
        public Station NouvelleStation { get; set; }
        public int NouveauStationId { get; set; }
        public ICommand ValiderCapteurCommand { get; }

        public AjouterCapteurViewModel(IUserSessionService userSession,
            INavigationService navigationService,
            ICapteurService capteurService,
            IStationService stationService,
            IDialogService dialogService)
        {
            _userSession = userSession;
            _navigationService = navigationService;
            _capteurService = capteurService;
            _stationService = stationService;
            _dialogService = dialogService;

            Capteurs = new ObservableCollection<Capteur>(_capteurService.ObtenirTous());
            NouveauStationId = (int)_userSession.ConnectedUser.StationId;

            ValiderCapteurCommand = new RelayCommand(() =>
            {
                if (CapteurSelectionne != null && CapteurSelectionne.EstLivre)
                {
                    NouvelleStation = _stationService.ObtenirParId(NouveauStationId);

                    CapteurSelectionne.Station = NouvelleStation;
                    CapteurSelectionne.StationId = NouvelleStation.Id;
                    _capteurService.ModifierCapteur(CapteurSelectionne);

                    _dialogService.ShowMessage("Capteur ajouté !");

                    OnPropertyChanged(nameof(NouvelleStation));
                    OnPropertyChanged(nameof(NouveauStationId));
                }
                else
                {
                    _dialogService.ShowMessage("Veuillez sélectionner un capteur livré.");
                }
            });
        }
    }
}

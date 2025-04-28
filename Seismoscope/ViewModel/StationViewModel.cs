using Seismoscope.Enums;
using Seismoscope.Model;
using Seismoscope.Model.DAL;
using Seismoscope.Model.Interfaces;
using Seismoscope.Utils;
using Seismoscope.Utils.Commands;
using Seismoscope.Utils.Services.Interfaces;
using Seismoscope.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Seismoscope.ViewModel
{
    public class StationViewModel : BaseViewModel, IParameterReceiver
    {
        private readonly IUserSessionService _userSession;
        private readonly IStationService _stationService;
        private readonly ICapteurService _capteurService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        public string Nom { get; private set; }
        public string Localisation { get; private set; }
        public string Code { get; private set; }
        public string Responsable { get; private set; }
        public string Etat { get; private set; }
        public string DateInstallation { get; private set; }
        public User Utilisateur { get; private set; }

        public ObservableCollection<Capteur> Capteurs { get; set; } = new();
        public ObservableCollection<Capteur> TousCapteurs { get; set; } = new();
        public ICommand ModifierCapteurCommand { get; }
        public ICommand EffacerCapteurCommand { get; }
        public ICommand AjouterCapteurCommand { get; }

        public ICommand AllerVersHistoriqueCommand { get; }
        public ICommand VerifierCapteurCommand { get; }
        public Capteur CapteurSelectionne { get; set; }
        public Station NouvelleStation { get; set; }
        public int NouveauStationId { get; set; }

        public StationViewModel(
            IUserSessionService userSession,
            IStationService stationService,
            INavigationService navigationService,
            ICapteurService capteurService,
            IDialogService dialogService)
        {
            _userSession = userSession;
            _stationService = stationService;
            _navigationService = navigationService;
            _capteurService = capteurService;
            _dialogService = dialogService;

            Utilisateur = _userSession.ConnectedUser;
            OnPropertyChanged(nameof(Utilisateur));

            TousCapteurs = new ObservableCollection<Capteur>(
                _capteurService.ObtenirTous()
                    .Where(c => c.EstLivre &&
                                (c.StationId == null || c.StationId == 0) &&
                                (c.DateInstallation == default || c.DateInstallation == DateTime.MinValue))
            );

            ModifierCapteurCommand = new RelayCommand<Capteur>(capteur =>
            {
                _navigationService.NavigateTo<CapteurViewModel>(capteur.Id);
            });

            VerifierCapteurCommand = new RelayCommand<Capteur>(capteur =>
            {
                _navigationService.NavigateTo<DonneesCapteurViewModel>(capteur.Id);
            });

            EffacerCapteurCommand = new RelayCommand<Capteur>(capteur =>
            {
                if (capteur == null) return;

                var result = _dialogService.ShowConfirmation(
                    $"Voulez-vous vraiment supprimer le capteur \"{capteur.Nom}\" ?",
                    "Confirmation");

                if (result == true && capteur.EstDesactive)
                {
                    Capteurs.Remove(capteur);
                    TousCapteurs.Remove(capteur);
                    _capteurService.SupprimerCapteur(capteur);
                }
                else if (!capteur.EstDesactive)
                {
                    _dialogService.ShowMessage($"Le capteur \"{capteur.Nom}\" est actif, ne peut pas être supprimé.", "Information");
                }
            });

            AjouterCapteurCommand = new RelayCommand(() =>
            {
                if (CapteurSelectionne == null)
                {
                    _dialogService.ShowMessage("Veuillez sélectionner un capteur.", "Erreur");
                    return;
                }

                if (!CapteurSelectionne.EstLivre)
                {
                    _dialogService.ShowMessage("Ce capteur n'est pas livrable.", "Erreur");
                    return;
                }

                Station stationCible = NouvelleStation;

                if (stationCible == null && Utilisateur.StationId != null)
                {
                    stationCible = _stationService.ObtenirParId(Utilisateur.StationId.Value);
                }

                if (stationCible == null)
                {
                    _dialogService.ShowMessage("Aucune station cible n’a été trouvée.", "Erreur");
                    return;
                }

                CapteurSelectionne.Station = stationCible;
                CapteurSelectionne.StationId = stationCible.Id;
                CapteurSelectionne.DateInstallation = DateTime.Now;
                _capteurService.ModifierCapteur(CapteurSelectionne);

                Capteurs.Add(CapteurSelectionne);
                TousCapteurs.Remove(CapteurSelectionne);

                _dialogService.ShowMessage("Capteur ajouté !", "Succès");
                CapteurSelectionne = null;
                OnPropertyChanged(nameof(CapteurSelectionne));
            });
            AllerVersHistoriqueCommand = new RelayCommand(() =>
            {
                _navigationService.NavigateTo<HistoriqueEvenementsViewModel>();
            });

        }

        public void Receive(object parameter)
        {
            if (parameter is int stationId)
            {
                Station? station = _stationService.ObtenirParId(stationId);
                if (station != null)
                {
                    NouvelleStation = station;

                    Nom = station.Nom;
                    Localisation = station.Localisation;
                    Code = station.Code;
                    Responsable = station.Responsable;
                    Etat = station.Etat.ToString();
                    DateInstallation = station.DateInstallation.ToShortDateString();

                    Capteurs.Clear();
                    foreach (var capteur in station.Capteurs)
                        Capteurs.Add(capteur);

                    OnPropertyChanged(nameof(NouvelleStation));
                    OnPropertyChanged(nameof(Nom));
                    OnPropertyChanged(nameof(Localisation));
                    OnPropertyChanged(nameof(Code));
                    OnPropertyChanged(nameof(Responsable));
                    OnPropertyChanged(nameof(Etat));
                    OnPropertyChanged(nameof(DateInstallation));
                    OnPropertyChanged(nameof(Capteurs));
                }
            }
        }
    }

}

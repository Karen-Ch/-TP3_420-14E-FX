using Seismoscope.Enums;
using Seismoscope.Model;
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Seismoscope.ViewModel
{
    /// <summary>
    /// Logique d'interaction pour CapteurView.xaml
    /// </summary>
    public partial class CapteurViewModel : BaseViewModel, IParameterReceiver
    {
        private readonly IUserSessionService _userSession;
        private readonly ICapteurService _capteurService;
        private readonly INavigationService _navigation;
        private readonly IDialogService _dialogService;

        public ICommand SauvegarderModificationsCommand => new RelayCommand(SauvegarderModifications);
        public ICommand ToggleActivationCommand => new RelayCommand(ToggleActivation);

        public int Id { get; set; }
        public string Nom { get; set; }
        public string Type { get; set; }
        public Etat Statut { get; set; }

        private double _frequenceCollecte;
        public double FrequenceCollecte
        {
            get => _frequenceCollecte;
            set { _frequenceCollecte = value; OnPropertyChanged(); }
        }

        private double _seuilAlerte;
        public double SeuilAlerte
        {
            get => _seuilAlerte;
            set { _seuilAlerte = value; OnPropertyChanged(); }
        }

        public DateTime DateInstallation { get; set; }

        private bool _estDesactive;
        public bool EstDesactive
        {
            get => _estDesactive;
            set { _estDesactive = value; OnPropertyChanged(); }
        }

        public bool EstLivre { get; set; }
        public int? StationId { get; set; }
        public Station Station { get; set; }

        public ObservableCollection<Capteur> Capteurs { get; set; } = new();

        public CapteurViewModel(
            IUserSessionService userSession,
            ICapteurService capteurService,
            INavigationService navigation,
            IDialogService dialogService)
        {
            _userSession = userSession;
            _capteurService = capteurService;
            _navigation = navigation;
            _dialogService = dialogService;
        }

        public void Receive(object parameter)
        {
            if (parameter is int capteurId)
            {
                var capteur = _capteurService.ObtenirParId(capteurId);
                if (capteur != null)
                {
                    Id = capteur.Id;
                    Nom = capteur.Nom;
                    Type = capteur.Type;
                    Statut = capteur.Statut;
                    FrequenceCollecte = capteur.FrequenceCollecte;
                    SeuilAlerte = capteur.SeuilAlerte;
                    DateInstallation = capteur.DateInstallation;
                    EstDesactive = capteur.EstDesactive;
                    EstLivre = capteur.EstLivre;
                    StationId = capteur.StationId;

                    OnPropertyChanged(nameof(Nom));
                    OnPropertyChanged(nameof(Type));
                    OnPropertyChanged(nameof(Statut));
                    OnPropertyChanged(nameof(DateInstallation));
                    OnPropertyChanged(nameof(EstLivre));
                    OnPropertyChanged(nameof(StationId));
                }
            }
        }

        private void ToggleActivation()
        {
            EstDesactive = !EstDesactive;
            var capteur = _capteurService.ObtenirParId(Id);
            if (capteur != null)
            {
                capteur.EstDesactive = EstDesactive;
                _capteurService.ModifierCapteur(capteur);
                _dialogService.ShowMessage("Statut du capteur mis à jour.", "Succès");
                _navigation.NavigateTo<StationViewModel>();
            }
        }

        private void SauvegarderModifications()
        {
            if (FrequenceCollecte <= 0)
            {
                _dialogService.ShowMessage("La fréquence de collecte doit être un nombre positif.", "Valeur invalide");
                return;
            }

            if (SeuilAlerte <= 0)
            {
                _dialogService.ShowMessage("Le seuil d’alerte doit être un nombre positif.", "Valeur invalide");
                return;
            }

            var capteur = _capteurService.ObtenirParId(Id);
            if (capteur != null)
            {
                capteur.FrequenceCollecte = FrequenceCollecte;
                capteur.SeuilAlerte = SeuilAlerte;
                _capteurService.ModifierCapteur(capteur);
                _dialogService.ShowMessage("Paramètres mis à jour avec succès.", "Succès");
                _navigation.NavigateTo<StationViewModel>();
            }
        }
    }
}

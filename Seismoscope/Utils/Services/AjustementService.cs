using Seismoscope.Utils.Services.Interfaces.Seismoscope.Services.Interfaces;
using Seismoscope.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Seismoscope.Model;

namespace Seismoscope.Utils.Services
{
    public class AjustementService:IAjustementService
    {
        private readonly ICapteurService _capteurService;
        private readonly IJournalService _journalService;
        private bool _frequenceAugmenteeRecemment = false;

        public AjustementService(ICapteurService capteurService, IJournalService journalService)
        {
            _capteurService = capteurService;
            _journalService = journalService;
        }
        public void AppliquerRegles(Capteur capteur, List<double> lecturesRecues)
        {
            if (capteur == null || lecturesRecues == null || lecturesRecues.Count == 0)
                return;

            bool evenementDetecte = false;

            if (AjusterSeuilPourForteAmplitude(capteur, lecturesRecues))
                evenementDetecte = true;

            if (!evenementDetecte && AjusterSeuilPourMicroSecousses(capteur, lecturesRecues))
                return;

            if (AugmenterFrequencePourEvenementCritique(capteur, lecturesRecues))
                evenementDetecte = true;

            if (!evenementDetecte)
                ReinitialiserFrequenceApresInactivite(capteur, lecturesRecues);

            if (evenementDetecte)
            {
                capteur.DernierEvenement = DateTime.Now;
                _capteurService.ModifierCapteur(capteur);
            }
        }
        private bool AjusterSeuilPourForteAmplitude(Capteur capteur, List<double> lectures)
        {
            double seuilActuel = capteur.SeuilAlerte;
            double seuilMax = capteur.SeuilMax;

            foreach (double lecture in lectures)
            {
                if (lecture > seuilActuel * 1.3)
                {
                    double nouveauSeuil = Math.Min(seuilActuel * 1.1, seuilMax);
                    if (nouveauSeuil > seuilActuel)
                    {
                        capteur.SeuilAlerte = nouveauSeuil;
                        _journalService.Enregistrer(capteur.Id, "Hausse du seuil d’alerte",
                            $"Amplitude {lecture} mm > 130% de {seuilActuel} mm → seuil ajusté à {nouveauSeuil:F1} mm.");
                        _capteurService.ModifierCapteur(capteur);
                    }
                    return true;
                }
            }

            return false;
        }
        private bool AjusterSeuilPourMicroSecousses(Capteur capteur, List<double> lectures)
        {
            double seuilActuel = capteur.SeuilAlerte;
            double seuilMin = capteur.SeuilMin;

            if (lectures.Count < 5) return false;

            bool cinqProches = true;
            for (int i = lectures.Count - 5; i < lectures.Count; i++)
            {
                double l = lectures[i];
                if (l < seuilActuel * 0.8 || l >= seuilActuel)
                {
                    cinqProches = false;
                    break;
                }
            }

            if (cinqProches)
            {
                double nouveauSeuil = Math.Max(seuilActuel * 0.9, seuilMin);
                if (nouveauSeuil < seuilActuel)
                {
                    capteur.SeuilAlerte = nouveauSeuil;
                    _journalService.Enregistrer(capteur.Id, "Réduction du seuil d’alerte",
                        $"5 lectures proches du seuil (≥ 80%) → seuil réduit à {nouveauSeuil:F1} mm.");
                    _capteurService.ModifierCapteur(capteur);
                }
                return true;
            }

            return false;
        }
        private bool AugmenterFrequencePourEvenementCritique(Capteur capteur, List<double> lectures)
        {
            double seuilActuel = capteur.SeuilAlerte;
            double frequenceActuelle = capteur.FrequenceCollecte;
            double frequenceMinimale = capteur.FrequenceMinimale;

            foreach (double lecture in lectures)
            {
                if (lecture > seuilActuel * 1.6)
                {
                    if (_frequenceAugmenteeRecemment)
                        return false;

                    double nouvelleFrequence = Math.Max(frequenceActuelle * 0.9, frequenceMinimale);

                    if (nouvelleFrequence < frequenceActuelle - 0.1)
                    {
                        capteur.FrequenceCollecte = nouvelleFrequence;
                        _journalService.Enregistrer(capteur.Id, "Augmentation de la fréquence de collecte",
                            $"Amplitude {lecture} mm > 160% du seuil → fréquence ajustée à 1 lecture / {nouvelleFrequence:F1} s.");
                        _capteurService.ModifierCapteur(capteur);

                        _frequenceAugmenteeRecemment = true; 
                        return true;
                    }

                    return false;
                }
            }

            _frequenceAugmenteeRecemment = false;
            return false;
        }


        private void ReinitialiserFrequenceApresInactivite(Capteur capteur, List<double> lectures)
        {
            double frequenceActuelle = capteur.FrequenceCollecte;
            double frequenceParDefaut = capteur.FrequenceParDefaut;

            if (frequenceActuelle >= frequenceParDefaut) return;

            bool conditionsTemps = capteur.DernierEvenement.HasValue &&
                                   capteur.DernierEvenement.Value.AddMinutes(2) < DateTime.Now;

            bool conditionsLectures = lectures.Count >= 10;

            if (conditionsTemps || conditionsLectures)
            {
                capteur.FrequenceCollecte = frequenceParDefaut;
                _journalService.Enregistrer(capteur.Id, "Réinitialisation de la fréquence",
                    $"Aucune activité depuis {(conditionsTemps ? "2 minutes" : "10 lectures")} → fréquence réinitialisée à 1 lecture / {frequenceParDefaut:F1} sec.");
                _capteurService.ModifierCapteur(capteur);
            }
        }

    }
}

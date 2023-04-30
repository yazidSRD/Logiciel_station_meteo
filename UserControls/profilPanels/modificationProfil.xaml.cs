// Importations de namespaces et classes externes
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace projet23_Station_météo_WPF.UserControls.profilPanels
{
    /// <summary>
    /// Fenêtre d'affichage du profil d'un utilisateur et le modifier
    /// Modification plus ou moins avancée en fonction des droits
    /// </summary>
    public partial class modificationProfil : Window
    {
        // Données du profil
        public Dictionary<string, string> profil;

        // Suite du profil
        public bool safeguard = false;
        public bool delete = false;

        // Nouveau profil ou pas
        bool newProfil = false;

        // Constructeur de la classe
        public modificationProfil(Dictionary<string, string> profil, bool newProfil)
        {
            // Initialisation des composants
            InitializeComponent();

            // Enregistrement des paramètres dans les propriétés de l'instance courante
            this.profil = profil;

            // Initialise le mot de passe à null
            this.profil["Mdp"] = null;

            // Initialise la variable newProfil à newProfil
            this.newProfil = newProfil;

            // Définit le titre de la fenêtre
            this.Title = "modification du profil";

            // Affecte les valeurs des champs de saisie avec les valeurs du profil
            Nom.Text = this.profil["Nom"];
            Prenom.Text = this.profil["Prenom"];
            Tel.Text = this.profil["Tel"];
            Identifiant.Text = this.profil["Identifiant"];
            Fonction.Text = this.profil["Fonction"];
            Droit.Text = this.profil["Droit"];

            // Cache les champs d'identifiant, de fonction et de droit si l'utilisateur n'a pas les droits d'administrateur
            if ((string)App.Current.Properties["Droit"] == "0") {
                gridIdentifiant.Visibility = Visibility.Hidden;
                gridFonction.Visibility = Visibility.Hidden;
                gridDroit.Visibility = Visibility.Hidden;
                this.Height = 314;
                this.Width = 320;
            }

            // Si on crée un nouveau profil
            if (this.newProfil)
            {
                // Définit le titre de la fenêtre
                this.Title = "nouveau profil";

                // Affiche le champ de saisie du mot de passe
                gridPassWord.Visibility = Visibility.Visible;

                // Cache la case à cocher de vérification du mot de passe
                checkPassWord.Visibility = Visibility.Hidden;

                // Cache le bouton de suppression du profil
                buttonSup.Visibility = Visibility.Hidden;
            } else
            {
                // Si l'utilisateur n'a pas les droits d'administrateur, cache le bouton de suppression du profil
                if ((string)App.Current.Properties["Droit"] != "1") buttonSup.Visibility = Visibility.Hidden;
            }

        }

        // Cette fonction est appelée lorsqu'on coche ou décoche la case pour modifier le mot de passe
        private void showPassWord(object sender, RoutedEventArgs e)
        {
            // Si la case est cochée, on affiche la grille de saisie du mot de passe
            if (((CheckBox)sender).IsChecked == true) gridPassWord.Visibility = Visibility.Visible;
            // Sinon, on la masque
            else gridPassWord.Visibility = Visibility.Hidden;
        }

        // Cette fonction est appelée lorsqu'on clique sur le bouton pour tout annuler
        private void annuler(object sender, RoutedEventArgs e)
        {
            // On ferme la fenêtre sans rien faire
            this.Close();
        }
        // Cette fonction est appelée lorsqu'on clique sur le bouton pour enregistrer
        private void save(object sender, RoutedEventArgs e)
        {
            // On vérifie que les données saisies sont valides
            if (!verification()) return;

            // On met à jour les données du profil avec les valeurs saisies
            this.profil["Nom"] = Nom.Text;
            this.profil["Prenom"] = Prenom.Text;
            this.profil["Tel"] = Tel.Text;
            this.profil["Identifiant"] = Identifiant.Text;
            this.profil["Fonction"] = Fonction.Text;
            this.profil["Droit"] = Droit.Text;

            // Si la grille de saisie du mot de passe est visible ou si on est en train de créer un nouveau profil,
            // on met à jour le mot de passe du profil avec la valeur saisie
            if (gridPassWord.Visibility == Visibility.Visible || this.newProfil) this.profil["Mdp"] = Mdp.Text;

            // On indique que les modifications ont été sauvegardées
            this.safeguard = true;
            // On ferme la fenêtre
            this.Close();
        }

        // Cette fonction est appelée lorsqu'on clique sur le bouton pour supprimer
        private void supprimer(object sender, RoutedEventArgs e)
        {
            // On indique que le profil doit être supprimé
            this.delete = true;
            // On ferme la fenêtre
            this.Close();
        }

        // Cette méthode effectue des vérifications sur les champs de saisie du formulaire
        private bool verification()
        {
            bool flag = true;

            // Définition des tailles maximales pour chaque champ
            int maxNom = 50;
            int maxPrenom = 50;
            int maxIdentifiant = 50;
            int maxFonction = 50;
            int maxPassword = 50;
            int maxTel = 11;
            int maxDroit = 11;

            // Expression régulière pour vérifier que le champ Nom contient uniquement des lettres et des espaces
            // Vérification de la taille maximale du champ Nom
            Regex regexNom = new Regex("^[a-zA-Z ]*$");
            if (!regexNom.IsMatch(Nom.Text) || Nom.Text.Length > maxNom || Nom.Text.Length < 3)
            {   
                // Affichage d'un signal sur le champ en question
                // Marqueur pour indiquer que la vérification a échoué
                signal(Nom);
                flag = false;
            }

            // Expression régulière pour vérifier que le champ Prénom contient uniquement des lettres et des espaces
            // Vérification de la taille maximale du champ Prénom
            Regex regexPrenom = new Regex("^[a-zA-Z ]*$");
            if (!regexPrenom.IsMatch(Prenom.Text) || Prenom.Text.Length > maxPrenom || Prenom.Text.Length < 3)
            {
                // Affichage d'un signal sur le champ en question
                // Marqueur pour indiquer que la vérification a échoué
                signal(Prenom);
                flag = false;
            }

            // Expression régulière pour vérifier que le champ Téléphone contient uniquement des chiffres et a une longueur de 10 caractères
            // Vérification de la taille maximale du champ Téléphone
            Regex regexTel = new Regex("^[0-9]{10}$");
            if (!regexTel.IsMatch(Tel.Text) || Tel.Text.Length > maxTel)
            {
                // Affichage d'un signal sur le champ en question
                // Marqueur pour indiquer que la vérification a échoué
                signal(Tel);
                flag = false;
            }

            if (gridPassWord.Visibility == Visibility.Visible || this.newProfil)
            {
                // Expression régulière pour vérifier que le mot de passe contient au moins 1 lettre majuscule, 1 lettre minuscule et 1 chiffre
                // Vérification de la taille maximale du mot de passe
                Regex regexPassword = new Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).{8,}$");
                if (!regexPassword.IsMatch(Mdp.Text) || Mdp.Text.Length > maxPassword)
                {
                    // Affichage d'un signal sur le champ en question
                    // Marqueur pour indiquer que la vérification a échoué
                    signal(Mdp);
                    flag = false;
                }
            }

            if ((string)App.Current.Properties["Droit"] != "0")
            {
                // Expression régulière pour vérifier que le champ Identifiant contient uniquement des lettres, des chiffres et des tirets
                // Vérification de la taille maximale du champ Identifiant
                Regex regexIdentifiant = new Regex("^[a-zA-Z0-9-]*$");
                if (!regexIdentifiant.IsMatch(Identifiant.Text) || Identifiant.Text.Length > maxIdentifiant || Identifiant.Text.Length < 3)
                {
                    // Affichage d'un signal sur le champ en question
                    // Marqueur pour indiquer que la vérification a échoué
                    signal(Identifiant);
                    flag = false;
                }

                // régulière pour vérifier que le champ Fonction contient uniquement des lettres et des espaces
                // Vérification de la taille maximale du champ Fonction
                Regex regexFonction = new Regex("^[a-zA-Z ]*$");
                if (!regexFonction.IsMatch(Fonction.Text) || Fonction.Text.Length > maxFonction || Fonction.Text.Length < 3)
                {
                    // Affichage d'un signal sur le champ en question
                    // Marqueur pour indiquer que la vérification a échoué
                    signal(Fonction);
                    flag = false;
                }

                // Conversion du champ Droit en entier
                // Vérification de la taille maximale du champ Droit
                int droit;
                if (!int.TryParse(Droit.Text, out droit) || Droit.Text.Length > maxDroit || (droit != 0 && droit != 1))
                {
                    // Affichage d'un signal sur le champ en question
                    // Marqueur pour indiquer que la vérification a échoué
                    signal(Droit);
                    flag = false;
                }
            }

            // Retourne le résultat
            return flag;
        }
        async void signal(dynamic element)
        {
            // Définition de la couleur du bord de l'élément en cas d'erreur
            Brush color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#abadb3"));

            // Boucle de répétition pour clignoter le bord de l'élément 4 fois avec une pause de 250ms
            for (int i = 0; i < 4; i++)
            {
                element.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#851f37"));
                element.BorderThickness = new Thickness(2);
                await Task.Delay(250);
                element.BorderBrush = color;
                element.BorderThickness = new Thickness(1);
                await Task.Delay(250);
            }
        }

    }
}

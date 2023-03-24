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
    /// Logique d'interaction pour modificationProfil.xaml
    /// </summary>
    public partial class modificationProfil : Window
    {
        public Dictionary<string, string> profil;
        public bool safeguard = false;
        public bool delete = false;
        bool newProfil = false;

        public modificationProfil(Dictionary<string, string> profil, bool newProfil)
        {
            InitializeComponent();
            this.profil = profil;
            this.profil["Mdp"] = null;
            this.newProfil = newProfil;

            this.Title = "modification du profil";

            Nom.Text = this.profil["Nom"];
            Prenom.Text = this.profil["Prenom"];
            Tel.Text = this.profil["Tel"];
            Identifiant.Text = this.profil["Identifiant"];
            Fonction.Text = this.profil["Fonction"];
            Droit.Text = this.profil["Droit"];
            if ((string)App.Current.Properties["Droit"] == "0") {
                gridIdentifiant.Visibility = Visibility.Hidden;
                gridFonction.Visibility = Visibility.Hidden;
                gridDroit.Visibility = Visibility.Hidden;
                this.Height = 314;
                this.Width = 320;
            }

            if (this.newProfil)
            {
                this.Title = "nouveau profil";
                gridPassWord.Visibility = Visibility.Visible;
                checkPassWord.Visibility = Visibility.Hidden;
                buttonSup.Visibility = Visibility.Hidden;
            } else
            {
                if ((string)App.Current.Properties["Droit"] != "1") buttonSup.Visibility = Visibility.Hidden;
            }

        }
        private void showPassWord(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true) gridPassWord.Visibility = Visibility.Visible;
            else gridPassWord.Visibility = Visibility.Hidden;
        }
        private void annuler(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void save(object sender, RoutedEventArgs e)
        {
            if (!verification()) return;

            this.profil["Nom"] = Nom.Text;
            this.profil["Prenom"] = Prenom.Text;
            this.profil["Tel"] = Tel.Text;
            this.profil["Identifiant"] = Identifiant.Text;
            this.profil["Fonction"] = Fonction.Text;
            this.profil["Droit"] = Droit.Text;

            if (gridPassWord.Visibility == Visibility.Visible || this.newProfil) this.profil["Mdp"] = Mdp.Text;

            this.safeguard = true;
            this.Close();
        }
        private void supprimer(object sender, RoutedEventArgs e)
        {
            this.delete = true;
            this.Close();
        }
        private bool verification()
        {
            bool flag = true;

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
                signal(Nom);
                flag = false;
            }

            // Expression régulière pour vérifier que le champ Prénom contient uniquement des lettres et des espaces
            // Vérification de la taille maximale du champ Prénom
            Regex regexPrenom = new Regex("^[a-zA-Z ]*$");
            if (!regexPrenom.IsMatch(Prenom.Text) || Prenom.Text.Length > maxPrenom || Prenom.Text.Length < 3)
            {
                signal(Prenom);
                flag = false;
            }

            // Expression régulière pour vérifier que le champ Téléphone contient uniquement des chiffres et a une longueur de 10 caractères
            // Vérification de la taille maximale du champ Téléphone
            Regex regexTel = new Regex("^[0-9]{10}$");
            if (!regexTel.IsMatch(Tel.Text) || Tel.Text.Length > maxTel)
            {
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
                    signal(Identifiant);
                    flag = false;
                }

                // régulière pour vérifier que le champ Fonction contient uniquement des lettres et des espaces
                // Vérification de la taille maximale du champ Fonction
                Regex regexFonction = new Regex("^[a-zA-Z ]*$");
                if (!regexFonction.IsMatch(Fonction.Text) || Fonction.Text.Length > maxFonction || Fonction.Text.Length < 3)
                {
                    signal(Fonction);
                    flag = false;
                }

                // Conversion du champ Droit en entier
                // Vérification de la taille maximale du champ Droit
                int droit;
                if (!int.TryParse(Droit.Text, out droit) || Droit.Text.Length > maxDroit || (droit != 0 && droit != 1))
                {
                    Console.WriteLine(int.TryParse(Droit.Text, out droit));
                    signal(Droit);
                    flag = false;
                }
            }

            return flag;
        }
        async void signal(dynamic element)
        {
            Brush color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#abadb3"));

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

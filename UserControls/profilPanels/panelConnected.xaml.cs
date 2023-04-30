// Importations de namespaces et classes externes
using projet23_Station_météo_WPF.code;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace projet23_Station_météo_WPF.UserControls.profilPanels
{
    /// <summary>
    /// Fenêtre pour les utilisateurs connectés
    /// Permet de :
    /// Enregistrer les utilisateurs
    /// Effacer les utilisateurs
    /// Modifier les utilisateurs
    /// </summary>
    public partial class panelConnected : UserControl
    {
        public delegate void refreshDelegate();

        // Constructeur de la classe
        public panelConnected()
        {
            // Initialisation des composants
            InitializeComponent();

            // Actualisation de la fenêtre
            refresh();

            // Si utilisateur a les doits afficher le buton de création de profil
            if ((string)App.Current.Properties["Droit"] != "1") buttonNewProfil.Visibility = Visibility.Hidden;
        }

        // Cette méthode met à jour les éléments d'interface utilisateur qui affichent les propriétés de l'utilisateur courant
        void refresh()
        {
            Nom.Text = (string)App.Current.Properties["Nom"];
            Prenom.Text = (string)App.Current.Properties["Prenom"];
            Identifiant.Text = (string)App.Current.Properties["Identifiant"];
            Tel.Text = (string)App.Current.Properties["Tel"];
            Fonction.Text = (string)App.Current.Properties["Fonction"];

            // Lance une nouvelle tâche sur un thread séparé pour rafraîchir le DataGrid contenant les utilisateurs
            new Thread(refreshDataGrid).Start();
        }

        // Méthode pour récupérer la liste de comptes à partir du serveur
        void refreshDataGrid()
        {
            // Utilise la classe Http pour obtenir une liste de comptes à partir du serveur
            List<Dictionary<string, string>> jsonData = new Http().getAllCompte((string)App.Current.Properties["Identifiant"], (string)App.Current.Properties["Mdp"]).Result;

            // Utilise la déléguée refreshDelegate pour passer la liste des comptes à la méthode refreshDataGridDelegate
            Dispatcher.BeginInvoke(new refreshDelegate(() => refreshDataGridDelegate(jsonData)), DispatcherPriority.Render);
        }

        // Méthode pour afficher la liste de comptes reçu
        void refreshDataGridDelegate(List<Dictionary<string, string>> jsonData)
        {
            // Vider le tableau
            dataGrid.Items.Clear();
            dataGrid.ItemsSource = jsonData;
        }

        // Appeler leur du clic sur le bouton de déconnexion
        private void deconnexionClick(object sender, RoutedEventArgs e)
        {
            // Création d'un nouveau Thread pour exécuter la tâche en arrière-plan
            new Thread(async () =>
            {
                // Effacer les valeurs de l'identifiant et du mot de passe dans le fichier de configuration
                await new configModifier().edit("saveIdentifiant", "");
                await new configModifier().edit("saveMdp", "");
            }).Start();

            // Réinitialiser les propriétés de l'utilisateur
            App.Current.Properties["Nom"] = null;
            App.Current.Properties["Prenom"] = null;
            App.Current.Properties["Identifiant"] = null;
            App.Current.Properties["Tel"] = null;
            App.Current.Properties["Fonction"] = null;
            App.Current.Properties["Droit"] = null;
            App.Current.Properties["Mdp"] = null;

            // Obtenir le panel de profil parent pour aficher la fençtre de connexion
            profilPanel profilPanel = (profilPanel)((System.Windows.FrameworkElement)this.Parent).Parent;
            Dispatcher.BeginInvoke(new profilPanel.refreshDelegate(profilPanel.refresh), DispatcherPriority.Render);
        }
        private void modifMyProfilButton(object sender, RoutedEventArgs e)
        {
            // Créer un dictionnaire pour stocker les informations de profil de l'utilisateur
            Dictionary<string, string> profil = new Dictionary<string, string>();


            // Ajouter les informations de profil au dictionnaire
            profil.Add("ID", (string)App.Current.Properties["ID"]);
            profil.Add("Nom", (string)App.Current.Properties["Nom"]);
            profil.Add("Prenom", (string)App.Current.Properties["Prenom"]);
            profil.Add("Identifiant", (string)App.Current.Properties["Identifiant"]);
            profil.Add("Tel", (string)App.Current.Properties["Tel"]);
            profil.Add("Fonction", (string)App.Current.Properties["Fonction"]);
            profil.Add("Droit", (string)App.Current.Properties["Droit"]);

            // Appelez la méthode modifProfil pour modifier le profil avec les informations du dictionnaire
            modifProfil(profil);
        }

        // Appeler leur du clic sur le bouton de modification d'un profil dans le tableau
        private void modifProfilButton(object sender, RoutedEventArgs e)
        {
            // Vérifie que l'utilisateur a le droit de modifier un profil
            if ((string)App.Current.Properties["Droit"] == "0") return;

            // Récupère les informations de la ligne sélectionnée dans la DataGrid sous forme de dictionnaire
            var rowDictionary = ((System.Windows.FrameworkElement)sender).DataContext as Dictionary<string, string>;

            // Si la ligne sélectionnée est un dictionnaire valide, lance la modification du profil
            if (rowDictionary != null) modifProfil(rowDictionary);
        }
        private void modifProfil(Dictionary<string, string> profil)
        {
            // Création de la fenêtre de modification de profil avec les informations à modifier
            modificationProfil popup = new modificationProfil(profil, false);

            // Affichage de la fenêtre pour l'utilisateur
            popup.ShowDialog();

            // Si l'utilisateur a choisi de sauvegarder les modifications, on lance un thread pour sauvegarder le profil modifié
            if (popup.safeguard) new Thread(() => saveProfil(popup.profil)).Start();
            // Si l'utilisateur a choisi de supprimer le profil, on lance un thread pour supprimer le profil
            else if (popup.delete) new Thread(() => deleteProfil(popup.profil)).Start();
        }
        private void saveProfil(Dictionary<string, string> profil)
        {
            // Appelle la méthode editCompte de la classe Http pour éditer le compte utilisateur et récupère le résultat
            bool saved = new Http().editCompte((string)App.Current.Properties["Identifiant"], (string)App.Current.Properties["Mdp"], profil).Result;

            // Vérifie si les modifications ont été sauvegardées
            if (saved)
            {
                // Si l'utilisateur a modifié son propre mot de passe, met à jour la propriété "Mdp" de l'application avec le nouveau mot de passe
                if (profil["ID"] == (string)App.Current.Properties["ID"] && profil["Mdp"] != null) App.Current.Properties["Mdp"] = profil["Mdp"];

                // Récupère le profilPanel parent de l'élément courant et appelle sa méthode refresh() pour rafraîchir les données du profil
                profilPanel profilPanel = (profilPanel)((System.Windows.FrameworkElement)this.Parent).Parent;
                Dispatcher.BeginInvoke(new profilPanel.refreshDelegate(profilPanel.refresh), DispatcherPriority.Render);
            }
        }
        private void deleteProfil(Dictionary<string, string> profil)
        {
            // Supprime le compte associé au profil donné en utilisant une requête HTTP
            bool saved = new Http().deleteCompte((string)App.Current.Properties["Identifiant"], (string)App.Current.Properties["Mdp"], profil["ID"]).Result;

            // Si la suppression s'est bien passée
            if (saved)
            {
                // Actualise la vue du panneau de profil
                profilPanel profilPanel = (profilPanel)((System.Windows.FrameworkElement)this.Parent).Parent;
                Dispatcher.BeginInvoke(new profilPanel.refreshDelegate(profilPanel.refresh), DispatcherPriority.Render);
            }
        }
        private void newProfilButton(object sender, RoutedEventArgs e)
        {
            // Création d'un nouveau dictionnaire profil vide
            Dictionary<string, string> profil = new Dictionary<string, string>();

            // Ajout de clés vides au dictionnaire
            profil.Add("Nom", "");
            profil.Add("Prenom", "");
            profil.Add("Identifiant", "");
            profil.Add("Tel", "");
            profil.Add("Fonction", "");
            profil.Add("Droit", "");

            // Ouverture d'une fenêtre de modification du profil
            modificationProfil popup = new modificationProfil(profil, true);
            popup.ShowDialog();

            // Si l'utilisateur clique sur "Enregistrer" dans la fenêtre de modification, crée un nouveau Thread pour sauvegarder les modifications
            if (popup.safeguard) new Thread(() =>
            {
                // Appel à la méthode newCompte pour créer un nouveau profil
                bool saved = new Http().newCompte((string)App.Current.Properties["Identifiant"], (string)App.Current.Properties["Mdp"], profil).Result;
                if (saved)
                {
                    // Rafraîchissement de la liste des profils
                    profilPanel profilPanel = (profilPanel)((System.Windows.FrameworkElement)this.Parent).Parent;
                    Dispatcher.BeginInvoke(new profilPanel.refreshDelegate(profilPanel.refresh), DispatcherPriority.Render);
                }
            }).Start();
        }
    }
}

// Importations de namespaces et classes externes
using projet23_Station_météo_WPF.code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
    /// Fenêtre de connexion des utilisateurs
    /// </summary>
    public partial class profilConnexion : UserControl
    {
        // Booléens pour le suivi de l'état de connexion
        private bool connexionInProgress = false;
        private bool stayConnected = false;

        // Définition d'un délégué pour la mise à jour de l'interface utilisateur
        delegate void delegateMessageBox();

        // Constructeur de la classe
        public profilConnexion()
        {
            // Initialisation des composants
            InitializeComponent();
        }

        // Appeler leur du clic sur le bouton de connexion
        private void connexion(object sender, RoutedEventArgs e)
        {
            connexion();
        }

        // Méthode de vérification des informations avant d'essayer de se connecter
        async public void connexion()
        {
            // Vérifie si la connexion est déjà en cours
            if (connexionInProgress) return;

            // Si non, on indique qu'une connexion est en cours
            connexionInProgress = true;

            // flag sert à indiquer si des champs sont vides ou non
            bool flag = false;

            // Si le champ login est vide
            if (login.Text == "")
            {
                // on met flag à true et on surligne le champ login
                flag = true; signal(login);
            }

            // Si le champ password est vide
            if (password.Password == "")
            {
                // on met flag à true et on surligne le champ password
                flag = true; signal(password);
            }

            // Si au moins un des champs est vide
            if (flag) {
                // On attend 2 secondes avant de continuer
                await Task.Delay(2000);

                // On indique que la connexion est terminée
                connexionInProgress = false;

                // On quitte la méthode
                return;
            }

            // Sinon, on récupère le login et le mot de passe
            string loginT = login.Text;
            string passwordT = password.Password;

            // On lance la méthode getConnexction dans un nouveau thread en passant en paramètres le login et le mot de passe
            new Thread(() => getConnexction(loginT, passwordT)).Start();
        }

        // Cette méthode est utilisée pour signaler une erreur visuellement en clignotant la bordure de l'élément donné
        async void signal(dynamic element)
        {
            // On accède au parent de type Grid de l'élément donné
            element = (Grid)element.Parent;
            // On accède au parent de type Border de l'élément précédent
            element = (Border)element.Parent;

            // On sauvegarde la couleur d'origine de la bordure de l'élément.
            Brush color = element.BorderBrush;

            // On effectue un clignotement de la bordure en changeant sa couleur et son épaisseur, avec une pause de 250ms entre chaque changement
            for (int i =0; i<4; i++)
            {
                element.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#851f37"));
                element.BorderThickness = new Thickness(2);
                await Task.Delay(250);
                element.BorderBrush = color;
                element.BorderThickness = new Thickness(1);
                await Task.Delay(250);
            }
        }

        // Cette méthode permet de se connecter en appelant la méthode "getCompte" de la classe "Http"
        // Les identifiants de connexion sont passés en paramètres
        public void getConnexction(string login, string password)
        {
            // On affiche le chargement
            loadingBar.start(false, true);
            try
            {
                // On appelle la méthode "getCompte" de la classe "Http" pour récupérer les données du compte
                Dictionary<string, string> jsonData = new Http().getCompte(login, password).Result;

                // Si les données sont nulles, cela signifie que la connexion a échoué
                if (jsonData == null)
                {
                    // On affiche une boîte de dialogue pour informer l'utilisateur que la connexion a échoué
                    Dispatcher.BeginInvoke(new delegateMessageBox(() => {
                        System.Windows.Forms.MessageBox.Show("Impossible de se connecter au server.\n\nIl est possible que:\n - Vous ne soyez pas connecté\n - Que le serveur ne soit pas connecté\n - Que l'identifiant ou le mot de passe correspondent pas\n\nSi le problème persiste, veuillez contacter un administrateur.",
                        "Connexion erreur",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                    }), DispatcherPriority.Render);

                    // On indique que la connexion n'est plus en cours et on arrête le chargement
                    connexionInProgress = false;
                    loadingBar.stop();
                    return;
                }

                // On stocke l'identifiant et le mot de passe dans les propriétés de l'application.
                App.Current.Properties["Identifiant"] = jsonData["Identifiant"];
                App.Current.Properties["Mdp"] = password;

                // Si l'utilisateur a choisi de rester connecté, on enregistre les identifiants dans la configuration
                if (this.stayConnected == true)
                {
                    // On lance une tâche asynchrone pour enregistrer les identifiants dans la configuration
                    new Thread(async () =>
                    {
                        await new configModifier().edit("saveIdentifiant", (string)App.Current.Properties["Identifiant"]);
                        await new configModifier().edit("saveMdp", (string)App.Current.Properties["Mdp"]);
                    }).Start();
                }

                // On récupère le panneau de profil et on actualise l'affichage
                profilPanel profilPanel = (profilPanel)((System.Windows.FrameworkElement)this.Parent).Parent;
                Dispatcher.BeginInvoke(new profilPanel.refreshDelegate(profilPanel.refresh), DispatcherPriority.Render);
            }  catch (Exception e) {
                Console.WriteLine(e);
            }

            // On indique que la connexion n'est plus en cours et on arrête le chargement
            connexionInProgress = false;
            loadingBar.stop();
        }

        // Appeler leur du clic sur le bouton pour rester connecté
        private void stayConnectedClick(object sender, RoutedEventArgs e)
        {
            stayConnected = ((CheckBox)sender).IsChecked == true;
        }
    }
}

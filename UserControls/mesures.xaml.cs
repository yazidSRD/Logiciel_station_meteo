// Importations de namespaces et classes externes
using projet23_Station_météo_WPF.UserControls.mesuresUi;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using projet23_Station_météo_WPF.code;

namespace projet23_Station_météo_WPF.UserControls
{
    /// <summary>
    /// Fenêtre par défaut qui permet de voir les dernières mesures sur le serveur
    /// Gestion du responsive
    /// </summary>
    public partial class mesures : UserControl
    {
        // Thread pour l'actualisation périodique
        Thread autoRefreshTh;

        // Fenêtre actuellement ouverte
        dynamic mesureUi;

        // Définition d'un délégué pour la mise à jour de l'interface utilisateur
        delegate void delegateMessageBox();

        // Constructeur de la classe
        public mesures()
        {
            // Initialisation des composants
            InitializeComponent();

            // Récupère le premier enfant de la grille Gui et le stocke dans la variable mesureUi
            mesureUi = Gui.Children[0];

            // Crée un nouveau thread pour la fonction autoRefresh et le démarre
            autoRefreshTh = new Thread(autoRefresh);
            autoRefreshTh.Start();

            // Démarre la lecture de la vidéo dans le lecteur vidéo
            //videoPlayer.Play();
        }

        // Methode appelée lors du déchargement de l'élément
        public void dell(object sender, RoutedEventArgs e)
        {
            // Arrête le thread de rafraîchissement
            autoRefreshTh.Abort();
        }

        // Methode de rafraîchissement automatique des données
        public void autoRefresh()
        {
            // Boucle infinie pour rafraîchir périodiquement les données
            while (true)
            {
                // Appelle la fonction refreshData() pour mettre à jour les données
                refreshData();

                // Récupère le temps de rafraîchissement à partir des propriétés de l'application et attend le temps spécifié
                Thread.Sleep(Convert.ToInt32(((string)App.Current.Properties["refreshTimer"])) * 1000);
            }
        }

        // Methode de recuperation des données
        public void refreshData()
        {
            // Début du chargement
            loadingBar.start(false, true);

            // Récupération de la date d'hier au format souhaité
            DateTime yesterday = DateTime.Now.AddDays(-1);
            string yesterdayFormatted = yesterday.ToString("yyyy-MM-dd HH:mm");

            // Récupération des données depuis le serveur en utilisant l'API Http
            List<Dictionary<string, string>> jsonData = new Http().get("WHERE DateHeureReleve > '" + yesterdayFormatted + "' ORDER BY DateHeureReleve DESC").Result;

            // Vérification si les données ont été récupérées avec succès
            if (jsonData == null || jsonData.Count == 0) {

                // Si la connexion au serveur a échoué, afficher un message d'erreur
                Dispatcher.BeginInvoke(new delegateMessageBox(() => {
                    System.Windows.Forms.MessageBox.Show("Impossible de se connecter au server.\n\nIl est possible que:\n - Vous ne soyez pas connecté\n - Que le serveur ne soit pas connecté\n\nSi le problème persiste, veuillez contacter un administrateur.",
                    "Connexion erreur",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                }), DispatcherPriority.Render);

                // Arrêt du chargement et sortie de la methode
                loadingBar.stop();
                return;
            };

            // Initialisation d'un dictionnaire pour stocker les données mesurées
            Dictionary<string, List<Int32>> listD = new Dictionary<string, List<Int32>>()
            {
                {"Temperature", new List<Int32>()},
                {"Hygrometrie", new List<Int32>()},
                {"VitesseVent", new List<Int32>()},
                {"PressionAtmospherique", new List<Int32>()},
                {"Pluviometre", new List<Int32>()},
                {"DirectionVent", new List<Int32>()},
                {"RayonnementSolaire", new List<Int32>()}
            };

            // Initialisation d'une liste pour stocker les dates correspondantes aux données mesurées
            List<string> listDate = new List<string>();

            // Parcourir toutes les données récupérées depuis le serveur
            foreach (Dictionary<string, string> relevemeteo in jsonData)
            {
                // Pour chaque donnée mesurée, ajouter sa valeur correspondante dans le dictionnaire correspondant
                foreach (var userControl in listD)
                {

                    listD[userControl.Key].Add(Convert.ToInt32(relevemeteo[userControl.Key]));
                }

                // Ajouter la date de la donnée mesurée à la liste de dates
                listDate.Add(relevemeteo["DateHeureReleve"]);
            }

            // Mettre à jour l'interface utilisateur avec les données mesurées et les dates correspondantes
            mesureUi.refreshData(listD, listDate);

            // Arrêt du chargement
            loadingBar.stop();
        }
        private void uiRefesh(object sender, SizeChangedEventArgs e)
        {
            /*Console.WriteLine(e.NewSize.Height + "  " + e.NewSize.Width);*/

            // On crée une liste dynamique de UIs (interfaces utilisateur)
            List<dynamic> uis = new List<dynamic>() { new mesures2(), new mesures5(), new mesures10(), new mesures4(), new mesures7(), new mesures9(), new mesures8(), new mesures3(), new mesures1() };

            // On parcourt chaque UI dans la liste
            foreach (dynamic ui in uis)
            {
                // Si la hauteur minimale et la largeur minimale de l'UI sont plus petites que la taille de la fenêtre
                if (ui.MinHeight <= e.NewSize.Height && ui.MinWidth <= e.NewSize.Width)
                {
                    // Si le type de l'UI n'est pas le même que celui de l'UI actuellement affiché
                    if (ui.GetType() != mesureUi.GetType())
                    {
                        // On change l'UI affiché
                        mesureUi = ui;
                        Gui.Children.Clear();
                        Gui.Children.Add(mesureUi);

                        // On arrête et redémarre le thread d'actualisation automatique
                        autoRefreshTh.Abort();
                        autoRefreshTh = new Thread(autoRefresh);
                        autoRefreshTh.Start();
                    }

                    // On quitte la boucle car on a trouvé l'UI qui convient
                    return;
                }
            }
        }
        // Méthode appelée à la fin de la vidéo
        private void LoopVideo(object sender, RoutedEventArgs e)
        {
            // On remet la vidéo au début et on la recommence
            //videoPlayer.Position = TimeSpan.Zero;
            //videoPlayer.Play();
        }
    }
}

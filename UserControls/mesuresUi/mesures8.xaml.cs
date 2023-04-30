// Importations de namespaces et classes externes
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;

namespace projet23_Station_météo_WPF.UserControls.mesuresUi
{
    /// <summary>
    /// Affichage personnalisé 
    /// </summary>
    public partial class mesures8 : UserControl
    {
        // Thread pour l'actualisation
        Thread refreshUiTh;

        // Définition d'un délégué pour la mise à jour de l'interface utilisateur
        delegate void illustrationDelegate();

        // Icon temps actuelle
        string urlIllustration;

        // Constructeur de la classe
        public mesures8()
        {
            // Initialisation des composants
            InitializeComponent();

            // Crée un nouveau thread pour l'actualisation
            refreshUiTh = new Thread(refreshUi);
            refreshUiTh.Start();
        }

        // Cette méthode met à jour l'interface utilisateur
        public void refreshUi()
        {
            // Les informations sont récupérées depuis les propriétés de l'application
            // Pour chaque capteur, on appelle la méthode setInfo de son contrôle utilisateur
            controlTemp.graph.setInfo("temperature", ((string)App.Current.Properties["unitTemp"]), "images/icons/temperature0.png", 49);
            controlHygro.graph.setInfo("hygrometrie", ((string)App.Current.Properties["unitHygro"]), "images/icons/hygrometrie.png", 49);
            controlVitesseVent.graph.setInfo("vitesse du vent", ((string)App.Current.Properties["unitVvent"]), "images/icons/vitesseVent.png", 49);
            controlPressionAtmospherique.graph.setInfo("pression atmosphérique", ((string)App.Current.Properties["unitPresAtmo"]), "images/icons/pression.png", 49);
            controlPluviometre.graph.setInfo("pluviometre", ((string)App.Current.Properties["unitPluv"]), "images/icons/pluviometrie.png", 49);
            controlDirectionVent.graph.setInfo("direction du vent", ((string)App.Current.Properties["unitDvent"]), "images/icons/DirectionVent.png", 49);
            controlPressionAtmospherique2.graph.setInfo("pression atmosphérique", ((string)App.Current.Properties["unitPresAtmo"]), "images/icons/pression.png", 49);
            controlRayonnementSolaire.graph.setInfo("rayonnement solaire", ((string)App.Current.Properties["unitRaySol"]), "images/icons/rayonnementSolaire.png", 49);
        }

        // Cette méthode permet de rafraîchir l'illustration de la météo en fonction de l'URL de l'illustration passée en paramètre
        public void refreshIllustration()
        {
            // Création d'une nouvelle instance de BitmapImage
            System.Windows.Media.Imaging.BitmapImage logo = new System.Windows.Media.Imaging.BitmapImage();

            // Début de l'initialisation de l'image
            logo.BeginInit();

            // Spécification de l'URI source de l'image en utilisant l'URL passée en paramètre
            logo.UriSource = new Uri("https://www.weatherbit.io/static/img/icons/" + urlIllustration + ".png");

            // Fin de l'initialisation de l'image
            logo.EndInit();

            // Affectation de l'image ainsi créée à l'illustration à rafraîchir
            illustration.Source = logo;
        }
        public void refreshData(Dictionary<string, List<Int32>> data, List<string> date)
        {
            // Récupérer les prévisions météo
            List<Dictionary<string, string>> previsions = new Http().getPrevisions().Result;

            // Récupérer l'URL de l'illustration et la mettre à jour si elle existe
            urlIllustration = null;
            urlIllustration = previsions[0]["icon"];
            if (urlIllustration != null) Dispatcher.BeginInvoke(new illustrationDelegate(refreshIllustration), System.Windows.Threading.DispatcherPriority.Render);

            // Créer un dictionnaire avec les noms des contrôles et les objets correspondants
            Dictionary<string, List<dynamic>> listV = new Dictionary<string, List<dynamic>>()
            {
                {"Temperature", new List<dynamic>() { controlTemp}},
                {"Hygrometrie", new List<dynamic>() { controlHygro }},
                {"VitesseVent", new List<dynamic>() { controlVitesseVent }},
                {"PressionAtmospherique", new List<dynamic>() { controlPressionAtmospherique, controlPressionAtmospherique2 }},
                {"Pluviometre", new List<dynamic>() { controlPluviometre}},
                {"DirectionVent", new List<dynamic>() { controlDirectionVent }},
                {"RayonnementSolaire", new List<dynamic>() { controlRayonnementSolaire }},
            };

            // Parcourir le dictionnaire pour mettre à jour les valeurs des graphiques de chaque contrôle
            foreach (var userControls in listV)
            {
                foreach (dynamic userControl in userControls.Value)
                {
                    userControl.graph.setValues(data[userControls.Key], date);
                }
            }
        }
    }
}

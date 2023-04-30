// Importations de namespaces et classes externes
using projet23_Station_météo_WPF.code;
using projet23_Station_météo_WPF.UserControls;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace projet23_Station_météo_WPF
{
    /// <summary>
    /// Contient et permet de naviguer entre les différentes fenêtre
    /// </summary>
    public partial class MainWindow : Window
    {
        // Définition d'un délégué pour la mise à jour de l'interface utilisateur
        public delegate void delegateLoadingUi();

        // Booléens pour le suivi de l'état du menu
        bool disabledButtonOpenMenu = false;
        bool menuOpen = false;

        // Constructeur de la classe
        public MainWindow()
        {
            // Initialisation des composants
            InitializeComponent();

            // Ajout du MainWindow comme parent du contrôle de la barre de chargement
            loadingBar.mainWindow = this;

            // Chargement des configurations
            new configModifier().load();

            // Ajout du contrôle de mesures comme contenu initial
            WindowView.Children.Add(new mesures());
        }

        // Méthode pour gérer le clic sur le bouton du menu
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Vérification si le bouton est désactivé
            if (disabledButtonOpenMenu) return;

            // Ouverture ou fermeture du menu selon son état actuel
            if (menuOpen) closeMenu(false);
            else openMenu(false);
        }

        // Méthode asynchrone pour ouvrir le menu
        private async void openMenu(bool expand)
        {
            // Animation d'expansion du menu
            for (int i = 0; i < 205; i += 41)
            {
                BorderMenu.Width += 41;
                if (expand)
                {
                    var margin = WindowView.Margin;
                    margin.Left += 41;
                    WindowView.Margin = margin;
                }
                await Task.Delay(1);
            }

            // Mise à jour de l'état du menu
            menuOpen = true;
        }

        // Méthode asynchrone pour fermer le menu
        private async void closeMenu(bool unexpand)
        {
            // Animation de réduction du menu
            for (int i = 0; i < 205; i += 41)
            {
                BorderMenu.Width -= 41;
                if (unexpand)
                {
                    var margin = WindowView.Margin;
                    margin.Left -= 41;
                    WindowView.Margin = margin;
                }
                await Task.Delay(1);
            }

            // Mise à jour de l'état du menu
            menuOpen = false;
        }

        // Méthode pour fermer tous les panneaux ouverts et changer la couleur des boutons de menu
        private void CloseAllPanels(object sender)
        {
            try
            {
                // Effacement du contenu de la zone de visualisation
                WindowView.Children.Clear();
            } catch (Exception ex) { };

            // Parcours de tous les boutons du menu pour changer leur couleur
            foreach (Button button in BorderMenu.Children.OfType<Button>())
            {
                if (((Grid)button.Content).Children[0] is Rectangle)
                {
                    ((Rectangle)((Grid)button.Content).Children[0]).Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#000063AE");
                }
            }
            ((Rectangle)((Grid)((Button)sender).Content).Children[0]).Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF0063AE");
        }

        // Fonctions appelées lors du clic sur les boutons des différents panels
        private void PanelMesuresButton_Click(object sender, RoutedEventArgs e)
        {
            CloseAllPanels(sender);
            WindowView.Children.Add(new mesures());
        }
        private void PanelHistoriqueButton_Click(object sender, RoutedEventArgs e)
        {
            CloseAllPanels(sender);
            WindowView.Children.Add(new historique());
        }
        public void PanelProfilButton_Click(object sender, RoutedEventArgs e)
        {
            CloseAllPanels(sender);
            WindowView.Children.Add(new profilPanel());
        }
        private void PanelParametreButton_Click(object sender, RoutedEventArgs e)
        {
            CloseAllPanels(sender);
            WindowView.Children.Add(new settingsPanel());
        }
        private void mesuresTableButton_Click(object sender, RoutedEventArgs e)
        {
            CloseAllPanels(sender);
            WindowView.Children.Add(new mesuresTable());
        }
        private void PanelComparaisonsButton_Click(object sender, RoutedEventArgs e)
        {
            CloseAllPanels(sender);
            WindowView.Children.Add(new oneGraph());
        }

        // Cette méthode est appelée à chaque fois que la taille de la fenêtre change
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Vérifie si la nouvelle largeur de la fenêtre est supérieure ou égale à 1500 pixels et si le bouton pour ouvrir le menu n'est pas désactivé
            if (e.NewSize.Width >= 1500 && !disabledButtonOpenMenu)
            {
                
                // Si c'est le cas, désactive le bouton pour ouvrir le menu
                disabledButtonOpenMenu = true;
                
                // Si le menu n'est pas déjà ouvert, appelle la méthode pour l'ouvrir
                // Sinon, boucle sur les éléments du menu pour les décaler vers la droite
                if (!menuOpen) openMenu(true);
                else
                {
                    for (int i = 0; i < 205; i += 41)
                    {
                        var margin = WindowView.Margin;
                        margin.Left += 41;
                        WindowView.Margin = margin;
                    }
                }
            }

            // Si la nouvelle largeur de la fenêtre est inférieure à 1500 pixels et que le bouton pour ouvrir le menu est désactivé
            else if (e.NewSize.Width < 1500 && disabledButtonOpenMenu)
            {
            
                // Si le menu est ouvert, appelle la méthode pour le fermer
                if (menuOpen) closeMenu(true);
                
                // Réactive le bouton pour ouvrir le menu
                disabledButtonOpenMenu = false;
            }
        }

        // Elle appelle la méthode "cancellation()" sur l'objet "loadingBar" pour annuler le processus de chargement en cours
        private void loadingAnnul(object sender, RoutedEventArgs e)
        {
            loadingBar.cancellation();
        }
    }
}

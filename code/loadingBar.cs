// Importations de namespaces et classes externes
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media;

namespace projet23_Station_météo_WPF.code
{
    static class loadingBar
    {
        // Référence à la fenêtre principale de l'application
        public static MainWindow mainWindow;

        // Flag pour l'annulation de la tâche en cours
        public static bool annul = false;

        // Flag pour l'annulation de la tâche en cours
        static Thread threadLoadingAnimation;

        // Affiche la barre de chargement avec les options spécifiées
        public static void start(bool progressBar = true, bool loadingAnimation = true, string loadingDescription = "", bool annulButton = false)
        {

            // Affiche la barre de chargement sur l'UI thread
            mainWindow.Dispatcher.BeginInvoke(new MainWindow.delegateLoadingUi(() => {
                mainWindow.loadingUi.Visibility = Visibility.Visible;

                // Affiche ou masque la barre de progression en fonction de la valeur de progressBar
                if (progressBar) ((ProgressBar)mainWindow.loadingUi.Children[0]).Visibility = Visibility.Visible;
                else ((ProgressBar)mainWindow.loadingUi.Children[0]).Visibility = Visibility.Hidden;

                // Initialise la valeur de la barre de progression à 0
                ((ProgressBar)mainWindow.loadingUi.Children[0]).Value = 0;

                // Affiche ou masque l'animation de chargement en fonction de la valeur de loadingAnimation
                if (loadingAnimation) ((Image)mainWindow.loadingUi.Children[1]).Visibility = Visibility.Visible;
                else ((Image)mainWindow.loadingUi.Children[1]).Visibility = Visibility.Hidden;

                // Affiche ou masque la description de chargement en fonction de la valeur de loadingDescription
                if (loadingDescription != "") ((TextBlock)mainWindow.loadingUi.Children[2]).Visibility = Visibility.Visible;
                else ((TextBlock)mainWindow.loadingUi.Children[2]).Visibility = Visibility.Hidden;

                // Définit la description de chargement
                ((TextBlock)mainWindow.loadingUi.Children[2]).Text = loadingDescription;

                // Affiche ou masque le bouton d'annulation en fonction de la valeur de annulButton
                if (annulButton) ((Button)mainWindow.loadingUi.Children[4]).Visibility = Visibility.Visible;
                else ((Button)mainWindow.loadingUi.Children[4]).Visibility = Visibility.Hidden;

            }), DispatcherPriority.Render);

            // Réinitialise le flag d'annulation
            annul = false;
            if (loadingAnimation)
            {
                // Initialise le thread pour l'animation de chargement
                threadLoadingAnimation = new Thread(LoadingAnimation);

                // Démarre le thread
                threadLoadingAnimation.Start();
            }
        }

        // Met à jour la valeur de la barre de progression
        public static void refresh(int value=0)
        {
            // Met à jour la barre de progression sur l'UI thread
            mainWindow.Dispatcher.BeginInvoke(new MainWindow.delegateLoadingUi(() => {

                // Définit la valeur de la barre de progression
                ((ProgressBar)mainWindow.loadingUi.Children[0]).Value = value;
            }), DispatcherPriority.Render);
        }

        // Arrête la barre de chargement
        public static void stop()
        {
            // Attend un peu pour que la barre de progression atteigne 100%
            System.Threading.Thread.Sleep(500);
            mainWindow.Dispatcher.BeginInvoke(new MainWindow.delegateLoadingUi(() => {
                // Masque la barre de chargement
                mainWindow.loadingUi.Visibility = Visibility.Hidden;
            }), DispatcherPriority.Render);

            // Arrête le thread d'animation de chargement
            threadLoadingAnimation.Abort();
        }

        // Animation de chargement
        public static void LoadingAnimation()
        {
            while (true) {
                mainWindow.Dispatcher.BeginInvoke(new MainWindow.delegateLoadingUi(() => {
                    // Crée une nouvelle transformation de rotation en ajoutant 45 degrés à l'angle actuel
                    RotateTransform rotateTransform = new RotateTransform(((RotateTransform)((Image)mainWindow.loadingUi.Children[1]).RenderTransform).Angle+45);
                    
                    // Applique la transformation à l'image de chargement
                    ((Image)mainWindow.loadingUi.Children[1]).RenderTransform = rotateTransform;
                }), DispatcherPriority.Render);

                // Pause le thread pendant 100ms avant de recommencer la rotation
                Thread.Sleep(100);
            }
        }

        // Elle est appelée par le bouton d'annulation dans la fenêtre de chargement
        // Cette méthode permet d'annuler le chargement en cours
        public static void cancellation()
        {
            annul = true;
        }
    }
}

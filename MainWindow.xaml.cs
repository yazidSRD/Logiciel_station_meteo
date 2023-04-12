using projet23_Station_météo_WPF.code;
using projet23_Station_météo_WPF.UserControls;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace projet23_Station_météo_WPF
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void delegateLoadingUi();
        bool disabledButtonOpenMenu = false;
        bool menuOpen = false;
        //public Serveur serveur;
        
        public MainWindow()
        {
            InitializeComponent();
            loadingBar.mainWindow = this;
            new configModifier().load();
            WindowView.Children.Add(new mesures());
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (disabledButtonOpenMenu) return;
            if (menuOpen) closeMenu(false);
            else openMenu(false);
        }
        private async void openMenu(bool expand)
        {
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
            menuOpen = true;
        }
        private async void closeMenu(bool unexpand)
        {
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
            menuOpen = false;
        }
        private void CloseAllPanels(object sender)
        {
            //in progress
            try
            {
                //WindowView.Children.Remove(WindowView.Children[1]);
                WindowView.Children.Clear();
            } catch (Exception ex) { };
            foreach (Button button in BorderMenu.Children.OfType<Button>())
            {
                if (((Grid)button.Content).Children[0] is Rectangle)
                {
                    ((Rectangle)((Grid)button.Content).Children[0]).Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#000063AE");
                }
            }
            ((Rectangle)((Grid)((Button)sender).Content).Children[0]).Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF0063AE");
        }
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
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width >= 1500 && !disabledButtonOpenMenu)
            {
                disabledButtonOpenMenu = true;
                if (!menuOpen) openMenu(true);
                else {
                    for (int i = 0; i < 205; i += 41)
                    {
                        var margin = WindowView.Margin;
                        margin.Left += 41;
                        WindowView.Margin = margin;
                    }
                }
            } else if (e.NewSize.Width < 1500 && disabledButtonOpenMenu)
            {
                if (menuOpen) closeMenu(true);
                disabledButtonOpenMenu = false;
            }
        }
        private void loadingAnnul(object sender, RoutedEventArgs e)
        {
            loadingBar.cancellation();
        }
        /*private void previsionButton_Click(object sender, RoutedEventArgs e)
        {
            CloseAllPanels(sender);
            WindowView.Children.Add(new previsions());
        }*/
    }
}

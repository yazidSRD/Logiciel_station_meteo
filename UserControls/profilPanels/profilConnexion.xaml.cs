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
    /// Logique d'interaction pour profilConnexion.xaml
    /// </summary>
    public partial class profilConnexion : UserControl
    {
        private bool connexionInProgress = false;
        private bool stayConnected = false;

        public profilConnexion()
        {
            InitializeComponent();
        }
        private void connexion(object sender, RoutedEventArgs e)
        {
            connexion();
        }
        async public void connexion()
        {
            if (connexionInProgress) return;
            connexionInProgress = true;
            bool flag = false;

            if (login.Text == "")
            {
                flag = true; signal(login);
            }

            if (password.Password == "")
            {
                flag = true; signal(password);
            }

            if (flag) {
                await Task.Delay(2000);
                connexionInProgress = false;
                return;
            }

            string loginT = login.Text;
            string passwordT = password.Password;

            new Thread(() => getConnexction(loginT, passwordT)).Start();
        }
        async void signal(dynamic element)
        {
            element = (Grid)element.Parent;
            element = (Border)element.Parent;

            Brush color = element.BorderBrush;

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
        public void getConnexction(string login, string password)
        {
            try
            {
                List<Dictionary<string, string>> jsonData = new Http().getCompte(login, password).Result;

                if (jsonData == null || jsonData.Count == 0) 
                {
                    connexionInProgress = false;
                    return;
                }

                App.Current.Properties["Identifiant"] = jsonData[0]["Identifiant"];
                App.Current.Properties["Mdp"] = password;

                if (this.stayConnected == true)
                {
                    new Thread(async () =>
                    {
                        await new configModifier().edit("saveIdentifiant", (string)App.Current.Properties["Identifiant"]);
                        await new configModifier().edit("saveMdp", (string)App.Current.Properties["Mdp"]);
                    }).Start();
                }

                profilPanel profilPanel = (profilPanel)((System.Windows.FrameworkElement)this.Parent).Parent;
                Dispatcher.BeginInvoke(new profilPanel.refreshDelegate(profilPanel.refresh), DispatcherPriority.Render);
            } catch(Exception ex){}
            connexionInProgress = false;
        }
        private void stayConnectedClick(object sender, RoutedEventArgs e)
        {
            stayConnected = ((CheckBox)sender).IsChecked == true;
        }
    }
}

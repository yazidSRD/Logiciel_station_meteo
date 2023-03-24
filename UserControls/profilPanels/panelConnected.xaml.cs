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
    /// Logique d'interaction pour profilConnexion.xaml
    /// </summary>
    public partial class panelConnected : UserControl
    {
        public delegate void refreshDelegate();
     
        public panelConnected()
        {
            InitializeComponent();
            refresh();
            if ((string)App.Current.Properties["Droit"] != "1") buttonNewProfil.Visibility = Visibility.Hidden;
        }
        void refresh()
        {
            Nom.Text = (string)App.Current.Properties["Nom"];
            Prenom.Text = (string)App.Current.Properties["Prenom"];
            Identifiant.Text = (string)App.Current.Properties["Identifiant"];
            Tel.Text = (string)App.Current.Properties["Tel"];
            Fonction.Text = (string)App.Current.Properties["Fonction"];

            new Thread(refreshDataGrid).Start();
        }
        void refreshDataGrid()
        {
            List<Dictionary<string, string>> jsonData = new Http().getAllCompte((string)App.Current.Properties["Identifiant"], (string)App.Current.Properties["Mdp"]).Result;

            Dispatcher.BeginInvoke(new refreshDelegate(() => refreshDataGridDelegate(jsonData)), DispatcherPriority.Render);
        }
        void refreshDataGridDelegate(List<Dictionary<string, string>> jsonData)
        {
            dataGrid.Items.Clear();
            dataGrid.ItemsSource = jsonData;
        }
        private void deconnexionClick(object sender, RoutedEventArgs e)
        {
            new Thread(async () =>
            {
                await new configModifier().edit("saveIdentifiant", "");
                await new configModifier().edit("saveMdp", "");
            }).Start();
            App.Current.Properties["Nom"] = null;
            App.Current.Properties["Prenom"] = null;
            App.Current.Properties["Identifiant"] = null;
            App.Current.Properties["Tel"] = null;
            App.Current.Properties["Fonction"] = null;
            App.Current.Properties["Droit"] = null;
            App.Current.Properties["Mdp"] = null;
            profilPanel profilPanel = (profilPanel)((System.Windows.FrameworkElement)this.Parent).Parent;
            Dispatcher.BeginInvoke(new profilPanel.refreshDelegate(profilPanel.refresh), DispatcherPriority.Render);
        }
        private void modifMyProfilButton(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> profil = new Dictionary<string, string>();

            profil.Add("ID", (string)App.Current.Properties["ID"]);
            profil.Add("Nom", (string)App.Current.Properties["Nom"]);
            profil.Add("Prenom", (string)App.Current.Properties["Prenom"]);
            profil.Add("Identifiant", (string)App.Current.Properties["Identifiant"]);
            profil.Add("Tel", (string)App.Current.Properties["Tel"]);
            profil.Add("Fonction", (string)App.Current.Properties["Fonction"]);
            profil.Add("Droit", (string)App.Current.Properties["Droit"]);

            modifProfil(profil);
        }
        private void modifProfilButton(object sender, RoutedEventArgs e)
        {
            if ((string)App.Current.Properties["Droit"] == "0") return;
            var rowDictionary = ((System.Windows.FrameworkElement)sender).DataContext as Dictionary<string, string>;
            if (rowDictionary != null) modifProfil(rowDictionary);
        }
        private void modifProfil(Dictionary<string, string> profil)
        {
            modificationProfil popup = new modificationProfil(profil, false);
            popup.ShowDialog();
            if (popup.safeguard) new Thread(() => saveProfil(popup.profil)).Start();
            else if (popup.delete) new Thread(() => deleteProfil(popup.profil)).Start();
        }
        private void saveProfil(Dictionary<string, string> profil)
        {
            bool saved = new Http().editCompte((string)App.Current.Properties["Identifiant"], (string)App.Current.Properties["Mdp"], profil).Result;
            if (saved)
            {
                if (profil["ID"] == (string)App.Current.Properties["ID"] && profil["Mdp"] != null) App.Current.Properties["Mdp"] = profil["Mdp"];

                profilPanel profilPanel = (profilPanel)((System.Windows.FrameworkElement)this.Parent).Parent;
                Dispatcher.BeginInvoke(new profilPanel.refreshDelegate(profilPanel.refresh), DispatcherPriority.Render);
            }
        }
        private void deleteProfil(Dictionary<string, string> profil)
        {
            bool saved = new Http().deleteCompte((string)App.Current.Properties["Identifiant"], (string)App.Current.Properties["Mdp"], profil["ID"]).Result;
            if (saved)
            {
                profilPanel profilPanel = (profilPanel)((System.Windows.FrameworkElement)this.Parent).Parent;
                Dispatcher.BeginInvoke(new profilPanel.refreshDelegate(profilPanel.refresh), DispatcherPriority.Render);
            }
        }
        private void newProfilButton(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> profil = new Dictionary<string, string>();

            profil.Add("Nom", "");
            profil.Add("Prenom", "");
            profil.Add("Identifiant", "");
            profil.Add("Tel", "");
            profil.Add("Fonction", "");
            profil.Add("Droit", "");

            modificationProfil popup = new modificationProfil(profil, true);
            popup.ShowDialog();

            if (popup.safeguard) new Thread(() =>
            {
                bool saved = new Http().newCompte((string)App.Current.Properties["Identifiant"], (string)App.Current.Properties["Mdp"], profil).Result;
                if (saved)
                {
                    profilPanel profilPanel = (profilPanel)((System.Windows.FrameworkElement)this.Parent).Parent;
                    Dispatcher.BeginInvoke(new profilPanel.refreshDelegate(profilPanel.refresh), DispatcherPriority.Render);
                }
            }).Start();
        }
    }
}

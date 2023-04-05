﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using projet23_Station_météo_WPF.code;
using projet23_Station_météo_WPF.UserControls.profilPanels;
using System.Windows.Threading;

namespace projet23_Station_météo_WPF.UserControls
{
    /// <summary>
    /// Logique d'interaction pour profilPanel.xaml
    /// </summary>
    public partial class profilPanel : UserControl
    {
        public delegate void refreshDelegate();
        delegate void delegateMessageBox();
        public profilPanel()
        {
            InitializeComponent();
            refresh();
            /*App.Current.Properties["profil"] = new Dictionary<string, dynamic>();*/
        }
        async public void refresh()
        {
            ((Grid)this.Content).Children.Clear();
            if (App.Current.Properties["Identifiant"] != null)
            {
                Dictionary<string, string> jsonData = await new Http().getCompte((string)App.Current.Properties["Identifiant"], (string)App.Current.Properties["Mdp"]);

                if (jsonData == null)
                {
                    App.Current.Properties["Identifiant"] = null;
                    refresh();
                    return;
                }

                App.Current.Properties["ID"] = jsonData["ID"];
                App.Current.Properties["Nom"] = jsonData["Nom"];
                App.Current.Properties["Prenom"] = jsonData["Prenom"];
                App.Current.Properties["Tel"] = jsonData["Tel"];
                App.Current.Properties["Fonction"] = jsonData["Fonction"];
                App.Current.Properties["Droit"] = jsonData["Droit"];

                ((Grid)this.Content).Children.Add(new panelConnected());
            } else if (((string)App.Current.Properties["saveIdentifiant"]) != "")
            {
                Dictionary<string, string> jsonData = await new Http().getCompte(((string)App.Current.Properties["saveIdentifiant"]), ((string)App.Current.Properties["saveMdp"]));

                if (jsonData == null || jsonData.Count == 0)
                {
                    new Thread(async () =>
                    {
                        await new configModifier().edit("saveIdentifiant", "");
                        await new configModifier().edit("saveMdp", "");
                    }).Start();
                    refresh();
                    return;
                }

                App.Current.Properties["ID"] = jsonData["ID"];
                App.Current.Properties["Nom"] = jsonData["Nom"];
                App.Current.Properties["Prenom"] = jsonData["Prenom"];
                App.Current.Properties["Identifiant"] = jsonData["Identifiant"];
                App.Current.Properties["Tel"] = jsonData["Tel"];
                App.Current.Properties["Fonction"] = jsonData["Fonction"];
                App.Current.Properties["Droit"] = jsonData["Droit"];
                App.Current.Properties["Mdp"] = ((string)App.Current.Properties["saveMdp"]);

                ((Grid)this.Content).Children.Add(new panelConnected());
            }
            else
            {
                ((Grid)this.Content).Children.Add(new profilConnexion());
            }
        }
    }
}

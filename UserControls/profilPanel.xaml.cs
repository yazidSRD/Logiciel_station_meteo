// Importations de namespaces et classes externes
using System;
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
    /// Redirigez l'utilisateur vers la fenêtre de connexion ou pas en fonction de son statut de connexion
    /// </summary>
    public partial class profilPanel : UserControl
    {
        // Définition d'un délégué pour la mise à jour de l'interface utilisateur
        public delegate void refreshDelegate();
        delegate void delegateMessageBox();

        // Constructeur de la classe
        public profilPanel()
        {
            // Initialisation des composants
            InitializeComponent();

            // Appel de la méthode refresh
            refresh();
        }

        // Elle est utilisée pour identifier l'état de connexion de l'utilisateur et ainsi choisir la bonne fenêtre principale
        async public void refresh()
        {
            // L'ensemble des éléments enfants du panneau principal sont effacés afin de permettre leur rechargement.
            ((Grid)this.Content).Children.Clear();

            // Si un identifiant est stocké en mémoire, alors les informations de ce compte sont récupérées.
            if (App.Current.Properties["Identifiant"] != null)
            {
                Dictionary<string, string> jsonData = await new Http().getCompte((string)App.Current.Properties["Identifiant"], (string)App.Current.Properties["Mdp"]);

                // Si aucune donnée n'a pu être récupérée pour ce compte, l'identifiant est effacé pour demander une nouvelle connexion
                if (jsonData == null)
                {
                    App.Current.Properties["Identifiant"] = null;
                    refresh();
                    return;
                }

                // Si des données ont été récupérées pour le compte, les informations de l'utilisateur sont stockées
                App.Current.Properties["ID"] = jsonData["ID"];
                App.Current.Properties["Nom"] = jsonData["Nom"];
                App.Current.Properties["Prenom"] = jsonData["Prenom"];
                App.Current.Properties["Tel"] = jsonData["Tel"];
                App.Current.Properties["Fonction"] = jsonData["Fonction"];
                App.Current.Properties["Droit"] = jsonData["Droit"];

                // Une nouvelle fenêtre pour les utilisateurs connectés est affichée
                ((Grid)this.Content).Children.Add(new panelConnected());

            } else if (((string)App.Current.Properties["saveIdentifiant"]) != "")
            {
                // Sinon si l'identifiant et le mot de passe sont stockés sur l'ordinateur de l'utilisateur, sont récupérés


                Dictionary<string, string> jsonData = await new Http().getCompte(((string)App.Current.Properties["saveIdentifiant"]), ((string)App.Current.Properties["saveMdp"]));

                // Si aucune donnée n'a pu être récupérée pour ce compte, l'identifiant et le mot de passe stockés sur l'ordinateur sont effacés
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

                // Si des données ont été récupérées pour le compte, les informations de l'utilisateur sont stockées
                App.Current.Properties["ID"] = jsonData["ID"];
                App.Current.Properties["Nom"] = jsonData["Nom"];
                App.Current.Properties["Prenom"] = jsonData["Prenom"];
                App.Current.Properties["Identifiant"] = jsonData["Identifiant"];
                App.Current.Properties["Tel"] = jsonData["Tel"];
                App.Current.Properties["Fonction"] = jsonData["Fonction"];
                App.Current.Properties["Droit"] = jsonData["Droit"];
                App.Current.Properties["Mdp"] = ((string)App.Current.Properties["saveMdp"]);

                // Une nouvelle fenêtre pour les utilisateurs connectés est affichée
                ((Grid)this.Content).Children.Add(new panelConnected());
            }
            else
            {
                // Si aucun identifiant n'est stocké en mémoire et aucune sur l'ordinateur
                // Une nouvelle fenêtre pour la connexion est affichée
                ((Grid)this.Content).Children.Add(new profilConnexion());
            }
        }
    }
}

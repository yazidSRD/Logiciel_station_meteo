// Importations de namespaces et classes externes
using projet23_Station_météo_WPF.code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace projet23_Station_météo_WPF.UserControls
{
    /// <summary>
    ///  Cette fenêtre permet de configurer des paramètres du logiciel
    /// </summary>
    public partial class settingsPanel : UserControl
    {
        // Regex qui accepte que les chiffres
        private static readonly Regex _regex = new Regex("[^0-9]+");

        // Définition d'un délégué pour la mise à jour de l'interface utilisateur
        delegate void delegateMessageBox();

        // Constructeur de la classe
        public settingsPanel()
        {
            // Initialisation des composants
            InitializeComponent();

            // Actualisation des inforamtion de la fenêtre
            refresh();
        }

        // Cette méthode rafraîchit les données de la fenêtre
        async void refresh()
        {
            // Récupération des données de configuration stockées dans les propriétés de l'application
            serverIp.Text = ((string)App.Current.Properties["serverIp"]);
            refreshTimer.Text = ((string)App.Current.Properties["refreshTimer"]);

            // Création d'un nouveau thread pour effectuer une requête HTTP asynchrone et récupérer les seuils stockés sur le serveur
            new Thread(async () =>
            {
                // Appel de la méthode getSeuils() de la classe Http et stockage du résultat dans une liste de chaînes de caractères
                List<string> jsonData = new Http().getSeuils().Result;

                // Vérification que la réponse de la requête n'est pas nulle et que la liste de seuils est non vide
                if (jsonData != null && jsonData.Count > 0)
                {
                    // Stockage des seuils dans les propriétés de l'application et mise à jour des fichiers de configuration correspondants
                    App.Current.Properties["seuilLvl1"] = jsonData[0];
                    App.Current.Properties["seuilLvl2"] = jsonData[1];
                    await new configModifier().edit("seuilLvl1", jsonData[0]);
                    await new configModifier().edit("seuilLvl2", jsonData[1]);
                } else
                {
                    // Affichage d'un message d'erreur si la réponse de la requête est nulle ou si la liste de seuils est vide
                    Dispatcher.BeginInvoke(new delegateMessageBox(() => {
                        System.Windows.Forms.MessageBox.Show("Impossible de se connecter au server.\n\nIl est possible que:\n - Vous ne soyez pas connecté\n - Que le serveur ne soit pas connecté\n\nSi le problème persiste, veuillez contacter un administrateur.",
                        "Connexion erreur",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                    }), DispatcherPriority.Render);

                }
            }).Start();

            // Pause de 50ms
            System.Threading.Thread.Sleep(50);

            // Appel de deux autres méthodes de rafraîchissement
            refreshSeuils();
            refreshUnitCheck();
        }

        // Fonction pour rafraîchir les seuils affichés
        void refreshSeuils()
        {

            // Désactiver l'édition des seuils si l'utilisateur n'est pas un administrateur
            if (App.Current.Properties["Identifiant"] == null || (string)App.Current.Properties["Droit"] != "1") gridSeuils.IsEnabled = false;

            // Récupérer les seuils de vent, température, pluviométrie et UV et les afficher
            seuil1VitesseVent.Text = ((string)App.Current.Properties["seuilLvl1"]).Split(new char[] { ';' })[0];
            seuil2VitesseVent.Text = ((string)App.Current.Properties["seuilLvl2"]).Split(new char[] { ';' })[0];

            seuil1Temperature.Text = ((string)App.Current.Properties["seuilLvl1"]).Split(new char[] { ';' })[1];
            seuil2Temperature.Text = ((string)App.Current.Properties["seuilLvl2"]).Split(new char[] { ';' })[1];

            seuil1Pluviometrie.Text = ((string)App.Current.Properties["seuilLvl1"]).Split(new char[] { ';' })[2];
            seuil2Pluviometrie.Text = ((string)App.Current.Properties["seuilLvl2"]).Split(new char[] { ';' })[2];

            seuil1UV.Text = ((string)App.Current.Properties["seuilLvl1"]).Split(new char[] { ';' })[3];
            seuil2UV.Text = ((string)App.Current.Properties["seuilLvl2"]).Split(new char[] { ';' })[3];
        }


        // Cette méthode permet de rafraîchir la sélection des unités de mesure dans les cases à cocher des listes ListBox
        void refreshUnitCheck()
        {
            // Création d'une liste contenant les listes ListBox
            List<ListBox> checkList = new List<ListBox>() { unit1, unit2, unit3, unit4, unit5, unit6 };

            // Parcours de chaque ListBox de la liste checkList pour désélectionner toutes les cases à cocher et retirer les gestionnaires d'événements
            foreach (ListBox element in checkList)
            {
                foreach (CheckBox item in element.Items)
                {
                    item.IsChecked = false;
                    item.Checked -= editUnit;
                }
            }

            // Parcours de chaque ListBox de la liste checkList pour sélectionner les cases à cocher correspondant aux unités de mesure sélectionnées et ajouter les gestionnaires d'événements
            foreach (ListBox element in checkList)
            {
                foreach (CheckBox item in element.Items)
                {
                    // Vérification de l'unité de mesure sélectionnée pour l'élément de la case à cocher et sélection de la case à cocher si l'unité de mesure correspond.
                    if ((string)item.Content == ((string)App.Current.Properties["unitDvent"]))
                    {
                        item.IsChecked = true;
                    }
                    else if ((string)item.Content == ((string)App.Current.Properties["unitHygro"]) || ((string)item.Content == "Point de rosée(°C)" && "pr" == ((string)App.Current.Properties["unitHygro"])))
                    {
                        item.IsChecked = true;
                    }
                    else if ((string)item.Content == ((string)App.Current.Properties["unitPluv"]))
                    {
                        item.IsChecked = true;
                    }
                    else if ((string)item.Content == ((string)App.Current.Properties["unitPresAtmo"]))
                    {
                        item.IsChecked = true;
                    }
                    else if ((string)item.Content == ((string)App.Current.Properties["unitRaySol"]))
                    {
                        item.IsChecked = true;
                    }
                    else if ((string)item.Content == ((string)App.Current.Properties["unitTemp"]))
                    {
                        item.IsChecked = true;
                    }
                    else if ((string)item.Content == ((string)App.Current.Properties["unitVvent"]) || ((string)item.Content == "nœuds(nd)" && "nd" == ((string)App.Current.Properties["unitVvent"])))
                    {
                        item.IsChecked = true;
                    }
                    else
                    {
                        item.IsChecked = false;
                    }
                    item.Checked += editUnit;
                }
            }
        }

        // Cette méthode est appelée lorsque le texte est modifié dans la zone de texte "serverIp"
        private void serverIp_TextChanged(object sender, KeyboardFocusChangedEventArgs e)
        {
            new configModifier().edit("serverIp", serverIp.Text);
        }

        // Cette fonction est appelée lorsque le texte est modifié dans la zone de texte "refreshTimer"
        private void refreshTimer_TextChanged(object sender, KeyboardFocusChangedEventArgs e)
        {
            new configModifier().edit("refreshTimer", refreshTimer.Text);
        }

        // Cette fonction est appelée lorsque le texte est modifié dans la zone de texte "seuils"
        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        // Cette méthode vérifie si le texte saisi est valide
        private static bool IsTextAllowed(string text)
        {
            // Retourne true si le texte ne contient pas de caractères invalides selon la regex spécifiée
            return !_regex.IsMatch(text);
        }

        // Cette fonction est appelée lorsque la zone de texte d'édition de seuil perd le focus
        private void editSeuil_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // Modifie le seuil correspondant sur le serveur à l'aide de la fonction editSeuil() de la classe Http
            new Http().editSeuil((string)App.Current.Properties["Identifiant"], (string)App.Current.Properties["Mdp"], ((string)((TextBox)sender).Tag).Split(new char[] { ';' })[0], ((string)((TextBox)sender).Tag).Split(new char[] { ';' })[1], Convert.ToInt32(((TextBox)sender).Text));
        }

        // Modifie le seuil correspondant sur le serveur à l'aide de la fonction editSeuil() de la classe Http
        private async void editUnit(object sender, RoutedEventArgs e)
        {
            string t = (string)((CheckBox)sender).Content;
            if (t == "Point de rosée(°C)") t = "pr";
            else if (t == "nœuds(nd)") t = "nd";

            // Modifie l'unité correspondante dans les propriétés de l'application en utilisant la classe configModifier()
            await new configModifier().edit((string)((ListBox)((CheckBox)sender).Parent).Tag, t);

            // Rafraîchit la liste de vérification des unités
            refreshUnitCheck();
        }
    }
}

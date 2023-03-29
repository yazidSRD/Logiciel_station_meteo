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
    /// Logique d'interaction pour settingsPanel.xaml
    /// </summary>
    public partial class settingsPanel : UserControl
    {
        private static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text
        delegate void delegateMessageBox();

        public settingsPanel()
        {
            InitializeComponent();
            refresh();
        }
        async void refresh()
        {

            serverIp.Text = ((string)App.Current.Properties["serverIp"]);
            refreshTimer.Text = ((string)App.Current.Properties["refreshTimer"]);

            new Thread(async () =>
            {
                List<string> jsonData = new Http().getSeuils().Result;

                if (jsonData != null && jsonData.Count > 0)
                {
                    App.Current.Properties["seuilLvl1"] = jsonData[0];
                    App.Current.Properties["seuilLvl2"] = jsonData[1];
                    await new configModifier().edit("seuilLvl1", jsonData[0]);
                    await new configModifier().edit("seuilLvl2", jsonData[1]);
                } else
                {
                    Dispatcher.BeginInvoke(new delegateMessageBox(() => {
                        System.Windows.Forms.MessageBox.Show("dddImpossible de se connecter au server.\n\nIl est possible que:\n - Vous ne soyez pas connecté\n - Que le serveur ne soit pas connecté\n\nSi le problème persiste, veuillez contacter un administrateur.",
                        "Connexion erreur",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                    }), DispatcherPriority.Render);

                }
            }).Start();
            
            System.Threading.Thread.Sleep(50);

            refreshSeuils();
            refreshUnitCheck();
        }
        void refreshSeuils()
        {
            if (App.Current.Properties["Identifiant"] == null || (string)App.Current.Properties["Droit"] != "1") gridSeuils.IsEnabled = false;
            seuil1VitesseVent.Text = ((string)App.Current.Properties["seuilLvl1"]).Split(new char[] { ';' })[0];
            seuil2VitesseVent.Text = ((string)App.Current.Properties["seuilLvl2"]).Split(new char[] { ';' })[0];

            seuil1Temperature.Text = ((string)App.Current.Properties["seuilLvl1"]).Split(new char[] { ';' })[1];
            seuil2Temperature.Text = ((string)App.Current.Properties["seuilLvl2"]).Split(new char[] { ';' })[1];

            seuil1Pluviometrie.Text = ((string)App.Current.Properties["seuilLvl1"]).Split(new char[] { ';' })[2];
            seuil2Pluviometrie.Text = ((string)App.Current.Properties["seuilLvl2"]).Split(new char[] { ';' })[2];

            seuil1UV.Text = ((string)App.Current.Properties["seuilLvl1"]).Split(new char[] { ';' })[3];
            seuil2UV.Text = ((string)App.Current.Properties["seuilLvl2"]).Split(new char[] { ';' })[3];
        }
        void refreshUnitCheck()
        {
            List<ListBox> checkList = new List<ListBox>() { unit1, unit2, unit3, unit4, unit5, unit6 };

            foreach (ListBox element in checkList)
            {
                foreach (CheckBox item in element.Items)
                {
                    item.IsChecked = false;
                    item.Checked -= editUnit;
                }
            }

            foreach (ListBox element in checkList)
            {
                foreach (CheckBox item in element.Items)
                {
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
        private void serverIp_TextChanged(object sender, KeyboardFocusChangedEventArgs e)
        {
            new configModifier().edit("serverIp", serverIp.Text);
        }
        private void refreshTimer_TextChanged(object sender, KeyboardFocusChangedEventArgs e)
        {
            new configModifier().edit("refreshTimer", refreshTimer.Text);
        }
        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        private void editSeuil_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            new Http().editSeuil((string)App.Current.Properties["Identifiant"], (string)App.Current.Properties["Mdp"], ((string)((TextBox)sender).Tag).Split(new char[] { ';' })[0], ((string)((TextBox)sender).Tag).Split(new char[] { ';' })[1], Convert.ToInt32(((TextBox)sender).Text));
        }
        private async void editUnit(object sender, RoutedEventArgs e)
        {
            string t = (string)((CheckBox)sender).Content;
            if (t == "Point de rosée(°C)") t = "pr";
            else if (t == "nœuds(nd)") t = "nd";
            await new configModifier().edit((string)((ListBox)((CheckBox)sender).Parent).Tag, t);
            refreshUnitCheck();
        }
    }
}

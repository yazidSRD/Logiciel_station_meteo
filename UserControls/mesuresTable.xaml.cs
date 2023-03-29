using MySqlX.XDevAPI.Relational;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace projet23_Station_météo_WPF.UserControls
{
    /// <summary>
    /// Logique d'interaction pour mesuresTable.xaml
    /// </summary>
    public partial class mesuresTable : System.Windows.Controls.UserControl
    {
        public delegate void refreshDelegate();
        delegate void delegateMessageBox();
        public mesuresTable()
        {
            InitializeComponent();
            dataGrid.Columns[1].Header += "(" + (string)App.Current.Properties["unitTemp"] + ")";
            dataGrid.Columns[2].Header += "(" + (string)App.Current.Properties["unitHygro"] + ")";
            dataGrid.Columns[3].Header += "(" + (string)App.Current.Properties["unitVvent"] + ")";
            dataGrid.Columns[4].Header += "(" + (string)App.Current.Properties["unitDvent"] + ")";
            dataGrid.Columns[5].Header += "(" + (string)App.Current.Properties["unitPresAtmo"] + ")";
            dataGrid.Columns[6].Header += "(" + (string)App.Current.Properties["unitPluv"] + ")";
            dataGrid.Columns[7].Header += "(" + (string)App.Current.Properties["unitRaySol"] + ")";
            Button_Click_Request(null, null);
        }
        void refreshDataGrid(string sqlText)
        {
            try
            {
                List<Dictionary<string, string>> jsonData = new Http().get(sqlText).Result;
                if (jsonData == null) {
                    Dispatcher.BeginInvoke(new delegateMessageBox(() => {
                        System.Windows.Forms.MessageBox.Show("Impossible de se connecter au server.\n\nIl est possible que:\n - Vous ne soyez pas connecté\n - Que le serveur ne soit pas connecté\n\nSi le problème persiste, veuillez contacter un administrateur.",
                        "Connexion erreur",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                    }), DispatcherPriority.Render);
                    return;
                };
                Dispatcher.BeginInvoke(new refreshDelegate(() => refreshDataGridDelegate(jsonData)), DispatcherPriority.Render);
            }
            catch (Exception ex) { return; }
        }
        void refreshDataGridDelegate(List<Dictionary<string, string>> jsonData)
        {
            dataGrid.ItemsSource = jsonData;
        }
        void Button_Click_Request(object sender, RoutedEventArgs e)
        {
            string sqlText = sql.Text;
            new Thread(() =>
            {
                refreshDataGrid(sqlText);
            }).Start();
        }
        void Button_Click_Download(object sender, RoutedEventArgs e)
        {
            string contenu = "<?xml version='1.0' encoding='UTF-8'?>\n";
            
            contenu += "<mesures>\n";

            foreach (Dictionary<string, string> item in dataGrid.Items)
            {
                contenu += "\t<mesure>\n";
                contenu += "\t\t<DateHeureReleve>"+ item["DateHeureReleve"] + "</DateHeureReleve>\n";
                contenu += "\t\t<Temperature>" + item["Temperature"] + "</Temperature>\n";
                contenu += "\t\t<Hygrometrie>" + item["Hygrometrie"] + "</Hygrometrie>\n";
                contenu += "\t\t<VitesseVent>" + item["VitesseVent"] + "</VitesseVent>\n";
                contenu += "\t\t<DirectionVent>" + item["DirectionVent"] + "</DirectionVent>\n";
                contenu += "\t\t<PressionAtmospherique>" + item["PressionAtmospherique"] + "</PressionAtmospherique>\n";
                contenu += "\t\t<Pluviometre>" + item["Pluviometre"] + "</Pluviometre>\n";
                contenu += "\t\t<RayonnementSolaire>" + item["RayonnementSolaire"] + "</RayonnementSolaire>\n";
                contenu += "\t</mesure>\n";
            }

            contenu += "</mesures>";

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "langage de balisage extensible (*.xml)|*.xml";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = File.CreateText(saveFileDialog.FileName))
                {
                    sw.Write(contenu);
                }

                Console.WriteLine("Le fichier texte a été créé avec succès et enregistré à l'emplacement suivant : " + saveFileDialog.FileName);
            }
        }
    }
}

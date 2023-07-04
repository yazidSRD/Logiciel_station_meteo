// Importations de namespaces et classes externes
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
using projet23_Station_météo_WPF.code;

namespace projet23_Station_météo_WPF.UserControls
{
    /// <summary>
    /// Permet de voir toutes les données dans un tableau et de les télécharger en XML
    /// </summary>
    public partial class mesuresTable : System.Windows.Controls.UserControl
    {
        // Définition d'un délégué pour la mise à jour de l'interface utilisateur
        public delegate void refreshDelegate();
        delegate void delegateMessageBox();

        // Thread pour le téléchargement des données en XML
        Thread threadDownloadInToXML;

        // Constructeur de la classe
        public mesuresTable()
        {
            // Initialisation des composants
            InitializeComponent();

            // Ajout des unités de mesure aux titres des colonnes du datagrid
            dataGrid.Columns[1].Header += "(" + (string)App.Current.Properties["unitTemp"] + ")";
            dataGrid.Columns[2].Header += "(" + (string)App.Current.Properties["unitHygro"] + ")";
            dataGrid.Columns[3].Header += "(" + (string)App.Current.Properties["unitVvent"] + ")";
            dataGrid.Columns[4].Header += "(" + (string)App.Current.Properties["unitDvent"] + ")";
            dataGrid.Columns[5].Header += "(" + (string)App.Current.Properties["unitPresAtmo"] + ")";
            dataGrid.Columns[6].Header += "(" + (string)App.Current.Properties["unitPluv"] + ")";
            dataGrid.Columns[7].Header += "(" + (string)App.Current.Properties["unitRaySol"] + ")";

            // Appel de la méthode Button_Click_Request
            Button_Click_Request(null, null);
        }
        void refreshDataGrid()
        {
            try
            {
                // Appel à la méthode get() de la classe Http pour récupérer toutes les données du serveur
                List<Dictionary<string, string>> jsonData = new Http().get("ORDER BY DateHeureReleve DESC").Result;

                // Si la réponse est nulle, afficher un message d'erreur et arrêter le chargement
                if (jsonData == null) {
                    Dispatcher.BeginInvoke(new delegateMessageBox(() => {
                        System.Windows.Forms.MessageBox.Show("Impossible de se connecter au server.\n\nIl est possible que:\n - Vous ne soyez pas connecté\n - Que le serveur ne soit pas connecté\n\nSi le problème persiste, veuillez contacter un administrateur.",
                        "Connexion erreur",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                    }), DispatcherPriority.Render);
                    loadingBar.stop();
                    return;
                };

                // Mettre à jour le DataGrid en invoquant la méthode refreshDataGridDelegate avec les données récupérées
                Dispatcher.BeginInvoke(new refreshDelegate(() => refreshDataGridDelegate(jsonData)), DispatcherPriority.Render);
            }
            catch (Exception ex) {
                Console.Write(ex);}
            
            // Arrêter le chargement
            loadingBar.stop();
        }

        // On met à jour les valeurs dans le tableau
        void refreshDataGridDelegate(List<Dictionary<string, string>> jsonData)
        {
            dataGrid.ItemsSource = jsonData;
        }

        // Appeler leur du clic sur le bouton de chargement des données
        void Button_Click_Request(object sender, RoutedEventArgs e)
        {
            // On lance la barre de chargement
            loadingBar.start(true, true, "téléchargement");

            // Démarrage d'un nouveau thread pour effectuer des opérations de réseau
            new Thread(() =>
            {
                refreshDataGrid();
            }).Start();
        }

        // Appeler leur du clic sur le bouton de téléchargemnt des données en XML
        void Button_Click_Download(object sender, RoutedEventArgs e)
        {
            // Création d'un objet SaveFileDialog pour permettre à l'utilisateur de choisir l'emplacement et le nom du fichier XML
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "langage de balisage extensible (*.xml)|*.xml";

            // Si l'utilisateur a cliqué sur le bouton "OK" pour enregistrer le fichier
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Création d'un nouveau thread pour exécuter la fonction downloadInToXML() avec le nom du fichier choisi par l'utilisateur en tant que paramètre
                threadDownloadInToXML = new Thread(new ParameterizedThreadStart(downloadInToXML));
                threadDownloadInToXML.Start(saveFileDialog.FileName);
            }
        }
        void downloadInToXML(object path)
        {
            // Démarrage de la barre de progression
            loadingBar.start(true, true, "conversion en cours", true);
            Thread.Sleep(100);

            // Ouverture du fichier en écriture
            using (StreamWriter sw = File.CreateText((string)path))
            {
                // Ajout de l'en-tête XML
                string contenu = "<?xml version='1.0' encoding='UTF-8'?>\n";

                // Ajout de la balise d'ouverture pour les mesures
                contenu += "<mesures>\n";
                sw.Write(contenu);

                int total = 0;
                int last = 0;
                int x = 0;
                contenu = "";

                // Boucle à travers chaque élément dans la grille de données
                foreach (Dictionary<string, string> item in dataGrid.Items)
                {
                    // Si l'utilisateur a annulé le téléchargement, sortir de la boucle
                    if (loadingBar.annul)
                    {
                        break;
                    }

                    // Écrire d'une mesure à la fois pour être plus rapide
                    if (++x > 1)
                    {
                        sw.Write(contenu);
                        contenu = "";
                        x = 0;
                    }

                    // Mettre à jour la barre de progression
                    total++;
                    if ((total*100/dataGrid.Items.Count) - last >= 5)
                    {
                        last = (int)(total * 100 / dataGrid.Items.Count);
                        loadingBar.refresh(last);
                    }

                    // Ajout de chaque mesure au contenu XML
                    contenu += "\t<mesure>\n";
                    contenu += "\t\t<DateHeureReleve>" + item["DateHeureReleve"] + "</DateHeureReleve>\n";
                    contenu += "\t\t<Temperature>" + item["Temperature"] + "</Temperature>\n";
                    contenu += "\t\t<Hygrometrie>" + item["Hygrometrie"] + "</Hygrometrie>\n";
                    contenu += "\t\t<VitesseVent>" + item["VitesseVent"] + "</VitesseVent>\n";
                    contenu += "\t\t<DirectionVent>" + item["DirectionVent"] + "</DirectionVent>\n";
                    contenu += "\t\t<PressionAtmospherique>" + item["PressionAtmospherique"] + "</PressionAtmospherique>\n";
                    contenu += "\t\t<Pluviometre>" + item["Pluviometre"] + "</Pluviometre>\n";
                    contenu += "\t\t<RayonnementSolaire>" + item["RayonnementSolaire"] + "</RayonnementSolaire>\n";
                    contenu += "\t</mesure>\n";
                }

                // Écrire le contenu final dans le fichier XML
                if (x != 0) sw.Write(contenu);
                contenu = "</mesures>";
                sw.Write(contenu);
            }

            // Si l'utilisateur a annulé le téléchargement, supprimer le fichier XML incomplet
            if (loadingBar.annul) 
            try
            {
                File.Delete((string)path);
            } catch (IOException ex) {
                Console.WriteLine(ex);
            }

            // Arrêter la barre de progression
            loadingBar.stop();
        }
    }
}

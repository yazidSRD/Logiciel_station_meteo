// Importations de namespaces et classes externes
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Threading;
using System.Windows.Forms;
using LiveCharts.Maps;
using System.Windows.Threading;
using projet23_Station_météo_WPF.code;

namespace projet23_Station_météo_WPF.UserControls
{
    /// <summary>
    /// Afficher les diverses données sur un seul graphique
    /// </summary>
    public partial class oneGraph : System.Windows.Controls.UserControl
    {
        // Période durant laquelle des données sont disponibles
        DateTime maxDate;
        DateTime minDate;

        // Définition d'un délégué pour la mise à jour de l'interface utilisateur
        delegate void refreshDelegate();
        delegate void delegateMessageBox();

        // Constructeur de la classe
        public oneGraph()
        {
            // Initialisation des composants
            InitializeComponent();

            // Met à jour le format de date
            graph.graph.date = "yyyy-MM-dd HH:mm";

            // Démarrage d'un nouveau thread pour effectuer des opérations de réseau
            new Thread(() =>
            {
                // Récupération des dates min et max depuis le serveur
                string mindate = new Http().getDate("MIN").Result;
                string maxdate = new Http().getDate("MAX").Result;

                // Vérification que les dates ont bien été récupérées
                if (mindate == "" || maxdate == "")
                {
                    // Si une erreur de connexion est survenue, afficher une boîte de dialogue
                    Dispatcher.BeginInvoke(new delegateMessageBox(() => {
                        System.Windows.Forms.MessageBox.Show("Impossible de se connecter au server.\n\nIl est possible que:\n - Vous ne soyez pas connecté\n - Que le serveur ne soit pas connecté\n\nSi le problème persiste, veuillez contacter un administrateur.",
                        "Connexion erreur",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                    }), DispatcherPriority.Render);
                }

                // Tentative de conversion des dates récupérées en objets DateTime
                try
                {
                    minDate = DateTime.Parse(mindate);
                }
                catch { }
                try
                {
                    maxDate = DateTime.Parse(maxdate);
                }
                catch
                {
                    // Si la date max n'a pas pu être convertie, utiliser la date actuelle
                    minDate = DateTime.Now;
                }

                // Actualisation de l'interface graphique
                Dispatcher.BeginInvoke(new refreshDelegate(refreshUi), DispatcherPriority.Render);
            }).Start();
        }

        // Méthode pour actualisation de l'interface graphique
        public void refreshUi()
        {
            // Configuration des bornes de date pour les sélecteurs de date
            startDate.DisplayDateStart = minDate;
            startDate.DisplayDateEnd = maxDate;
            endDate.DisplayDateStart = minDate;
            endDate.DisplayDateEnd = maxDate;
        }

        // Méthode qui permet de vérifier si les dates sélectionnées par l'utilisateur sont valides
        void refrshDate(object sender, RoutedEventArgs e)
        {
            // Vérification si les deux dates sont sélectionnées
            if (startDate.SelectedDate != null && endDate.SelectedDate != null)
            {
                // Vérification si la date sélectionnée est la date de début
                if (((DatePicker)sender).SelectedDate == startDate.SelectedDate)
                    // Vérification si la date de début est postérieure à la date de fin, si oui, on met la date de fin à la date de début
                    if (startDate.SelectedDate > endDate.SelectedDate) endDate.SelectedDate = startDate.SelectedDate;
                // Vérification si la date sélectionnée est la date de fin
                if (((DatePicker)sender).SelectedDate == endDate.SelectedDate)
                    // Vérification si la date de fin est antérieure à la date de début, si oui, on met la date de début à la date de fin
                    if (startDate.SelectedDate > endDate.SelectedDate) startDate.SelectedDate = endDate.SelectedDate;
            };
        }

        // Cette méthode est appelée lorsque le bouton de recherche de données est cliqué
        // Elle vérifie que les dates de début et de fin ont été sélectionnées et récupère leurs valeurs
        // En fonction du type de recherche sélectionné dans la liste déroulante, la méthode ajuste les dates de début et de fin pour qu'elles correspondent à la plage de temps appropriée
        // Elle lance ensuite une recherche de données dans un nouveau thread
        void dataSearch(object sender, RoutedEventArgs e)
        {
            // Vérification si les deux dates sont sélectionnées
            if (startDate.SelectedDate != null && endDate.SelectedDate != null)
            {
                // Vérification si les deux dates sont sélectionnées
                string sDate = startDate.SelectedDate.ToString();
                string eDate = endDate.SelectedDate.ToString();
                int index = type.SelectedIndex;

                // Ajustement des dates en fonction du type de recherche sélectionné
                switch (index)
                {
                    // Année
                    case 9:
                    case 8:
                    case 7:
                        graph.graph.date = "yyyy";
                        sDate = DateTime.Parse(sDate).ToString("yyyy") + "-01-01";
                        eDate = DateTime.Parse(eDate).ToString("yyyy") + "-12-" + DateTime.DaysInMonth(DateTime.Parse(eDate).Year, DateTime.Parse(eDate).Month);
                        break;
                    // Mois
                    case 6:
                    case 5:
                    case 4:
                        graph.graph.date = "yyyy-MM";
                        sDate = DateTime.Parse(sDate).ToString("yyyy-MM") + "-01";
                        eDate = DateTime.Parse(eDate).ToString("yyyy-MM") + "-" + DateTime.DaysInMonth(DateTime.Parse(eDate).Year, DateTime.Parse(eDate).Month);
                        break;
                    // Jour
                    case 3:
                    case 2:
                    case 1:
                        graph.graph.date = "yyyy-MM-dd";
                        sDate = DateTime.Parse(sDate).ToString("yyyy-MM-dd");
                        eDate = DateTime.Parse(eDate).ToString("yyyy-MM-dd");
                        break;
                    // Heure
                    case 0:
                    default:
                        graph.graph.date = "yyyy-MM-dd HH:mm";
                        sDate = DateTime.Parse(sDate).ToString("yyyy-MM-dd");
                        eDate = DateTime.Parse(eDate).ToString("yyyy-MM-dd");
                        break;
                }

                // Mise à jour des dates de début et de fin avec les nouvelles valeurs
                startDate.SelectedDate = DateTime.Parse(sDate);
                endDate.SelectedDate = DateTime.Parse(eDate);

                // Lancement de la recherche de données dans un nouveau thread
                new Thread(() =>
                {
                    dataSearch(sDate + " 00:00:00", eDate + " 23:59:59.9", index);
                }).Start();
            }
        }

        // Cette méthode permet de récupérer les données météorologiques comprises entre deux dates spécifiées et de les afficher dans le graphique
        void dataSearch(string startDate, string endDate, int index)
        {
            // On lance la barre de chargement
            loadingBar.start(false, true);

            // On effectue une requête HTTP pour récupérer les données météorologiques
            List<Dictionary<string, string>> jsonData = new Http().get("WHERE DateHeureReleve BETWEEN '" + startDate + "' AND '" + endDate + "' ORDER BY DateHeureReleve DESC").Result;

            // Si la requête HTTP échoue, on affiche un message d'erreur
            if (jsonData == null) {
                Dispatcher.BeginInvoke(new delegateMessageBox(() => {
                    System.Windows.Forms.MessageBox.Show("Impossible de se connecter au server.\n\nIl est possible que:\n - Vous ne soyez pas connecté\n - Que le serveur ne soit pas connecté\n\nSi le problème persiste, veuillez contacter un administrateur.",
                    "Connexion erreur",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                }), DispatcherPriority.Render);

                // On arrête la barre de chargement et on quitte la méthode
                loadingBar.stop();
                return;
            };

            // On crée un dictionnaire pour stocker les données récupérées par type de mesure
            Dictionary<string, List<Int32>> listData = new Dictionary<string, List<Int32>>()
                {
                    {"Temperature", new List<Int32>()},
                    {"Hygrometrie", new List<Int32>()},
                    {"VitesseVent", new List<Int32>()},
                    {"PressionAtmospherique", new List<Int32>()},
                    {"Pluviometre", new List<Int32>()},
                    {"DirectionVent", new List<Int32>()},
                    {"RayonnementSolaire", new List<Int32>()}
                };

            // On crée une liste pour stocker les dates des relevés météorologiques
            List<string> listDate = new List<string>();

            // On récupère toutes les données dans la plage de dates spécifiée en fonction d'index
            if (index == 0)
            {
                if (jsonData.Count > 432/7)
                {
                    DialogResult result = System.Windows.Forms.MessageBox.Show("Vous sélectionnez beaucoup de données(" + jsonData.Count*7 + "), il peut il y avoir de forts ralentissments.\nÉtês-vous sûr de vouloir continuer?", "ATTENTION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        System.Windows.Forms.MessageBox.Show("À vos risques et périls.", "ATTENTION", MessageBoxButtons.OK);
                    }
                    else
                    {
                        loadingBar.stop();
                        return;
                    }
                }
                foreach (Dictionary<string, string> relevemeteo in jsonData)
                {
                    foreach (var userControl in listData)
                    {
                        listData[userControl.Key].Add(Convert.ToInt32(relevemeteo[userControl.Key]));
                    }
                    listDate.Add(relevemeteo["DateHeureReleve"]);
                }
            }
            else if (index == 1)
            {
                if (jsonData.Count > 17520/7)
                {
                    DialogResult result = System.Windows.Forms.MessageBox.Show("Vous sélectionnez beaucoup de données(" + jsonData.Count*7 + "), il peut il y avoir de forts ralentissments.\nÉtês-vous sûr de vouloir continuer?", "ATTENTION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        System.Windows.Forms.MessageBox.Show("À vos risques et périls.", "ATTENTION", MessageBoxButtons.OK);
                    }
                    else
                    {
                        loadingBar.stop();
                        return;
                    }
                }
                string date = "2000-01-01 00:00:00";
                Dictionary<string, Int32> sommeValue = new Dictionary<string, Int32>();
                int nValue = 0;
                foreach (Dictionary<string, string> relevemeteo in jsonData)
                {
                    if (DateTime.Parse(date).ToString("yyyy-MM-dd") != DateTime.Parse(relevemeteo["DateHeureReleve"]).ToString("yyyy-MM-dd"))
                    {
                        if (nValue != 0)
                        {
                            foreach (var userControl in listData)
                            {
                                listData[userControl.Key].Add(sommeValue[userControl.Key] / nValue);
                            }
                            listDate.Add(date);
                        }
                        date = relevemeteo["DateHeureReleve"];
                        foreach (var userControl in listData)
                        {
                            sommeValue[userControl.Key] = 0;
                        }
                        nValue = 0;
                    }
                    nValue++;
                    foreach (var userControl in listData)
                    {
                        sommeValue[userControl.Key] += Convert.ToInt32(relevemeteo[userControl.Key]);
                    }
                }
                if (nValue != 0)
                {
                    foreach (var userControl in listData)
                    {
                        listData[userControl.Key].Add(sommeValue[userControl.Key] / nValue);
                    }
                    listDate.Add(date);
                }
            }
            else if (index == 2)
            {
                if (jsonData.Count > 17520/7)
                {
                    DialogResult result = System.Windows.Forms.MessageBox.Show("Vous sélectionnez beaucoup de données(" + jsonData.Count*7 + "), il peut il y avoir de forts ralentissments.\nÉtês-vous sûr de vouloir continuer?", "ATTENTION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        System.Windows.Forms.MessageBox.Show("À vos risques et périls.", "ATTENTION", MessageBoxButtons.OK);
                    }
                    else
                    {
                        loadingBar.stop();
                        return;
                    }
                }
                string date = "2000-01-01 00:00:00";
                Dictionary<string, double> minValues = new Dictionary<string, double>();
                bool flag = false;
                foreach (Dictionary<string, string> relevemeteo in jsonData)
                {
                    if (DateTime.Parse(date).ToString("yyyy-MM-dd") != DateTime.Parse(relevemeteo["DateHeureReleve"]).ToString("yyyy-MM-dd"))
                    {
                        if (flag)
                        {
                            foreach (var userControl in listData)
                            {
                                listData[userControl.Key].Add(Convert.ToInt32(minValues[userControl.Key]));
                            }
                            listDate.Add(date);
                        }
                        date = relevemeteo["DateHeureReleve"];
                        foreach (var userControl in listData)
                        {
                            minValues[userControl.Key] = double.PositiveInfinity;
                        }
                        flag = false;
                    }
                    foreach (var userControl in listData)
                    {
                        if (minValues[userControl.Key] > Convert.ToInt32(relevemeteo[userControl.Key]))
                        {
                            minValues[userControl.Key] = Convert.ToInt32(relevemeteo[userControl.Key]);
                            flag = true;
                        };
                    }
                }
                if (flag)
                {
                    foreach (var userControl in listData)
                    {
                        listData[userControl.Key].Add(Convert.ToInt32(minValues[userControl.Key]));
                    }
                    listDate.Add(date);
                }
            }
            else if (index == 3)
            {
                if (jsonData.Count > 17520/7)
                {
                    DialogResult result = System.Windows.Forms.MessageBox.Show("Vous sélectionnez beaucoup de données(" + jsonData.Count*7 + "), il peut il y avoir de forts ralentissments.\nÉtês-vous sûr de vouloir continuer?", "ATTENTION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        System.Windows.Forms.MessageBox.Show("À vos risques et périls.", "ATTENTION", MessageBoxButtons.OK);
                    }
                    else
                    {
                        loadingBar.stop();
                        return;
                    }
                }
                string date = "2000-01-01 00:00:00";
                Dictionary<string, double> mixValues = new Dictionary<string, double>();
                bool flag = false;
                foreach (Dictionary<string, string> relevemeteo in jsonData)
                {
                    if (DateTime.Parse(date).ToString("yyyy-MM-dd") != DateTime.Parse(relevemeteo["DateHeureReleve"]).ToString("yyyy-MM-dd"))
                    {
                        if (flag)
                        {
                            foreach (var userControl in listData)
                            {
                                listData[userControl.Key].Add(Convert.ToInt32(mixValues[userControl.Key]));
                            }
                            listDate.Add(date);
                        }
                        date = relevemeteo["DateHeureReleve"];
                        foreach (var userControl in listData)
                        {
                            mixValues[userControl.Key] = double.NegativeInfinity;
                        }
                        flag = false;
                    }
                    foreach (var userControl in listData)
                    {
                        if (mixValues[userControl.Key] < Convert.ToInt32(relevemeteo[userControl.Key]))
                        {
                            mixValues[userControl.Key] = Convert.ToInt32(relevemeteo[userControl.Key]);
                            flag = true;
                        };
                    }
                }
                if (flag)
                {
                    foreach (var userControl in listData)
                    {
                        listData[userControl.Key].Add(Convert.ToInt32(mixValues[userControl.Key]));
                    }
                    listDate.Add(date);
                }
            }
            else if (index == 4)
            {
                if (jsonData.Count > 525600/7)
                {
                    DialogResult result = System.Windows.Forms.MessageBox.Show("Vous sélectionnez beaucoup de données(" + jsonData.Count*7 + "), il peut il y avoir de forts ralentissments.\nÉtês-vous sûr de vouloir continuer?", "ATTENTION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        System.Windows.Forms.MessageBox.Show("À vos risques et périls.", "ATTENTION", MessageBoxButtons.OK);
                    }
                    else
                    {
                        loadingBar.stop();
                        return;
                    }
                }
                string date = "2000-01-01 00:00:00";
                Dictionary<string, Int32> sommeValue = new Dictionary<string, Int32>();
                int nValue = 0;
                foreach (Dictionary<string, string> relevemeteo in jsonData)
                {
                    if (DateTime.Parse(date).ToString("yyyy-MM") != DateTime.Parse(relevemeteo["DateHeureReleve"]).ToString("yyyy-MM"))
                    {
                        if (nValue != 0)
                        {
                            foreach (var userControl in listData)
                            {
                                listData[userControl.Key].Add(sommeValue[userControl.Key] / nValue);
                            }
                            listDate.Add(date);
                        }
                        date = relevemeteo["DateHeureReleve"];
                        foreach (var userControl in listData)
                        {
                            Console.WriteLine(userControl.Key);
                            sommeValue[userControl.Key] = 0;
                        }
                        nValue = 0;
                    }
                    nValue++;
                    foreach (var userControl in listData)
                    {
                        sommeValue[userControl.Key] += Convert.ToInt32(relevemeteo[userControl.Key]);
                    }
                }
                if (nValue != 0)
                {
                    foreach (var userControl in listData)
                    {
                        listData[userControl.Key].Add(sommeValue[userControl.Key] / nValue);
                    }
                    listDate.Add(date);
                }
            }
            else if (index == 5)
            {
                if (jsonData.Count > 525600 / 7)
                {
                    DialogResult result = System.Windows.Forms.MessageBox.Show("Vous sélectionnez beaucoup de données(" + jsonData.Count*7 + "), il peut il y avoir de forts ralentissments.\nÉtês-vous sûr de vouloir continuer?", "ATTENTION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        System.Windows.Forms.MessageBox.Show("À vos risques et périls.", "ATTENTION", MessageBoxButtons.OK);
                    }
                    else
                    {
                        loadingBar.stop();
                        return;
                    }
                }
                string date = "2000-01-01 00:00:00";
                Dictionary<string, double> minValues = new Dictionary<string, double>();
                bool flag = false;
                foreach (Dictionary<string, string> relevemeteo in jsonData)
                {
                    if (DateTime.Parse(date).ToString("yyyy-MM") != DateTime.Parse(relevemeteo["DateHeureReleve"]).ToString("yyyy-MM"))
                    {
                        if (flag)
                        {
                            foreach (var userControl in listData)
                            {
                                listData[userControl.Key].Add(Convert.ToInt32(minValues[userControl.Key]));
                            }
                            listDate.Add(date);
                        }
                        date = relevemeteo["DateHeureReleve"];
                        foreach (var userControl in listData)
                        {
                            minValues[userControl.Key] = double.PositiveInfinity;
                        }
                        flag = false;
                    }
                    foreach (var userControl in listData)
                    {
                        if (minValues[userControl.Key] > Convert.ToInt32(relevemeteo[userControl.Key]))
                        {
                            minValues[userControl.Key] = Convert.ToInt32(relevemeteo[userControl.Key]);
                            flag = true;
                        };
                    }
                }
                if (flag)
                {
                    foreach (var userControl in listData)
                    {
                        listData[userControl.Key].Add(Convert.ToInt32(minValues[userControl.Key]));
                    }
                    listDate.Add(date);
                }
            }
            else if (index == 6)
            {
                if (jsonData.Count > 525600 / 7)
                {
                    DialogResult result = System.Windows.Forms.MessageBox.Show("Vous sélectionnez beaucoup de données(" + jsonData.Count*7 + "), il peut il y avoir de forts ralentissments.\nÉtês-vous sûr de vouloir continuer?", "ATTENTION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        System.Windows.Forms.MessageBox.Show("À vos risques et périls.", "ATTENTION", MessageBoxButtons.OK);
                    }
                    else
                    {
                        loadingBar.stop();
                        return;
                    }
                }
                string date = "2000-01-01 00:00:00";
                Dictionary<string, double> mixValues = new Dictionary<string, double>();
                bool flag = false;
                foreach (Dictionary<string, string> relevemeteo in jsonData)
                {
                    if (DateTime.Parse(date).ToString("yyyy-MM") != DateTime.Parse(relevemeteo["DateHeureReleve"]).ToString("yyyy-MM"))
                    {
                        if (flag)
                        {
                            foreach (var userControl in listData)
                            {
                                listData[userControl.Key].Add(Convert.ToInt32(mixValues[userControl.Key]));
                            }
                            listDate.Add(date);
                        }
                        date = relevemeteo["DateHeureReleve"];
                        foreach (var userControl in listData)
                        {
                            mixValues[userControl.Key] = double.NegativeInfinity;
                        }
                        flag = false;
                    }
                    foreach (var userControl in listData)
                    {
                        if (mixValues[userControl.Key] < Convert.ToInt32(relevemeteo[userControl.Key]))
                        {
                            mixValues[userControl.Key] = Convert.ToInt32(relevemeteo[userControl.Key]);
                            flag = true;
                        };
                    }
                }
                if (flag)
                {
                    foreach (var userControl in listData)
                    {
                        listData[userControl.Key].Add(Convert.ToInt32(mixValues[userControl.Key]));
                    }
                    listDate.Add(date);
                }
            }
            else if (index == 7)
            {
                if (jsonData.Count > 6307200 / 7)
                {
                    DialogResult result = System.Windows.Forms.MessageBox.Show("Vous sélectionnez beaucoup de données(" + jsonData.Count*7 + "), il peut il y avoir de forts ralentissments.\nÉtês-vous sûr de vouloir continuer?", "ATTENTION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        System.Windows.Forms.MessageBox.Show("À vos risques et périls.", "ATTENTION", MessageBoxButtons.OK);
                    }
                    else
                    {
                        loadingBar.stop();
                        return;
                    }
                }
                string date = "2000-01-01 00:00:00";
                Dictionary<string, Int32> sommeValue = new Dictionary<string, Int32>();
                int nValue = 0;
                foreach (Dictionary<string, string> relevemeteo in jsonData)
                {
                    if (DateTime.Parse(date).ToString("yyyy") != DateTime.Parse(relevemeteo["DateHeureReleve"]).ToString("yyyy"))
                    {
                        if (nValue != 0)
                        {
                            foreach (var userControl in listData)
                            {
                                listData[userControl.Key].Add(sommeValue[userControl.Key] / nValue);
                            }
                            listDate.Add(date);
                        }
                        date = relevemeteo["DateHeureReleve"];
                        foreach (var userControl in listData)
                        {
                            sommeValue[userControl.Key] = 0;
                        }
                        nValue = 0;
                    }
                    nValue++;
                    foreach (var userControl in listData)
                    {
                        sommeValue[userControl.Key] += Convert.ToInt32(relevemeteo[userControl.Key]);
                    }
                }
                if (nValue != 0)
                {
                    foreach (var userControl in listData)
                    {
                        listData[userControl.Key].Add(sommeValue[userControl.Key] / nValue);
                    }
                    listDate.Add(date);
                }
            }
            else if (index == 8)
            {
                if (jsonData.Count > 6307200/7)
                {
                    DialogResult result = System.Windows.Forms.MessageBox.Show("Vous sélectionnez beaucoup de données(" + jsonData.Count*7 + "), il peut il y avoir de forts ralentissments.\nÉtês-vous sûr de vouloir continuer?", "ATTENTION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        System.Windows.Forms.MessageBox.Show("À vos risques et périls.", "ATTENTION", MessageBoxButtons.OK);
                    }
                    else
                    {
                        loadingBar.stop();
                        return;
                    }
                }
                string date = "2000-01-01 00:00:00";
                Dictionary<string, double> minValues = new Dictionary<string, double>();
                bool flag = false;
                foreach (Dictionary<string, string> relevemeteo in jsonData)
                {
                    if (DateTime.Parse(date).ToString("yyyy") != DateTime.Parse(relevemeteo["DateHeureReleve"]).ToString("yyyy"))
                    {
                        if (flag)
                        {
                            foreach (var userControl in listData)
                            {
                                listData[userControl.Key].Add(Convert.ToInt32(minValues[userControl.Key]));
                            }
                            listDate.Add(date);
                        }
                        date = relevemeteo["DateHeureReleve"];
                        foreach (var userControl in listData)
                        {
                            minValues[userControl.Key] = double.PositiveInfinity;
                        }
                        flag = false;
                    }
                    foreach (var userControl in listData)
                    {
                        if (minValues[userControl.Key] > Convert.ToInt32(relevemeteo[userControl.Key]))
                        {
                            minValues[userControl.Key] = Convert.ToInt32(relevemeteo[userControl.Key]);
                            flag = true;
                        };
                    }
                }
                if (flag)
                {
                    foreach (var userControl in listData)
                    {
                        listData[userControl.Key].Add(Convert.ToInt32(minValues[userControl.Key]));
                    }
                    listDate.Add(date);
                }
            }
            else if (index == 9)
            {
                if (jsonData.Count > 6307200/7)
                {
                    DialogResult result = System.Windows.Forms.MessageBox.Show("Vous sélectionnez beaucoup de données(" + jsonData.Count*7 + "), il peut il y avoir de forts ralentissments.\nÉtês-vous sûr de vouloir continuer?", "ATTENTION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        System.Windows.Forms.MessageBox.Show("À vos risques et périls.", "ATTENTION", MessageBoxButtons.OK);
                    }
                    else
                    {
                        loadingBar.stop();
                        return;
                    }
                }
                string date = "2000-01-01 00:00:00";
                Dictionary<string, double> mixValues = new Dictionary<string, double>();
                bool flag = false;
                foreach (Dictionary<string, string> relevemeteo in jsonData)
                {
                    if (DateTime.Parse(date).ToString("yyyy") != DateTime.Parse(relevemeteo["DateHeureReleve"]).ToString("yyyy"))
                    {
                        if (flag)
                        {
                            foreach (var userControl in listData)
                            {
                                listData[userControl.Key].Add(Convert.ToInt32(mixValues[userControl.Key]));
                            }
                            listDate.Add(date);
                        }
                        date = relevemeteo["DateHeureReleve"];
                        foreach (var userControl in listData)
                        {
                            mixValues[userControl.Key] = double.NegativeInfinity;
                        }
                        flag = false;
                    }
                    foreach (var userControl in listData)
                    {
                        if (mixValues[userControl.Key] < Convert.ToInt32(relevemeteo[userControl.Key]))
                        {
                            mixValues[userControl.Key] = Convert.ToInt32(relevemeteo[userControl.Key]);
                            flag = true;
                        };
                    }
                }
                if (flag)
                {
                    foreach (var userControl in listData)
                    {
                        listData[userControl.Key].Add(Convert.ToInt32(mixValues[userControl.Key]));
                    }
                    listDate.Add(date);
                }
            }
            else
            {
                loadingBar.stop();
                return;
            }

            // On met à jour les valeurs dans le graphique
            graph.graph.setValues(listData, listDate);

            // On arrête la barre de chargement
            loadingBar.stop();
        }
    }
}

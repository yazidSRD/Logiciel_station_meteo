using projet23_Station_météo_WPF.UserControls.mesuresUi;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace projet23_Station_météo_WPF.UserControls
{
    /// <summary>
    /// Logique d'interaction pour mesures.xaml
    /// </summary>
    public partial class mesures : UserControl
    {
        Thread autoRefreshTh;
        dynamic mesureUi;
        delegate void delegateMessageBox();

        public mesures()
        {
            InitializeComponent();
            mesureUi = Gui.Children[0];
            autoRefreshTh = new Thread(autoRefresh);
            autoRefreshTh.Start();
            videoPlayer.Play();
        }
        public void dell(object sender, RoutedEventArgs e)
        {
            autoRefreshTh.Abort();
        }
        public void autoRefresh()
        {
            while (true)
            {
                refreshData();
                Thread.Sleep(Convert.ToInt32(((string)App.Current.Properties["refreshTimer"])) * 1000);
            }
        }
        public void refreshData()
        {
            DateTime yesterday = DateTime.Now.AddDays(-1);
            string yesterdayFormatted = yesterday.ToString("yyyy-MM-dd HH:mm");

            List<Dictionary<string, string>> jsonData = new Http().get("* FROM relevemeteo WHERE DateHeureReleve > '" + yesterdayFormatted + "'").Result;
            if (jsonData == null) {
                Dispatcher.BeginInvoke(new delegateMessageBox(() => {
                    System.Windows.Forms.MessageBox.Show("Impossible de se connecter au server.\n\nIl est possible que:\n - Vous ne soyez pas connecté\n - Que le serveur ne soit pas connecté\n\nSi le problème persiste, veuillez contacter un administrateur.",
                    "Connexion erreur",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                }), DispatcherPriority.Render);
                return;
            };

            Dictionary<string, List<Int32>> listD = new Dictionary<string, List<Int32>>()
            {
                {"Temperature", new List<Int32>()},
                {"Hygrometrie", new List<Int32>()},
                {"VitesseVent", new List<Int32>()},
                {"PressionAtmospherique", new List<Int32>()},
                {"Pluviometre", new List<Int32>()},
                {"DirectionVent", new List<Int32>()},
                {"RayonnementSolaire", new List<Int32>()}
            };

            List<string> listDate = new List<string>();

            foreach (Dictionary<string, string> relevemeteo in jsonData)
            {
                foreach (var userControl in listD)
                {

                    listD[userControl.Key].Add(Convert.ToInt32(relevemeteo[userControl.Key]));
                }
                listDate.Add(relevemeteo["DateHeureReleve"]);
            }

            mesureUi.refreshData(listD, listDate);
        }
        private void uiRefesh(object sender, SizeChangedEventArgs e)
        {
            /*Console.WriteLine(e.NewSize.Height + "  " + e.NewSize.Width);*/
            List<dynamic> uis = new List<dynamic>() { new mesures2(), new mesures5(), new mesures10(), new mesures4(), new mesures7(), new mesures9(), new mesures8(), new mesures3(), new mesures1() };

            foreach (dynamic ui in uis)
            {
                if (ui.MinHeight <= e.NewSize.Height && ui.MinWidth <= e.NewSize.Width)
                {
                    if (ui.GetType() != mesureUi.GetType())
                    {
                        mesureUi = ui;
                        Gui.Children.Clear();
                        Gui.Children.Add(mesureUi);
                        autoRefreshTh.Abort();
                        autoRefreshTh = new Thread(autoRefresh);
                        autoRefreshTh.Start();
                    }
                    return;
                }
            }
        }
        private void LoopVideo(object sender, RoutedEventArgs e)
        {
            videoPlayer.Position = TimeSpan.Zero;
            videoPlayer.Play();
        }
    }
}

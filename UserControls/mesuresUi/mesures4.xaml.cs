using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;

namespace projet23_Station_météo_WPF.UserControls.mesuresUi
{
    /// <summary>
    /// Logique d'interaction pour mesures1.xaml
    /// </summary>
    public partial class mesures4 : UserControl
    {
        Thread refreshUiTh;
        delegate void illustrationDelegate();
        string urlIllustration;

        public mesures4()
        {
            InitializeComponent();
            refreshUiTh = new Thread(refreshUi);
            refreshUiTh.Start();
        }
        public void refreshUi()
        {
            controlTemp.graph.setInfo("temperature", ((string)App.Current.Properties["unitTemp"]), "images/icons/temperature0.png", 49);
            controlHygro.graph.setInfo("hygrometrie", ((string)App.Current.Properties["unitHygro"]), "images/icons/hygrometrie.png", 49);
            controlVitesseVent.graph.setInfo("vitesse du vent", ((string)App.Current.Properties["unitVvent"]), "images/icons/vitesseVent.png", 49);
            controlPressionAtmospherique.graph.setInfo("pression atmosphérique", ((string)App.Current.Properties["unitPresAtmo"]), "images/icons/pression.png", 49);
            controlPluviometre.graph.setInfo("pluviometre", ((string)App.Current.Properties["unitPluv"]), "images/icons/pluviometrie.png", 49);
            controlDirectionVent.graph.setInfo("direction du vent", ((string)App.Current.Properties["unitDvent"]), "images/icons/DirectionVent.png", 49);
            controlRayonnementSolaire.graph.setInfo("rayonnement solaire", ((string)App.Current.Properties["unitRaySol"]), "images/icons/rayonnementSolaire.png", 49);
            controlTemp2.graph.setInfo("temperature", ((string)App.Current.Properties["unitTemp"]), "images/icons/temperature0.png", 49);
        }
        public void refreshIllustration()
        {
            System.Windows.Media.Imaging.BitmapImage logo = new System.Windows.Media.Imaging.BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri("https://www.weatherbit.io/static/img/icons/" + urlIllustration + ".png");
            logo.EndInit();

            illustration.Source = logo;
        }
        public void refreshData(Dictionary<string, List<Int32>> data, List<string> date)
        {
            List<Dictionary<string, string>> previsions = new Http().getPrevisions().Result;
            urlIllustration = null;
            urlIllustration = previsions[0]["icon"];
            if (urlIllustration != null) Dispatcher.BeginInvoke(new illustrationDelegate(refreshIllustration), System.Windows.Threading.DispatcherPriority.Render);

            Dictionary<string, List<dynamic>> listV = new Dictionary<string, List<dynamic>>()
            {
                {"Temperature", new List<dynamic>() { controlTemp, controlTemp2}},
                {"Hygrometrie", new List<dynamic>() { controlHygro }},
                {"VitesseVent", new List<dynamic>() { controlVitesseVent }},
                {"PressionAtmospherique", new List<dynamic>() { controlPressionAtmospherique }},
                {"Pluviometre", new List<dynamic>() { controlPluviometre}},
                {"DirectionVent", new List<dynamic>() { controlDirectionVent }},
                {"RayonnementSolaire", new List<dynamic>() { controlRayonnementSolaire }},
            };

            foreach (var userControls in listV)
            {
                foreach (dynamic userControl in userControls.Value)
                {
                    userControl.graph.setValues(data[userControls.Key], date);
                }
            }
        }
    }
}

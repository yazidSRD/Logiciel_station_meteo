using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;

namespace projet23_Station_météo_WPF.UserControls.mesuresUi
{
    /// <summary>
    /// Logique d'interaction pour mesures1.xaml
    /// </summary>
    public partial class mesures2 : UserControl
    {
        Thread refreshUiTh;

        public mesures2()
        {
            InitializeComponent();
            refreshUiTh = new Thread(refreshUi);
            refreshUiTh.Start();
        }
        public void refreshUi()
        {
            controlTemp.graph.setInfo("temperature", ((string)App.Current.Properties["unitTemp"]), "images/icons/temperature0.png", 49);
            controlHygro.graph.setInfo("hygrometrie", ((string)App.Current.Properties["unitHygro"]), "images/icons/hygrometrie.png", 49);
            controlVitesseVent.graph.setInfo("vitesse du vent", ((string)App.Current.Properties["unitVvent"]), "images/icons/vitesseVent.png", 49); ;
            controlPressionAtmospherique.graph.setInfo("pression atmosphérique", ((string)App.Current.Properties["unitPresAtmo"]), "images/icons/pression.png", 49);
            controlPluviometre.graph.setInfo("pluviometre", ((string)App.Current.Properties["unitPluv"]), "images/icons/pluviometrie.png", 49);
            controlDirectionVent.graph.setInfo("direction du vent", ((string)App.Current.Properties["unitDvent"]), "images/icons/DirectionVent.png", 49);
            controlTemp2.graph.setInfo("temperature", ((string)App.Current.Properties["unitTemp"]), "images/icons/temperature0.png", 49);
            controlPluviometre2.graph.setInfo("pluviometre", ((string)App.Current.Properties["unitPluv"]), "images/icons/pluviometrie.png", 49);
            controlPressionAtmospherique2.graph.setInfo("pression atmosphérique", ((string)App.Current.Properties["unitPresAtmo"]), "images/icons/pression.png", 49);
            controlRayonnementSolaire.graph.setInfo("rayonnement solaire", ((string)App.Current.Properties["unitRaySol"]), "images/icons/rayonnementSolaire.png", 49);
        }
        public void refreshData(Dictionary<string, List<Int32>> data, List<string> date)
        {
            Dictionary<string, List<dynamic>> listV = new Dictionary<string, List<dynamic>>()
            {
                {"Temperature", new List<dynamic>() { controlTemp, controlTemp2}},
                {"Hygrometrie", new List<dynamic>() { controlHygro }},
                {"VitesseVent", new List<dynamic>() { controlVitesseVent }},
                {"PressionAtmospherique", new List<dynamic>() { controlPressionAtmospherique, controlPressionAtmospherique2 }},
                {"Pluviometre", new List<dynamic>() { controlPluviometre, controlPluviometre2}},
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

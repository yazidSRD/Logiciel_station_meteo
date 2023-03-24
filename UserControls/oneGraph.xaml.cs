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

namespace projet23_Station_météo_WPF.UserControls
{
    /// <summary>
    /// Logique d'interaction pour oneGraph.xaml
    /// </summary>
    public partial class oneGraph : System.Windows.Controls.UserControl
    {
        DateTime maxDate;
        DateTime minDate;
        delegate void refreshDelegate();
        public oneGraph()
        {
            InitializeComponent();

            graph.graph.date = "yyyy-MM-dd HH:mm";

            new Thread(() =>
            {
                string mindate = new Http().getDate("MIN").Result;
                string maxdate = new Http().getDate("MAX").Result;

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
                    minDate = DateTime.Now;
                }
                Dispatcher.BeginInvoke(new refreshDelegate(refreshUi), DispatcherPriority.Render);
            }).Start();
        }
        public void refreshUi()
        {
            startDate.DisplayDateStart = minDate;
            startDate.DisplayDateEnd = maxDate;
            endDate.DisplayDateStart = minDate;
            endDate.DisplayDateEnd = maxDate;
        }
        void refrshDate(object sender, RoutedEventArgs e)
        {
            if (startDate.SelectedDate != null && endDate.SelectedDate != null)
            {
                if (((DatePicker)sender).SelectedDate == startDate.SelectedDate)
                    if (startDate.SelectedDate > endDate.SelectedDate) endDate.SelectedDate = startDate.SelectedDate;
                if (((DatePicker)sender).SelectedDate == endDate.SelectedDate)
                    if (startDate.SelectedDate > endDate.SelectedDate) startDate.SelectedDate = endDate.SelectedDate;
            };
        }
        void dataSearch(object sender, RoutedEventArgs e)
        {
            if (startDate.SelectedDate != null && endDate.SelectedDate != null)
            {
                string sDate = startDate.SelectedDate.ToString();
                string eDate = endDate.SelectedDate.ToString();
                int index = type.SelectedIndex;

                switch (index)
                {
                    case 9:
                    case 8:
                    case 7:
                        graph.graph.date = "yyyy";
                        sDate = DateTime.Parse(sDate).ToString("yyyy") + "-01-01";
                        eDate = DateTime.Parse(eDate).ToString("yyyy") + "-12-" + DateTime.DaysInMonth(DateTime.Parse(eDate).Year, DateTime.Parse(eDate).Month);
                        break;
                    case 6:
                    case 5:
                    case 4:
                        graph.graph.date = "yyyy-MM";
                        sDate = DateTime.Parse(sDate).ToString("yyyy-MM") + "-01";
                        eDate = DateTime.Parse(eDate).ToString("yyyy-MM") + "-" + DateTime.DaysInMonth(DateTime.Parse(eDate).Year, DateTime.Parse(eDate).Month);
                        break;
                    case 3:
                    case 2:
                    case 1:
                        graph.graph.date = "yyyy-MM-dd";
                        sDate = DateTime.Parse(sDate).ToString("yyyy-MM-dd");
                        eDate = DateTime.Parse(eDate).ToString("yyyy-MM-dd");
                        break;
                    case 0:
                    default:
                        graph.graph.date = "yyyy-MM-dd HH:mm";
                        sDate = DateTime.Parse(sDate).ToString("yyyy-MM-dd");
                        eDate = DateTime.Parse(eDate).ToString("yyyy-MM-dd");
                        break;
                }
                startDate.SelectedDate = DateTime.Parse(sDate);
                endDate.SelectedDate = DateTime.Parse(eDate);


                new Thread(() =>
                {
                    dataSearch(sDate + " 00:00:00", eDate + " 23:59:59.9", index);
                }).Start();
            }
            else
            {
                Console.WriteLine("pas de dates");
            }
        }
        void dataSearch(string startDate, string endDate, int index)
        {
            List<Dictionary<string, string>> jsonData = new Http().get("* FROM relevemeteo WHERE DateHeureReleve BETWEEN '" + startDate + "' AND '" + endDate + "'").Result;
            if (jsonData == null) return;

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

            List<string> listDate = new List<string>();

            if (index == 0)
            {
                if (jsonData.Count > 432/7)
                {
                    DialogResult result = System.Windows.Forms.MessageBox.Show("Vous sélectionnez beaucoup de données(" + jsonData.Count*7 + "), il peut il y avoir de forts ralentissments.\nÉtês-vous sûr de vouloir continuer?", "ATTETION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        System.Windows.Forms.MessageBox.Show("À vos risques et périls.", "ATTETION", MessageBoxButtons.OK);
                    }
                    else
                    {
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
                    DialogResult result = System.Windows.Forms.MessageBox.Show("Vous sélectionnez beaucoup de données(" + jsonData.Count*7 + "), il peut il y avoir de forts ralentissments.\nÉtês-vous sûr de vouloir continuer?", "ATTETION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        System.Windows.Forms.MessageBox.Show("À vos risques et périls.", "ATTETION", MessageBoxButtons.OK);
                    }
                    else
                    {
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
                    DialogResult result = System.Windows.Forms.MessageBox.Show("Vous sélectionnez beaucoup de données(" + jsonData.Count*7 + "), il peut il y avoir de forts ralentissments.\nÉtês-vous sûr de vouloir continuer?", "ATTETION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        System.Windows.Forms.MessageBox.Show("À vos risques et périls.", "ATTETION", MessageBoxButtons.OK);
                    }
                    else
                    {
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
                    DialogResult result = System.Windows.Forms.MessageBox.Show("Vous sélectionnez beaucoup de données(" + jsonData.Count*7 + "), il peut il y avoir de forts ralentissments.\nÉtês-vous sûr de vouloir continuer?", "ATTETION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        System.Windows.Forms.MessageBox.Show("À vos risques et périls.", "ATTETION", MessageBoxButtons.OK);
                    }
                    else
                    {
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
                    DialogResult result = System.Windows.Forms.MessageBox.Show("Vous sélectionnez beaucoup de données(" + jsonData.Count*7 + "), il peut il y avoir de forts ralentissments.\nÉtês-vous sûr de vouloir continuer?", "ATTETION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        System.Windows.Forms.MessageBox.Show("À vos risques et périls.", "ATTETION", MessageBoxButtons.OK);
                    }
                    else
                    {
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
                    DialogResult result = System.Windows.Forms.MessageBox.Show("Vous sélectionnez beaucoup de données(" + jsonData.Count*7 + "), il peut il y avoir de forts ralentissments.\nÉtês-vous sûr de vouloir continuer?", "ATTETION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        System.Windows.Forms.MessageBox.Show("À vos risques et périls.", "ATTETION", MessageBoxButtons.OK);
                    }
                    else
                    {
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
                    DialogResult result = System.Windows.Forms.MessageBox.Show("Vous sélectionnez beaucoup de données(" + jsonData.Count*7 + "), il peut il y avoir de forts ralentissments.\nÉtês-vous sûr de vouloir continuer?", "ATTETION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        System.Windows.Forms.MessageBox.Show("À vos risques et périls.", "ATTETION", MessageBoxButtons.OK);
                    }
                    else
                    {
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
                    DialogResult result = System.Windows.Forms.MessageBox.Show("Vous sélectionnez beaucoup de données(" + jsonData.Count*7 + "), il peut il y avoir de forts ralentissments.\nÉtês-vous sûr de vouloir continuer?", "ATTETION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        System.Windows.Forms.MessageBox.Show("À vos risques et périls.", "ATTETION", MessageBoxButtons.OK);
                    }
                    else
                    {
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
                    DialogResult result = System.Windows.Forms.MessageBox.Show("Vous sélectionnez beaucoup de données(" + jsonData.Count*7 + "), il peut il y avoir de forts ralentissments.\nÉtês-vous sûr de vouloir continuer?", "ATTETION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        System.Windows.Forms.MessageBox.Show("À vos risques et périls.", "ATTETION", MessageBoxButtons.OK);
                    }
                    else
                    {
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
                    DialogResult result = System.Windows.Forms.MessageBox.Show("Vous sélectionnez beaucoup de données(" + jsonData.Count*7 + "), il peut il y avoir de forts ralentissments.\nÉtês-vous sûr de vouloir continuer?", "ATTETION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        System.Windows.Forms.MessageBox.Show("À vos risques et périls.", "ATTETION", MessageBoxButtons.OK);
                    }
                    else
                    {
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
                return;
            }

            /*foreach (Dictionary<string, string> relevemeteo in jsonData)
            {
                foreach (var userControl in listData)
                {
                    listData[userControl.Key].Add(Convert.ToInt32(relevemeteo[userControl.Key]));
                    *//*if (userControl.Key == "Pluviometre")
                    {
                        listData[userControl.Key].Add((Int16)(Convert.ToInt32(relevemeteo[userControl.Key]) / 1000));
                    }
                    else
                    {
                        listData[userControl.Key].Add(Convert.ToInt16(relevemeteo[userControl.Key]));
                    }*//*
                }
                listDate.Add(relevemeteo["DateHeureReleve"]);
            }*/

            graph.graph.setValues(listData, listDate);

            /*Dictionary<string, List<dynamic>> listV = new Dictionary<string, List<dynamic>>()
            {
                {"Temperature", new List<dynamic>() { graph }},
                {"Hygrometrie", new List<dynamic>() { graph }},
                {"VitesseVent", new List<dynamic>() { graph }},
                {"PressionAtmospherique", new List<dynamic>() { graph }},
                {"Pluviometre", new List<dynamic>() { graph }},
                {"DirectionVent", new List<dynamic>() { graph }},
                {"RayonnementSolaire", new List<dynamic>() { graph }}
            };

            foreach (var userControls in listV)
            {
                foreach (dynamic userControl in userControls.Value)
                {
                    userControl.graph.setValues(listData[userControls.Key], listDate);
                }
            }*/
        }
    }
}

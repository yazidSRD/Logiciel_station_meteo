// Importations de namespaces et classes externes
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace projet23_Station_météo_WPF.code
{
    public class Graph
    {
        string name, unite, icon;
        public string date;
        double maxValues;
        List<Int32> values;
        Dictionary<string, List<Int32>> oneValues;
        List<String> dates;

        public TextBlock textBlockName;
        public TextBlock textBlockValue;
        public TextBlock textBlockMax;
        public TextBlock textBlockMoy;
        public TextBlock textBlockMin;
        public Image imageIcon;
        public StackPanel StackPanelValues;
        public Grid boussole;
        public CartesianChart chart;
        public CartesianChart oneChart;
        Action delegateRefresh;

        public Graph(Action delegateRefresh)
        {
            values = new List<Int32>();
            dates = new List<String>();
            this.delegateRefresh = delegateRefresh;
        }
        public void setInfo(string name, string unite, string icon, double maxV = double.PositiveInfinity, string date = "yyyy-MM-dd HH:mm")
        {
            this.name = name;
            this.unite = unite;
            this.icon = icon;
            this.maxValues = maxV;
            this.date = date;

            delegateRefresh();
        }
        public void addValue(int value, string date)
        {
            values.Insert(0, Convert.ToInt16(value));
            if (values.Count == maxValues + 1) values.RemoveAt(0);
            dates.Insert(0, date);
            if (dates.Count == maxValues + 1) dates.RemoveAt(0);
            delegateRefresh();
        }
        public void setValues(List<Int32> values, List<string> date)
        {
            this.values = values;
            this.oneValues = null;
            this.dates = date;
            delegateRefresh();
        }
        public void setValues(Dictionary<string, List<Int32>> oneValues, List<string> date)
        {
            this.values = null;
            this.oneValues = new Dictionary<string, List<Int32>>();

            foreach (KeyValuePair<string, List<Int32>> _values in oneValues)
            {
                string key = _values.Key;

                switch (key)
                {
                    case "RayonnementSolaire":
                        key = "Rayonnement solaire";
                        break;
                    case "Pluviometre":
                        key = "Pluviometrie";
                        break;
                    case "Hygrometrie":
                        key = "Humidité";
                        break;
                    case "VitesseVent":
                        key = "Vitesse du vent";
                        break;
                    case "PressionAtmospherique":
                        key = "Pression atmospherique";
                        break;
                    case "DirectionVent":
                        key = "Direction du vent";
                        break;
                }

                this.oneValues.Add(key, _values.Value);
            }

            this.dates = date;
            delegateRefresh();
        }
        private Grid createGrid(string date, int value, string unite, string name)
        {
            Thickness myThickness = new Thickness();

            Grid grid = new Grid();
            grid.Height = 30;

            Rectangle rectangle = new Rectangle();
            rectangle.VerticalAlignment = VerticalAlignment.Top;
            rectangle.Height = 2;
            rectangle.Width = 160;
            rectangle.Stroke = (Brush)(new System.Windows.Media.BrushConverter()).ConvertFromString("#FF969696");
            grid.Children.Add(rectangle);

            TextBlock textBloc = new TextBlock();
            textBloc.Text = DateTime.Parse(date).ToString("dd/MM/yy HH:mm");
            textBloc.HorizontalAlignment = HorizontalAlignment.Left;
            textBloc.VerticalAlignment = VerticalAlignment.Top;
            textBloc.Width = 83;
            myThickness.Left = 5;
            myThickness.Top = 7;
            myThickness.Right = 0;
            myThickness.Bottom = 0;
            textBloc.Margin = myThickness;
            grid.Children.Add(textBloc);

            textBloc = new TextBlock();
            textBloc.Text = ":";
            textBloc.HorizontalAlignment = HorizontalAlignment.Left;
            textBloc.VerticalAlignment = VerticalAlignment.Top;
            textBloc.Width = 4;
            myThickness.Left = 88;
            myThickness.Top = 7;
            myThickness.Right = 0;
            myThickness.Bottom = 0;
            textBloc.Margin = myThickness;
            grid.Children.Add(textBloc);

            Border border = new Border();
            border.HorizontalAlignment = HorizontalAlignment.Left;
            border.VerticalAlignment = VerticalAlignment.Top;
            border.Height = 20;
            myThickness.Left = 92;
            myThickness.Top = 5;
            myThickness.Right = 0;
            myThickness.Bottom = 0;
            border.Margin = myThickness;
            myThickness.Left = 0;
            myThickness.Top = 0;
            myThickness.Right = 2;
            myThickness.Bottom = 2;
            border.BorderThickness = myThickness;
            border.CornerRadius = new CornerRadius(7);
            setColorBorderUi(border, name, value);

            textBloc = new TextBlock();
            textBloc.Text = $"{value}{unite}";
            textBloc.TextWrapping = TextWrapping.Wrap;
            textBloc.FontWeight = FontWeights.Bold;
            textBloc.HorizontalAlignment = HorizontalAlignment.Center;
            textBloc.VerticalAlignment = VerticalAlignment.Center;
            myThickness.Left = 5;
            myThickness.Top = 0;
            myThickness.Right = 3;
            myThickness.Bottom = 0;
            textBloc.Margin = myThickness;
            border.Child = textBloc;

            grid.Children.Add(border);

            return grid;
        }
        private void setColorBorderUi(Border border, string name, int var)
        {
            switch (name)
            {
                case "vitesse du vent":
                    border.Background = (Brush)(new System.Windows.Media.BrushConverter()).ConvertFromString(((string)App.Current.Properties["colorSeuils"]).Split(new char[] { ';' })[seuil(0, var)]);
                    border.BorderBrush = (Brush)(new System.Windows.Media.BrushConverter()).ConvertFromString(((string)App.Current.Properties["colorSeuils"]).Split(new char[] { ';' })[seuil(0, var) + 1]);
                    break;
                case "temperature":
                    border.Background = (Brush)(new System.Windows.Media.BrushConverter()).ConvertFromString(((string)App.Current.Properties["colorSeuils"]).Split(new char[] { ';' })[seuil(1, var)]);
                    border.BorderBrush = (Brush)(new System.Windows.Media.BrushConverter()).ConvertFromString(((string)App.Current.Properties["colorSeuils"]).Split(new char[] { ';' })[seuil(1, var) + 1]);
                    break;
                case "pluviometre":
                    border.Background = (Brush)(new System.Windows.Media.BrushConverter()).ConvertFromString(((string)App.Current.Properties["colorSeuils"]).Split(new char[] { ';' })[seuil(0, var)]);
                    border.BorderBrush = (Brush)(new System.Windows.Media.BrushConverter()).ConvertFromString(((string)App.Current.Properties["colorSeuils"]).Split(new char[] { ';' })[seuil(0, var) + 1]);
                    break;
                case "rayonnement solaire":
                    border.Background = (Brush)(new System.Windows.Media.BrushConverter()).ConvertFromString(((string)App.Current.Properties["colorSeuils"]).Split(new char[] { ';' })[seuil(3, var)]);
                    border.BorderBrush = (Brush)(new System.Windows.Media.BrushConverter()).ConvertFromString(((string)App.Current.Properties["colorSeuils"]).Split(new char[] { ';' })[seuil(3, var) + 1]);
                    break;
                default:
                    border.Background = (Brush)(new System.Windows.Media.BrushConverter()).ConvertFromString("#00000000");
                    border.BorderBrush = (Brush)(new System.Windows.Media.BrushConverter()).ConvertFromString("#00000000");
                    break;
            }
        }
        private int seuil(int key, int var)
        {
            if (new unitConversion().Conversion(Convert.ToInt32(((string)App.Current.Properties["seuilLvl1"]).Split(new char[] { ';' })[key]), unite, null) > var)
            {
                return 0;
            }
            else if (new unitConversion().Conversion(Convert.ToInt32(((string)App.Current.Properties["seuilLvl2"]).Split(new char[] { ';' })[key]), unite, null) > var)
            {
                return 2;
            }
            return 4;
        }
        public void refresh()
        {
            if (!(textBlockName is null))
            {
                textBlockName.Text = name;
            }

            if (!(imageIcon is null))
            {
                try
                {
                    imageIcon.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("pack://application:,,,/projet23-Station météo WPF;component/" + icon);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            if (!(textBlockValue is null))
            {
                if (values.Count != 0)
                {
                    textBlockValue.Text = $"{values.First()}{unite}";
                    if (((Viewbox)textBlockValue.Parent).Parent is Border)
                    {
                        setColorBorderUi((Border)((Viewbox)textBlockValue.Parent).Parent, name, values.First());
                    }
                }
                else
                {
                    textBlockValue.Text = "Ø";
                    if (((Viewbox)textBlockValue.Parent).Parent is Border)
                    {
                        setColorBorderUi((Border)((Viewbox)textBlockValue.Parent).Parent, null, 0);
                    }
                }
            }

            if (!(textBlockMax is null))
            {
                if (values.Count != 0)
                {
                    textBlockMax.Text = $"{values.Max()}{unite}";
                }
                else
                {
                    textBlockMax.Text = "Ø";
                }

            }

            if (!(textBlockMoy is null))
            {
                if (values.Count != 0)
                {
                    int sum = 0;
                    for (int i = 0; i < values.Count; i++)
                    {
                        sum += values[i];
                    }
                    textBlockMoy.Text = $"{sum / values.Count}{unite}";
                }
                else
                {
                    textBlockMoy.Text = "Ø";
                }
            }

            if (!(textBlockMin is null))
            {
                if (values.Count != 0)
                {
                    textBlockMin.Text = $"{values.Min()}{unite}";
                }
                else
                {
                    textBlockMin.Text = "Ø";
                }

            }

            if (!(StackPanelValues is null))
            {
                StackPanelValues.Children.Clear();
                if (values.Count != 0)
                {
                    for (int i = 0; i < values.Count; i++)
                    {
                        StackPanelValues.Children.Add(createGrid(dates[i], values[i], unite, name));
                    }
                }
            }

            if (!(boussole is null))
            {
                if (values.Count != 0)
                {
                    boussole.RenderTransformOrigin = new Point(0.5, 0.5);
                    boussole.RenderTransform = new RotateTransform(values.First());
                }
                else
                {
                    boussole.RenderTransformOrigin = new Point(0.5, 0.5);
                    boussole.RenderTransform = new RotateTransform(0);
                }
            }

            if (!(chart is null))
            {
                if (values.Count != 0)
                {
                    chart.AxisX = new LiveCharts.Wpf.AxesCollection()
                    {
                        new LiveCharts.Wpf.Axis()
                        {
                            ShowLabels = false,
                            Separator = new LiveCharts.Wpf.Separator()
                            {
                                Step = 1.0,
                                IsEnabled = false
                            },
                            LabelFormatter = val =>
                            {
                                return DateTime.Parse(dates[(dates.Count-1)-Convert.ToInt16(val)]).ToString(this.date);
                            }
                        }
                    };
                    int type = Array.FindIndex((new string[] { "vitesse du vent", "temperature", "pluviometre", "rayonnement solaire" }), str => str == name);
                    int start = 100000;
                    int seuilLvl1 = 0;
                    int seuilLvl2 = 0;
                    int end = 0;
                    if (type != -1)
                    {
                        start = -100000;
                        seuilLvl1 = new unitConversion().Conversion(Convert.ToInt32(((string)App.Current.Properties["seuilLvl1"]).Split(new char[] { ';' })[type]), unite, null);
                        seuilLvl2 = new unitConversion().Conversion(Convert.ToInt32(((string)App.Current.Properties["seuilLvl2"]).Split(new char[] { ';' })[type]), unite, null);
                        end = 100000;
                    }

                    chart.AxisY = new LiveCharts.Wpf.AxesCollection()
                    {
                        new LiveCharts.Wpf.Axis()
                        {
                            Sections = new SectionsCollection()
                            {
                                new AxisSection {
                                    
                                    Value = start,
                                    SectionWidth = 100000+seuilLvl1,
                                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3F"+((string)App.Current.Properties["colorSeuils"]).Split(new char[] { ';' })[0].Substring(3)))
                                },
                                new AxisSection {
                                    Value = seuilLvl1,
                                    SectionWidth = seuilLvl2-seuilLvl1,
                                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3F"+((string)App.Current.Properties["colorSeuils"]).Split(new char[] { ';' })[2].Substring(3)))
                                },
                                new AxisSection {
                                    Value = seuilLvl2,
                                    SectionWidth= end,
                                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3F"+((string)App.Current.Properties["colorSeuils"]).Split(new char[] { ';' })[4].Substring(3)))
                                }
                            }
                        }
                    };

                    chart.Series = new SeriesCollection { };

                    var temporalCv = new double[values.Count];

                    for (var i = 0; i < values.Count; i++)
                    {
                        temporalCv[i] = values[i];
                    }

                    ChartValues<double> chartValues = new ChartValues<double>();
                    chartValues.AddRange(temporalCv.Reverse());

                    chart.Series.Add(new LineSeries
                    {
                        Title = name,
                        Values = chartValues,
                        LineSmoothness = 0,
                        PointGeometry = DefaultGeometries.Square,
                        PointGeometrySize = 10,
                        LabelPoint = point => point.Y + unite,
                        Fill = new SolidColorBrush(Colors.Transparent)
                    });

                    chart.Zoom = ZoomingOptions.X;
                    chart.Pan = PanningOptions.X;
                    chart.LegendLocation = LegendLocation.Top;
                }
            }

            if (!(oneChart is null))
            {
                if (oneValues.Count != 0)
                {
                    Console.WriteLine("s"+date);
                    oneChart.AxisX = new LiveCharts.Wpf.AxesCollection()
                    {
                        new LiveCharts.Wpf.Axis()
                        {

                            ShowLabels = false,
                            //Title = "dates",
                            Separator = new LiveCharts.Wpf.Separator()
                            {
                                Step = 1.0,
                                IsEnabled = false
                            },
                            LabelFormatter = val =>
                            {

                                return DateTime.Parse(dates[(dates.Count-1)-Convert.ToInt16(val)]).ToString(date);
                                /*try
                                {
                                    if (Convert.ToInt16(val)-1 >= 0)
                                    {
                                        if (DateTime.Parse(dates[(dates.Count-1)-Convert.ToInt16(val)]).ToString("d") == DateTime.Parse(dates[dates.Count-Convert.ToInt16(val)]).ToString("d"))
                                        {
                                             return DateTime.Parse(dates[(dates.Count-1)-Convert.ToInt16(val)]).ToString("HH:mm");
                                        } else  return DateTime.Parse(dates[(dates.Count-1)-Convert.ToInt16(val)]).ToString("d");
                                    } else return DateTime.Parse(dates[(dates.Count-1)-Convert.ToInt16(val)]).ToString("d");
                                } catch (Exception er)
                                {
                                    return "";
                                }*/
                            },
                            //LabelsRotation=45
                        }
                    };
                }
                oneChart.Series = new SeriesCollection { };

                int x = 0;

                foreach (KeyValuePair<string, List<Int32>> _values in oneValues)
                {
                    var temporalCv = new double[_values.Value.Count];

                    for (var i = 0; i < _values.Value.Count; i++)
                    {
                        temporalCv[i] = _values.Value[i];
                    }

                    ChartValues<double> chartValues = new ChartValues<double>();
                    chartValues.AddRange(temporalCv.Reverse());

                    Brush brush = Brushes.White;

                    try
                    {
                        brush = ((Button)((Grid)((Grid)oneChart.Parent).Children[1]).Children[x]).BorderBrush;
                    } catch (Exception ex) { }

                    oneChart.Series.Add(new LineSeries
                    {
                        /*Title = _values.Key,*/
                        Title = "",
                        Values = chartValues,
                        LineSmoothness = 0,
                        PointGeometry = DefaultGeometries.Circle,
                        PointGeometrySize =8,
                        Stroke = brush,
                        LabelPoint = point => {

                            string text = _values.Key + ": " + point.Y;
                            switch(_values.Key)
                            {
                                case "Temperature":
                                    text += (string)App.Current.Properties["unitTemp"];
                                    break;
                                case "Humidité":
                                    text += (string)App.Current.Properties["unitHygro"];
                                    break;
                                case "Vitesse du vent":
                                    text += (string)App.Current.Properties["unitVvent"];
                                    break;
                                case "Pression atmospherique":
                                    text += (string)App.Current.Properties["unitPresAtmo"];
                                    break;
                                case "Pluviometrie":
                                    text += (string)App.Current.Properties["unitPluv"];
                                    break;
                                case "Direction du vent":
                                    text += (string)App.Current.Properties["unitDvent"];
                                    break;
                                case "Rayonnement solaire":
                                    text += (string)App.Current.Properties["unitRaySol"];
                                    break;
                            }
                            
                            return text;
                        },
                        Fill = new SolidColorBrush(Colors.Transparent)//(Color)ColorConverter.ConvertFromString("#00000000")
                    });
                    x++;
                }

                oneChart.Zoom = ZoomingOptions.X;
                oneChart.Pan = PanningOptions.X;
                oneChart.LegendLocation = LegendLocation.Bottom;

                foreach (Button button in ((Grid)((Grid)oneChart.Parent).Children[1]).Children) button.Background = button.BorderBrush;
            }
        }
        public void hiddenOrNoValues(object sender, System.EventArgs e)
        {
            if (oneValues == null) return;

            string txt = (string)((Button)sender).Content;


            string[] keys = oneValues.Keys.ToArray();
            int i = Array.FindIndex(keys, key => key == txt);
            if (i == -1) return;
            if (((LiveCharts.Wpf.LineSeries)this.oneChart.Series[i]).Visibility == Visibility.Visible) {
                ((LiveCharts.Wpf.LineSeries)this.oneChart.Series[i]).Visibility = Visibility.Hidden;
                ((Button)sender).Background = (Brush)new BrushConverter().ConvertFrom("#FFDDDDDD");
            } else {
                ((LiveCharts.Wpf.LineSeries)this.oneChart.Series[i]).Visibility = Visibility.Visible;
                Brush color = ((Button)sender).BorderBrush;
                ((Button)sender).Background = color;
            }
        }
    }
}

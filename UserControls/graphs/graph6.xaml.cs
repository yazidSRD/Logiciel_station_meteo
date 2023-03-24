using projet23_Station_météo_WPF.code;
using System.Windows.Controls;
using System.Windows.Threading;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;


namespace projet23_Station_météo_WPF.UserControls.graphs
{
    /// <summary>
    /// Logique d'interaction pour graph4.xaml
    /**/
    /// </summary>
    public partial class graph6 : UserControl
    {

        public Graph graph;
        delegate void refreshDelegate();

        public graph6()
        {
            InitializeComponent();
            graph = new Graph(refresh);
            foreach (Button button in buttons.Children) button.Click += graph.hiddenOrNoValues;
            setUi();
        }
        public void refresh()
        {
            Dispatcher.BeginInvoke(new refreshDelegate(graph.refresh), DispatcherPriority.Render);
        }
        public void setUi()
        {
            graph.oneChart = chart;
        }
    }
}

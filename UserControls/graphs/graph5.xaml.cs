using projet23_Station_météo_WPF.code;
using System.Windows.Controls;
using System.Windows.Threading;

namespace projet23_Station_météo_WPF.UserControls.graphs
{
    /// <summary>
    /// Logique d'interaction pour graph4.xaml
    /**/
    /// </summary>
    public partial class graph5 : UserControl
    {

        public Graph graph;
        delegate void refreshDelegate();

        public graph5()
        {
            InitializeComponent();
            graph = new Graph(refresh);
            setUi();
        }
        public void refresh()
        {
            Dispatcher.BeginInvoke(new refreshDelegate(graph.refresh), DispatcherPriority.Render);
        }
        public void setUi()
        {
            graph.imageIcon = iconBox;
            graph.chart = chart;
        }
    }
}

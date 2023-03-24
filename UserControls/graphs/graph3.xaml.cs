using projet23_Station_météo_WPF.code;
using System.Windows.Controls;
using System.Windows.Threading;

namespace projet23_Station_météo_WPF.UserControls.graphs
{
    /// <summary>
    /// Logique d'interaction pour graph3.xaml
    /// </summary>

    public partial class graph3 : UserControl
    {
        public Graph graph;
        delegate void refreshDelegate();

        public graph3()
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
            graph.textBlockName = nameBox;
            graph.imageIcon = iconBox;
            graph.textBlockMax = max;
            graph.textBlockMoy = moy;
            graph.textBlockMin = min;
            graph.StackPanelValues = values;
        }
    }
}

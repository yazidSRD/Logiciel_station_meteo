using projet23_Station_météo_WPF.code;
using System.Windows.Controls;
using System.Windows.Threading;

namespace projet23_Station_météo_WPF.UserControls.graphs
{
    /// <summary>
    /// Logique d'interaction pour graph1.xaml
    /// </summary>
    public partial class graph1 : UserControl
    {
        public Graph graph;
        delegate void refreshDelegate();

        public graph1()
        {
            InitializeComponent();
            graph = new Graph(refresh);
            setUi();
        }
        public void refresh()
        {
            Dispatcher.BeginInvoke(new refreshDelegate(graph.refresh), DispatcherPriority.Render);
        }
        private void setUi()
        {
            graph.textBlockName = nameBox;
            graph.imageIcon = iconBox;
            graph.textBlockValue = valueBox;
        }
    }
}

// Importations de namespaces et classes externes
using projet23_Station_météo_WPF.code;
using System.Windows.Controls;
using System.Windows.Threading;

namespace projet23_Station_météo_WPF.UserControls.graphs
{
    /// <summary>
    /// Affichage personnalisé
    /// </summary>
    public partial class graph5 : UserControl
    {
        // Class qui va gérer l'affichage
        public Graph graph;

        // Définition d'un délégué pour la mise à jour de l'interface utilisateur
        delegate void refreshDelegate();

        // Constructeur de la classe
        public graph5()
        {
            InitializeComponent();
            graph = new Graph(refresh);
            setUi();
        }

        // Méthode pour mettre à jour l'interface utilisateur
        public void refresh()
        {
            Dispatcher.BeginInvoke(new refreshDelegate(graph.refresh), DispatcherPriority.Render);
        }

        // Méthode pour configurer l'interface utilisateur
        public void setUi()
        {
            graph.imageIcon = iconBox;
            graph.chart = chart;
        }
    }
}

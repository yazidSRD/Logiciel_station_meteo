// Importations de namespaces et classes externes
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
    /// Affichage personnalisé
    /// </summary>
    public partial class graph6 : UserControl
    {
        // Class qui va gérer l'affichage
        public Graph graph;

        // Définition d'un délégué pour la mise à jour de l'interface utilisateur
        delegate void refreshDelegate();

        // Constructeur de la classe
        public graph6()
        {
            InitializeComponent();
            graph = new Graph(refresh);
            foreach (Button button in buttons.Children) button.Click += graph.hiddenOrNoValues;
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
            graph.oneChart = chart;
        }
    }
}

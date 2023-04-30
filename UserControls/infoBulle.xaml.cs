// Importations de namespaces et classes externes
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace projet23_Station_météo_WPF.UserControls
{
    /// <summary>
    /// Élément cliquable pour afficher plus d'information dans un message box
    /// </summary>
    public partial class infoBulle : System.Windows.Controls.UserControl
    {
        public infoBulle()
        {
            InitializeComponent();
        }

        // Lors du clic sur l'image, afficher une boîte de dialogue avec un message personnalisé
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show(((string)this.Tag).Replace("\\n", "\n"), "informations", MessageBoxButtons.OK);
        }
    }
}

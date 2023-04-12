using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace projet23_Station_météo_WPF.code
{
    static class loadingBar
    {
        public static MainWindow mainWindow;
        public static bool annul = false;
        public static void start()
        {
            mainWindow.Dispatcher.BeginInvoke(new MainWindow.delegateLoadingUi(() => {
                ((Grid)mainWindow.progressBar.Parent).Visibility = Visibility.Visible;
                mainWindow.progressBar.Value = 0;
            }), DispatcherPriority.Render);
            annul = false;
        }
        public static void refresh(int value=0)
        {
            mainWindow.Dispatcher.BeginInvoke(new MainWindow.delegateLoadingUi(() => {
                mainWindow.progressBar.Value = value;
            }), DispatcherPriority.Render);
        }
        public static void stop()
        {
            System.Threading.Thread.Sleep(500);
            mainWindow.Dispatcher.BeginInvoke(new MainWindow.delegateLoadingUi(() => {
                ((Grid)mainWindow.progressBar.Parent).Visibility = Visibility.Hidden;
                mainWindow.progressBar.Value = 0;
            }), DispatcherPriority.Render);
        }
    }
}

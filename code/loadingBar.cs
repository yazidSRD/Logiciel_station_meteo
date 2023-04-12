using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media;

namespace projet23_Station_météo_WPF.code
{
    static class loadingBar
    {
        public static MainWindow mainWindow;
        public static bool annul = false;
        static Thread threadLoadingAnimation;
        public static void start(bool progressBar = true, bool loadingAnimation = true, string loadingDescription = "", bool annulButton = false)
        {
            mainWindow.Dispatcher.BeginInvoke(new MainWindow.delegateLoadingUi(() => {
                mainWindow.loadingUi.Visibility = Visibility.Visible;

                if (progressBar) ((ProgressBar)mainWindow.loadingUi.Children[0]).Visibility = Visibility.Visible;
                else ((ProgressBar)mainWindow.loadingUi.Children[0]).Visibility = Visibility.Hidden;
                ((ProgressBar)mainWindow.loadingUi.Children[0]).Value = 0;

                if (loadingAnimation) ((Image)mainWindow.loadingUi.Children[1]).Visibility = Visibility.Visible;
                else ((Image)mainWindow.loadingUi.Children[1]).Visibility = Visibility.Hidden;

                if (loadingDescription != "") ((TextBlock)mainWindow.loadingUi.Children[2]).Visibility = Visibility.Visible;
                else ((TextBlock)mainWindow.loadingUi.Children[2]).Visibility = Visibility.Hidden;
                ((TextBlock)mainWindow.loadingUi.Children[2]).Text = loadingDescription;

                if (annulButton) ((Button)mainWindow.loadingUi.Children[4]).Visibility = Visibility.Visible;
                else ((Button)mainWindow.loadingUi.Children[4]).Visibility = Visibility.Hidden;

            }), DispatcherPriority.Render);
            annul = false;
            if (loadingAnimation)
            {
                threadLoadingAnimation = new Thread(LoadingAnimation);
                threadLoadingAnimation.Start();
            }
        }
        public static void refresh(int value=0)
        {
            mainWindow.Dispatcher.BeginInvoke(new MainWindow.delegateLoadingUi(() => {
                ((ProgressBar)mainWindow.loadingUi.Children[0]).Value = value;
            }), DispatcherPriority.Render);
        }
        public static void stop()
        {
            System.Threading.Thread.Sleep(500);
            mainWindow.Dispatcher.BeginInvoke(new MainWindow.delegateLoadingUi(() => {
                mainWindow.loadingUi.Visibility = Visibility.Hidden;
            }), DispatcherPriority.Render);
            threadLoadingAnimation.Abort();
        }
        public static void LoadingAnimation()
        {
            while (true) {
                mainWindow.Dispatcher.BeginInvoke(new MainWindow.delegateLoadingUi(() => {
                    RotateTransform rotateTransform = new RotateTransform(((RotateTransform)((Image)mainWindow.loadingUi.Children[1]).RenderTransform).Angle+45);
                    ((Image)mainWindow.loadingUi.Children[1]).RenderTransform = rotateTransform;
                }), DispatcherPriority.Render);
                Thread.Sleep(100);
            }
        }
        public static void cancellation()
        {
            annul = true;
        }
    }
}

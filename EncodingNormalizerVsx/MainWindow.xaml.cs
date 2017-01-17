using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EncodingNormalizerVsx
{
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //ContentRendered
        }

        public new UserControl Content { set; get; }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LoadStoryboard_Completed(object sender, EventArgs e)
        {
            ((Panel)SplashPanel.Parent).Children.Remove(SplashPanel);
            SplashPanel = null;
            SplashLogo = null;

            Frame.Navigate(Content);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Tobii.EyeTracking.IO;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ZoomPrototype zoom;
        private ConnectToTracker connect;
        
        public MainWindow()
        {
            Library.Init();
            InitializeComponent();
            connect = new ConnectToTracker();
        }

        private void ZoomPrototypeButton_OnClick(object sender, RoutedEventArgs e)
        {
            zoom = new ZoomPrototype();
            this.Hide();
            zoom.Show();
        }

        private void EyeTracker_onClick(object sender, RoutedEventArgs e)
        {
            connect.Show();
        }
    }
}

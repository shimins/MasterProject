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
        private ZoomPrototype _zoom;
        private readonly Tracker _tracker;
        
        public MainWindow()
        {
            Library.Init();
            InitializeComponent();
            _zoom = new ZoomPrototype();
            _tracker = new Tracker(_zoom);
        }

        private void ZoomPrototypeButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Hide();
            _zoom.Show();
            GlobalValue.MapTracking = true;
        }

        private void EyeTracker_onClick(object sender, RoutedEventArgs e)
        {
            _tracker.Show();
        }
    }
}

using System;
using System.Windows;
using Microsoft.Maps.MapControl.WPF;
using Tobii.EyeTracking.IO;

namespace WPF
{
    /// <summary>
    /// Interaction logic for ZoomPrototype.xaml
    /// </summary>
    public partial class ZoomPrototype : Window
    {

        private Location centerLocation;
           
        public ZoomPrototype(Tracker tracker)
        {
            InitializeComponent();
            map.Mode = new AerialMode(true);
            this.centerLocation = map.Center;
        }
    }
}

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using WindowsInput;
using Microsoft.Maps.MapControl.WPF;

namespace WPF
{
    /// <summary>
    /// Interaction logic for ZoomPrototype.xaml
    /// </summary>
    public partial class ZoomPrototype : Window
    {
        private Location _currentLocation;

        private Point point;
        
        public ZoomPrototype()
        {
            InitializeComponent();
            map.Mode = new AerialMode(true);
            _currentLocation = map.Center;
            

        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(long dwFlags, double dx, double dy, long cButtons, long dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;




        public void setNewGazePoint(Point point)
        {

        }

        //private void tracker_eyeGazeMovement(Point point)
        //{
        //    Console.WriteLine("we need to move");
        //    Location direction = map.ViewportPointToLocation(point);
        //    map.Center = new Location((_currentLocation.Latitude + (direction.Latitude / map.ZoomLevel / map.ZoomLevel)),
        //        (_currentLocation.Longitude + (direction.Longitude / (map.ZoomLevel * map.ZoomLevel))));

        //    _currentLocation = map.Center;
        //}

        private void Window_Closed(object sender, EventArgs e)
        {
            GlobalValue.MapTracking = false;
        }

        //private void BingMapsAPIProjection()
        //{
        //    var pixelGlobeSize = this.PixelTileSize * Math.Pow(2d, map.ZoomLevel);
        //    XPixelsToDegreesRatio = pixelGlobeSize / 360d;
        //    this.YPixelsToRadiansRatio = pixelGlobeSize / (2d * Math.PI);
        //    var halfPixelGlobeSize = Convert.ToSingle(pixelGlobeSize / 2d);
        //    this.PixelGlobeCenter = new Point(
        //        halfPixelGlobeSize, halfPixelGlobeSize);
        //}

        //public Point FromCoordinatesToPixel(Point coordinates)
        //{
        //    var x = Math.Round(this.PixelGlobeCenter.X
        //        + (coordinates.X * this.XPixelsToDegreesRatio));
        //    var f = Math.Min(
        //        Math.Max(
        //             Math.Sin(coordinates.Y * RadiansToDegreesRatio),
        //            -0.9999d),
        //        0.9999d);
        //    var y = Math.Round(this.PixelGlobeCenter.Y + .5d *
        //        Math.Log((1d + f) / (1d - f)) * -this.YPixelsToRadiansRatio);
        //    return new Point(Convert.ToSingle(x), Convert.ToSingle(y));
        //}

        //public Point FromPixelToCoordinates(Point pixel)
        //{
        //    var longitude = (pixel.X - this.PixelGlobeCenter.X) /
        //        this.XPixelsToDegreesRatio;
        //    var latitude = (2 * Math.Atan(Math.Exp(
        //        (pixel.Y - this.PixelGlobeCenter.Y) / -this.YPixelsToRadiansRatio))
        //        - Math.PI / 2) * DegreesToRadiansRatio;
        //    return new Point(
        //        Convert.ToSingle(latitude),
        //        Convert.ToSingle(longitude));
        //}

        //private void EyeTracker_onClick(object sender, RoutedEventArgs e)
        //{
        //    if (_tracker.Visibility == Visibility.Hidden)
        //    {
        //        _tracker.Show();
        //    }
        //    else if (_tracker.Visibility == Visibility.Visible)
        //    {
        //    }
        //    else
        //    {
        //        _tracker = new Tracker();
        //        _tracker.Show();
        //    }
        //}
    }
}

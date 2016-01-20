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
        
        private Tracker tracker;


        public ZoomPrototype(Tracker tracker)
        {
            InitializeComponent();
            map.Mode = new AerialMode(true);

            this.tracker = tracker;
            tracker.EyeGazeMovement += tracker_eyeGazeMovement;

        }

        private void tracker_eyeGazeMovement(object sender, EventArgs e)
        {
            Console.WriteLine("we need to move");
            Location location = map.ViewportPointToLocation(GlobalValue.Point);

            map.Center = location;
            Console.WriteLine(map.Center);
            //BingMapsAPIProjection();
            //var point = FromPixelToCoordinates(GlobalValue.Point);
            //map.Center = new Location(point.X, point.Y);

        }

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

        private void EyeTracker_onClick(object sender, RoutedEventArgs e)
        {
            if (tracker.Visibility == Visibility.Hidden)
            {
                tracker.Show();
            }
            else if (tracker.Visibility == Visibility.Visible)
            {
            }
            else
            {
                tracker = new Tracker();
                tracker.Show();
            }
        }
    }
}

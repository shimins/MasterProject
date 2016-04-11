using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Prototype4
{
    /// <summary>
    /// Interaction logic for MapControll.xaml
    /// </summary>
    public partial class MapControll
    {
        public event EventHandler zoneHaveChanged;

        private ZoneEnum zoneEnum;
        private int currentZone = 4;

        public MapControll()
        {
            InitializeComponent();

            TransformGroup group = new TransformGroup();
            ScaleTransform st = new ScaleTransform();
            group.Children.Add(st);
            TranslateTransform tt = new TranslateTransform();
            group.Children.Add(tt);
            mapElement.RenderTransform = group;
            mapElement.RenderTransformOrigin = new Point(0.0, 0.0);

            zoneEnum = new ZoneEnum();
        }

        public UIElement GetMapElement()
        {
            return mapElement;
        }

        private TranslateTransform getTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform)
                .Children.First(tr => tr is TranslateTransform);
        }

        private ScaleTransform getScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform)
                .Children.First(tr => tr is ScaleTransform);
        }

        private void zoom_event(double zoomFactor, Point current)
        {
            if (mapElement != null)
            {
                var st = getScaleTransform(mapElement);
                var tt = getTransform(mapElement);

                double zoom = zoomFactor > 0 ? -.01 * st.ScaleX : .01 * st.ScaleX;

                if (st.ScaleX + zoom > .1 && st.ScaleY + zoom < 5)
                {
                    Point relative = mapElement.PointFromScreen(current);
                    relative.X += 447;


                    double abosuluteX = relative.X * st.ScaleX + tt.X;
                    double abosuluteY = relative.Y * st.ScaleY + tt.Y;

                    st.ScaleX += zoom;
                    st.ScaleY += zoom;

                    tt.X = abosuluteX - relative.X * st.ScaleX;
                    tt.Y = abosuluteY - relative.Y * st.ScaleY;
                    CheckZoneChange(mapElement.PointFromScreen(current));
                }
            }
        }

        private void EyeMoveDuringAction(Point current)
        {
            if (mapElement == null) return;
            if (mapElement.PointFromScreen(current).X < 0 || mapElement.PointFromScreen(current).X > mapElement.RenderSize.Width
                || mapElement.PointFromScreen(current).Y < 0 || mapElement.PointFromScreen(current).X > mapElement.RenderSize.Width)
                return;
            var tt = getTransform(mapElement);
            tt.X -= (current.X - (Width / 2 + 447)) * 0.025;
            tt.Y -= (current.Y - Height / 2) * 0.025;
            CheckZoneChange(mapElement.PointFromScreen(current));
        }

        private void CheckZoneChange(Point current)
        {
            ZoneBorder zoneBorder = zoneEnum._zoneList.SingleOrDefault(x => x.IsInside(current));

            if (zoneBorder != null && zoneEnum._zoneList.IndexOf(zoneBorder) != currentZone)
            {
                currentZone = zoneEnum._zoneList.IndexOf(zoneBorder);
                EventHandler handler = zoneHaveChanged;
                handler?.Invoke(this, EventArgs.Empty);
            }
        }

        public int getCurrentZone()
        {
            return currentZone;
        }

        public void MapInteraction(bool zoomActionButtonDown, Point current, double zoomfactor)
        {
            if (zoomActionButtonDown)
            {
                zoom_event(zoomfactor, current);
            }
            else
            {
                EyeMoveDuringAction(current);
            }
        }
    }
}

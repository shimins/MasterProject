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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Prototype4
{
    /// <summary>
    /// Interaction logic for MapControll.xaml
    /// </summary>
    public partial class MapControll : UserControl
    {
        
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
        }

        public UIElement GetMapElement()
        {
            return this.mapElement;
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

        private void zoom_event(double zoomFactor, Point _current)
        {
            if (mapElement != null)
            {


                var st = getScaleTransform(mapElement);
                var tt = getTransform(mapElement);

                double zoom = zoomFactor > 0 ? -.01 * st.ScaleX : .01 * st.ScaleX;
                Console.WriteLine("zoom");

                if (st.ScaleX + zoom > .1 && st.ScaleY + zoom < 5)
                {
                    Point relative = mapElement.PointFromScreen(_current);


                    double abosuluteX = relative.X * st.ScaleX + tt.X;
                    double abosuluteY = relative.Y * st.ScaleY + tt.Y;

                    st.ScaleX += zoom;
                    st.ScaleY += zoom;

                    tt.X = abosuluteX - relative.X * st.ScaleX;
                    tt.Y = abosuluteY - relative.Y * st.ScaleY;
                }
            }
        }

        private void EyeMoveDuringAction(Point _current)
        {
            if (mapElement == null) return;
            if (mapElement.PointFromScreen(_current).X < 0 || mapElement.PointFromScreen(_current).X > mapElement.RenderSize.Width
                || mapElement.PointFromScreen(_current).Y < 0 || mapElement.PointFromScreen(_current).X > mapElement.RenderSize.Width)
                return;
            var tt = getTransform(mapElement);
            tt.X -= (_current.X - (Width / 2 + 447)) * 0.025;
            tt.Y -= (_current.Y - Height / 2) * 0.025;
        }

        public void mapInteraction(bool zoomActionButtonDown, Point current, double zoomfactor)
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

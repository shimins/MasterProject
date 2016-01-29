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

namespace MouseMode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Point start;
        private Point origin;
        private UIElement child = null;
        
        public MainWindow()
        {
            InitializeComponent();
            this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;


            this.child = Image;
            TransformGroup group = new TransformGroup();
            ScaleTransform st = new ScaleTransform();
            group.Children.Add(st);
            TranslateTransform tt = new TranslateTransform();
            group.Children.Add(tt);
            child.RenderTransform = group;
            child.RenderTransformOrigin = new Point(0.0, 0.0);
        }

        private TranslateTransform getTransform(UIElement element)
        {
            return (TranslateTransform) ((TransformGroup) element.RenderTransform)
                .Children.First(tr => tr is TranslateTransform);
        }

        private ScaleTransform getScaleTransform(UIElement element)
        {
            return (ScaleTransform) ((TransformGroup) element.RenderTransform)
                .Children.First(tr => tr is ScaleTransform);
        }

        private void reset()
        {
            if (Image != null)
            {
                var st = getScaleTransform(Image);
                st.ScaleX = 1.0;
                st.ScaleY = 1.0;

                var tt = getTransform(Image);
                tt.X = 0.0;
                tt.Y = 0.0;
            }
        }

        private void zoom_event(object sender, MouseWheelEventArgs e)
        {
            if (Image != null)
            {
                var st = getScaleTransform(Image);
                var tt = getTransform(Image);

                double zoom = e.Delta > 0 ? .2 : -.2;
                if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
                    return;

                Point relative = e.GetPosition(child);

                var abosuluteX = relative.X * st.ScaleX + tt.X;
                var abosuluteY = relative.Y * st.ScaleY + tt.Y;


                double factor = 0.5;
                st.ScaleX += zoom * factor;
                st.ScaleY += zoom * factor;
                
                tt.X = abosuluteX - relative.X * st.ScaleX;
                tt.Y = abosuluteY - relative.Y * st.ScaleY;
            }
        }

        //calculate movement during action button down, calculate vectors and everything.
        //if we want to use eye tracker points instead maybe we can put it in here. on vector calculation
        //and the applie the new vector on transformation 
        private void eyeMoveDuringAction(object sender, MouseEventArgs eventArgs)
        {
            if (Image != null)
            {
                if (Image.IsMouseCaptured)
                {
                    var tt = getTransform(Image);
                    Vector vector = start - eventArgs.GetPosition(ViewBox);
                    tt.X = origin.X - vector.X;
                    tt.Y = origin.Y - vector.Y;
                }
            }
        }


        //start recording of mouse cursor position and capture the mouse cursor on the image
        private void actionButtonDown(object sender, MouseButtonEventArgs eventArgs)
        {
            if (Image != null)
            {
                var tt = getTransform(Image);
                start = eventArgs.GetPosition(ViewBox);
                origin = new Point(tt.X, tt.Y);
                Image.CaptureMouse();
                ViewBox.Cursor = Cursors.Hand;
            }
        }


        //stop recording of mouse cursor and end movement.
        private void actionButtonUp(object sender, MouseButtonEventArgs eventArgs)
        {
            if (Image != null)
            {
                Image.ReleaseMouseCapture();
                ViewBox.Cursor = Cursors.Arrow;
            }
        }

        private void reset(object sender, MouseButtonEventArgs eventArgs)
        {
            reset();
        }
    }
}

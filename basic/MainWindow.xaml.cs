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

namespace basic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private UIElement child = null;
        private Point relative;
        private Point panValue;

        private bool buttonDown;


        public MainWindow()
        {
            InitializeComponent();

            child = image;
            TransformGroup group = new TransformGroup();
            ScaleTransform st = new ScaleTransform();
            group.Children.Add(st);
            TranslateTransform tt = new TranslateTransform();
            group.Children.Add(tt);
            child.RenderTransform = group;
            child.RenderTransformOrigin = new Point(0.0, 0.0);

            panValue = new Point(image.Width/2, image.Height/2);
        }


        private void Rectangle_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            buttonDown = true;
        }

        private void Rectangle_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (child != null && buttonDown == true)
            {
                var st = GetScaleTransform(child);
                var tt = GetTranslateTransform(child);


                double moveX = e.GetPosition(Grid).X - Width/2 > 0 ? -10 : 10;
                double moveY = e.GetPosition(Grid).Y - Height / 2 > 0 ? -10 : 10;

                tt.X += moveX;
                tt.Y += moveY;




            }
        }

        private TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is TranslateTransform);
        }

        private ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is ScaleTransform);
        }

        private void Rectangle_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {

            if (child != null)
            {
                var st = GetScaleTransform(child);
                var tt = GetTranslateTransform(child);

                double zoom = e.Delta > 0 ? .2 : -.2;
                if (!(e.Delta > 0) && (st.ScaleX < .2 || st.ScaleY < .2))
                    return;



                //Point point = e.GetPosition(child);
                Console.WriteLine(e.GetPosition(child));
                //Console.WriteLine(panValue);
                //Console.WriteLine(child.PointFromScreen());

                Console.WriteLine(tt.X);

                Console.WriteLine();

                double abosuluteX = panValue.X * st.ScaleX + tt.X;
                double abosuluteY = panValue.Y * st.ScaleY + tt.Y;


                st.ScaleX += zoom;
                st.ScaleY += zoom;



                tt.X = abosuluteX - panValue.X * st.ScaleX;
                tt.Y = abosuluteY - panValue.Y * st.ScaleY;

            }
        }

        private void Rectangle_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            buttonDown = false;

        }
    }
}

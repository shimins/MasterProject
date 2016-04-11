using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Prototype4.Swipe
{
    public class View : Selector
    {
        #region Private Fields
        private ContentControl _partCurrentItem;
        private ContentControl _partPreviousItem;
        private ContentControl _partNextItem;
        private FrameworkElement _partRoot;
        private FrameworkElement _partContainer;
        private double _fromValue;
        private double _elasticFactor = 1.0;
        #endregion

        #region Constructor
        static View()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(View), new FrameworkPropertyMetadata(typeof(View)));
            SelectedIndexProperty.OverrideMetadata(typeof(View), new FrameworkPropertyMetadata(-1, OnSelectedIndexChanged));
        }

        public View()
        {
            CommandBindings.Add(new CommandBinding(NextCommand, OnNextExecuted, OnNextCanExecute));
            CommandBindings.Add(new CommandBinding(PreviousCommand, OnPreviousExecuted, OnPreviousCanExecute));
        }
        #endregion

        #region Private methods
        private void OnRootManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            _fromValue = e.TotalManipulation.Translation.X;
            if (_fromValue > 0)
            {
                if (SelectedIndex > 0)
                {
                    SelectedIndex -= 1;
                }
            }
            else
            {
                if (SelectedIndex < Items.Count - 1)
                {
                    SelectedIndex += 1;
                }
            }

            if (_elasticFactor < 1)
            {
                RunSlideAnimation(0, ((MatrixTransform)_partRoot.RenderTransform).Matrix.OffsetX);
            }
            _elasticFactor = 1.0;
        }

        private void OnRootManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (!(_partRoot.RenderTransform is MatrixTransform))
            {
                _partRoot.RenderTransform = new MatrixTransform();
            }

            Matrix matrix = ((MatrixTransform)_partRoot.RenderTransform).Matrix;
            var delta = e.DeltaManipulation;

            if ((SelectedIndex == 0 && delta.Translation.X > 0 && _elasticFactor > 0)
                || (SelectedIndex == Items.Count - 1 && delta.Translation.X < 0 && _elasticFactor > 0))
            {
                _elasticFactor -= 0.05;
            }

            matrix.Translate(delta.Translation.X * _elasticFactor, 0);
            _partRoot.RenderTransform = new MatrixTransform(matrix);

            e.Handled = true;
        }

        private void OnRootManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = _partContainer;
            e.Handled = true;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshViewPort(SelectedIndex);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (SelectedIndex >= 0)
            {
                RefreshViewPort(SelectedIndex);
            }
        }
        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as View;

            control.OnSelectedIndexChanged(e);
        }

        private void OnSelectedIndexChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!EnsureTemplateParts())
            {
                return;
            }

            if ((int)e.NewValue >= 0 && (int)e.NewValue < Items.Count)
            {
                double toValue = (int)e.OldValue < (int)e.NewValue ? -ActualWidth : ActualWidth;
                //Debug.WriteLine(toValue + " - vs - " + fromValue);
                RunSlideAnimation(toValue, _fromValue);
            }
        }

        private void RefreshViewPort(int selectedIndex)
        {
            if (!EnsureTemplateParts())
            {
                return;
            }

            Canvas.SetLeft(_partPreviousItem, -ActualWidth);
            Canvas.SetLeft(_partNextItem, ActualWidth);
            _partRoot.RenderTransform = new TranslateTransform();

            var currentItem = GetItemAt(selectedIndex);
            object nextItem;
            object previousItem;
            if (selectedIndex == 0)
            {
                nextItem = GetItemAt(selectedIndex + 1);
                previousItem = GetItemAt(Items.Count - 1);
            }
            else if (selectedIndex == Items.Count - 1)
            {
                nextItem = GetItemAt(0);
                previousItem = GetItemAt(selectedIndex - 1);
            }
            else
            {
                nextItem = GetItemAt(selectedIndex + 1);
                previousItem = GetItemAt(selectedIndex - 1);
            }

            _partCurrentItem.Content = currentItem;
            _partNextItem.Content = nextItem;
            _partPreviousItem.Content = previousItem;
        }

        public void RunSlideAnimation(double toValue, double fromValue = 0)
        {
            if (!(_partRoot.RenderTransform is TranslateTransform))
            {
                _partRoot.RenderTransform = new TranslateTransform();
            }

            var story = AnimationFactory.Instance.GetAnimation(_partRoot, toValue, fromValue);
            story.Completed += (s, e) =>
            {
                //Debug.WriteLine(toValue + " -|- " + fromValue);
                RefreshViewPort(SelectedIndex);
            };
            story.Begin();
        }

        private object GetItemAt(int index)
        {
            if (index < 0 || index >= Items.Count)
            {
                return null;
            }

            return Items[index];
        }

        private bool EnsureTemplateParts()
        {
            return _partCurrentItem != null &&
                _partNextItem != null &&
                _partPreviousItem != null &&
                _partRoot != null;
        }

        private void OnPreviousCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedIndex > 0;
        }

        private void OnPreviousExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SelectedIndex -= 1;
        }

        private void OnNextCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedIndex < (Items.Count - 1);
        }

        private void OnNextExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SelectedIndex += 1;
        }
        #endregion

        #region Commands

        public static RoutedUICommand NextCommand = new RoutedUICommand("Next", "Next", typeof(View));
        public static RoutedUICommand PreviousCommand = new RoutedUICommand("Previous", "Previous", typeof(View));

        #endregion

        #region Override methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _partPreviousItem = GetTemplateChild("PART_PreviousItem") as ContentControl;
            _partNextItem = GetTemplateChild("PART_NextItem") as ContentControl;
            _partCurrentItem = GetTemplateChild("PART_CurrentItem") as ContentControl;
            _partRoot = GetTemplateChild("PART_Root") as FrameworkElement;
            _partContainer = GetTemplateChild("PART_Container") as FrameworkElement;

            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;
            _partRoot.ManipulationStarting += OnRootManipulationStarting;
            _partRoot.ManipulationDelta += OnRootManipulationDelta;
            _partRoot.ManipulationCompleted += OnRootManipulationCompleted;
        }
        #endregion
    }
}

using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Swipe
{
    public class AnimationFactory
    {
        public static AnimationFactory Instance => new AnimationFactory();

        public Storyboard GetAnimation(DependencyObject target, double to, double from)
        {
            Storyboard story = new Storyboard();
            Storyboard.SetTargetProperty(story, new PropertyPath("(TextBlock.RenderTransform).(TranslateTransform.X)"));
            Storyboard.SetTarget(story, target);

            var doubleAnimation = new DoubleAnimationUsingKeyFrames();

            EasingDoubleKeyFrame fromFrame = new EasingDoubleKeyFrame(from);
            fromFrame.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseInOut };
            fromFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0));

            EasingDoubleKeyFrame toFrame = new EasingDoubleKeyFrame(to);
            toFrame.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseInOut };
            toFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(1000));

            doubleAnimation.KeyFrames.Add(fromFrame);
            doubleAnimation.KeyFrames.Add(toFrame);
            story.Children.Add(doubleAnimation);

            return story;
        }
    }
}

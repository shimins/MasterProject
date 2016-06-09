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

            var fromFrame = new EasingDoubleKeyFrame(from)
            {
                EasingFunction = new ExponentialEase() {EasingMode = EasingMode.EaseIn},
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))
            };

            var toFrame = new EasingDoubleKeyFrame(to)
            {
                EasingFunction = new QuadraticEase() {EasingMode = EasingMode.EaseOut},
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(600))
            };

            doubleAnimation.KeyFrames.Add(fromFrame);
            doubleAnimation.KeyFrames.Add(toFrame);
            story.Children.Add(doubleAnimation);

            return story;
        }
    }
}

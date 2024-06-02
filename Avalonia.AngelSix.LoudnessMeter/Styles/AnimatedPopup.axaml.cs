using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using System;

namespace Avalonia.AngelSix.LoudnessMeter.Styles
{
    public class AnimatedPopup : ContentControl
    {
        /// <summary>
        /// Store the control desired size
        /// </summary>
        private Size _desiredSize;

        /// <summary>
        /// A flag for when we are animating
        /// </summary>
        private bool _isAnimating;

        /// <summary>
        /// The animation UI timer
        /// </summary>
        private DispatcherTimer _animationTimer;

        /// <summary>
        /// The current position in the animation
        /// </summary>
        private int _animationCurrentTick;


        /// <summary>
        /// CTOR
        /// </summary>
        public AnimatedPopup()
        {
            // Get a 60 FPS timespan
            var framerate = TimeSpan.FromSeconds(1 / 60.0);

            // Make a new dispatcher timer
            _animationTimer = new DispatcherTimer()
            {
                // Set the timer to run 60 times a second
                Interval = framerate,
            };

            // Fix for 3 seconds
            var animationTime = TimeSpan.FromSeconds(0.2);

            // Calculate total ticks that make up the animation time
            var totalTicks = (int)(animationTime.TotalSeconds / framerate.TotalSeconds);

            // Keep track of current tick
            _animationCurrentTick = 0;

            // Callback on every tick
            _animationTimer.Tick += (sender, e) =>
            {
                // Increment the tick
                _animationCurrentTick++;

                // Set animating flag
                _isAnimating = true;

                // If we have reached the total ticks...
                if (_animationCurrentTick > totalTicks)
                {
                    // Stop this animation timer
                    _animationTimer.Stop();

                    // Clear animating flag
                    _isAnimating = false;

                    // Break out of code
                    return;
                }

                // Get percentage of the way through the current animation
                var percentageAnimated = (float)_animationCurrentTick / totalTicks;

                // Make an animation easing
                var easing = new QuadraticEaseIn();

                // Calculate final width and height
                var finalWidth = _desiredSize.Width * easing.Ease(percentageAnimated);
                var finalHeight = _desiredSize.Height * easing.Ease(percentageAnimated);

                // Do our animation
                Width = finalWidth;
                Height = finalHeight;

                Console.WriteLine($"Timer tick {_animationCurrentTick}");
            };
        }

        public override void Render(DrawingContext context)
        {
            // If we are not animating...
            if (!_isAnimating)
            {
                // Set desired size only once (which including margin, so remove that from our calculation)
                _desiredSize = DesiredSize - Margin;

                // Reset animation position
                _animationCurrentTick = 0;

                //Start timer
                _animationTimer.Start();

            }

            base.Render(context);
        }
    }
}

using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading;


namespace Avalonia.AngelSix.LoudnessMeter.Styles
{
    public partial class AnimatedPopup : ContentControl
    {
        private int _animationCurrentTick;                                  // - The current position in the animation
        private double _originalOpacity;                                    // - Store the controls originalOpacityvalue at startup
        private bool _isAnimating;                                          // - A flag for when we are animating
        private bool _isFirstAnimation = true;                              // - Indicates if this is the first time we are animating 
        private bool _isSizeFound;                                          // - Keeps track of if we have found the desired 100% width/height auto size
        private DispatcherTimer _animationTimer;                            // - The animation UI timer
        private Timer _sizingTimer;                                         // - The timeout timer to detect when auto sizing has finished firing
        private Size _desiredSize;                                          // - Store the control desired size
        private TimeSpan _frameRate = TimeSpan.FromSeconds(1 / 60.0);       // - Get a 60 FPS timespan


        /// <summary>
        /// Calculate total ticks that make up the animation time
        /// </summary>
        private int _TotalTicks => (int)(_animationTime.TotalSeconds / _frameRate.TotalSeconds);

        /// <summary>
        /// Indicates if the control is currently opened
        /// </summary>
        public bool IsOpened => _animationCurrentTick >= _TotalTicks;



        //#########################################################################################################################
        #region IsOpen DirectProperty

        private bool _isOpen;

        /// <summary>
        /// IsOpen DirectProperty definition
        /// </summary>
        public static readonly DirectProperty<AnimatedPopup, bool> IsOpenProperty =
            AvaloniaProperty.RegisterDirect<AnimatedPopup, bool>(nameof(IsOpen),
                o => o.IsOpen,
                (o, v) => o.IsOpen = v);

        /// <summary>
        /// Gets or sets the IsOpen property. This DirectProperty 
        /// indicates Sets whether the control should be open or closed
        /// </summary>
        public bool IsOpen
        {
            get => _isOpen;
            set => SetAndRaise(IsOpenProperty, ref _isOpen, value);
        }

        #endregion // IsOpen DirectProperty



        //#########################################################################################################################
        #region AnimationTime DirectProperty

        private TimeSpan _animationTime = TimeSpan.FromSeconds(3);

        /// <summary>
        /// AnimationTime DirectProperty definition
        /// </summary>
        public static readonly DirectProperty<AnimatedPopup, TimeSpan> AnimationTimeProperty =
            AvaloniaProperty.RegisterDirect<AnimatedPopup, TimeSpan>(nameof(AnimationTime),
                o => o.AnimationTime,
                (o, v) => o.AnimationTime = v);

        /// <summary>
        /// Gets or sets the AnimationTime property. This DirectProperty 
        /// indicates ....
        /// </summary>
        public TimeSpan AnimationTime
        {
            get => _animationTime;
            set => SetAndRaise(AnimationTimeProperty, ref _animationTime, value);
        }

        #endregion // AnimationTime DirectProperty


        /// <summary>
        /// CTOR
        /// </summary>
        public AnimatedPopup()
        {
            // Remember original controls opacity
            _originalOpacity = Opacity;

            // Set to invisible
            Opacity = 0;

            // Make a new dispatcher timer
            _animationTimer = new DispatcherTimer()
            {
                // Set the timer to run 60 times a second
                Interval = _frameRate,
            };

            _sizingTimer = new Timer(t =>
            {
                // If we already calculated the size...
                if (_isSizeFound)
                {
                    // No longer accept new sizez
                    return;
                }

                // We have now found our desired size
                _isSizeFound = true;

                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    // Set the desired size
                    _desiredSize = DesiredSize - Margin;

                    // Update animation
                    UpdateAnimation();
                });

            });

            // Fix for 3 seconds
            var animationTime = TimeSpan.FromSeconds(0.2);

            // Callback on every tick
            _animationTimer.Tick += (s, e) => AnimationTick();
        }



        //#########################################################################################################################
        #region Commands

        [RelayCommand]
        public void BeginOpen()
        {
            IsOpen = true;

            // Update animation
            UpdateAnimation();
        }


        [RelayCommand]
        public void BeginClose()
        {
            IsOpen = false;

            // Update animation
            UpdateAnimation();
        }

        #endregion // Commands


        public override void Render(DrawingContext context)
        {
            // If we have not yet found desired size...
            if (!_isSizeFound)
                _sizingTimer.Change(100, int.MaxValue);

            base.Render(context);
        }


        /// <summary>
        /// Update controls sizes based on the next tick of an animation
        /// </summary>
        private void AnimationTick()
        {
            // If this is the firstcall after calculating the desired size...
            if (_isFirstAnimation)
            {
                // Clear the flag
                _isFirstAnimation = false;
                
                // Stop this animation timer
                _animationTimer.Stop();

                // Reset opacity
                Opacity = _originalOpacity;

                // Set the final size
                Width = _isOpen ? _desiredSize.Width : 0;
                Height = _isOpen ? _desiredSize.Height : 0;

                // Do on this tick
                return;
            }

            //// Increment the tick
            //_animationCurrentTick++;

            // If we have reached the end of our animation...
            if ((_isOpen && _animationCurrentTick >= _TotalTicks)
                || (!_isOpen && _animationCurrentTick == 0))
            {
                // Stop this animation timer
                _animationTimer.Stop();

                // Set the final size
                Width = _isOpen ? _desiredSize.Width : 0;
                Height = _isOpen ? _desiredSize.Height : 0;

                // Clear animating flag
                _isAnimating = false;

                // Break out of code
                return;
            }


            // Set animating flag
            _isAnimating = true;

            // Move the tick in the right direction
            _animationCurrentTick += _isOpen ? 1 : -1;

            // Get percentage of the way through the current animation
            var percentageAnimated = (float)_animationCurrentTick / _TotalTicks;

            // Make an animation easing
            var easing = new QuadraticEaseIn();

            // Calculate final width and height
            var finalWidth = _desiredSize.Width * easing.Ease(percentageAnimated);
            var finalHeight = _desiredSize.Height * easing.Ease(percentageAnimated);

            // Do our animation
            Width = finalWidth;
            Height = finalHeight;

            Console.WriteLine($"Timer tick {_animationCurrentTick}");
        }


        /// <summary>
        /// Calculate and start any new required animations
        /// </summary>
        private void UpdateAnimation()
        {
            // Do nothing if we still haven't found our initial size
            if (!_isSizeFound)
                return;

            // Start the animation thread again
            _animationTimer.Start();
        }
    }
}
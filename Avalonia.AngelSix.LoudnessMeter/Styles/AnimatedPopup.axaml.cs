using Avalonia;
using Avalonia.Animation;
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
        private Control _underlayControl;                                   // - The underlay control for closing this popup

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
            set
            {
                // If the value has not changed...
                if (value == _isOpen)
                {
                    // Do nothing
                    return;
                }

                // If we are opening...
                if (value)
                {
                    // If the parent is a Grid...
                    if (Parent is Grid grid)
                    {
                        Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            // Set grid row/column span
                            if (grid.RowDefinitions?.Count > 0)
                            {
                                _underlayControl.SetValue(Grid.RowSpanProperty, grid.RowDefinitions?.Count);
                            }

                            if (grid.ColumnDefinitions?.Count > 0)
                            {
                                _underlayControl.SetValue(Grid.ColumnSpanProperty, grid.ColumnDefinitions?.Count);
                            }

                            // Insert the underlay control
                            if (!grid.Children.Contains(_underlayControl))
                            {
                                grid.Children.Insert(0, _underlayControl);
                            }
                        });
                    }
                }
                // If closing...
                else
                {
                    // If the control is currently fully open...
                    if (IsOpened)
                    {
                        // Update desired size
                        UpdateDesiredSize();
                    }
                }

                // Update animation
                UpdateAnimation();

                // Raise the property changed event
                SetAndRaise(IsOpenProperty, ref _isOpen, value);
            }
        }

        #endregion // IsOpen DirectProperty



        //#########################################################################################################################
        #region IsAnimateOpacity DirectProperty

        private bool _isAnimateOpacity = true;

        /// <summary>
        /// IsAnimateOpacity DirectProperty definition
        /// </summary>
        public static readonly DirectProperty<AnimatedPopup, bool> IsAnimateOpacityProperty =
            AvaloniaProperty.RegisterDirect<AnimatedPopup, bool>(nameof(IsAnimateOpacity),
                o => o.IsAnimateOpacity,
                (o, v) => o.IsAnimateOpacity = v);

        /// <summary>
        /// Gets or sets the IsAnimateOpacity property. This DirectProperty 
        /// indicates ....
        /// </summary>
        public bool IsAnimateOpacity
        {
            get => _isAnimateOpacity;
            set => SetAndRaise(IsAnimateOpacityProperty, ref _isAnimateOpacity, value);
        }

        #endregion // IsAnimateOpacity DirectProperty



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



        //#########################################################################################################################
        #region UnderlayOpacity DirectProperty

        private double _underlayOpacity = 0.2;

        /// <summary>
        /// UnderlayOpacity DirectProperty definition
        /// </summary>
        public static readonly DirectProperty<AnimatedPopup, double> UnderlayOpacityProperty =
            AvaloniaProperty.RegisterDirect<AnimatedPopup, double>(nameof(UnderlayOpacity),
                o => o.UnderlayOpacity,
                (o, v) => o.UnderlayOpacity = v);

        /// <summary>
        /// Gets or sets the UnderlayOpacity property. This DirectProperty 
        /// indicates underlay opacity.
        /// </summary>
        public double UnderlayOpacity
        {
            get => _underlayOpacity;
            set => SetAndRaise(UnderlayOpacityProperty, ref _underlayOpacity, value);
        }

        #endregion // UnderlayOpacity DirectProperty



        /// <summary>
        /// CTOR
        /// </summary>
        public AnimatedPopup()
        {
            // Make a new underlay control
            _underlayControl = new Border
            {
                Background = Brushes.Black,
                Opacity = 0,
                ZIndex = 9,
            };

            // On press, close popup
            _underlayControl.PointerPressed += (s, a) => BeginClose();

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
                    // Update desired size
                    UpdateDesiredSize();

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
        }


        [RelayCommand]
        public void BeginClose()
        {
            IsOpen = false;
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
                AnimationComplete();

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
                AnimationComplete();

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

            // Animate opacity
            if (IsAnimateOpacity)
            {
                Opacity = _originalOpacity * easing.Ease(percentageAnimated);
            }

            // Animate underlay
            _underlayControl.Opacity = _underlayOpacity * easing.Ease(percentageAnimated);

            Console.WriteLine($"Timer tick {_animationCurrentTick}");
        }

        /// <summary>
        /// Should be callsed, when an open or close transition has complete
        /// </summary>
        private void AnimationComplete()
        {
            // If open ...
            if (_isOpen)
            {
                // Set Size to desired size
                Width = double.NaN;
                Height = double.NaN;

                // Make shure opacity is set to original value
                Opacity = _originalOpacity;
            }
            // If closed ...
            else
            {
                // Set Size to 0
                Width = 0;
                Height = 0;

                // If the parent is a Grid...
                if (Parent is Grid grid
                    && grid.Children.Contains(_underlayControl))
                {
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        // Reset opacity
                        _underlayControl.Opacity = 0;

                        // Remove underlay
                        if (grid.Children.Contains(_underlayControl))
                        {
                            grid.Children.Remove(_underlayControl);
                        }
                    });
                }
            }
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


        /// <summary>
        /// Update the animation desired size based on current visuals desired size
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void UpdateDesiredSize()
            => _desiredSize = DesiredSize - Margin;
    }
}

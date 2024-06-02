using Avalonia.Controls.Primitives;

namespace Avalonia.AngelSix.LoudnessMeter.Styles;

public class LargeLabelControl : TemplatedControl
{

    /// <summary>
    /// LargeText StyledProperty definition
    /// </summary>
    public static readonly StyledProperty<string> LargeTextProperty =
        AvaloniaProperty.Register<LargeLabelControl, string>(nameof(LargeText), "LARGE TEXT");

    /// <summary>
    /// Gets or sets the LargeText property. This StyledProperty
    /// indicates Large text on the templated field.
    /// </summary>
    public string LargeText
    {
        get => GetValue(LargeTextProperty);
        set => SetValue(LargeTextProperty, value);
    }


    /// <summary>
    /// SmallText StyledProperty definition
    /// </summary>
    public static readonly StyledProperty<string> SmallTextProperty =
        AvaloniaProperty.Register<LargeLabelControl, string>(nameof(SmallText), "SMALL TEXT");

    /// <summary>
    /// Gets or sets the SmallText property. This StyledProperty
    /// indicates Small text on the templated field.
    /// </summary>
    public string SmallText
    {
        get => GetValue(SmallTextProperty);
        set => SetValue(SmallTextProperty, value);
    }
}
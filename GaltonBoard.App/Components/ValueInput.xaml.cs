using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using GaltonBoard.Model.Enums;

namespace GaltonBoard.App.Components;

public partial class ValueInput : UserControl, INotifyPropertyChanged
{
    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set
        {
            SetValue(LabelProperty, value);
            OnPropertyChanged();
        }
    }

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register("Label", typeof(string), typeof(ValueInput), new PropertyMetadata(""));

    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set
        {
            SetValue(ValueProperty, value);
            OnPropertyChanged();
        }
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(string), typeof(ValueInput), new PropertyMetadata(""));

    public double Min
    {
        get => (double)GetValue(MinProperty);
        set
        {
            SetValue(MinProperty, value);
            OnPropertyChanged();
        }
    }

    public static readonly DependencyProperty MinProperty =
        DependencyProperty.Register("Min", typeof(double), typeof(ValueInput), new PropertyMetadata(double.MinValue));

    public double Max
    {
        get => (double)GetValue(MaxProperty);
        set
        {
            SetValue(MaxProperty, value);
            OnPropertyChanged();
        }
    }

    public static readonly DependencyProperty MaxProperty =
        DependencyProperty.Register("Max", typeof(double), typeof(ValueInput), new PropertyMetadata(double.MaxValue));

    public InputEnum InputType
    {
        get => (InputEnum)GetValue(InputTypeProperty);
        set
        {
            SetValue(InputTypeProperty, value);
            OnPropertyChanged();
        }
    }

    public static readonly DependencyProperty InputTypeProperty =
        DependencyProperty.Register("InputType", typeof(InputEnum), typeof(ValueInput), new PropertyMetadata(InputEnum.Integer));

    public int TextBoxWidth
    {
        get => (int)GetValue(TextBoxWidthProperty);
        set
        {
            SetValue(TextBoxWidthProperty, value);
            OnPropertyChanged();
        }
    }

    public static readonly DependencyProperty TextBoxWidthProperty =
        DependencyProperty.Register("TextBoxWidth", typeof(int), typeof(ValueInput), new PropertyMetadata(0));


    public ValueInput()
    {
        InitializeComponent();
    }

    private void ValueInputBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(Value))
        {
            SetDefaultText();
        }
        else
        {
            ValueInputBox.Text = Value;
        }
    }

    private void ValueInputBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var newValue = ValueInputBox.Text;
        var textAllowed = IsTextAllowed(ValueInputBox.Text);

        if (string.IsNullOrEmpty(newValue))
        {
            return;
        }

        if (textAllowed)
        {
            Value = newValue;
        }
        else
        {
            ValueInputBox.Text = Value;
        }
    }

    private void ValueInputBox_OnLostFocus(object sender, RoutedEventArgs e)
    {
        if (ValueInputBox.Text is ['-'])
        {
            SetDefaultText();
        }

        if (string.IsNullOrEmpty(ValueInputBox.Text))
        {
            SetDefaultText();
        }

        Value = ValueInputBox.Text;
    }

    private void ValueInputBox_OnGotFocus(object sender, RoutedEventArgs e)
    {
        Value = ValueInputBox.Text;
    }

    private bool IsTextAllowed(string text)
    {
        switch (InputType)
        {
            case InputEnum.Integer:
            {
                var length = text.Length;
                var canParse = int.TryParse(text, out var result);
                if (canParse && (result < Min || result > Max))
                    return false;
                if (text is ['-']) return true;

                return canParse && !(result == 0 && length != 1);
            }
            case InputEnum.Double:
            {
                var canParse = double.TryParse(text, out var result);
                if (canParse && (result < Min || result > Max))
                    return false;
                if (text is ['-']) return true;

                return canParse;
            }
            case InputEnum.String:
            default:
                return true;
        }
    }
    
    private void SetDefaultText()
    {
        const double tolerance = 0.00000000001;
        var text = ValueInputBox.Text;
        
        if (Math.Abs(Max - double.MaxValue) < tolerance && Math.Abs(Min - double.MinValue) < tolerance)
        {
            ValueInputBox.Text = InputType switch
            {
                InputEnum.Integer => "0",
                InputEnum.Double => "0.0",
                InputEnum.String => "",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        else
        {
            ValueInputBox.Text = InputType switch
            {
                InputEnum.Integer => Min.ToString(CultureInfo.InvariantCulture),
                InputEnum.Double => Min.ToString(CultureInfo.InvariantCulture),
                InputEnum.String => "",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public void Reset()
    {
        SetDefaultText();
    }

    public bool IsValid()
    {
        var text = ValueInputBox.Text;
        var textAllowed = IsTextAllowed(ValueInputBox.Text);

        return !string.IsNullOrEmpty(text) && textAllowed;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void SetValue(string value)
    {
        Value = value;
        ValueInputBox.Text = value;
    }
}
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace AvalonMaterialEdit.Internal
{
    public static class ClearText
    {
        public static readonly RoutedCommand ClearCommand = new RoutedCommand();

        public static bool GetHandlesClearCommand(DependencyObject obj)
            => (bool)obj.GetValue(HandlesClearCommandProperty);

        public static void SetHandlesClearCommand(DependencyObject obj, bool value)
            => obj.SetValue(HandlesClearCommandProperty, value);

        public static readonly DependencyProperty HandlesClearCommandProperty =
            DependencyProperty.RegisterAttached("HandlesClearCommand", typeof(bool), typeof(ClearText), new PropertyMetadata(false, OnHandlesClearCommandChanged));

        private static void OnHandlesClearCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement )
            {
                var element = (UIElement)d;
                if ((bool)e.NewValue)
                {
                    element.CommandBindings.Add(new CommandBinding(ClearCommand, OnClearCommand));
                }
                else
                {
                    for (int i = element.CommandBindings.Count - 1; i >= 0; i--)
                    {
                        if (element.CommandBindings[i].Command == ClearCommand)
                        {
                            element.CommandBindings.RemoveAt(i);
                        }
                    }
                }
            }

            
        }
        static void OnClearCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var t = e.Source?.GetType();
            if(t == typeof(DatePicker))
            {
                var c = (DatePicker)(sender);
                c.SetCurrentValue(DatePicker.SelectedDateProperty, null);
            }
            else if (t == typeof(TextBox))
            {
                var c = (TextBox)(sender);
                c.SetCurrentValue(TextBox.TextProperty, null);
            }
            else if (t == typeof(ComboBox))
            {
                var c = (ComboBox)(sender);
                c.SetCurrentValue(ComboBox.TextProperty, null);
                c.SetCurrentValue(Selector.SelectedItemProperty, null);

            }
            else if (t == typeof(PasswordBox))
            {
                var c = (PasswordBox)(sender);
                c.Password = null;
            }
            else if (t == typeof(ICSharpCode.AvalonEdit.TextEditor))
            {
                var c = (ICSharpCode.AvalonEdit.TextEditor)(sender);
                c.Text = null;
            }
            
            e.Handled = true;
        }

    }
}
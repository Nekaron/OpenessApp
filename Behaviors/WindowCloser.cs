using System;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace OpenessApp.Behaviors
{
    public class WindowCloser : Behavior<Window>
    {
        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.Register(nameof(DialogResult), typeof(bool?), typeof(WindowCloser),
                new PropertyMetadata(DialogResultChanged));

        public bool? DialogResult
        {
            get => (bool?)GetValue(DialogResultProperty);
            set => SetValue(DialogResultProperty, value);
        }

        private static void DialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
            {
                if (!window.IsLoaded || !window.IsVisible)
                {
                    // Verzögert setzen, sobald das Fenster wirklich sichtbar ist
                    window.Loaded += (s, ev) =>
                    {
                        TrySetDialogResult(window, e.NewValue);
                    };
                }
                else
                {
                    TrySetDialogResult(window, e.NewValue);
                }
            }
        }

        private static void TrySetDialogResult(Window window, object newValue)
        {
            // Muss mit ShowDialog geöffnet worden sein, sonst Exception
            if (newValue is bool result && window != null && window.IsVisible)
            {
                try
                {
                    window.DialogResult = result;
                }
                catch (InvalidOperationException)
                {
                    // Ignorieren, falls Fenster nicht als Dialog angezeigt wurde
                }
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Closed += (s, e) => SetCurrentValue(DialogResultProperty, null);
        }
    }
}

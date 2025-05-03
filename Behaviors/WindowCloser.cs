using System;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace OpenessApp.Behaviors
{
    public class WindowCloser : Behavior<Window>
    {
        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.Register(
                nameof(DialogResult),
                typeof(bool?),
                typeof(WindowCloser),
                new PropertyMetadata(null, OnDialogResultChanged));

        public bool? DialogResult
        {
            get => (bool?)GetValue(DialogResultProperty);
            set => SetValue(DialogResultProperty, value);
        }

        private static void OnDialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window && e.NewValue is bool dialogResult)
            {
                if (window.IsLoaded)
                    window.DialogResult = dialogResult;
                else
                    window.Loaded += (_, __) => window.DialogResult = dialogResult;
            }
        }
    }
}

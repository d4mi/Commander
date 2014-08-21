using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace TotalCommander.Common
{
    public static class FocusBehavior
    {
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached(
                "IsFocused", 
                typeof(bool),
                typeof(FocusBehavior),
                new FrameworkPropertyMetadata(
                    false,
                    new PropertyChangedCallback(onIsFocusedPropertyChanged)));

        public static bool GetIsFocused(DependencyObject element)
        {
            return (bool)element.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(DependencyObject element, bool value)
        {
            element.SetValue(IsFocusedProperty, value);
        }

        private static void onIsFocusedPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            UIElement target = element as UIElement;
            if (target == null)
            {
                return;
            }

            bool isFocused = (bool)args.NewValue;
            if (!isFocused)
            {
                return;
            }

            target.Dispatcher.BeginInvoke(
                (Action)(() =>
                {
                    if (target.Focusable)
                    {
                        target.Focus();

                    }
                }), DispatcherPriority.Input);
        }
    }
}

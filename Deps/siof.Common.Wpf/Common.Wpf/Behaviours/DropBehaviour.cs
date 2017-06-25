using siof.Common.Extensions;
using System.Windows;
using System.Windows.Input;

namespace siof.Common.Wpf.Behaviours
{
    public static class DropBehaviour
    {
        public static readonly DependencyProperty DropCommandProperty = DependencyProperty.RegisterAttached("DropCommand", typeof(ICommand), typeof(DropBehaviour), new PropertyMetadata(DropCommandChanged));

        public static void SetDropCommand(this UIElement element, ICommand command)
        {
            element.SetValue(DropCommandProperty, command);
        }

        private static ICommand GetDropCommand(UIElement element)
        {
            return (ICommand)element.GetValue(DropCommandProperty);
        }

        private static void DropCommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as UIElement).IfNotNull(element =>
            {
                element.Drop -= DropHandler;
                element.Drop += DropHandler;
            });
        }

        private static void DropHandler(object sender, DragEventArgs e)
        {
            (sender as UIElement).IfNotNull(element =>
            {
                ICommand command = GetDropCommand(element);
                command.Execute(e.Data);

                e.Handled = true;
            });
        }
    }
}

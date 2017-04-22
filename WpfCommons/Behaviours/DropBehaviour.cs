using System.Windows;
using System.Windows.Input;

namespace WpfCommons.Behaviours
{
    public static class DropBehavior
    {
        private static readonly DependencyProperty DropCommandProperty = DependencyProperty.RegisterAttached( "DropCommand", typeof(ICommand), typeof(DropBehavior), new PropertyMetadata(DropCommandChanged) );

        public static void SetDropCommand(this UIElement element, ICommand command)
        {
            element.SetValue( DropCommandProperty, command );
        }

        private static ICommand GetDropCommand(UIElement element)
        {
            return (ICommand)element.GetValue(DropCommandProperty);
        }

        private static void DropCommandChanged( DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            UIElement element = obj as UIElement;
            if (element == null)
                return;

            element.Drop += (s, e) =>
            {
                ICommand command = GetDropCommand(element);
                command.Execute(e.Data);

                e.Handled = true;
            };
        }
    }
}

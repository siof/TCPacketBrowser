using System;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;

namespace siof.Common.Wpf
{
    public class RelayCommand<T> : ICommand, IDisposable
    {
        private readonly WeakAction<T> _execute;
        private readonly WeakFunc<T, bool> _canExecute;
        private Dispatcher _dispatcher;

        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _execute = new WeakAction<T>(execute);

            if (canExecute != null)
            {
                _canExecute = new WeakFunc<T, bool>(canExecute);
            }
        }

        public event EventHandler _canExecuteChanged;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    _canExecuteChanged += value;
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (_canExecute != null)
                {
                    _canExecuteChanged -= value;
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            try
            {
                if (_dispatcher == null)
                    _dispatcher = Dispatcher.CurrentDispatcher;

                if (_canExecute == null)
                {
                    return true;
                }

                if (_canExecute.IsStatic || _canExecute.IsAlive)
                {
                    if (parameter == null && typeof(T).IsValueType)
                    {
                        return _canExecute.Execute(default(T));
                    }

                    if (parameter is T)
                    {
                        return (_canExecute.Execute((T)parameter));
                    }

                    return _canExecute.Execute();
                }

                return false;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public virtual void Execute(object parameter)
        {
            try
            {
                if (_dispatcher == null)
                    _dispatcher = Dispatcher.CurrentDispatcher;

                var val = parameter;
                if (parameter != null
                    && parameter.GetType() != typeof(T))
                {
                    if (parameter is IConvertible)
                    {
                        val = Convert.ChangeType(parameter, typeof(T), null);
                    }
                }

                if (_execute != null
                    && (_execute.IsStatic || _execute.IsAlive))
                {
                    if (val == null)
                    {
                        if (typeof(T).IsValueType)
                        {
                            _execute.Execute(default(T));
                        }
                        else
                        {
                            _execute.Execute((T)val);
                        }
                    }
                    else
                    {
                        _execute.Execute((T)val);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void UpdateCanExecuteState()
        {
            EventHandler handler = Interlocked.CompareExchange(ref _canExecuteChanged, null, null);
            if (handler != null)
            {
                if (_dispatcher == null || Thread.CurrentThread == _dispatcher.Thread)
                    handler.Invoke(this, EventArgs.Empty);
                else
                    _dispatcher.Invoke(handler, this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            if (_canExecute != null)
                _canExecute.MarkForDeletion();

            if (_execute != null)
                _execute.MarkForDeletion();

            if (_canExecuteChanged != null)
            {
                var delgates = _canExecuteChanged.GetInvocationList().ToList();
                foreach (var del in delgates)
                {
                    CanExecuteChanged -= (EventHandler)del;
                }
            }
        }
    }
}

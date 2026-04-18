using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace Calculator
{
    public class RelayCommand(Action<object> execute, Predicate<object>? canExecute = null) : ICommand
    {
        private readonly Action<object> _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        private readonly Predicate<object> _canExecute = canExecute ?? (_ => true);

        public bool CanExecute(object? parameter)
        {
            return _canExecute(parameter!);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter!);
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    public class FrontendViewModel : INotifyPropertyChanged
    {
        public string UpperText
        {
            get => field;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(UpperText));
                }
            }
        }

        public string LowerText
        {
            get => field;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(LowerText));
                }
            }
        }

        public ICommand NumberCommand { get; }

        public FrontendViewModel()
        {
            UpperText = string.Empty;
            LowerText = string.Empty;
            NumberCommand = new RelayCommand(NumberButtonClicked, CanClickNumberButton);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NumberButtonClicked(object parameter)
        {
            if (parameter is string number)
            {
                LowerText += number;
            }
        }

        private bool CanClickNumberButton(object parameter)
        {
            return true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}

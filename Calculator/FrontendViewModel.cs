using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ObservableCollection<string> CalculationHistory { get; }

        public ICommand AppendTextCommand { get; }

        public ICommand AppendFunctionCommand { get; }

        public ICommand ClearCommand { get; }

        public ICommand InsertAnswerCommand { get; }

        public ICommand DeleteLastCharCommand { get; }

        public ICommand EqualsCommand { get; }

        public ICommand NumberKeyPressCommand { get; }

        public ICommand ClearCalculationHistoryCommand { get; }

        public FrontendViewModel()
        {
            UpperText = string.Empty;
            LowerText = string.Empty;
            _expression = string.Empty;
            CalculationHistory = new ObservableCollection<string>();
            AppendTextCommand = new RelayCommand(AppendTextButtonClicked);
            AppendFunctionCommand = new RelayCommand(AppendFunctionButtonClicked);
            ClearCommand = new RelayCommand(ClearButtonClicked);
            DeleteLastCharCommand = new RelayCommand(DeleteLastCharacter);
            EqualsCommand = new RelayCommand(EqualsButtonClicked);
            NumberKeyPressCommand = new RelayCommand(NumberKeyPressed);
            ClearCalculationHistoryCommand = new RelayCommand(ClearCalculationHistory);
            InsertAnswerCommand = new RelayCommand(InsertAnswerButtonClicked);
        }

        private string _expression;

        private double _calculationResult;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AppendTextButtonClicked(object parameter)
        {
            if (parameter is string number)
            {
                LowerText += number;
            }
        }

        private void AppendFunctionButtonClicked(object parameter)
        {
            if (parameter is string functionName)
            {
                LowerText += functionName + "(";
            }
        }

        private void ClearButtonClicked(object parameter)
        {
            ClearText();
        }

        private void DeleteLastCharacter(object parameter)
        {
            if (!string.IsNullOrEmpty(LowerText))
            {
                LowerText = LowerText[..^1];
            }
        }

        private void EqualsButtonClicked(object parameter)
        {
            _expression = LowerText;
            Stack<char> parenthesesStack = new();
            for (int i = 0; i < _expression.Length; i++)
            {
                if (_expression[i] == '(')
                {
                    parenthesesStack.Push('(');
                }
                else if (_expression[i] == ')')
                {
                    if (parenthesesStack.Count == 0)
                    {
                        UpperText = "Error";
                        LowerText = "Mismatched parentheses";
                        return;
                    }
                    parenthesesStack.Pop();
                }
            }
            while (parenthesesStack.Count > 0)
            {
                _expression += ')';
                parenthesesStack.Pop();
            }
            bool hasResult = CalculateResult(_expression);
            if (hasResult)
            {
                CalculationHistory.Add($"{UpperText} =\n{LowerText}");
            }
        }

        private void NumberKeyPressed(object parameter)
        {
            if (parameter is not KeyEventArgs e)
            {
                return;
            }

            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                LowerText += (e.Key - Key.D0).ToString();
                return;
            }
            if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                LowerText += (e.Key - Key.NumPad0).ToString();
                return;
            }
        }

        private void InsertAnswerButtonClicked(object parameter)
        {
            LowerText += _calculationResult.ToString();
        }

        private void ClearCalculationHistory(object parameter)
        {
            CalculationHistory.Clear();
        }

        /// <summary>
        /// Calculates the result of the expression using the BackendCalculator and updates the UpperText and LowerText properties accordingly. 
        /// If an error occurs during calculation, it displays an error message in the LowerText property.
        /// </summary>
        /// <param name="expression">The mathematical expression to be evaluated.</param>
        /// <returns>true if no error occurred during calculation; otherwise, false.</returns>
        private bool CalculateResult(string expression)
        {
            try
            {
                BackendCalculator calculator = new(new Tokenizer(expression));
                _calculationResult = calculator.Result;
                UpperText = expression;
                LowerText = _calculationResult.ToString();
                return true;
            }
            catch (DivideByZeroException)
            {
                UpperText = "Error";
                LowerText = "Division by zero is not allowed";
                return false;
            }
            catch (Exception ex)
            {
                UpperText = "Error";
                LowerText = ex.Message;
                return false;
            }
        }

        private void ClearText()
        {
            if (LowerText != string.Empty)
            {
                LowerText = string.Empty;
            }
            else
            {
                UpperText = string.Empty;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}

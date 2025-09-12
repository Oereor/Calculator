using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        private string _expression = "";
        private bool _divisionWithRemainder = false;
        private bool _showFraction = false;

        public MainWindow()
        {
            InitializeComponent();
            // 绑定下拉框事件
            DivisionModeCombo.SelectionChanged += DivisionModeCombo_SelectionChanged;
            ResultModeCombo.SelectionChanged += ResultModeCombo_SelectionChanged;
            // 按钮事件
            foreach (var child in GetAllButtons())
            {
                if (child is Button btn)
                {
                    btn.Click += Button_Click;
                }
            }
            UpdateDisplay();
        }

        private void DivisionModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _divisionWithRemainder = DivisionModeCombo.SelectedIndex == 1;
            UpdateDisplay();
        }
        private void ResultModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _showFraction = ResultModeCombo.SelectedIndex == 1;
            UpdateDisplay();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string content = btn.Content.ToString();
                switch (content)
                {
                    case "DEL":
                        if (_expression.Length > 0)
                            _expression = _expression.Substring(0, _expression.Length - 1);
                        break;
                    case "CLEAR":
                        _expression = "";
                        break;
                    case "=":
                        // nothing, just update
                        break;
                    case "+": case "-": case "×": case "÷":
                        if (_expression.Length > 0 && !HasOperator(_expression))
                            _expression += content;
                        break;
                    default:
                        _expression += content;
                        break;
                }
                UpdateDisplay();
            }
        }
        private void UpdateDisplay()
        {
            ExpressionTextBox.Text = _expression;
            string error;
            var result = Function.Calculate(_expression, _divisionWithRemainder, _showFraction, out error);
            ResultTextBox.Text = error == null ? result : "错误: " + error;
        }
        private bool HasOperator(string expr)
        {
            return expr.Contains("+") || expr.Contains("-") || expr.Contains("×") || expr.Contains("÷") || expr.Contains("*") || expr.Contains("/");
        }
        private UIElement[] GetAllButtons()
        {
            var grid = (Grid)this.FindName("mainGrid");
            if (grid == null) grid = (Grid)this.Content;
            var btns = new System.Collections.Generic.List<UIElement>();
            foreach (UIElement child in grid.Children)
            {
                if (child is Grid g)
                {
                    foreach (UIElement b in g.Children)
                        if (b is Button) btns.Add(b);
                }
            }
            return btns.ToArray();
        }
    }
}

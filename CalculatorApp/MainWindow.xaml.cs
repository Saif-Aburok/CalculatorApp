using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CalculatorApp
{
    public partial class MainWindow : Window
    {
        double firstNumber = 0;
        string currentOperator = "";
        bool isOperatorPressed = false;
        string fullExpression = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            AppendNumber(button.Content.ToString());
        }

        private void AppendNumber(string number)
        {
            if (txtDisplay.Text == "Error") txtDisplay.Text = "";
            txtDisplay.Text += number;
        }

        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ApplyOperator(button.Content.ToString());
        }

        private void ApplyOperator(string op)
        {
            if (!string.IsNullOrEmpty(txtDisplay.Text))
            {
                if (double.TryParse(txtDisplay.Text, out double result))
                {
                    firstNumber = result;
                    currentOperator = op;
                    txtDisplay.Text += " " + currentOperator + " ";
                    isOperatorPressed = true;
                }
            }
        }

        private void Percent_Click(object sender, RoutedEventArgs e)
        {
            ApplyPercent();
        }

        private void ApplyPercent()
        {
            try
            {
                string[] parts = txtDisplay.Text.Split(' ');

                if (parts.Length == 1 && double.TryParse(parts[0], out double singleNumber))
                {
                    double res = singleNumber / 100;
                    txtDisplay.Text = res.ToString();
                }
                else if (parts.Length >= 3 && double.TryParse(parts[2], out double secondNumber))
                {
                    double percentValue = 0;

                    if (currentOperator == "*" || currentOperator == "/")
                    {
                        percentValue = secondNumber / 100;
                    }
                    else if (currentOperator == "+" || currentOperator == "-")
                    {
                        percentValue = firstNumber * (secondNumber / 100);
                    }

                    txtDisplay.Text = parts[0] + " " + currentOperator + " " + percentValue;
                }
            }
            catch
            {
                txtDisplay.Text = "Error";
            }
        }

        private void Equal_Click(object sender, RoutedEventArgs e)
        {
            CalculateResult();
        }

        private void CalculateResult()
        {
            try
            {
                string[] parts = txtDisplay.Text.Split(' ');
                if (parts.Length >= 3)
                {
                    if (double.TryParse(parts[2], out double secondNumber))
                    {
                        double finalResult = 0;
                        fullExpression = txtDisplay.Text;

                        switch (currentOperator)
                        {
                            case "+": finalResult = firstNumber + secondNumber; break;
                            case "-": finalResult = firstNumber - secondNumber; break;
                            case "*": finalResult = firstNumber * secondNumber; break;
                            case "/":
                                if (secondNumber != 0) finalResult = firstNumber / secondNumber;
                                else { txtDisplay.Text = "Error"; return; }
                                break;
                        }

                        txtDisplay.Text = finalResult.ToString();
                        string timeStamp = DateTime.Now.ToString("hh:mm tt");
                        lstHistory.Items.Insert(0, $"{fullExpression} = {finalResult}   [{timeStamp}]");
                    }
                }
            }
            catch
            {
                txtDisplay.Text = "Error";
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            txtDisplay.Text = "";
            firstNumber = 0;
            currentOperator = "";
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            PerformBackspace();
        }

        private void PerformBackspace()
        {
            if (txtDisplay.Text.Length > 0)
            {
                if (txtDisplay.Text.EndsWith(" "))
                {
                    txtDisplay.Text = txtDisplay.Text.Substring(0, txtDisplay.Text.Length - 3);
                    currentOperator = "";
                }
                else
                {
                    txtDisplay.Text = txtDisplay.Text.Substring(0, txtDisplay.Text.Length - 1);
                }
            }
        }

        private void ClearHistory_Click(object sender, RoutedEventArgs e)
        {
            lstHistory.Items.Clear();
            MessageBox.Show("تم تنظيف سجل العمليات بنجاح!", "السجل", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void History_Toggle_Click(object sender, RoutedEventArgs e)
        {
            HistoryGrid.Visibility = HistoryGrid.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9))
                AppendNumber((e.Key - Key.D0).ToString());
            else if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad0 + 9))
                AppendNumber((e.Key - Key.NumPad0).ToString());
            else if (e.Key == Key.Add || (e.Key == Key.OemPlus && Keyboard.Modifiers == ModifierKeys.Shift)) ApplyOperator("+");
            else if (e.Key == Key.Subtract || e.Key == Key.OemMinus) ApplyOperator("-");
            else if (e.Key == Key.Multiply) ApplyOperator("*");
            else if (e.Key == Key.Divide || e.Key == Key.OemQuestion) ApplyOperator("/");
            else if (e.Key == Key.Enter || e.Key == Key.OemPlus) CalculateResult();
            else if (e.Key == Key.Back) PerformBackspace();
            else if (e.Key == Key.Escape) Clear_Click(null, null);
        }
    }
}

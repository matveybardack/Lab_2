using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using ClassLibraryWPCalculator.Services;
using ClassLibraryWPCalculator;

namespace WpfAppWPCalculator
{
    public partial class MainWindow : Window
    {
        private readonly WPCalculatorService _service = new WPCalculatorService();
        private readonly ExpressionParser _expressionParser = new ExpressionParser();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Operation_Checked(object sender, RoutedEventArgs e)
        {
            UpdateConditions();
        }

        private void Postcondition_Checked(object sender, RoutedEventArgs e)
        {
            UpdateConditions();
        }

        private void UpdateConditions()
        {
            string selectedOperation = GetSelectedOperation();
            string selectedPostcondition = GetSelectedPostType();

            // Заполняем поля в зависимости от выбора
            if (!string.IsNullOrEmpty(selectedOperation))
            {
                switch (selectedOperation)
                {
                    case "Scenario1":
                        tbOldPrecondition.Text = "a > 0";
                        tbOldPostcondition.Text = "a*a>0";
                        break;
                    case "Scenario2":
                        tbOldPrecondition.Text = "r > 0";
                        tbOldPostcondition.Text = "3.14*r*r>0";
                        break;
                    case "Scenario3":
                        tbOldPrecondition.Text = "a > 0 && b > 0";
                        tbOldPostcondition.Text = "a*a + b*b > 0";
                        break;
                }
            }

            if (!string.IsNullOrEmpty(selectedPostcondition))
            {
                // Заполняем tbStatementChange примерами операторов
                switch (selectedPostcondition)
                {
                    case "Assignment":
                        switch (selectedOperation)
                        {
                            case "Scenario1":
                                tbStatementChange.Text = "a := a * a";  // меняем переменную a
                                break;
                            case "Scenario2":
                                tbStatementChange.Text = "r := 3.14 * r * r";  // меняем переменную r
                                break;
                            case "Scenario3":
                                tbOldPrecondition.Text = "r > 0";
                                tbOldPostcondition.Text = "2 * 3.14 * r > 0";
                                break;
                        }
                        break;

                    case "Branching":
                        switch (selectedOperation)
                        {
                            case "Scenario1":
                                tbStatementChange.Text = "if (a>0) then a:=a else a := a";
                                break;
                            case "Scenario2":
                                tbStatementChange.Text = "if (r > 0) then r := r  else r := r";
                                break;
                            case "Scenario3":
                                tbStatementChange.Text = "if (r > 0) then r:= r else r:=r";
                                break;
                        }
                        break;

                    case "Sequence":
                        switch (selectedOperation)
                        {
                            case "Scenario1":
                                tbStatementChange.Text = "temp := a; a := temp * temp";
                                break;
                            case "Scenario2":
                                tbStatementChange.Text = "temp := r; r := temp * temp";
                                break;
                            case "Scenario3":
                                tbStatementChange.Text = "temp1 := a; temp2 := b; a := temp1 * temp1 + temp2 * temp2";
                                break;
                        }
                        break;
                }
            }

            btnCalculate.IsEnabled = !string.IsNullOrEmpty(selectedOperation) && !string.IsNullOrEmpty(selectedPostcondition);
        }
        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            ClearValidationVisuals();
            lbTrace.Items.Clear();

            try
            {
                string selectedOperation = GetSelectedOperation();
                string selectedPostcondition = GetSelectedPostType();

                if (string.IsNullOrEmpty(selectedOperation) || string.IsNullOrEmpty(selectedPostcondition))
                    throw new ArgumentException("Выберите сценарий и тип постусловия.");

                // Используем старое постусловие как Q
                string userPost = tbOldPostcondition.Text?.Trim();
                if (string.IsNullOrWhiteSpace(userPost))
                    throw new ArgumentException("Старое постусловие не может быть пустым.");

                if (!ContainsInequalityOperator(userPost))
                    throw new ArgumentException("Постусловие должно содержать знак сравнения: >, <, >= или <=.");

                // Используем пользовательский ввод из tbStatementChange
                string statementInput = tbStatementChange.Text?.Trim();
                if (string.IsNullOrWhiteSpace(statementInput))
                    throw new ArgumentException("Поле 'Изменение постусловия' не может быть пустым.");

                string wpResult;

                if (selectedPostcondition == "Assignment")
                {
                    if (!_expressionParser.TryParseAssignment(statementInput))
                        throw new ArgumentException("Некорректный оператор присваивания.");

                    wpResult = _service.CalculateForAssignment(statementInput, userPost);
                }
                else if (selectedPostcondition == "Branching")
                {
                    if (!_expressionParser.TryParseIf(statementInput))
                        throw new ArgumentException("Некорректный оператор ветвления.");

                    wpResult = _service.CalculateForIf(statementInput, userPost);
                }
                else // Sequence
                {
                    var parts = statementInput.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                              .Select(p => p.Trim())
                                              .Where(p => !string.IsNullOrWhiteSpace(p))
                                              .ToArray();

                    if (parts.Length == 0)
                        throw new ArgumentException("Последовательность пуста.");

                    foreach (var p in parts)
                        if (!_expressionParser.TryParseAssignment(p))
                            throw new ArgumentException($"Некорректный оператор присваивания в последовательности: {p}");

                    var stack = new Stack<string>();
                    foreach (var p in parts.Reverse())
                        stack.Push(p);

                    wpResult = _service.CalculateForSequence(stack, userPost);
                }

                // Отобразить результат
                tbWpResult.Text = $"wp(S, Q) = {wpResult}";

                // Добавить шаги в трейс
                lbTrace.Items.Add($"1. Выбрана операция: {selectedOperation}");
                lbTrace.Items.Add($"2. Выбран тип: {selectedPostcondition}");
                lbTrace.Items.Add($"3. Старое предусловие: {tbOldPrecondition.Text}");
                lbTrace.Items.Add($"4. Старое постусловие (Q): {userPost}");
                lbTrace.Items.Add($"5. Оператор(ы): {statementInput}");
                lbTrace.Items.Add($"6. Рассчитано wp(S, Q) = {wpResult}");
                lbTrace.Items.Add("---- шаги движка ----");

                // Добавить все строки из глобального трейса
                foreach (var msg in WpTrace.GetAll())
                    lbTrace.Items.Add(msg);

                if (lbTrace.Items.Count > 0)
                    lbTrace.ScrollIntoView(lbTrace.Items[lbTrace.Items.Count - 1]);

                btnHoareTriad.IsEnabled = true;
            }
            catch (ArgumentException aex)
            {
                ShowValidationError(aex.Message);
            }
            catch (InvalidOperationException ioex)
            {
                MessageBox.Show("Ошибка вычисления: " + ioex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Неожиданная ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            // Очистка всех полей
            tbOldPrecondition.Text = "";
            tbOldPostcondition.Text = "";
            tbStatementChange.Text = "";
            tbWpResult.Text = "";
            tbHoareTriad.Text = "";

            // Очистка пошагового трейса
            lbTrace.Items.Clear();
            WpTrace.Clear();

            // Сброс RadioButton'ов
            rbScenario1.IsChecked = false;
            rbScenario2.IsChecked = false;
            rbScenario3.IsChecked = false;
            rbAssignment.IsChecked = false;
            rbBranching.IsChecked = false;
            rbSequence.IsChecked = false;

            // Деактивация кнопки триады Хоара
            btnHoareTriad.IsEnabled = false;

            ClearValidationVisuals();
        }

        private void BtnHoareTriad_Click(object sender, RoutedEventArgs e)
        {
            string precondition = string.IsNullOrEmpty(tbWpResult.Text) ? "P" : tbWpResult.Text.Replace("wp(S, Q) = ", "");
            string postcondition = string.IsNullOrEmpty(tbOldPostcondition.Text) ? "Q" : tbOldPostcondition.Text;

            string hoareTriad = $"{{ {precondition} }}\nS\n{{ {postcondition} }}";

            tbHoareTriad.Text = hoareTriad;

            lbTrace.Items.Add($"7. Сформирована триада Хоара:");
            lbTrace.Items.Add($"   {{ {precondition} }}");
            lbTrace.Items.Add($"   S");
            lbTrace.Items.Add($"   {{ {postcondition} }}");

            if (lbTrace.Items.Count > 0)
                lbTrace.ScrollIntoView(lbTrace.Items[lbTrace.Items.Count - 1]);
        }

        // -------------------- вспомогательные методы --------------------

        private string GetSelectedOperation()
        {
            if (rbScenario1.IsChecked == true) return "Scenario1";
            if (rbScenario2.IsChecked == true) return "Scenario2";
            if (rbScenario3.IsChecked == true) return "Scenario3";
            return string.Empty;
        }

        private string GetSelectedPostType()
        {
            if (rbAssignment.IsChecked == true) return "Assignment";
            if (rbBranching.IsChecked == true) return "Branching";
            if (rbSequence.IsChecked == true) return "Sequence";
            return string.Empty;
        }

        private bool ContainsInequalityOperator(string s)
        {
            return Regex.IsMatch(s ?? string.Empty, @"(<=|>=|<|>)");
        }

        private void ShowValidationError(string message)
        {
            tbStatementChange.BorderBrush = Brushes.Red;
            MessageBox.Show(message, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void ClearValidationVisuals()
        {
            tbStatementChange.ClearValue(System.Windows.Controls.Control.BorderBrushProperty);
        }
    }
}
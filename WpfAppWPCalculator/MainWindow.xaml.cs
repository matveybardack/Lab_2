using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using ClassLibraryWPCalculator;

namespace WpfAppWPCalculator
{
    public partial class MainWindow : Window
    {
        // Парсер и движок (конкретные реализации из библиотеки)
        private readonly ExpressionParser _parser = new ExpressionParser();
        private readonly WPCalculator _wpEngine = new WPCalculator();

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
            string selectedOperation = "";
            string selectedPostcondition = "";

            // Определяем выбранную операцию
            if (rbScenario1.IsChecked == true) selectedOperation = "Scenario1";
            else if (rbScenario2.IsChecked == true) selectedOperation = "Scenario2";
            else if (rbScenario3.IsChecked == true) selectedOperation = "Scenario3";

            // Определяем выбранный способ редактирования
            if (rbAssignment.IsChecked == true) selectedPostcondition = "Assignment";
            else if (rbBranching.IsChecked == true) selectedPostcondition = "Branching";
            else if (rbSequence.IsChecked == true) selectedPostcondition = "Sequence";

            // Заполняем поля в зависимости от выбора (как у тебя было)
            if (!string.IsNullOrEmpty(selectedOperation))
            {
                switch (selectedOperation)
                {
                    case "Scenario1":
                        tbOldPrecondition.Text = "a > 0";
                        tbOldPostcondition.Text = "S = a^2";
                        tbNewPrecondition.Text = "a > 0";
                        break;
                    case "Scenario2":
                        tbOldPrecondition.Text = "r > 0";
                        tbOldPostcondition.Text = "S = π * r^2";
                        tbNewPrecondition.Text = "r > 0";
                        break;
                    case "Scenario3":
                        tbOldPrecondition.Text = "a > 0, b > 0";
                        tbOldPostcondition.Text = "c = a * b";
                        tbNewPrecondition.Text = "a > 0, b > 0";
                        break;
                }
            }

            if (!string.IsNullOrEmpty(selectedPostcondition))
            {
                switch (selectedPostcondition)
                {
                    case "Assignment":
                        switch (selectedOperation)
                        {
                            case "Scenario1":
                                tbNewPostcondition.Text = "s := a * a";
                                break;
                            case "Scenario2":
                                tbNewPostcondition.Text = "s := 3.14 * r * r";
                                break;
                            case "Scenario3":
                                tbNewPostcondition.Text = "c := a * b";
                                break;
                        }
                        break;
                    case "Branching":
                        switch (selectedOperation)
                        {
                            case "Scenario1":
                                tbNewPostcondition.Text = "if (a > 0) then s := a * a else s := 0";
                                break;
                            case "Scenario2":
                                tbNewPostcondition.Text = "if (r > 0) then s := 3.14 * r * r else s := 0";
                                break;
                            case "Scenario3":
                                tbNewPostcondition.Text = "if (a > 0 && b > 0) then c := a * b else c := 0";
                                break;
                        }
                        break;
                    case "Sequence":
                        switch (selectedOperation)
                        {
                            case "Scenario1":
                                tbNewPostcondition.Text = "a := a; s := a * a";
                                break;
                            case "Scenario2":
                                tbNewPostcondition.Text = "r := r; s := 3.14 * r * r";
                                break;
                            case "Scenario3":
                                tbNewPostcondition.Text = "a := a; b := b; c := a * b";
                                break;
                        }
                        break;
                }
            }

            // Активируем кнопку расчета если выбраны оба параметра
            btnCalculate.IsEnabled = !string.IsNullOrEmpty(selectedOperation) && !string.IsNullOrEmpty(selectedPostcondition);
        }

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            // Сбрасываем визуальные ошибки
            ClearValidationVisuals();

            try
            {
                // 1) Очистить предыдущий трейс
                WpTrace.Clear();
                lbTrace.Items.Clear();

                // 2) Собрать входные данные
                string selectedOperation = GetSelectedOperation(); // "Scenario1" / "Scenario2" / "Scenario3"
                string selectedPostType = GetSelectedPostType();   // "Assignment" / "Branching" / "Sequence"
                string userPostcondition = tbNewPostcondition.Text?.Trim();

                // 3) Валидация: постусловие не пустое и содержит знак сравнения
                if (string.IsNullOrWhiteSpace(userPostcondition))
                    throw new ArgumentException("Постусловие не может быть пустым.");

                if (!ContainsInequalityOperator(userPostcondition))
                    throw new ArgumentException("Постусловие должно содержать знак сравнения: >, <, >= или <=.");

                // 4) Построить оператор S(или последовательность) в зависимости от сценария и типа
                //    (в реальном приложении можно давать пользователю возможность редактировать S напрямую,
                //     но здесь используем заранее определённые шаблоны по сценарию)
                var statements = BuildStatementsForScenario(selectedOperation, selectedPostType);

                // 5) Валидировать синтаксис операторов с помощью ExpressionParser
                if (selectedPostType == "Assignment")
                {
                    if (!_parser.TryParseAssignment(statements[0]))
                        throw new ArgumentException("Неверный синтаксис присваивания: " + statements[0]);
                }
                else if (selectedPostType == "Branching")
                {
                    if (!_parser.TryParseIf(statements[0]))
                        throw new ArgumentException("Неверный синтаксис if-оператора: " + statements[0]);
                }
                else // Sequence
                {
                    foreach (var st in statements)
                    {
                        if (!_parser.TryParseAssignment(st))
                            throw new ArgumentException("Неверный синтаксис в последовательности: " + st);
                    }
                }

                // 6) Вызвать нужный метод движка
                string wpResult = "";
                if (selectedPostType == "Assignment")
                {
                    wpResult = _wpEngine.CalculateForAssignment(statements[0], userPostcondition);
                }
                else if (selectedPostType == "Branching")
                {
                    wpResult = _wpEngine.CalculateForIf(statements[0], userPostcondition);
                }
                else // Sequence
                {
                    var stack = new Stack<string>();
                    // push в порядке выполнения, чтобы Pop доставал последний оператор (как ожидает движок)
                    foreach (var st in statements)
                        stack.Push(st);

                    wpResult = _wpEngine.CalculateForSequence(stack, userPostcondition);
                }

                // 7) Отобразить результат и трейс
                tbNewPrecondition.Text = wpResult;
                tbWpResult.Text = $"wp(S, Q) = {wpResult}";

                // Показать все шаги траса в lbTrace (и скроллить вниз)
                foreach (var msg in WpTrace.GetAll())
                    lbTrace.Items.Add(msg);

                if (lbTrace.Items.Count > 0)
                    lbTrace.ScrollIntoView(lbTrace.Items[lbTrace.Items.Count - 1]);

                // Активируем триаду Хоара (как у тебя)
                btnHoareTriad.IsEnabled = true;
            }
            catch (ArgumentException argEx)
            {
                ShowValidationError(argEx.Message);
            }
            catch (InvalidOperationException invEx)
            {
                ShowValidationError("Ошибка упрощения/вычисления: " + invEx.Message);
            }
            catch (Exception ex)
            {
                // Общая обработка неожиданных ошибок
                MessageBox.Show("Произошла ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            // Очистка всех полей
            tbOldPrecondition.Text = "";
            tbNewPrecondition.Text = "";
            tbOldPostcondition.Text = "";
            tbNewPostcondition.Text = "";
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
            // Формирование триады Хоара
            string precondition = string.IsNullOrEmpty(tbNewPrecondition.Text) ? "P" : tbNewPrecondition.Text;
            string postcondition = string.IsNullOrEmpty(tbNewPostcondition.Text) ? "Q" : tbNewPostcondition.Text;

            string hoareTriad = $"{{ {precondition} }}\nS\n{{ {postcondition} }}";

            // Отображение триады Хоара
            tbHoareTriad.Text = hoareTriad;

            // Добавление в пошаговый трейс
            lbTrace.Items.Add($"7. Сформирована триада Хоара:");
            lbTrace.Items.Add($"   {{ {precondition} }}");
            lbTrace.Items.Add($"   S");
            lbTrace.Items.Add($"   {{ {postcondition} }}");

            // Прокручиваем к последнему элементу
            if (lbTrace.Items.Count > 0)
                lbTrace.ScrollIntoView(lbTrace.Items[lbTrace.Items.Count - 1]);
        }

        /// <summary>
        /// Возвращает выбранный сценарий в виде ключа
        /// </summary>
        private string GetSelectedOperation()
        {
            if (rbScenario1.IsChecked == true) return "Scenario1";
            if (rbScenario2.IsChecked == true) return "Scenario2";
            if (rbScenario3.IsChecked == true) return "Scenario3";
            return string.Empty;
        }

        /// <summary>
        /// Возвращает выбранный тип постусловия (Assignment / Branching / Sequence)
        /// </summary>
        private string GetSelectedPostType()
        {
            if (rbAssignment.IsChecked == true) return "Assignment";
            if (rbBranching.IsChecked == true) return "Branching";
            if (rbSequence.IsChecked == true) return "Sequence";
            return string.Empty;
        }

        /// <summary>
        /// Проверяет наличие знака сравнения в постусловии
        /// </summary>
        private bool ContainsInequalityOperator(string s)
        {
            return Regex.IsMatch(s, @"(<=|>=|<|>)");
        }

        /// <summary>
        /// Формирует набор операторов (1 элемент для Assignment/If, несколько для Sequence)
        /// по выбранному сценарию и типу.
        /// </summary>
        private string[] BuildStatementsForScenario(string scenarioKey, string postType)
        {
            // Возвращаем массив строк — для Assignment/Branching длина 1, для Sequence — несколько.
            switch (postType)
            {
                case "Assignment":
                    switch (scenarioKey)
                    {
                        case "Scenario1": return new[] { "s := a * a" };
                        case "Scenario2": return new[] { "s := 3.14 * r * r" };
                        case "Scenario3": return new[] { "c := a * b" };
                    }
                    break;

                case "Branching":
                    switch (scenarioKey)
                    {
                        case "Scenario1": return new[] { "if (a > 0) then s := a * a else s := 0" };
                        case "Scenario2": return new[] { "if (r > 0) then s := 3.14 * r * r else s := 0" };
                        case "Scenario3": return new[] { "if (a > 0 && b > 0) then c := a * b else c := 0" };
                    }
                    break;

                case "Sequence":
                    switch (scenarioKey)
                    {
                        case "Scenario1": return new[] { "a := a", "s := a * a" };
                        case "Scenario2": return new[] { "r := r", "s := 3.14 * r * r" };
                        case "Scenario3": return new[] { "a := a", "b := b", "c := a * b" };
                    }
                    break;
            }

            // По умолчанию пустой набор — сигнализируем ошибкой на вызове метода-валидаторе
            return new string[0];
        }

        /// <summary>
        /// Подсветить поле постусловия и показать сообщение об ошибке.
        /// </summary>
        private void ShowValidationError(string message)
        {
            // Подсветить поле постусловия
            tbNewPostcondition.BorderBrush = Brushes.Red;
            MessageBox.Show(message, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Снять визуальную подсветку ошибок.
        /// </summary>
        private void ClearValidationVisuals()
        {
            tbNewPostcondition.ClearValue(System.Windows.Controls.Control.BorderBrushProperty);
        }
    }
}

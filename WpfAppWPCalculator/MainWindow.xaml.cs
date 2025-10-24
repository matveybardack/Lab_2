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
        // Сервис для вычисления слабейшего предусловия, обертка над движком WPCalculator
        private readonly WPCalculatorService _service = new WPCalculatorService();

        // Парсер выражений, проверяет корректность присваиваний и ветвлений
        private readonly ExpressionParser _expressionParser = new ExpressionParser();

        public MainWindow()
        {
            InitializeComponent(); // Инициализация XAML интерфейса
        }

        // Обработчик выбора сценария
        private void Operation_Checked(object sender, RoutedEventArgs e)
        {
            UpdateConditions(); // Обновляем отображение старых и новых условий
        }

        // Обработчик выбора типа редактирования постусловия (Assignment, Branching, Sequence)
        private void Postcondition_Checked(object sender, RoutedEventArgs e)
        {
            UpdateConditions(); // Обновляем отображение
        }

        // Метод обновления условий в интерфейсе в зависимости от выбранного сценария и типа постусловия
        private void UpdateConditions()
        {
            string selectedOperation = GetSelectedOperation(); // Получаем выбранный сценарий
            string selectedPostcondition = GetSelectedPostType(); // Получаем выбранный тип постусловия

            // Заполняем старые предусловия и постусловия в зависимости от сценария
            if (!string.IsNullOrEmpty(selectedOperation))
            {
                switch (selectedOperation)
                {
                    case "Scenario1":
                        tbOldPrecondition.Text = "r > 0; h = 2";       // Старое предусловие
                        tbOldPostcondition.Text = "2*3.14*2*r>0";      // Старое постусловие
                        break;
                    case "Scenario2":
                        tbOldPrecondition.Text = "r > 0; h = 2";
                        tbOldPostcondition.Text = "4*r>0";
                        break;
                    case "Scenario3":
                        tbOldPrecondition.Text = "r > 0";
                        tbOldPostcondition.Text = "2 * 3.14 * r > 0";
                        break;
                }
            }

            // Автозаполнение примера оператора в поле tbStatementChange
            if (!string.IsNullOrEmpty(selectedPostcondition))
            {
                switch (selectedPostcondition)
                {
                    case "Assignment": // Присвоение
                        switch (selectedOperation)
                        {
                            case "Scenario1":
                                tbStatementChange.Text = "r := r ";  // Пример присвоения для квадрата
                                break;
                            case "Scenario2":
                                tbStatementChange.Text = "r := r";  // Пример для круга
                                break;
                            case "Scenario3":
                                tbStatementChange.Text = "r := r";  // Пример для гипотенузы
                                break;
                        }
                        break;

                    case "Branching": // Ветвление (if)
                        switch (selectedOperation)
                        {
                            case "Scenario1":
                                tbStatementChange.Text = "if (r > 0) then r := a else r := b";
                                break;
                            case "Scenario2":
                                tbStatementChange.Text = "if (r > 0) then r := r  else r := r";
                                break;
                            case "Scenario3":
                                tbStatementChange.Text = "if (r > 0) then r:= r else r:=r";
                                break;
                        }
                        break;

                    case "Sequence": // Последовательность присваиваний
                        switch (selectedOperation)
                        {
                            case "Scenario1":
                                tbStatementChange.Text = "r:=r; r:=r";
                                break;
                            case "Scenario2":
                                tbStatementChange.Text = "r:=r; r:=r";
                                break;
                            case "Scenario3":
                                tbStatementChange.Text = "r:=r; r:=r";
                                break;
                        }
                        break;
                }
            }

            // Кнопка "Рассчитать" активна только если выбран сценарий и тип постусловия
            btnCalculate.IsEnabled = !string.IsNullOrEmpty(selectedOperation) && !string.IsNullOrEmpty(selectedPostcondition);
        }

        // Обработчик кнопки "Рассчитать wp"
        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            ClearValidationVisuals(); // Убираем визуальные подсказки ошибок
            lbTrace.Items.Clear();    // Очищаем список шагов

            try
            {
                string selectedOperation = GetSelectedOperation();
                string selectedPostcondition = GetSelectedPostType();

                if (string.IsNullOrEmpty(selectedOperation) || string.IsNullOrEmpty(selectedPostcondition))
                    throw new ArgumentException("Выберите сценарий и тип постусловия.");

                // Берём старое постусловие как Q
                string userPost = tbOldPostcondition.Text?.Trim();
                if (string.IsNullOrWhiteSpace(userPost))
                    throw new ArgumentException("Старое постусловие не может быть пустым.");

                if (!ContainsInequalityOperator(userPost))
                    throw new ArgumentException("Постусловие должно содержать знак сравнения: >, <, >= или <=.");

                // Берём оператор/изменение постусловия от пользователя
                string statementInput = tbStatementChange.Text?.Trim();
                if (string.IsNullOrWhiteSpace(statementInput))
                    throw new ArgumentException("Поле 'Изменение постусловия' не может быть пустым.");

                string wpResult;

                // Разные ветви вычисления в зависимости от типа редактирования
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

                    // Проверка корректности каждой строки
                    foreach (var p in parts)
                        if (!_expressionParser.TryParseAssignment(p))
                            throw new ArgumentException($"Некорректный оператор присваивания в последовательности: {p}");

                    // Переводим в стек (top = последний элемент)
                    var stack = new Stack<string>();
                    foreach (var p in parts.Reverse())
                        stack.Push(p);

                    wpResult = _service.CalculateForSequence(stack, userPost);
                }

                // Выводим результат в интерфейс
                tbWpResult.Text = $"wp(S, Q) = {wpResult}";

                // Добавляем пошаговый трейс
                lbTrace.Items.Add($"1. Выбрана операция: {selectedOperation}");
                lbTrace.Items.Add($"2. Выбран тип: {selectedPostcondition}");
                lbTrace.Items.Add($"3. Старое предусловие: {tbOldPrecondition.Text}");
                lbTrace.Items.Add($"4. Старое постусловие (Q): {userPost}");
                lbTrace.Items.Add($"5. Оператор(ы): {statementInput}");
                lbTrace.Items.Add($"6. Рассчитано wp(S, Q) = {wpResult}");
                lbTrace.Items.Add("---- шаги движка ----");

                // Добавляем все строки из глобального WpTrace
                foreach (var msg in WpTrace.GetAll())
                    lbTrace.Items.Add(msg);

                if (lbTrace.Items.Count > 0)
                    lbTrace.ScrollIntoView(lbTrace.Items[lbTrace.Items.Count - 1]);

                btnHoareTriad.IsEnabled = true; // Активируем кнопку формирования триады Хоара
            }
            catch (ArgumentException aex)
            {
                ShowValidationError(aex.Message); // Валидация ввода
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

        // Очистка всех полей интерфейса
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            tbOldPrecondition.Text = "";
            tbOldPostcondition.Text = "";
            tbStatementChange.Text = "";
            tbWpResult.Text = "";
            tbHoareTriad.Text = "";

            lbTrace.Items.Clear();
            WpTrace.Clear();

            rbScenario1.IsChecked = false;
            rbScenario2.IsChecked = false;
            rbScenario3.IsChecked = false;
            rbAssignment.IsChecked = false;
            rbBranching.IsChecked = false;
            rbSequence.IsChecked = false;

            btnHoareTriad.IsEnabled = false;

            ClearValidationVisuals();
        }

        // Формирование триады Хоара на основе wp и старого постусловия
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

        // Определяем выбранный сценарий
        private string GetSelectedOperation()
        {
            if (rbScenario1.IsChecked == true) return "Scenario1";
            if (rbScenario2.IsChecked == true) return "Scenario2";
            if (rbScenario3.IsChecked == true) return "Scenario3";
            return string.Empty;
        }

        // Определяем выбранный тип постусловия
        private string GetSelectedPostType()
        {
            if (rbAssignment.IsChecked == true) return "Assignment";
            if (rbBranching.IsChecked == true) return "Branching";
            if (rbSequence.IsChecked == true) return "Sequence";
            return string.Empty;
        }

        // Проверка, содержит ли постусловие оператор сравнения
        private bool ContainsInequalityOperator(string s)
        {
            return Regex.IsMatch(s ?? string.Empty, @"(<=|>=|<|>)");
        }

        // Отображение ошибки валидации (красная рамка + сообщение)
        private void ShowValidationError(string message)
        {
            tbStatementChange.BorderBrush = Brushes.Red;
            MessageBox.Show(message, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        // Очистка визуальных подсказок ошибок
        private void ClearValidationVisuals()
        {
            tbStatementChange.ClearValue(System.Windows.Controls.Control.BorderBrushProperty);
        }
    }
}

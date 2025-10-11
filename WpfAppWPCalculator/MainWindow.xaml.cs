using System.Windows;

namespace WpfAppWPCalculator
{
    public partial class MainWindow : Window
    {
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

            // Заполняем поля в зависимости от выбора
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
                                tbNewPostcondition.Text = "a:=";
                                break;
                            case "Scenario2":
                                tbNewPostcondition.Text = "r:=";
                                break;
                            case "Scenario3":
                                tbNewPostcondition.Text = "a:= ; b:=";
                                break;
                        }
                        break;
                    case "Branching":
                        tbNewPostcondition.Text = $"if ( ) then {tbOldPostcondition.Text} else {tbOldPostcondition.Text}";
                        break;
                    case "Sequence":
                        switch (selectedOperation)
                        {
                            case "Scenario1":
                                tbNewPostcondition.Text = "(a:=; )";
                                break;
                            case "Scenario2":
                                tbNewPostcondition.Text = "(r:=; )";
                                break;
                            case "Scenario3":
                                tbNewPostcondition.Text = "a:= ; b:=";
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

            // Обновляем результат wp
            if (!string.IsNullOrEmpty(selectedOperation) && !string.IsNullOrEmpty(selectedPostcondition))
            {
                tbWpResult.Text = $"wp(S, Q) = {tbNewPrecondition.Text}";

                // Добавляем записи в пошаговый трейс
                lbTrace.Items.Add($"1. Выбрана операция: {selectedOperation}");
                lbTrace.Items.Add($"2. Выбран тип: {selectedPostcondition}");
                lbTrace.Items.Add($"3. Старое предусловие: {tbOldPrecondition.Text}");
                lbTrace.Items.Add($"4. Старое постусловие: {tbOldPostcondition.Text}");
                lbTrace.Items.Add($"5. Рассчитано wp(S, Q) = {tbNewPrecondition.Text}");
                lbTrace.Items.Add($"6. Новое постусловие: {tbNewPostcondition.Text}");

                // Прокручиваем к последнему элементу
                if (lbTrace.Items.Count > 0)
                    lbTrace.ScrollIntoView(lbTrace.Items[lbTrace.Items.Count - 1]);

                // Активируем кнопку триады Хоара
                btnHoareTriad.IsEnabled = true;
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

            // Сброс RadioButton'ов
            rbScenario1.IsChecked = false;
            rbScenario2.IsChecked = false;
            rbScenario3.IsChecked = false;
            rbAssignment.IsChecked = false;
            rbBranching.IsChecked = false;
            rbSequence.IsChecked = false;

            // Деактивация кнопки триады Хоара
            btnHoareTriad.IsEnabled = false;
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
    }
}
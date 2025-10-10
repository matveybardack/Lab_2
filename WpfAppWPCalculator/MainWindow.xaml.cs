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
            CheckSelections();
            UpdateSelectedOperator();
        }

        private void Postcondition_Checked(object sender, RoutedEventArgs e)
        {
            CheckSelections();
            UpdateSelectedOperator();
        }

        private void CheckSelections()
        {
            // Проверяем, выбрана ли операция и способ редактирования
            bool isOperationSelected = rbScenario1.IsChecked == true || rbScenario2.IsChecked == true;
            bool isPostconditionSelected = rbAssignment.IsChecked == true || rbBranching.IsChecked == true || rbSequence.IsChecked == true;

            // Активируем кнопку расчета только если оба выбора сделаны
            if (isOperationSelected && isPostconditionSelected)
            {
                btnCalculate.IsEnabled = true;
            }
            else
            {
                btnCalculate.IsEnabled = false;
            }
        }

        private void UpdateSelectedOperator()
        {
            string operation = "";
            string postcondition = "";

            // Определяем выбранную операцию
            if (rbScenario1.IsChecked == true) operation = "Операция 1";
            else if (rbScenario2.IsChecked == true) operation = "Операция 2";

            // Определяем выбранный способ редактирования
            if (rbAssignment.IsChecked == true) postcondition = "Присвоение";
            else if (rbBranching.IsChecked == true) postcondition = "Ветвление";
            else if (rbSequence.IsChecked == true) postcondition = "Последовательность";

            // Обновляем отображение выбранного оператора
            if (!string.IsNullOrEmpty(operation) && !string.IsNullOrEmpty(postcondition))
            {
                //tbSelectedOperator.Text = $"{operation} - {postcondition}";
            }
            else
            {
                //tbSelectedOperator.Text = "";
            }
        }

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            // Логика расчета слабейшего предусловия
            string selectedOperation = "";
            string selectedPostcondition = "";

            // Определяем выбранную операцию
            if (rbScenario1.IsChecked == true) selectedOperation = "Operation1";
            else if (rbScenario2.IsChecked == true) selectedOperation = "Operation2";

            // Определяем выбранный способ редактирования
            if (rbAssignment.IsChecked == true) selectedPostcondition = "Assignment";
            else if (rbBranching.IsChecked == true) selectedPostcondition = "Branching";
            else if (rbSequence.IsChecked == true) selectedPostcondition = "Sequence";

            // Заполняем поля в зависимости от выбора
            tbOldPrecondition.Text = $"P (для {selectedOperation})";
            tbOldPostcondition.Text = $"Q (для {selectedPostcondition})";
            tbNewPrecondition.Text = $"wp(S, Q)";
            tbNewPostcondition.Text = $"Новое постусловие для {selectedOperation} - {selectedPostcondition}";

            // Обновляем результат wp
            tbWpResult.Text = $"wp(S, Q) для {selectedOperation} - {selectedPostcondition}";

            // Добавляем записи в пошаговый трейс
            lbTrace.Items.Add($"1. Выбрана операция: {selectedOperation}");
            lbTrace.Items.Add($"2. Выбран тип: {selectedPostcondition}");
            lbTrace.Items.Add($"3. Рассчитано wp(S, Q)");
            lbTrace.Items.Add($"4. Старое предусловие: P");
            lbTrace.Items.Add($"5. Новое предусловие: wp(S, Q)");

            // Прокручиваем к последнему элементу
            if (lbTrace.Items.Count > 0)
                lbTrace.ScrollIntoView(lbTrace.Items[lbTrace.Items.Count - 1]);

            // Активируем кнопку триады Хоара
            btnHoareTriad.IsEnabled = true;
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            // Очистка всех полей и сброс выбора
            tbOldPrecondition.Text = "";
            tbNewPrecondition.Text = "";
            tbOldPostcondition.Text = "";
            tbNewPostcondition.Text = "";
            tbWpResult.Text = "";
            tbHoareTriad.Text = "";
            //tbSelectedOperator.Text = "";

            // Очистка пошагового трейса
            lbTrace.Items.Clear();

            // Сброс RadioButton'ов
            rbScenario1.IsChecked = false;
            rbScenario2.IsChecked = false;
            rbAssignment.IsChecked = false;
            rbBranching.IsChecked = false;
            rbSequence.IsChecked = false;

            // Деактивация кнопок
            btnCalculate.IsEnabled = false;
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
            lbTrace.Items.Add($"6. Сформирована триада Хоара:");
            lbTrace.Items.Add($"   {{ {precondition} }}");
            lbTrace.Items.Add($"   S");
            lbTrace.Items.Add($"   {{ {postcondition} }}");

            // Прокручиваем к последнему элементу
            if (lbTrace.Items.Count > 0)
                lbTrace.ScrollIntoView(lbTrace.Items[lbTrace.Items.Count - 1]);
        }
    }
}
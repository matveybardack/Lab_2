using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfAppWPCalculator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
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

            // Показываем область условий только если оба выбора сделаны
            if (isOperationSelected && isPostconditionSelected)
            {
                gbConditions.Visibility = Visibility.Visible;
                btnCalculate.IsEnabled = true;
            }
            else
            {
                gbConditions.Visibility = Visibility.Collapsed;
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
                tbSelectedOperator.Text = $"{operation} - {postcondition}";
            }
            else
            {
                tbSelectedOperator.Text = "";
            }
        }

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            // Логика расчета слабейшего предусловия
            // В зависимости от выбранных опций заполняем поля

            string selectedOperation = "";
            string selectedPostcondition = "";

            // Определяем выбранную операцию
            if (rbScenario1.IsChecked == true) selectedOperation = "Operation1";
            else if (rbScenario2.IsChecked == true) selectedOperation = "Operation2";

            // Определяем выбранный способ редактирования
            if (rbAssignment.IsChecked == true) selectedPostcondition = "Assignment";
            else if (rbBranching.IsChecked == true) selectedPostcondition = "Branching";
            else if (rbSequence.IsChecked == true) selectedPostcondition = "Sequence";

            // Заполняем поля в зависимости от выбора (пример)
            tbOldPrecondition.Text = $"P (для {selectedOperation})";
            tbOldPostcondition.Text = $"Q (для {selectedPostcondition})";
            tbNewPrecondition.Text = $"wp(S, Q)";
            tbNewPostcondition.Text = $"Новое постусловие для {selectedOperation} - {selectedPostcondition}";
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            // Очистка всех полей и сброс выбора
            tbOldPrecondition.Text = "";
            tbNewPrecondition.Text = "";
            tbOldPostcondition.Text = "";
            tbNewPostcondition.Text = "";
            tbSelectedOperator.Text = "";

            // Сброс RadioButton'ов
            rbScenario1.IsChecked = false;
            rbScenario2.IsChecked = false;
            rbAssignment.IsChecked = false;
            rbBranching.IsChecked = false;
            rbSequence.IsChecked = false;

            // Скрываем область условий
            gbConditions.Visibility = Visibility.Collapsed;
            btnCalculate.IsEnabled = false;
        }
    }
}


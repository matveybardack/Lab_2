```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibraryWPCalculator.Services
{
    /// <summary>
    /// Сервис-обёртка над движком WPCalculator и глобальным трейсом WpTrace.
    /// Экземпляр этого класса создаётся на фронте (WPF) и предоставляет методы,
    /// описанные в IWPCalculatorService: очищает глобальный трейс перед вызовом,
    /// делегирует работу движку и аккуратно клонирует стек для последовательности.
    /// ВАЖНО: синтаксическая/семантическая валидация входных строк предполагается на уровне UI.
    /// </summary>
    public class WPCalculatorService : IWPCalculatorService
    {
        private readonly ClassLibraryWPCalculator.WPCalculator _engine;

        /// <summary>
        /// Конструктор. Можно передать собственную реализацию движка (для тестов).
        /// </summary>
        public WPCalculatorService(ClassLibraryWPCalculator.WPCalculator engine = null)
        {
            _engine = engine ?? new ClassLibraryWPCalculator.WPCalculator();
        }

        /// <summary>
        /// Вычисляет слабейшее предусловие для присваивания.
        /// Очищает глобальный трейс WpTrace перед вызовом.
        /// Не выполняет синтаксической валидации (UI отвечает за это).
        /// Бросает ArgumentException если входы равны null/пусты.
        /// </summary>
        public string CalculateForAssignment(string assignment, string postcondition)
        {
            WpTrace.Clear();

            if (string.IsNullOrWhiteSpace(assignment))
                throw new ArgumentException("Присваивание не может быть пустым.", nameof(assignment));
            if (string.IsNullOrWhiteSpace(postcondition))
                throw new ArgumentException("Постусловие не может быть пустым.", nameof(postcondition));

            // делегируем работу движку (движок сам пишет шаги в WpTrace)
            var result = _engine.CalculateForAssignment(assignment, postcondition);
            return result;
        }

        /// <summary>
        /// Вычисляет слабейшее предусловие для if-оператора.
        /// Очищает глобальный трейс WpTrace перед вызовом.
        /// Не выполняет синтаксической валидации (UI отвечает за это).
        /// Бросает ArgumentException если входы равны null/пусты.
        /// </summary>
        public string CalculateForIf(string ifStatement, string postcondition)
        {
            WpTrace.Clear();

            if (string.IsNullOrWhiteSpace(ifStatement))
                throw new ArgumentException("if-выражение не может быть пустым.", nameof(ifStatement));
            if (string.IsNullOrWhiteSpace(postcondition))
                throw new ArgumentException("Постусловие не может быть пустым.", nameof(postcondition));

            var result = _engine.CalculateForIf(ifStatement, postcondition);
            return result;
        }

        /// <summary>
        /// Вычисляет слабейшее предусловие для последовательности присваиваний.
        /// Очищает глобальный трейс WpTrace перед вызовом.
        /// Клонирует стек, чтобы не модифицировать объект вызывающего кода.
        /// Не выполняет синтаксической валидации присваиваний (UI отвечает за это).
        /// Бросает ArgumentException если стек null/пуст или postcondition null/пуст.
        /// </summary>
        public string CalculateForSequence(Stack<string> assignments, string postcondition)
        {
            WpTrace.Clear();

            if (assignments == null || assignments.Count == 0)
                throw new ArgumentException("Стек присваиваний не может быть пустым.", nameof(assignments));
            if (string.IsNullOrWhiteSpace(postcondition))
                throw new ArgumentException("Постусловие не может быть пустым.", nameof(postcondition));

            // клонируем стек, чтобы не разрушить оригинал (Stack.ToArray возвращает массив с top первым)
            var clone = new Stack<string>(assignments.ToArray().Reverse());

            var result = _engine.CalculateForSequence(clone, postcondition);
            return result;
        }
    }
}

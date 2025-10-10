using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ClassLibraryWPCalculator
{
    public class WPCalculator
    {
        // Обработка оператора присваивания.
        public string CalculateForAssignment(string assignment, string postcondition)
        {
            var match = Regex.Match(assignment, @"\s*(\w+)\s*:=\s*(.*)");

            var variable = match.Groups[1].Value;
            var expression = match.Groups[2].Value;

            // Просто заменяем все вхождения переменной на выражение в постусловии.
            // Это простая реализация, которая не использует дерево выражений.
            string weakestPrecondition = Regex.Replace(postcondition, $@"\b{variable}\b", $"({expression})");

            WpTrace.Add(string.Format(assignment + " ; " + postcondition + " -> " + weakestPrecondition));

            // Упрощаем выражение
            InequalitySimplifier simplifier = new InequalitySimplifier();
            weakestPrecondition = simplifier.SimplificateInequality(weakestPrecondition);

            WpTrace.Add(weakestPrecondition);

            return weakestPrecondition;
        }

        // Обработка If
        public string CalculateForIf(string ifStatement, string postcondition)
        {
            // Простейший парсинг для "if (B) then S1 else S2"
            var ifRegex = new Regex(@"if\s*\((.*)\)\s*then\s*(.*)\s*else\s*(.*)");
            var match = ifRegex.Match(ifStatement);

            var conditionB = match.Groups[1].Value.Trim();
            var statementS1 = match.Groups[2].Value.Trim();
            var statementS2 = match.Groups[3].Value.Trim();

            // wp(if B then S1 else S2, R) = (B ∧ wp(S1,R)) ∨ (¬B ∧ wp(S2,R))
            var wpS1 = CalculateForAssignment(statementS1, postcondition);
            var wpS2 = CalculateForAssignment(statementS2, postcondition);

            // Формируем итоговое предусловие. Упрощение (например, not B) здесь не делается.
            var notConditionB = conditionB.Contains(">") ? conditionB.Replace(">", "<=") : conditionB.Replace("<", ">="); // Простое отрицание для примера
            return $"({conditionB} && {wpS1}) || ({notConditionB} && {wpS2})";
        }

        // Обработка последовательности
        public string CalculateForSequence(Stack<string> assignments, string postcondition)
        {
            // Базовый случай рекурсии: если стек пуст — вернуть текущее постусловие
            if (assignments == null || assignments.Count == 0)
                return postcondition;

            // 1. Вытаскиваем верхнее присваивание (оно выполняется последним)
            string currentAssignment = (string)assignments.Pop();

            // 2. Вычисляем слабейшее постусловие для этого присваивания
            string newPostcondition = CalculateForAssignment(currentAssignment, postcondition);

            // 3. Рекурсивно вызываем для оставшихся операторов
            return CalculateForSequence(assignments, newPostcondition);
        }
    }
}

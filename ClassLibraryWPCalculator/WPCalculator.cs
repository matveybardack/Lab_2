using System.Text.RegularExpressions;

namespace ClassLibraryWPCalculator
{
    public class WPCalculator
    {
        // Этот метод будет обрабатывать оператор присваивания.
        // Ожидает оператор в формате "переменная := выражение"
        public string CalculateForAssignment(string assignment, string postcondition)
        {
            var match = Regex.Match(assignment, @"\s*(\w+)\s*:=\s*(.*)");
            if (!match.Success)
            {
                return "Error: Invalid assignment format.";
            }

            var variable = match.Groups[1].Value;
            var expression = match.Groups[2].Value;

            // Просто заменяем все вхождения переменной на выражение в постусловии.
            // Это простая реализация, которая не использует дерево выражений.
            string weakestPrecondition = Regex.Replace(postcondition, $@"\b{variable}\b", $"({expression})");

            return weakestPrecondition;
        }

        public string CalculateForIf(string ifStatement, string postcondition)
        {
            // Простейший парсинг для "if (B) then S1 else S2"
            var ifRegex = new Regex(@"if\s*\((.*)\)\s*then\s*(.*)\s*else\s*(.*)");
            var match = ifRegex.Match(ifStatement);

            if (!match.Success)
            {
                return "Error: Invalid if statement format.";
            }

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
    }
}

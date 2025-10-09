using System.Text.RegularExpressions;

namespace ClassLibraryWPCalculator
{
    public class ExpressionParser
    {
        // Метод для разбора оператора присваивания
        public bool TryParseAssignment(string input, out string variable, out string expression)
        {
            variable = null;
            expression = null;

            var match = Regex.Match(input, @"\s*(\w+)\s*:=\s*(.*)");
            if (match.Success)
            {
                variable = match.Groups[1].Value;
                expression = match.Groups[2].Value.Trim();
                return true;
            }

            return false;
        }
    }
}

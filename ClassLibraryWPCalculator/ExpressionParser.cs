using System.Text.RegularExpressions;

namespace ClassLibraryWPCalculator
{
    public class ExpressionParser
    {
        // Метод для разбора оператора присваивания
        public bool TryParseAssignment(string input)
        {

            var match = Regex.Match(input, @"\s*(\w+)\s*:=\s*(.*)");
            if (match.Success)
                return true;

            return false;
        }

        // Простейший парсинг для "if (B) then S1 else S2"
        public bool TryParseIf(string ifStatement)
        {
            var ifRegex = new Regex(@"if\s*\((.*)\)\s*then\s*(.*)\s*else\s*(.*)");
            var match = ifRegex.Match(ifStatement);

            if (match.Success)
            {
                var statementS1 = match.Groups[2].Value.Trim();
                var statementS2 = match.Groups[3].Value.Trim();

                return TryParseAssignment(statementS1) && TryParseAssignment(statementS2);

            }

            return false;
        }
    }
}

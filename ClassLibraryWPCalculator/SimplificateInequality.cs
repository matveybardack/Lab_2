using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClassLibraryWPCalculator
{
    public class InequalitySimplifier : ISimplificateInequality
    {
        public string SimplificateInequality(string inequality)
        {
            // Найдём знак неравенства
            var match = Regex.Match(inequality, @"(<=|>=|<|>)");
            if (!match.Success)
                throw new ArgumentException("Не найден знак неравенства");

            string op = match.Value;

            string left = inequality.Substring(0, match.Index).Trim();
            string right = inequality.Substring(match.Index + op.Length).Trim();

            // Определяем имя переменной
            var varMatch = Regex.Match(left, @"[a-zA-Z_]\w*");
            if (!varMatch.Success)
                throw new ArgumentException("Не найдена переменная в левой части");

            string varName = varMatch.Value;

            // Вычисляем коэффициент и константу
            double rightValue = EvaluateExpression(right, varName, 0);
            double leftAt0 = EvaluateExpression(left, varName, 0);
            double leftAt1 = EvaluateExpression(left, varName, 1);

            double k = leftAt1 - leftAt0; // коэффициент при переменной
            double c = leftAt0;           // свободный член

            if (Math.Abs(k) < 1e-9)
                throw new InvalidOperationException("Не удалось выделить переменную — коэффициент равен 0.");

            // Преобразуем неравенство: k*x + c op r → x op' (r - c)/k
            double rhs = (rightValue - c) / k;
            string finalOp = op;

            if (k < 0)
                finalOp = InvertOperator(op);

            rhs = Math.Round(rhs, 10); // аккуратное округление

            // Формируем результат
            return string.Format("{0} {1} {2}", varName, finalOp, rhs);
        }

        private string InvertOperator(string op)
        {
            if (op == "<") return ">";
            if (op == ">") return "<";
            if (op == "<=") return ">=";
            if (op == ">=") return "<=";
            return op;
        }

        private double EvaluateExpression(string expr, string varName, double varValue)
        {
            // Заменяем abs() → Math.Abs()
            expr = Regex.Replace(expr, @"\babs\s*\(", "Math.Abs(");

            // Подставляем переменную
            expr = Regex.Replace(expr, @"\b" + Regex.Escape(varName) + @"\b", "(" + varValue.ToString(System.Globalization.CultureInfo.InvariantCulture) + ")");

            // Вычисляем выражение
            try
            {
                DataTable table = new DataTable();
                object result = table.Compute(expr, "");
                return Convert.ToDouble(result);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ошибка вычисления выражения: " + expr, ex);
            }
        }
    }
}

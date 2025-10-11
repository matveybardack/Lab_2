using ClassLibraryWPCalculator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WpfAppWPCalculator
{
    /// <summary>
    /// Парсер простых выражений для движка wp.
    /// Реализует IExpressionParser — проверяет корректность присваиваний и if-операторов.
    /// Поддерживает базовую валидацию имени переменной, правой части присваивания (должна быть непустой
    /// арифметической/функциональной записью) и if-ветвей с обязательными присваиваниями.
    /// </summary>
    public class ExpressionParser : IExpressionParser
    {
        /// <summary>
        /// Проверка правильности написания присваивания.
        /// Принимает варианты с пробелами и без: "x := 10", "a:=a+1", "count := abs(value) + 5".
        /// Метод не выполняет вычислений, только синтаксическую валидацию.
        /// </summary>
        public bool TryParseAssignment(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            // Общая форма: <var> := <expression>
            var match = Regex.Match(input, @"^\s*([a-zA-Z_]\w*)\s*:=\s*(.+)$", RegexOptions.Singleline);
            if (!match.Success)
                return false;

            string varName = match.Groups[1].Value;
            string rhs = match.Groups[2].Value.Trim();

            // Имя переменной — базовая проверка
            if (!IsValidVariableName(varName))
                return false;

            // Правая часть не должна быть пустой и должна быть валидным "арифметическим" выражением
            if (string.IsNullOrWhiteSpace(rhs))
                return false;

            if (!IsBalancedParentheses(rhs))
                return false;

            if (!IsLikelyArithmeticExpression(rhs))
                return false;

            return true;
        }

        /// <summary>
        /// Проверка правильности написания if-оператора вида:
        /// if (B) then S1 else S2
        /// где S1 и S2 — обязательные присваивания.
        /// Возвращает true только если синтаксис подходит и обе ветви содержат корректные присваивания.
        /// </summary>
        public bool TryParseIf(string ifStatement)
        {
            if (string.IsNullOrWhiteSpace(ifStatement))
                return false;

            var ifRegex = new Regex(@"^\s*if\s*\((.+?)\)\s*then\s*(.+?)\s*else\s*(.+)\s*$", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var match = ifRegex.Match(ifStatement);
            if (!match.Success)
                return false;

            string condition = match.Groups[1].Value.Trim();
            string thenPart = match.Groups[2].Value.Trim();
            string elsePart = match.Groups[3].Value.Trim();

            // Условие не пустое, должно содержать сравнение (>, <, >=, <=, ==, =)
            if (string.IsNullOrWhiteSpace(condition))
                return false;

            if (!ContainsComparisonOperator(condition))
                return false;

            if (!IsBalancedParentheses(condition))
                return false;

            // Ветви должны быть корректными присваиваниями (мы требуем присваивания)
            if (!TryParseAssignment(thenPart))
                return false;

            if (!TryParseAssignment(elsePart))
                return false;

            return true;
        }


        private bool IsValidVariableName(string name)
        {
            return Regex.IsMatch(name ?? "", @"^[a-zA-Z_]\w*$");
        }

        private bool ContainsComparisonOperator(string s)
        {
            return Regex.IsMatch(s ?? "", @"(<=|>=|<|>|==|=)");
        }

        private bool IsBalancedParentheses(string s)
        {
            int depth = 0;
            foreach (char c in s)
            {
                if (c == '(') depth++;
                else if (c == ')')
                {
                    depth--;
                    if (depth < 0) return false;
                }
            }
            return depth == 0;
        }

        private bool IsLikelyArithmeticExpression(string expr)
        {
            // Упростим: допускаем цифры, переменные, пробелы, точку (для дробей),
            // арифметические операторы + - * / ^, скобки, функцию abs и символы логических &| для составных (редко в RHS).
            // Требуем хотя бы один валидный токен: цифру или переменную.
            if (string.IsNullOrWhiteSpace(expr))
                return false;

            // Откроем expr для проверки на запрещённые символы
            // Разрешённые символы: буквы, цифры, + - * / ^ ( ) . , _ : space и символы < > = & | !
            if (!Regex.IsMatch(expr, @"^[0-9A-Za-z_\s\+\-\*\/\^\(\)\.\,<>!=&|:]+$"))
                return false;

            // Должен содержать хотя бы одну цифру или переменную
            if (!Regex.IsMatch(expr, @"[0-9A-Za-z_]"))
                return false;

            // Проверим, что нет подряд идущих операторов вида "++" или "+*" — простая эвристика
            if (Regex.IsMatch(expr, @"[\+\-\*\/\^]{2,}"))
                return false;

            // Допускаем использование abs(...) — проверим корректность вызовов (хотя бы синтаксически)
            foreach (Match m in Regex.Matches(expr, @"\babs\s*\(", RegexOptions.IgnoreCase))
            {
                // проверка просто на наличие соответствующей закрывающей скобки после позиции
                int pos = m.Index + m.Length - 1;
                if (pos < 0 || pos >= expr.Length)
                    return false;
                // найдём соответствующую закрывающую скобку
                int depth = 0;
                bool found = false;
                for (int i = pos; i < expr.Length; i++)
                {
                    if (expr[i] == '(') depth++;
                    else if (expr[i] == ')')
                    {
                        depth--;
                        if (depth == 0) { found = true; break; }
                    }
                }
                if (!found) return false;
            }

            return true;
        }
    }
}

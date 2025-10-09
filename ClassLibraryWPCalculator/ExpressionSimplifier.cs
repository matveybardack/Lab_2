using MathNet.Symbolics;
using System;

namespace ClassLibraryWPCalculator
{
    // По итогу не работает как надо, так как MathNet.Symbolics не умеет упрощать логические выражения
    public class ExpressionSimplifier
    {
        public string Simplify(string expression)
        {
            try
            {
                var result = Infix.ParseOrThrow(expression); // упрощённый вариант

                var simplified = Algebraic.Expand(result);
                return simplified.ToString();
            }
            catch
            {
                return expression;
            }
        }
    }
}
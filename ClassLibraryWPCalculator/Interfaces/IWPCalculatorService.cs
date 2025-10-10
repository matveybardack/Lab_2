using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryWPCalculator
{
    public interface IWPCalculatorService
    {
        // IExpressionParser - интерфейс для проверки мусора

        /// <summary>
        /// Вычисляет слабейшее постусловие с присваиванием
        /// </summary>
        /// <param name="assignment">присваивание вида x := 2*(x - 1) + 3 </param>
        /// <param name="postcondition">постусловие вида x > 9 </param>
        /// <returns>упрощенное слабейшее условие x > 4 </returns>
        /// Требует проверки предусловия и постусловия на правильность написания
        string CalculateForAssignment(string assignment, string postcondition);

        /// <summary>
        /// Вычисляет слабейшее постусловие с if-оператором
        /// </summary>
        /// <param name="ifStatement">выражение if вида if (x > y) then max := x else max := y</param>
        /// <param name="postcondition">постусловие вида "max > 100</param>
        /// <returns>упрощенное слабейшее условие (x > y && x > 100) || (x <= y && y > 100)</returns>
        /// Требует проверок для предусловия и постусловия на правильность написания
        string CalculateForIf(string ifStatement, string postcondition);

        /// <summary>
        /// Вычисляет слабейшее постусловие для последовательности присваиваний
        /// </summary>
        /// <param name="assignments">Стэк в котором лежат присваивания</param>
        /// <param name="postcondition">стандартное постусловие</param>
        /// <returns>упрощенное слабейшее условие см.CalculateForAssignment</returns>
        /// Для каждого отдельного присваивания нужна проверка
        string CalculateForSequence(Stack<string> assignments, string postcondition);
    }
}

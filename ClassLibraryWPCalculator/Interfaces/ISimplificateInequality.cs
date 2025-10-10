using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryWPCalculator
{
    // Используйте для проверки верности постусловия, также преобразует сложные постусловия с выражениями (+, -, abs())
    public interface ISimplificateInequality // !!!! Создавать ему сервисы не нужно !!!!
    {
        /// <summary>
        /// Преобразователь неравенства в простую форму
        /// </summary>
        /// <param name="inequality">неравенство (постусловия вида x > 20)</param>
        /// <exception cref="ArgumentException">Не найдена переменная в левой части</exception>
        /// /// <exception cref="ArgumentException">Не найден знак неравенства</exception>
        /// <exception cref="InvalidOperationException">Не удалось выделить переменную — коэффициент равен 0.</exception>
        /// <returns>упрощенное выражение</returns>
        string SimplificateInequality(string inequality);
    }
}

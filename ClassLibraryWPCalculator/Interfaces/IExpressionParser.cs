using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryWPCalculator.Interfaces
{
    public interface IExpressionParser // Можешь дополнить на дополнительные проверки
    {
        /// <summary>
        /// Проверка правильности написания предусловия с присваиванием
        /// </summary>
        /// <param name="input"> Примеры подходящих (шаблон @"\s*(\w+)\s*:=\s*(.*)")
        /// x := 10
        /// y := x + 2
        /// count := abs(value) + 5
        /// </param>
        /// <returns>true если удовлетворяет шаблону, иначе false</returns>
        bool TryParseAssignment(string input);

        /// <summary>
        /// Проверка правильности написания предусловия с if-выражением
        /// </summary>
        /// <param name="ifStatement">Примеры (шаблон @"if\s*\((.*)\)\s*then\s*(.*)\s*else\s*(.*)")
        /// if (x > 0) then y := 10 else y := 5 
        /// if (a <= b) then x := a else x := b  
        /// </param>
        /// <returns>true если удовлетворяет шаблону, иначе false</returns>
        bool TryParseIf(string ifStatement); //сделал обязательное присваивание, так как другой мусор обрабатывать я не придумал
    }
}

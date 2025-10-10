using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryWPCalculator
{
    // Отображение трейса
    public static  class WpTrace
    {
        // Очередь для хранения шагов вычислений
        private static Queue<string> _trace = new Queue<string>();

        // Доступ только через методы
        public static void Add(string message)
        {
            _trace.Enqueue(message);
        }

        public static IEnumerable<string> GetAll()
        {
            return _trace.ToArray();
        }

        public static void Clear()
        {
            _trace.Clear();
        }
    }
}

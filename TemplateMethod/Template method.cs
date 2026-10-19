using System;
using System.Collections.Generic;
using System.Text;

namespace laba1
{
    namespace TemplateMethodPattern
    {
        // ─── Абстрактный класс ────────────────────────────────────
        public abstract class ArrayProcessor
        {
            // Шаблонный метод (Алгоритм B)
            public void TemplateMethod(int n)
            {
                // Шаг 1-2: создать и заполнить массив
                double[] arrayX = InitArray(n);

                Console.Write("  Массив: ");
                foreach (double v in arrayX)
                    Console.Write($"{v:F4} ");
                Console.WriteLine();

                // Шаг 3: частная операция 1 → индекс
                int index = FindIndex(arrayX);
                Console.WriteLine($"  Найденный индекс: {index}");

                // Шаг 4: соседи
                double left = index > 0 ? arrayX[index - 1] : 0.0;
                double right = index < n - 1 ? arrayX[index + 1] : 0.0;
                double C = left + right;
                Console.WriteLine($"  C (сумма соседей): {C:F4}");

                // Шаг 5: частная операция 2
                double result = ComputeResult(arrayX, C);
                Console.WriteLine($"  Результат: {result:F6}");
            }

            // Вспомогательный: инициализация массива случайными числами [0,1)
            private double[] InitArray(int n)
            {
                var rnd = new Random(42);   // фиксированный seed для воспроизводимости
                double[] arr = new double[n];
                for (int i = 0; i < n; i++)
                    arr[i] = rnd.NextDouble();
                return arr;
            }

            // Частная операция 1: найти индекс
            public abstract int FindIndex(double[] array);

            // Частная операция 2: найти результирующее число
            public abstract double ComputeResult(double[] array, double C);
        }

        // ─── Конкретный класс A ───────────────────────────────────
        // ч.о.1: индекс ПЕРВОГО элемента > 0.5
        // ч.о.2: ПРОИЗВЕДЕНИЕ элементов > C
        public class ArrayProcessorA : ArrayProcessor
        {
            public override int FindIndex(double[] array)
            {
                for (int i = 0; i < array.Length; i++)
                    if (array[i] > 0.5) return i;
                return -1;  // не найден
            }

            public override double ComputeResult(double[] array, double C)
            {
                double product = 1.0;
                bool found = false;
                foreach (double v in array)
                {
                    if (v > C)
                    {
                        product *= v;
                        found = true;
                    }
                }
                return found ? product : 0.0;
            }

            public override string ToString() => "ArrayProcessorA";
        }

        // ─── Конкретный класс B ───────────────────────────────────
        // ч.о.1: индекс ПОСЛЕДНЕГО элемента > 0.5
        // ч.о.2: СУММА элементов < C
        public class ArrayProcessorB : ArrayProcessor
        {
            public override int FindIndex(double[] array)
            {
                int idx = -1;
                for (int i = 0; i < array.Length; i++)
                    if (array[i] > 0.5) idx = i;
                return idx;
            }

            public override double ComputeResult(double[] array, double C)
            {
                double sum = 0.0;
                foreach (double v in array)
                    if (v < C) sum += v;
                return sum;
            }

            public override string ToString() => "ArrayProcessorB";
        }
    }
}

using System;

namespace laba1.TemplateMethodPattern
{
    // Абстрактный класс. Алгоритм B:
    //  1. Задать размер массива n.
    //  2. Инициализировать массив случайными вещественными числами из [0, 1).
    //  3. Найти индекс index (частная операция 1).
    //  4. Сложить соседние элементы для элемента index -> C
    //     (отсутствующий сосед считается равным 0).
    //  5. Найти число result (частная операция 2).
    //  6. Вывести result.
    public abstract class ArrayProcessor
    {
        // Шаблонный метод: задаёт структуру алгоритма, не переопределяется
        public double TemplateMethod(int n, int? seed = null)
        {
            // Шаг 1: размер массива
            if (n <= 0)
                throw new ArgumentOutOfRangeException(nameof(n), "Размер массива должен быть положительным.");

            // Шаг 2: инициализация случайными числами
            double[] arrayX = InitArray(n, seed);
            Console.WriteLine($"\n  [{this}]");
            Console.Write("  Массив: ");
            foreach (double v in arrayX)
                Console.Write($"{v:F4} ");
            Console.WriteLine();

            // Шаг 3: частная операция 1
            int index = FindIndex(arrayX);

            // Шаг 4: C = сумма соседей элемента index
            double c = 0.0;
            if (index < 0)
            {
                Console.WriteLine("  Элемент больше 0.5 не найден, соседей нет: C = 0");
            }
            else
            {
                double left = index > 0 ? arrayX[index - 1] : 0.0;
                double right = index < arrayX.Length - 1 ? arrayX[index + 1] : 0.0;
                c = left + right;
                Console.WriteLine($"  Индекс: {index}, соседи: {left:F4} и {right:F4}");
            }
            Console.WriteLine($"  C = {c:F4}");

            // Шаг 5: частная операция 2
            double result = ComputeResult(arrayX, c);

            // Шаг 6: вывод результата
            Console.WriteLine($"  Результат: {result:F6}");
            return result;
        }

        // Общий для всех наследников шаг (не меняется)
        private static double[] InitArray(int n, int? seed)
        {
            Random rnd = seed.HasValue ? new Random(seed.Value) : new Random();
            double[] arr = new double[n];
            for (int i = 0; i < n; i++)
                arr[i] = rnd.NextDouble();      // [0, 1)
            return arr;
        }

        // Частная операция 1: найти индекс (-1, если подходящего элемента нет)
        public abstract int FindIndex(double[] array);

        // Частная операция 2: найти число result по массиву и числу C
        public abstract double ComputeResult(double[] array, double c);
    }
}
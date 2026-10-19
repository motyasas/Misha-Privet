namespace laba1.TemplateMethodPattern
{
    // Конкретный класс B
    // ч.о.1: индекс ПОСЛЕДНЕГО элемента > 0.5
    // ч.о.2: СУММА элементов < C
    public class ArrayProcessorB : ArrayProcessor
    {
        public override int FindIndex(double[] array)
        {
            for (int i = array.Length - 1; i >= 0; i--)
                if (array[i] > 0.5) return i;
            return -1;
        }

        public override double ComputeResult(double[] array, double c)
        {
            double sum = 0.0;
            foreach (double v in array)
                if (v < c) sum += v;
            return sum;
        }

        public override string ToString() => "Класс B: последний > 0.5, сумма элементов < C";
    }
}
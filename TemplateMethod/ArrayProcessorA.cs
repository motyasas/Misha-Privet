namespace laba1.TemplateMethodPattern
{
    // Конкретный класс A
    // ч.о.1: индекс ПЕРВОГО элемента > 0.5
    // ч.о.2: ПРОИЗВЕДЕНИЕ элементов > C
    public class ArrayProcessorA : ArrayProcessor
    {
        public override int FindIndex(double[] array)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] > 0.5) return i;
            return -1;
        }

        public override double ComputeResult(double[] array, double c)
        {
            double product = 1.0;
            bool found = false;
            foreach (double v in array)
            {
                if (v > c)
                {
                    product *= v;
                    found = true;
                }
            }
            return found ? product : 0.0;   // нет подходящих элементов -> 0
        }

        public override string ToString() => "Класс A: первый > 0.5, произведение элементов > C";
    }
}
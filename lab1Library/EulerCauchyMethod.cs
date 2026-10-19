using NCalc;
using System;
using System.Collections.Generic;

namespace laba1
{
    public class EulerCauchyMethod : IODESolver
    {
        private string _formula;  // инкапсуляция: поле закрыто

        public string Formula     // доступ через свойство
        {
            get { return _formula; }
            set { _formula = value; }
        }

        public EulerCauchyMethod(string formula)
        {
            _formula = formula;
        }

        private double F(double x, double y)
        {
            NCalc.Expression e = new NCalc.Expression(_formula);
            e.Parameters["x"] = x;
            e.Parameters["y"] = y;
            e.Parameters["e"] = Math.E;
            return Convert.ToDouble(e.Evaluate());
        }

        public List<(double X, double Y)> Solve(double x0, double y0, double xn, double h)
        {
            Console.WriteLine("\n  [Метод Эйлера-Коши]");
            Console.WriteLine($"  {"x",8}  {"y",14}");

            var result = new List<(double X, double Y)>();

            int n = (int)Math.Round((xn - x0) / h);
            double x = x0, y = y0;
            Console.WriteLine($"  {x,8:F4}  {y,14:F8}");
            result.Add((x, y));

            for (int i = 0; i < n; i++)
            {
                double k1 = F(x, y);
                double k2 = F(x + h, y + h * k1);
                y += h * (k1 + k2) / 2.0;
                x += h;
                Console.WriteLine($"  {x,8:F4}  {y,14:F8}");
                result.Add((x, y));
            }

            return result;
        }
    }
}

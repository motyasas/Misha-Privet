using NCalc;
using System;
using System.Collections.Generic;

namespace laba1
{
    public class RungeKutta2Method : IODESolver
    {
        private string _formula;

        public string Formula
        {
            get { return _formula; }
            set { _formula = value; }
        }

        public RungeKutta2Method(string formula)
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
            Console.WriteLine("\n  [Метод Рунге-Кутта 2-го порядка]");
            Console.WriteLine($"  {"x",8}  {"y",14}");

            var result = new List<(double X, double Y)>();

            int n = (int)Math.Round((xn - x0) / h);
            double x = x0, y = y0;
            Console.WriteLine($"  {x,8:F4}  {y,14:F8}");
            result.Add((x, y));

            for (int i = 0; i < n; i++)
            {
                double k1 = h * F(x, y);
                double k2 = h * F(x + h / 2.0, y + k1 / 2.0);
                y += k2;
                x += h;
                Console.WriteLine($"  {x,8:F4}  {y,14:F8}");
                result.Add((x, y));
            }

            return result;
        }
    }
}

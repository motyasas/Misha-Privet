using System;
using System.Collections.Generic;

namespace laba1
{
    // Контекст
    public class ODEContext
    {
        public IODESolver Solver { get; set; }

        public ODEContext(IODESolver solver)
        {
            Solver = solver;
        }

        public List<(double X, double Y)> Request(double x0, double y0, double xn, double h)
        {
            return Solver.Solve(x0, y0, xn, h);
        }
    }
}

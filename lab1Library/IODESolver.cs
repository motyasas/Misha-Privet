using System;
using System.Collections.Generic;

namespace laba1
{
    // Интерфейс стратегии
    public interface IODESolver
    {
        List<(double X, double Y)> Solve(double x0, double y0, double xn, double h);
    }
}

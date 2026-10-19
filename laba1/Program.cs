using System;
using System.Globalization;
using NCalc;
using laba1.TemplateMethodPattern;

class Program
{
    // Читает число, принимая и точку, и запятую как десятичный разделитель
    static double ReadDouble(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string s = (Console.ReadLine() ?? "").Trim().Replace(',', '.');
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                return value;
            Console.WriteLine("  Некорректное число, повторите ввод.");
        }
    }

    // Читает целое число из диапазона [min, max]
    static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            string s = (Console.ReadLine() ?? "").Trim();
            if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
                && value >= min && value <= max)
                return value;
            Console.WriteLine($"  Введите целое число от {min} до {max}.");
        }
    }

    // Проверяет, что выражение разбирается и вычисляется
    static bool CheckExpression(string expr, string title)
    {
        try
        {
            NCalc.Expression e = new NCalc.Expression(expr);
            e.Parameters["x"] = 1.0;
            e.Parameters["y"] = 1.0;
            e.Parameters["e"] = Math.E;
            Convert.ToDouble(e.Evaluate());
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nНе удалось разобрать {title}: \"{expr}\"");
            Console.WriteLine($"Причина: {ex.Message}");
            Console.WriteLine("Проверьте: точки вместо запятых, латинские буквы, Pow(x,2) вместо x^2.");
            return false;
        }
    }

    static void Main()
    {
        Console.WriteLine("Лабораторная работа 1");
        Console.WriteLine("  1 - Шаблон Стратегия (решение ОДУ)");
        Console.WriteLine("  2 - Шаблон Шаблонный метод (алгоритм B)");
        int part = ReadInt("Ваш выбор: ", 1, 2);

        if (part == 1) RunStrategy();
        else RunTemplateMethod();
    }

    // ───────────────────────── Шаблон Шаблонный метод ─────────────────────────
    static void RunTemplateMethod()
    {
        int n = ReadInt("\nВведите размер массива n: ", 1, 1_000_000);

        // Один и тот же seed -> оба класса обрабатывают одинаковый массив
        int seed = Environment.TickCount;

        ArrayProcessor a = new ArrayProcessorA();
        ArrayProcessor b = new ArrayProcessorB();

        a.TemplateMethod(n, seed);
        b.TemplateMethod(n, seed);
    }

    // ───────────────────────── Шаблон Стратегия ─────────────────────────
    static void RunStrategy()
    {
        Console.Write("\nВведите функцию f(x,y) (например x - y): ");
        string formula = (Console.ReadLine() ?? "").Trim();

        Console.Write("Введите точное решение y(x) (например x - 1 + 2*Exp(-x)): ");
        string exact = (Console.ReadLine() ?? "").Trim();

        if (!CheckExpression(formula, "функцию f(x,y)")) return;
        if (!CheckExpression(exact, "точное решение")) return;

        double x0 = ReadDouble("Введите x0: ");
        double y0 = ReadDouble("Введите y0: ");
        double xn = ReadDouble("Введите xn: ");
        double h = ReadDouble("Введите шаг h: ");

        Console.WriteLine("\nВыберите метод:");
        Console.WriteLine("  1 - Метод Эйлера-Коши");
        Console.WriteLine("  2 - Метод Рунге-Кутта 2-го порядка");
        Console.WriteLine("  3 - Метод Рунге-Кутта 4-го порядка");
        int choice = ReadInt("Ваш выбор: ", 1, 3);

        laba1.ODEContext ctx;
        laba1.MethodKind kind;

        switch (choice)
        {
            case 1:
                ctx = new laba1.ODEContext(new laba1.EulerCauchyMethod(formula));
                kind = laba1.MethodKind.EulerCauchy;
                break;
            case 2:
                ctx = new laba1.ODEContext(new laba1.RungeKutta2Method(formula));
                kind = laba1.MethodKind.RungeKutta2;
                break;
            default:
                ctx = new laba1.ODEContext(new laba1.RungeKutta4Method(formula));
                kind = laba1.MethodKind.RungeKutta4;
                break;
        }

        var result = ctx.Request(x0, y0, xn, h);

        // Точное решение
        Console.WriteLine("\n  [Точное решение]");
        Console.WriteLine($"  {"x",8}  {"y_exact",14}");

        int n = (int)Math.Round((xn - x0) / h);
        for (int i = 0; i <= n; i++)
        {
            double x = x0 + i * h;
            NCalc.Expression e = new NCalc.Expression(exact);
            e.Parameters["x"] = x;
            e.Parameters["e"] = Math.E;
            double yExact = Convert.ToDouble(e.Evaluate());
            Console.WriteLine($"  {x,8:F4}  {yExact,14:F8}");
        }

        // Генерация Excel-файла контрольного теста
        string path = "test.xlsx";
        try
        {
            new laba1.ExcelReportGenerator().Generate(path, kind, formula, exact, x0, y0, h, result);
            Console.WriteLine($"\nФайл {path} создан: {System.IO.Path.GetFullPath(path)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nНе удалось создать Excel-файл: {ex.Message}");
        }
    }
}
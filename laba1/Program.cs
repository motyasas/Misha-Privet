using System;
using System.Globalization;
using NCalc;

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
        Console.Write("Введите функцию f(x,y) (например x - y): ");
        string formula = Console.ReadLine()!.Trim();

        Console.Write("Введите точное решение y(x) (например x - 1 + 2*Exp(-x)): ");
        string exact = Console.ReadLine()!.Trim();

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
        Console.Write("Ваш выбор: ");
        int choice = int.Parse(Console.ReadLine()!);

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
            case 3:
                ctx = new laba1.ODEContext(new laba1.RungeKutta4Method(formula));
                kind = laba1.MethodKind.RungeKutta4;
                break;
            default:
                Console.WriteLine("Неверный выбор.");
                return;
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

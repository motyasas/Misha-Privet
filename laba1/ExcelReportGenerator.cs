using System;
using System.Collections.Generic;
using ClosedXML.Excel;

namespace laba1
{
    public enum MethodKind
    {
        EulerCauchy,
        RungeKutta2,
        RungeKutta4
    }

    /// <summary>
    /// Генерирует Excel-файл контрольного теста: независимый расчёт метода формулами Excel,
    /// сравнение с результатом программы и с точным решением.
    /// </summary>
    public class ExcelReportGenerator
    {
        private const int FirstRow = 8;      // первая строка данных
        private const string H = "$B$2";     // ячейки параметров
        private const string X0 = "$B$3";
        private const string Y0 = "$B$4";
        private const string Tol = "$B$5";

        public void Generate(string path, MethodKind kind, string formula, string exact,
                             double x0, double y0, double h,
                             IReadOnlyList<(double X, double Y)> program,
                             double tolerance = 1e-9)
        {
            int n = program.Count - 1;                 // число шагов
            int nk = kind == MethodKind.RungeKutta4 ? 4 : 2;

            // номера столбцов
            int cI = 1, cX = 2, cK = 3;
            int cY = cK + nk, cProg = cY + 1, cDiff = cY + 2, cStat = cY + 3, cEx = cY + 4, cErr = cY + 5;

            string Col(int c) => XLHelper.GetColumnLetterFromNumber(c);

            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Тест");

            // --- шапка и параметры ---
            ws.Cell("A1").Value = "Контрольный тест: " + Title(kind);
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A2").Value = "h";   ws.Cell("B2").Value = h;
            ws.Cell("A3").Value = "x0";  ws.Cell("B3").Value = x0;
            ws.Cell("A4").Value = "y0";  ws.Cell("B4").Value = y0;
            ws.Cell("A5").Value = "Допуск"; ws.Cell("B5").Value = tolerance;
            ws.Cell("C2").Value = "f(x, y) = " + formula;
            ws.Cell("C3").Value = "y(x) точное = " + exact;
            ws.Range("B2:B5").Style.Font.FontColor = XLColor.Blue;

            // --- заголовки таблицы ---
            string[] kNames = nk == 2
                ? new[] { "k1", "k2" }
                : new[] { "k1", "k2", "k3", "k4" };

            ws.Cell(FirstRow - 1, cI).Value = "i";
            ws.Cell(FirstRow - 1, cX).Value = "x";
            for (int j = 0; j < nk; j++) ws.Cell(FirstRow - 1, cK + j).Value = kNames[j];
            ws.Cell(FirstRow - 1, cY).Value = "y (Excel)";
            ws.Cell(FirstRow - 1, cProg).Value = "y (программа C#)";
            ws.Cell(FirstRow - 1, cDiff).Value = "|Δ| Excel–C#";
            ws.Cell(FirstRow - 1, cStat).Value = "Статус";
            ws.Cell(FirstRow - 1, cEx).Value = "y точное";
            ws.Cell(FirstRow - 1, cErr).Value = "|y Excel − y точное|";
            var head = ws.Range(FirstRow - 1, 1, FirstRow - 1, cErr);
            head.Style.Font.Bold = true;
            head.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F3864");
            head.Style.Font.FontColor = XLColor.White;

            // --- строки данных ---
            for (int i = 0; i <= n; i++)
            {
                int r = FirstRow + i;
                string X = $"B{r}";
                string Y = $"{Col(cY)}{r}";
                string[] K = new string[nk];
                for (int j = 0; j < nk; j++) K[j] = $"{Col(cK + j)}{r}";

                ws.Cell(r, cI).Value = i;
                ws.Cell(r, cX).FormulaA1 = $"={X0}+A{r}*{H}";

                // k1, k2, (k3, k4) — зависят от метода
                switch (kind)
                {
                    case MethodKind.EulerCauchy:
                        ws.Cell(r, cK).FormulaA1 = "=" + F(formula, X, Y);
                        ws.Cell(r, cK + 1).FormulaA1 = "=" + F(formula, $"{X}+{H}", $"{Y}+{H}*{K[0]}");
                        break;

                    case MethodKind.RungeKutta2:
                        ws.Cell(r, cK).FormulaA1 = $"={H}*" + F(formula, X, Y);
                        ws.Cell(r, cK + 1).FormulaA1 = $"={H}*" + F(formula, $"{X}+{H}/2", $"{Y}+{K[0]}/2");
                        break;

                    case MethodKind.RungeKutta4:
                        ws.Cell(r, cK).FormulaA1 = $"={H}*" + F(formula, X, Y);
                        ws.Cell(r, cK + 1).FormulaA1 = $"={H}*" + F(formula, $"{X}+{H}/2", $"{Y}+{K[0]}/2");
                        ws.Cell(r, cK + 2).FormulaA1 = $"={H}*" + F(formula, $"{X}+{H}/2", $"{Y}+{K[1]}/2");
                        ws.Cell(r, cK + 3).FormulaA1 = $"={H}*" + F(formula, $"{X}+{H}", $"{Y}+{K[2]}");
                        break;
                }

                // y(i): первая строка — y0, дальше — формула метода по предыдущей строке
                if (i == 0)
                {
                    ws.Cell(r, cY).FormulaA1 = $"={Y0}";
                }
                else
                {
                    int p = r - 1;
                    string yPrev = $"{Col(cY)}{p}";
                    string k1 = $"{Col(cK)}{p}", k2 = $"{Col(cK + 1)}{p}";
                    string next = kind switch
                    {
                        MethodKind.EulerCauchy => $"={yPrev}+{H}*({k1}+{k2})/2",
                        MethodKind.RungeKutta2 => $"={yPrev}+{k2}",
                        _ => $"={yPrev}+({k1}+2*{k2}+2*{Col(cK + 2)}{p}+{Col(cK + 3)}{p})/6"
                    };
                    ws.Cell(r, cY).FormulaA1 = next;
                }

                // результат программы (значение, не формула)
                ws.Cell(r, cProg).Value = program[i].Y;
                ws.Cell(r, cProg).Style.Font.FontColor = XLColor.Blue;
                ws.Cell(r, cProg).Style.Fill.BackgroundColor = XLColor.Yellow;

                ws.Cell(r, cDiff).FormulaA1 = $"=ABS({Y}-{Col(cProg)}{r})";
                ws.Cell(r, cStat).FormulaA1 = $"=IF({Col(cDiff)}{r}<={Tol},\"OK\",\"ОШИБКА\")";
                ws.Cell(r, cEx).FormulaA1 = "=" + ExcelFormulaTranslator.Translate(exact, X, "0");
                ws.Cell(r, cErr).FormulaA1 = $"=ABS({Y}-{Col(cEx)}{r})";
            }

            int last = FirstRow + n;
            ws.Range(FirstRow, cX, last, cEx).Style.NumberFormat.Format = "0.00000000";
            ws.Range(FirstRow, cDiff, last, cDiff).Style.NumberFormat.Format = "0.00E+00";
            ws.Range(FirstRow, cErr, last, cErr).Style.NumberFormat.Format = "0.00E+00";

            // подсветка статуса
            var stat = ws.Range(FirstRow, cStat, last, cStat);
            stat.AddConditionalFormat().WhenContains("OK").Fill.SetBackgroundColor(XLColor.LightGreen);
            stat.AddConditionalFormat().WhenContains("ОШИБКА").Fill.SetBackgroundColor(XLColor.LightPink);

            // итог
            int s = last + 2;
            ws.Cell(s, 1).Value = "max |Δ| Excel–C#";
            ws.Cell(s, 3).FormulaA1 = $"=MAX({Col(cDiff)}{FirstRow}:{Col(cDiff)}{last})";
            ws.Cell(s + 1, 1).Value = "Вывод";
            ws.Cell(s + 1, 3).FormulaA1 = $"=IF(C{s}<={Tol},\"ТЕСТ ПРОЙДЕН\",\"ТЕСТ НЕ ПРОЙДЕН\")";
            ws.Cell(s + 1, 3).Style.Font.Bold = true;

            // фиксированная ширина (AdjustToContents у ClosedXML падает на некоторых машинах)
            for (int c = 1; c <= cErr; c++)
                ws.Column(c).Width = 18;
            ws.Row(FirstRow - 1).Height = 32;
            ws.Range(FirstRow - 1, 1, FirstRow - 1, cErr).Style.Alignment.WrapText = true;

            wb.SaveAs(path);
        }

        // f(x, y) в синтаксисе Excel с подстановкой выражений вместо x и y
        private static string F(string formula, string xExpr, string yExpr)
            => ExcelFormulaTranslator.Translate(formula, xExpr, yExpr);

        private static string Title(MethodKind kind) => kind switch
        {
            MethodKind.EulerCauchy => "метод Эйлера-Коши",
            MethodKind.RungeKutta2 => "метод Рунге-Кутта 2-го порядка",
            _ => "метод Рунге-Кутта 4-го порядка"
        };
    }
}

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace laba1
{
    /// <summary>
    /// Переводит формулу из синтаксиса NCalc (то, что вводит пользователь)
    /// в синтаксис формул Excel, подставляя вместо x и y нужные выражения/ячейки.
    /// </summary>
    public static class ExcelFormulaTranslator
    {
        // NCalc -> Excel
        private static readonly Dictionary<string, string> Functions =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Exp"] = "EXP",
            ["Sin"] = "SIN",
            ["Cos"] = "COS",
            ["Tan"] = "TAN",
            ["Asin"] = "ASIN",
            ["Acos"] = "ACOS",
            ["Atan"] = "ATAN",
            ["Sqrt"] = "SQRT",
            ["Abs"] = "ABS",
            ["Pow"] = "POWER",   // в NCalc нет оператора ^, степень только через Pow(a, b)
            ["Log"] = "LN",      // Log(x) в NCalc — натуральный логарифм
            ["Log10"] = "LOG10"
        };

        // Идентификатор, не являющийся частью числа (1e-3) или другого слова;
        // группа 2 непустая, если за идентификатором идёт "(" — значит, это функция.
        private static readonly Regex Token =
            new Regex(@"(?<![\d.\w])([A-Za-z_]\w*)(\s*\()?", RegexOptions.Compiled);

        /// <param name="formula">Формула в синтаксисе NCalc, например "x - y" или "Exp(-x) * y"</param>
        /// <param name="xExpr">Что подставить вместо x (адрес ячейки или выражение)</param>
        /// <param name="yExpr">Что подставить вместо y</param>
        public static string Translate(string formula, string xExpr, string yExpr)
        {
            string result = Token.Replace(formula, m =>
            {
                string id = m.Groups[1].Value;
                bool isFunction = m.Groups[2].Success;

                if (isFunction)
                {
                    if (!Functions.TryGetValue(id, out string excelName))
                        throw new NotSupportedException($"Функция '{id}' не поддерживается при экспорте в Excel.");
                    return excelName + "(";
                }

                switch (id.ToLowerInvariant())
                {
                    case "x": return "(" + xExpr + ")";
                    case "y": return "(" + yExpr + ")";
                    case "e": return "EXP(1)";
                    case "pi": return "PI()";
                    default:
                        throw new NotSupportedException($"Неизвестная переменная '{id}'.");
                }
            });

            return "(" + result + ")";
        }
    }
}

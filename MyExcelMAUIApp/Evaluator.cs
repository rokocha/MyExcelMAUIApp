using Antlr4.Runtime;
using MathParserLibrary;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MyExcelMAUIApp
{
    public class Evaluator
    {
        private Dictionary<string, string> _formulas;

        public Evaluator(Dictionary<string, string> formulas)
        {
            _formulas = formulas;
        }

        public string Evaluate(string expression)
        {
            try
            {
                double result = EvaluateRecursively(expression, new HashSet<string>());
                return result.ToString();
            }
            catch
            {
                return "#ERROR";
            }
        }

        private double EvaluateRecursively(string expression, HashSet<string> visited)
        {
            var cellValues = new Dictionary<string, double>();
            var regex = new Regex(@"[A-Z]+[0-9]+");

            // Знаходимо всі комірки у формулі та рекурсивно обчислюємо їхні значення
            foreach (Match match in regex.Matches(expression))
            {
                string cellName = match.Value;

                // Захист від нескінченного циклу (наприклад, A1=B1, а B1=A1)
                if (visited.Contains(cellName))
                    throw new Exception("Циклічне посилання");

                visited.Add(cellName);

                double val = 0;
                if (_formulas.TryGetValue(cellName, out string content))
                {
                    if (content.StartsWith("="))
                    {
                        val = EvaluateRecursively(content.Substring(1), visited);
                    }
                    else
                    {
                        double.TryParse(content, out val);
                    }
                }

                cellValues[cellName] = val;
                visited.Remove(cellName);
            }

            // Передаємо вираз в ANTLR із підготовленими значеннями комірок
            var inputStream = new AntlrInputStream(expression);
            var lexer = new CalculatorLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new CalculatorParser(commonTokenStream);
            var context = parser.compileUnit();

            var visitor = new CalculatorVisitor(cellValues);

            // Беремо .expr(), щоб уникнути помилки з нулем
            return visitor.Visit(context.expr());
        }
    }
}
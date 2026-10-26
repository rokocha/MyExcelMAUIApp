using System;
using System.Collections.Generic;
using System.Data;
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
                // Використовуємо HashSet для відстеження зациклень (наприклад, A1=B1, а B1=A1)
                var visitedCells = new HashSet<string>();
                string parsedExpression = ReplaceCellReferences(expression, visitedCells);

                // DataTable - простий спосіб порахувати математичний рядок у C#
                var dataTable = new DataTable();
                var result = dataTable.Compute(parsedExpression, "");

                return result.ToString();
            }
            catch
            {
                return "#ERROR";
            }
        }

        private string ReplaceCellReferences(string expression, HashSet<string> visitedCells)
        {
            // Шукаємо всі посилання формату 
            var regex = new Regex(@"[A-Z]+[0-9]+");

            return regex.Replace(expression, match =>
            {
                string cellName = match.Value;

                if (visitedCells.Contains(cellName))
                {
                    throw new Exception("Виявлено циклічне посилання");
                }
                visitedCells.Add(cellName);

                if (_formulas.ContainsKey(cellName))
                {
                    string cellValue = _formulas[cellName];

                    if (cellValue.StartsWith("="))
                    {
                        // Якщо клітинка, на яку ми посилаємось, теж є формулою - рахуємо її рекурсивно
                        return ReplaceCellReferences(cellValue.Substring(1), visitedCells);
                    }

                    // Якщо клітинка порожня, повертаємо 0 для математики
                    return string.IsNullOrEmpty(cellValue) ? "0" : cellValue;
                }

                return "0";
            });
        }
    }
}
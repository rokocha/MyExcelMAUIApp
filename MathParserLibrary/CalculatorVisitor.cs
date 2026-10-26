using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Globalization;

namespace MathParserLibrary
{
    public class CalculatorVisitor : CalculatorBaseVisitor<double>
    {
        private readonly Dictionary<string, double> _cellValues;

        public CalculatorVisitor(Dictionary<string, double> cellValues)
        {
            _cellValues = cellValues;
        }

        public override double VisitParentheses([NotNull] CalculatorParser.ParenthesesContext context)
        {
            return Visit(context.expr());
        }

        public override double VisitMultDiv([NotNull] CalculatorParser.MultDivContext context)
        {
            double left = Visit(context.expr(0));
            double right = Visit(context.expr(1));
            return context.op.Text == "*" ? left * right : left / right;
        }

        public override double VisitAddSub([NotNull] CalculatorParser.AddSubContext context)
        {
            double left = Visit(context.expr(0));
            double right = Visit(context.expr(1));
            return context.op.Text == "+" ? left + right : left - right;
        }

        public override double VisitNumber([NotNull] CalculatorParser.NumberContext context)
        {
            return double.Parse(context.NUMBER().GetText(), CultureInfo.InvariantCulture);
        }

        public override double VisitCellRef([NotNull] CalculatorParser.CellRefContext context)
        {
            string cellName = context.CELL().GetText();
            return _cellValues.TryGetValue(cellName, out double val) ? val : 0;
        }
    }
}
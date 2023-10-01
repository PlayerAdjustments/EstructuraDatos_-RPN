using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace EstructuraDatos__RPN
{
    internal class Program
    {
        const double TriggerGrowthPercentage = 1;
        const double GrowthPercentage = 1;
        private static void GrowConsole()
        {
            if (((double)Console.CursorTop) / Console.BufferHeight <= TriggerGrowthPercentage)
            {
                int growth = (int)(Console.BufferHeight * GrowthPercentage);
                growth = Math.Max(growth, 1); //Grow at least by 1;
                growth = Math.Min(growth, Int16.MaxValue - 1); //Don't grow bigger than Int16.MaxValue - 1;
                Console.BufferHeight = growth;
            }
        }
        static void Main(string[] args)
        {
            GrowConsole();


            RPN rpn = new RPN();

            List<string> operation = new List<string>() { "1", "2", "*", "2", "+", "3", "5", "*", "+" };
            List<string> notacionLista = new List<string>() { "(", "5", "+", "5", ")", "(", "55", "+", "5", ")" };

            Clear();

            BackgroundColor = ConsoleColor.DarkCyan;
            WriteLine($"************* Pruebas *************");
            WriteLine(rpn.InfixCalculator("1*2+2+3*5") + "\n");
            WriteLine(rpn.EvaluateRPN("1 2 * 2 + 3 5 * +") + "\n");
            WriteLine(rpn.EvaluateRPN(operation) + "\n");
            WriteLine(rpn.ConvertToRPN(notacionLista) + "\n");
            WriteLine(rpn.InfixCalculator(notacionLista) + "\n");

            WriteLine(rpn.InfixCalculator("(-5) -5"));
            BackgroundColor = ConsoleColor.DarkGreen;
            
            WriteLine($"************* Balanceador *************");
            BalancePrueba(rpn);

            BackgroundColor = ConsoleColor.DarkYellow;
            WriteLine($"\n************** Convertidor **************");
            ConvertirPrueba(rpn);

            BackgroundColor = ConsoleColor.DarkMagenta;
            WriteLine($"\n************** Calculadora **************");
            ResolverPrueba(rpn);
            
        }

        public static void BalancePrueba(RPN rpn)
        {
            WriteLine("----------------------------------------------------------------");

            string _operator = "{(5+5)(2-3)[3/2]}";
            WriteLine(_operator);
            bool result = rpn.CheckParentheses(_operator);
            WriteLine(result);

            WriteLine("----------------------------------------------------------------");

            _operator = "{(5+5](2-3)[3/2]}";
            WriteLine(_operator);
            result = rpn.CheckParentheses(_operator);
            WriteLine(result);

            WriteLine("----------------------------------------------------------------");

            _operator = "2[3+4]";
            WriteLine(_operator);
            result = rpn.CheckParentheses(_operator);
            WriteLine(rpn.CheckParentheses(_operator));

            WriteLine("----------------------------------------------------------------");

            _operator = "(2[3+4]";
            WriteLine(_operator);
            result = rpn.CheckParentheses(_operator);
            WriteLine(result);
        }

        public static void ConvertirPrueba(RPN rpn)
        {
            WriteLine("----------------------------------------------------------------");

            string expression = "{(5+5)(2-3)[3/2]}";
            WriteLine(expression);
            string result = rpn.ConvertToRPN(expression);
            WriteLine(result);

            WriteLine("----------------------------------------------------------------");

            try
            {
                expression = "{(5+5](2-3)*[3/2]}";
                WriteLine(expression);
                result = rpn.ConvertToRPN(expression);
                WriteLine(result);
            }catch(Exception ex)
            {
                WriteLine(ex.ToString());
            }
            

            WriteLine("----------------------------------------------------------------"); 

            expression = "(55+5)2-3";
            WriteLine(expression);
            result = rpn.ConvertToRPN(expression);
            WriteLine(result);

            WriteLine("----------------------------------------------------------------");

            expression = "(55+5)2/3";
            WriteLine(expression);
            result = rpn.ConvertToRPN(expression);
            WriteLine(result);

            WriteLine("----------------------------------------------------------------");

            expression = "(55+5)(5-3)";
            WriteLine(expression);
            result = rpn.ConvertToRPN(expression);
            WriteLine(result);

            WriteLine("----------------------------------------------------------------");
        }

        public static void ResolverPrueba(RPN rpn)
        {
            WriteLine("----------------------------------------------------------------");

            string expression = "{(5+5)(2-3)[3/2]}";
            WriteLine(expression);
            string result = rpn.InfixCalculator(expression);
            WriteLine(result);

            WriteLine("----------------------------------------------------------------");

            expression = "(15/3)4";
            WriteLine(expression);
            result = rpn.InfixCalculator(expression);
            WriteLine(result);

            WriteLine("----------------------------------------------------------------");

            expression = "4(15/3)+8";
            WriteLine(expression);
            result = rpn.InfixCalculator(expression);
            WriteLine(result);

            WriteLine("----------------------------------------------------------------");

            expression = "3^3+3";
            WriteLine(expression);
            result = rpn.InfixCalculator(expression);
            WriteLine(result);

            WriteLine("----------------------------------------------------------------");
        }
    }
}

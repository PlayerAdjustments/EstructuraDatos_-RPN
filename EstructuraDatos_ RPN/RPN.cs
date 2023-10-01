using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Text.RegularExpressions;
//using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static System.Console;

namespace EstructuraDatos__RPN
{
    public class RPN
    {
        private ArrayStack<string> data = new ArrayStack<string>();
        //Puts operators and their priority value
        private Dictionary<string,int> operators = new Dictionary<string, int>()
        {
            {"^",3},{"*",2},{"/",2},{"+",1},{"-",1}
        };

        //In case of having negatives like -5-5, we could take the first -5 as a whole number since no operator is following it
        private Dictionary<string, string> negativeSymbols = new Dictionary<string, string>()
        {
            {"!","-1"}, {"!!","-2"}, {"!!!" , "-3"}, {"!!!!" , "-4"}, {"!!!!!" , "-5"}, {"!!!!!!" , "-6"}, {"!!!!!!!" , "-7"}, {"!!!!!!!!" , "-8"}, {"!!!!!!!!!" , "-9"}, {"~!" , "-0"},
        };
        
        /// <summary>
        /// Revisa si los parentesis estan balanceados.
        /// </summary>
        /// <param name="notacion"></param>
        /// <returns>
        /// True. Si estan balanceados
        /// False. Si no estan balanceados
        /// </returns>
        public bool CheckParentheses(string notacion)
        {
            data.Clear();
            foreach (char c in notacion)
            {
                if(c == '(' || c == '[' || c == '{')
                {
                    data.Push($"{c}");
                }

                if(c == ')')
                {
                    

                    if (data.Peek() != "(") { return false; } 

                    data.Pop();
                }

                if (c == ']')
                {
                    
                    if (data.Peek() != "[") { return false; }

                    data.Pop();
                }

                if (c == '}')
                {

                    if (data.Peek() != "{") { return false; }

                    data.Pop();
                }

            }

            if (!data.Empty) { return false; }

            return true;
        }

        /// <summary>
        /// Dada la notación, reemplaza llaves y corchetes por sus respectivos parentesis.
        /// </summary>
        /// <param name="notacion"></param>
        /// <returns>string con llaves y corchetes reemplazadas</returns>
        public string adaptString (string notacion)
        {
            string result = "";
            foreach (char c in notacion)
            {
                if(c == '[' || c == '{'){ result += '('; }
                if(c == ']' || c == '}'){ result += ')'; }

                if(c != '[' && c != '{' && c != ']' && c != '}' && c != ' ') result += c;
            }


            //WriteLine(result);
            return parenthesisMultiplication(result);
        }

        /// <summary>
        /// Revisa por casos donde se puedan usar negativos siendo 3 casos:
        /// 1. Incio del string => "-1+2"
        /// 2. Despues de un parentesis abierto => "(-1*2)"
        /// 3. En caso de que sea un numero mayor a 1 digito "-90"
        /// </summary>
        /// <param name="notacion"></param>
        /// <returns>string convertido usando ! para señalar la cantidad del primer digito del negativo</returns>
        public string detectNegatives(string notacion)
        {
            //Checks for cases like where the negative is at the start of the string: "-1*2" => "!*2"
            string[] patterns = { @"(^\-[0])", @"(^\-[1])", @"(^\-[2])", @"(^\-[3])", @"(^\-[4])", @"(^\-[5])", @"(^\-[6])", @"(^\-[7])", @"(^\-[8])", @"(^\-[9])" };
            string result = "";
            string[] replacements = { "~!", "!", "!!", "!!!", "!!!!", "!!!!!", "!!!!!!", "!!!!!!!", "!!!!!!!!", "!!!!!!!!!" };

            for (int i = 0; i < patterns.Length; i++)
            {
                result = Regex.Replace(notacion, patterns[i], replacements[i]);
                notacion = result;
                //WriteLine(result);
            }

            //Checks for cases like where the negative is at the start of a parenthesis: "(-1*2)" => "(!*2)"
            string[] nextPatterns = { @"\(\-0", @"\(\-1", @"\(\-2", @"\(\-3", @"\(\-4", @"\(\-5", @"\(\-6", @"\(\-7", @"\(\-8", @"\(\-9" };
            string[] nextReplacements = { "(~!", "(!", "(!!", "(!!!", "(!!!!", "(!!!!!", "(!!!!!!", "(!!!!!!", "(!!!!!!!!", "(!!!!!!!!!" };

            for (int i = 0; i < nextPatterns.Length; i++)
            {
                result = Regex.Replace(notacion, nextPatterns[i], nextReplacements[i]);
                notacion = result;
            }

            //Checks for cases where the negative have another operator like: "5(2/-3)" => "5(2/!!!)"
            string[] lastPatterns = { @"([*\^\/]|(?<!E)[\+\-])\-0", @"([*\^\/]|(?<!E)[\+\-])\-1", @"([*\^\/]|(?<!E)[\+\-])\-2", @"([*\^\/]|(?<!E)[\+\-])\-3", @"([*\^\/]|(?<!E)[\+\-])\-4", @"([*\^\/]|(?<!E)[\+\-])\-5", @"([*\^\/]|(?<!E)[\+\-])\-6", @"([*\^\/]|(?<!E)[\+\-])\-7", @"([*\^\/]|(?<!E)[\+\-])\-8", @"([*\^\/]|(?<!E)[\+\-])\-9" };
            string[] lastReplacements = { "$1~!", "$1!", "$1!!", "$1!!!", "$1!!!!", "$1!!!!!", "$1!!!!!!", "$1!!!!!!", "$1!!!!!!!!", "$1!!!!!!!!!" };

            for (int i = 0; i < lastPatterns.Length; i++)
            {
                result = Regex.Replace(notacion, lastPatterns[i], lastReplacements[i]);
                notacion = result;
            }

            return result;
        }

        /// <summary>
        ///  Mediante el uso de Regular Expression (REGEX) separa los operadores matematicos y parentesis. Introduciendolos en una lista.
        /// </summary>
        /// <param name="notacion"></param>
        /// <returns>Lista de elementos que conforman la notacion dada.</returns>
        public List<string> stringToList(string notacion)
        {
            /*Using regular expresions. 
                Gets a patterns and searches in a text.
                https://www.codesmith.io/blog/understanding-regex#:~:text=A%20Regex%2C%20or%20regular%20expression,to%20find%20what%20you%20need.
                Please refer out pattern "([*()\^\/]|(?<!E)[\+\-])" into:
                https://www.regextester.com
            */
            string pattern = @"([*()\^\/]|(?<!E)[\+\-])";

            string[] substrings = Regex.Split(notacion, pattern);

            string correspondentValue = "";

            List<string> notation = new List<string>();

            foreach (string substring in substrings)
            {
                if (!(string.IsNullOrEmpty(substring) || string.IsNullOrWhiteSpace(substring)))
                {
                    //Check if the substring is a negative of more than one digit ex. -90
                    if (Regex.IsMatch(substring, @"\!\d"))
                    {
                        char[] chars = substring.ToCharArray();
                        string symboledNumber = ""; string realNumber = ""; string holder = "";
                        foreach (char c in chars)
                        {
                            if (c == '!' || c == '~')
                            {
                                symboledNumber += c;
                            }
                            else
                            {
                                holder += c;
                            }
                        }

                        if (negativeSymbols.TryGetValue(symboledNumber, out correspondentValue))
                        {
                            realNumber = correspondentValue + holder;
                        }

                        notation.Add(realNumber);
                    } 
                    //Then it could be a number or operator
                    else if (!negativeSymbols.TryGetValue(substring.Trim(), out correspondentValue))
                    {
                        notation.Add(substring);
                    }
                    //It can just be a single digit negative ex. -1
                    else
                    {
                        notation.Add(correspondentValue);
                    }
                }
            }

            //foreach (string substring in notation) { Console.Write($"{substring} ");}
            
            return notation;
        }

        /// <summary>
        /// Usa Regular Expression (REGEX) para identificar casos de uso de parentesis como multiplicacion. Reemplazando el string agregando los * donde correspondan.
        /// </summary>
        /// <param name="notacion"></param>
        /// <returns>string modificado con los * agregados donde corresponden.</returns>
        public string parenthesisMultiplication(string notacion)
        {
            string[] patterns = { @"(\)\()", @"(\d)\(", @"\)(\d)" };
            string[] replacements = { ")*(", "$1*(", ")*$1" };
            string result = "";

            for(int i = 0; i < patterns.Length; i++)
            {
                result = Regex.Replace(notacion, patterns[i], replacements[i]);
                notacion = result;
                //WriteLine(result);
            }

            return detectNegatives(result);
        }

        /// <summary>
        /// Usa el algoritmo especificado en clase para tranformar la notacion infija en postfija.
        /// </summary>
        /// <param name="notacion"></param>
        /// <returns>string convertido de Infijo a Postfijo</returns>
        /// <exception cref="ArgumentException"></exception>
        public string ConvertToRPN(string notacion)
        {
            decimal value;
            if (!CheckParentheses(notacion)) throw new ArgumentException("Invalid parenthesis input");

            data.Clear();
            string adaptedstring = adaptString(notacion);
            List<string> notationToList = stringToList(adaptedstring);

            string ToRPN = ""; int currentPriority; int nextPriority;

            foreach (string s in notationToList)
            {
                //WriteLine(data.GetDataText());
                if (Decimal.TryParse(s, out value))
                {
                    //WriteLine($"{s} is Numeric");

                    ToRPN += $"{s} ";
                    //WriteLine($"RPN: {ToRPN}");
                }

                else if (s == ")")
                {
                    //WriteLine($"{s} is )");
                    bool foundLeftParenthesis = false;
                    while (!foundLeftParenthesis)
                    {
                        if (data.Empty) break;

                        string currentPop = data.Pop();

                        if (currentPop != "(") { ToRPN += $"{currentPop} "; }

                        else if (currentPop == "(") { foundLeftParenthesis = true; }
                    }
                    //WriteLine($"RPN: {ToRPN}");
                }

                else if (s == "(")
                {
                    data.Push(s);
                }

                else
                {
                    bool Controller = false;
                    //WriteLine($"{s} is operator");

                    while (!Controller)
                    {
                        if (data.Empty || data.Peek() == "(")
                        {
                            data.Push(s);
                            //WriteLine(data.GetDataText());
                            Controller = true;
                        }
                        else
                        {
                            operators.TryGetValue(s, out currentPriority);
                            operators.TryGetValue(data.Peek(), out nextPriority);
                            if (currentPriority > nextPriority)
                            {
                                data.Push(s);
                                Controller = true;
                            }
                            else
                            {
                                string currentPop = data.Pop();
                                if (currentPop != "(")
                                {
                                    ToRPN += $"{currentPop} ";
                                }
                            }
                        }
                    }
                }
            }

            while (!data.Empty)
            {
                //WriteLine($"RPN: {ToRPN}");
                ToRPN += $"{data.Pop()} ";
                //WriteLine($"RPN: {ToRPN}");
            }

            WriteLine(ToRPN);
            return ToRPN;
        }

        /// <summary>
        /// Usa el algoritmo especificado en clase para transformar una operacion infija dada en una lista a una postfija.
        /// </summary>
        /// <param name="notacionLista"></param>
        /// <returns> string resultado</returns>
        /// <exception cref="ArgumentException"></exception>
        public string ConvertToRPN(List<string> notacionLista)
        {
            if (notacionLista.Count(s => s != null || s != " " || s!= "") <= 0) throw new ArgumentNullException("Cannot have empty notation");


            string notacion = ""; decimal value;

            for (int i = 0; i < notacionLista.Count; i++)
            {
                if(i != notacionLista.Count - 1)
                {
                    notacion += $"{notacionLista[i]} ";
                }
                else
                {
                    notacion += notacionLista[i];
                }
            }

            if (!CheckParentheses(notacion)) throw new ArgumentException("Invalid parenthesis input");

            data.Clear();
            string adaptedstring = adaptString(notacion);
            List<string> notationToList = stringToList(adaptedstring);

            string ToRPN = ""; int currentPriority; int nextPriority;

            foreach (string s in notationToList)
            {
                //WriteLine(data.GetDataText());
                if (Decimal.TryParse(s, out value))
                {
                    //WriteLine($"{s} is Numeric");

                    ToRPN += $"{s} ";
                    //WriteLine($"RPN: {ToRPN}");
                }

                else if (s == ")")
                {
                    //WriteLine($"{s} is )");
                    bool foundLeftParenthesis = false;
                    while (!foundLeftParenthesis)
                    {
                        if (data.Empty) break;

                        string currentPop = data.Pop();

                        if (currentPop != "(") { ToRPN += $"{currentPop} "; }

                        else if (currentPop == "(") { foundLeftParenthesis = true; }
                    }
                    //WriteLine($"RPN: {ToRPN}");
                }

                else if (s == "(")
                {
                    data.Push(s);
                }

                else
                {
                    bool Controller = false;
                    //WriteLine($"{s} is operator");

                    while (!Controller)
                    {
                        if (data.Empty || data.Peek() == "(")
                        {
                            data.Push(s);
                            //WriteLine(data.GetDataText());
                            Controller = true;
                        }
                        else
                        {
                            operators.TryGetValue(s, out currentPriority);
                            operators.TryGetValue(data.Peek(), out nextPriority);
                            if (currentPriority > nextPriority)
                            {
                                data.Push(s);
                                Controller = true;
                            }
                            else
                            {
                                string currentPop = data.Pop();
                                if (currentPop != "(")
                                {
                                    ToRPN += $"{currentPop} ";
                                }
                            }
                        }
                    }
                }
            }

            while (!data.Empty)
            {
                //WriteLine($"RPN: {ToRPN}");
                ToRPN += $"{data.Pop()} ";
                //WriteLine($"RPN: {ToRPN}");
            }

            WriteLine(ToRPN);
            return ToRPN;
        }

        /// <summary>
        /// Calcula la operacion infija dada mediate el metodo postfijo
        /// </summary>
        /// <param name="operation"></param>
        /// <returns>string result</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string InfixCalculator(string operation)
        {
            if (string.IsNullOrEmpty(operation) || string.IsNullOrWhiteSpace(operation)) { throw new ArgumentNullException("Cannot have empty operations."); }

            double x = 0; double y = 0; double result = 0; 

            string RPNOperation = ConvertToRPN(operation);

            string[] tempArray = RPNOperation.Split(' ');
            string[] notacion = new string[tempArray.Length];

            Array.Copy(tempArray,notacion,tempArray.Length-1);

            while (notacion.Count(s => s!=null) > 1)
            {
                Array.Clear(tempArray, 0, tempArray.Length);
                //WriteLine(notacion[0]);
                for(int i = 0; i < notacion.Length; i++)
                {
                    if (notacion.Count(s => s != null) == 1) break;
                    if (operators.ContainsKey(notacion[i]))
                    {
                        x = double.Parse(notacion[i - 2]);
                        y = double.Parse(notacion[i - 1]);

                        if (notacion[i] == "+") result = x + y;
                        if (notacion[i] == "-") result = x - y;
                        if (notacion[i] == "*") result = x * y;
                        if (notacion[i] == "/") result = x / y;
                        if (notacion[i] == "^") result = Math.Pow(x,y);

                        Array.Copy(notacion, tempArray, i - 2);

                        //Gets the number of elements in the array not counting null values
                        int lastIndex = tempArray.Count(s => s!=null);
                        tempArray[lastIndex] = $"{result}";

                        int notacionCounter = 0;
                        for(int z = i; z < notacion.Length; z++) { if (notacion[z] != null) notacionCounter++; }
                        Array.Copy(notacion, i+1, tempArray, lastIndex+1, notacionCounter-1);
                        //WriteLine("\nTempArray");
                        //WriteLine("[{0}]", string.Join(", ",tempArray));

                        //WriteLine($"\n{tempArray.Length}\nNotacionArray");
                        //WriteLine("[{0}]", string.Join(", ", tempArray));

                        Array.Copy(tempArray,notacion,tempArray.Length);
                        //WriteLine("\nNotacionArray");
                        //WriteLine("[{0}]", string.Join(", ", tempArray));
                        break;
                    }
                }

            }

            if (string.IsNullOrEmpty(notacion[0]) || string.IsNullOrWhiteSpace(notacion[0])) { throw new ArgumentNullException("Cannot have empty operations."); }

            return notacion[0];
        }

        /// <summary>
        /// Calcula la operacion infija dada en forma de lista mediante el metodod postfijo.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns>string resultado</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string InfixCalculator(List<string> operation)
        {
            if (operation.Count(s=> s!=null || s!=" ") <= 0) { throw new ArgumentNullException("Cannot have empty operations."); }

            double x = 0; double y = 0; double result = 0;

            string RPNOperation = ConvertToRPN(operation);

            string[] tempArray = RPNOperation.Split(' ');
            string[] notacion = new string[tempArray.Length];

            Array.Copy(tempArray, notacion, tempArray.Length - 1);

            while (notacion.Count(s => s != null) > 1)
            {
                Array.Clear(tempArray, 0, tempArray.Length);
                //WriteLine(notacion[0]);
                for (int i = 0; i < notacion.Length; i++)
                {
                    if (notacion.Count(s => s != null) == 1) break;
                    if (operators.ContainsKey(notacion[i]))
                    {
                        x = double.Parse(notacion[i - 2]);
                        y = double.Parse(notacion[i - 1]);

                        if (notacion[i] == "+") result = x + y;
                        if (notacion[i] == "-") result = x - y;
                        if (notacion[i] == "*") result = x * y;
                        if (notacion[i] == "/") result = x / y;
                        if (notacion[i] == "^") result = Math.Pow(x, y);

                        Array.Copy(notacion, tempArray, i - 2);

                        //Gets the number of elements in the array not counting null values
                        int lastIndex = tempArray.Count(s => s != null);
                        tempArray[lastIndex] = $"{result}";

                        int notacionCounter = 0;
                        for (int z = i; z < notacion.Length; z++) { if (notacion[z] != null) notacionCounter++; }
                        Array.Copy(notacion, i + 1, tempArray, lastIndex + 1, notacionCounter - 1);
                        //WriteLine("\nTempArray");
                        //WriteLine("[{0}]", string.Join(", ",tempArray));

                        //WriteLine($"\n{tempArray.Length}\nNotacionArray");
                        //WriteLine("[{0}]", string.Join(", ", tempArray));

                        Array.Copy(tempArray, notacion, tempArray.Length);
                        //WriteLine("\nNotacionArray");
                        //WriteLine("[{0}]", string.Join(", ", tempArray));
                        break;
                    }
                }

            }

            if (string.IsNullOrEmpty(notacion[0]) || string.IsNullOrWhiteSpace(notacion[0])) { throw new ArgumentNullException("Cannot have empty operations."); }

            return notacion[0];
        }

        /// <summary>
        /// Dada un string con los elementos separados por espacios, resuelve la operacion en el string.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns>string result</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string EvaluateRPN(string operation)
        {
            if (string.IsNullOrEmpty(operation) || string.IsNullOrWhiteSpace(operation)) { throw new ArgumentNullException("Cannot have empty operations."); }

            double x = 0; double y = 0; double result = 0;

            string[] tempArray = operation.Split(' ');
            string[] notacion = new string[tempArray.Length];

            Array.Copy(tempArray, notacion, tempArray.Length);

            while (notacion.Count(s => s != null) > 1)
            {
                Array.Clear(tempArray, 0, tempArray.Length);
                //WriteLine(notacion[0]);
                for (int i = 0; i < notacion.Length; i++)
                {
                    if (notacion.Count(s => s != null) == 1) break;
                    if (operators.ContainsKey(notacion[i]))
                    {
                        x = double.Parse(notacion[i - 2]);
                        y = double.Parse(notacion[i - 1]);

                        if (notacion[i] == "+") result = x + y;
                        if (notacion[i] == "-") result = x - y;
                        if (notacion[i] == "*") result = x * y;
                        if (notacion[i] == "/") result = x / y;
                        if (notacion[i] == "^") result = Math.Pow(x, y);

                        Array.Copy(notacion, tempArray, i - 2);

                        //Gets the number of elements in the array not counting null values
                        int lastIndex = tempArray.Count(s => s != null);
                        tempArray[lastIndex] = $"{result}";

                        int notacionCounter = 0;
                        for (int z = i; z < notacion.Length; z++) { if (notacion[z] != null) notacionCounter++; }
                        Array.Copy(notacion, i + 1, tempArray, lastIndex + 1, notacionCounter - 1);
                        //WriteLine("\nTempArray");
                        //WriteLine("[{0}]", string.Join(", ",tempArray));

                        Array.Copy(tempArray, notacion, tempArray.Length);
                        //WriteLine("\nNotacionArray");
                        //WriteLine("[{0}]", string.Join(", ", tempArray));
                        break;
                    }
                }

            }

            if (string.IsNullOrEmpty(notacion[0]) || string.IsNullOrWhiteSpace(notacion[0])) { throw new ArgumentNullException("Cannot have empty operations."); }

            return notacion[0];

        }

        /// <summary>
        /// Dada una lista de strings con una operación postfija, calcula el resultado.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns>string resultado</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string EvaluateRPN(List<string> operation)
        {
            if (operation.Count(s => s!=null || s!= " ") == 0) { throw new ArgumentNullException("Cannot have empty operations."); }

            double x = 0; double y = 0; double result = 0;

            string[] tempArray = new string[operation.Count()];
            string[] notacion = new string[tempArray.Length];

            Array.Copy(operation.ToArray(), tempArray, operation.Count());
            Array.Copy(tempArray, notacion, tempArray.Length);

            while (notacion.Count(s => s != null) > 1)
            {
                Array.Clear(tempArray, 0, tempArray.Length);
                //WriteLine(notacion[0]);
                for (int i = 0; i < notacion.Length; i++)
                {
                    if (notacion.Count(s => s != null) == 1) break;
                    if (operators.ContainsKey(notacion[i]))
                    {
                        x = double.Parse(notacion[i - 2]);
                        y = double.Parse(notacion[i - 1]);

                        if (notacion[i] == "+") result = x + y;
                        if (notacion[i] == "-") result = x - y;
                        if (notacion[i] == "*") result = x * y;
                        if (notacion[i] == "/") result = x / y;
                        if (notacion[i] == "^") result = Math.Pow(x, y);

                        Array.Copy(notacion, tempArray, i - 2);

                        //Gets the number of elements in the array not counting null values
                        int lastIndex = tempArray.Count(s => s != null);
                        tempArray[lastIndex] = $"{result}";

                        int notacionCounter = 0;
                        for (int z = i; z < notacion.Length; z++) { if (notacion[z] != null) notacionCounter++; }
                        Array.Copy(notacion, i + 1, tempArray, lastIndex + 1, notacionCounter - 1);
                        //WriteLine("\nTempArray");
                        //WriteLine("[{0}]", string.Join(", ",tempArray));

                        Array.Copy(tempArray, notacion, tempArray.Length);
                        //WriteLine("\nNotacionArray");
                        //WriteLine("[{0}]", string.Join(", ", tempArray));
                        break;
                    }
                }

            }

            if (string.IsNullOrEmpty(notacion[0]) || string.IsNullOrWhiteSpace(notacion[0])) { throw new ArgumentNullException("Cannot have empty operations."); }

            return notacion[0];

        }

    }
}

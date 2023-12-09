using static System.Runtime.InteropServices.JavaScript.JSType;

using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;

namespace Calculator;

public class Calculator
{
    private string? error;
    public string? Error { get => error; }

    public readonly CompiledCode compiledCode;
    
    public Calculator(CompiledCode compiledCode)
    {
        this.compiledCode = compiledCode;
    }

    public double Compute(string source)
    {
        return 0;
    }

    public Token Compute()
    {

        if (compiledCode?.tokens == null)
            return null;

        Stack<Token> operands = new();
        Stack<string> operators = new();

        string suspect = "";
        int i = 0;
        while (i < compiledCode.tokens.Count)
        {
            Token token = compiledCode.tokens[i];
            if (token == null)
                return null;

            if (token.Type == TokenType.Goto)
            {
                i = (token as TokenGoto).ToToken;
                continue;
            }

            if (token.Type == TokenType.Operation)
            {
                suspect = (token as TokenOperation).Operation;
            }

            if (suspect == "(")
            {
                operators.Push(suspect);
            }
            else if (token.Type == TokenType.Constant)
            {
                operands.Push(token);
            }
            else if (suspect == ")")
            {
                while (operators.Count != 0 && operators.Peek() != "(")
                {
                    var val2 = operands.Pop();

                    var val1 = operands.Pop();

                    var op = operators.Pop();

                    operands.Push(Operate(val1, val2, op));
                }

                if (operators.Count != 0) operators.Pop();
            }
            else
            {
                var currentOperatorPriority = GetOperationPriority(suspect);
                
                while (operators.Count != 0 && GetOperationPriority(operators.Peek()) >= currentOperatorPriority)
                {
                    var val2 = operands.Pop();

                    var val1 = operands.Pop();

                    string op = operators.Pop();

                    operands.Push(Operate(val1, val2, op));
                }

                operators.Push(suspect);
            }
            i++;
        }

        while (operators.Count != 0 && operands.Count > 1)
        {
            var val2 = operands.Pop();

            var val1 = operands.Pop();

            string op = operators.Pop();

            operands.Push(Operate(val1, val2, op));
        }

        if (operators.Count != 0)
        {
            string op = operators.Pop();
            throw new InvalidOperationException($"operators stack is not empty at the end: {op}");
            //return double.NaN;
        }

        Token tokenretval = operands.Pop();

        if (operands.Count > 1)
        {
            //double op = operands.Pop();
            throw new InvalidOperationException($"operands stack is not empty at the end: ((op))");
            //return double.NaN;
        }

        return tokenretval;
    }

    private static TokenConstant Operate(Token left, Token right, string operation)
    {
        //if (left.Type == TokenType.Constant && right.Type == TokenType.Constant)
        //{
            ExpressionType type1 = ExpressionType.Undefined, type2 = ExpressionType.Undefined;
            int i1 = 0, i2 = 0;
            double d1 = 0, d2 = 0;
            string s1 = "", s2 = "";
            bool b1 = false, b2 = false;

            if (left is TokenInt)
            {
                type1 = ExpressionType.Int;
                i1 = (left as TokenInt).value;
            }
            else if (left is TokenDouble) {
                type1 = ExpressionType.Double;
                d1 = (left as TokenDouble).value;
            }
            else if (left is TokenString) {
                type1 = ExpressionType.Str;
                s1 = (left as TokenString).value;
            }
            else if (left is TokenBool) {
                type1 = ExpressionType.Bool;
                b1 = (left as TokenBool).value;
            }

            if (right is TokenInt)
            {
                type2 = ExpressionType.Int;
                i2 = (right as TokenInt).value;
            }
            else if (right is TokenDouble) {
                type2 = ExpressionType.Double;
                d2 = (right as TokenDouble).value;
            }
            else if (right is TokenString) {
                type2 = ExpressionType.Str;
                s2 = (right as TokenString).value;
            }
            else if (right is TokenBool) {
                type2 = ExpressionType.Bool;
                b2 = (right as TokenBool).value;
            }

            var resultType = TypeResolver.ResultingOperationType(operation, type1, type2);
            if (resultType == ExpressionType.Undefined)
                    throw new InvalidOperationException($"Incompatible types: {operation} {type1} {type2} ");

            int intRes = 0;
            double doubleRes = 0;
            string stringRes = ""; 
            bool boolRes = false;
            bool err = false;

            if ((type1 == ExpressionType.Double) || (type2 == ExpressionType.Double))
            {
                if (type1 == ExpressionType.Int)
                {
                    d1 = i1;
                }
                else if (type2 == ExpressionType.Int)
                {
                    d2 = i2;
                }
            }

            if (resultType == ExpressionType.Bool)
            {
                if (operation == "!" && type1 == ExpressionType.Bool)
                    boolRes = !b1;
                else if (type1 == type2 && type1 == ExpressionType.Bool)
                {
                    if (operation == "&&") boolRes = b1 && b2;
                    else if (operation == "||") boolRes = b1 || b2;
                    else err = true;
                }
                else if (type1 == type2 && type1 == ExpressionType.Int)
                {
                    if (operation == "==") boolRes = i1 == i2;
                    else if (operation == "!=") boolRes = i1 != i2;
                    else if (operation == "<=") boolRes = i1 <= i2;
                    else if (operation == ">=") boolRes = i1 >= i2;
                    else if (operation == "<") boolRes = i1 < i2;
                    else if (operation == ">") boolRes = i1 > i2;
                    else err = true;
                }
                else if ((type1 == ExpressionType.Double) || (type2 == ExpressionType.Double))
                {
                    if (operation == "==") boolRes = d1 == d2;
                    else if (operation == "!=") boolRes = d1 != d2;
                    else if (operation == "<=") boolRes = d1 <= d2;
                    else if (operation == ">=") boolRes = d1 >= d2;
                    else if (operation == "<") boolRes = d1 < d2;
                    else if (operation == ">") boolRes = d1 > d2;
                    else err = true;
                }
                else if (type1 == type2 && type1 == ExpressionType.Str)
                {
                    if (operation == "==") boolRes = s1 == s2;
                    else if (operation == "!=") boolRes = s1 != s2;
                    else if (operation == "<=") boolRes = s1.CompareTo(s2) <= 0;
                    else if (operation == ">=") boolRes = s1.CompareTo(s2) >= 0;
                    else if (operation == "<") boolRes = s1.CompareTo(s2) < 0;
                    else if (operation == ">") boolRes = s1.CompareTo(s2) > 0;
                    else err = true;
                }
                else
                {
                    err = true;
                }
            }
            else if (resultType == ExpressionType.Int)
            {
                if (type1 == type2 && type1 == ExpressionType.Int)
                {
                    if (operation == "+") intRes = i1 + i2;
                    else if (operation == "-") intRes = i1 - i2;
                    else if (operation == "*") intRes = i1 * i2;
                    else if (operation == "/") intRes = i1 / i2;
                    else if (operation == "%") intRes = i1 % i2;
                    else err = true;
                }
            }
            else if (resultType == ExpressionType.Double)
            {
                if (operation == "+") doubleRes = d1 + d2;
                else if (operation == "-") doubleRes = d1 - d2;
                else if (operation == "*") doubleRes = d1 * d2;
                else if (operation == "/") doubleRes = d1 / d2;
                else err = true;
            }
            else if (resultType == ExpressionType.Str)
            {
                if (type1 == type2 && type1 == ExpressionType.Str)
                    if (operation == "+") stringRes = s1 + s2;
                    else err = true;
            }
            else
            {
                err = true;
            }

            if (err)
            {
                throw new InvalidOperationException($"Invalid operation: {operation} {type1} {type2} ");
            }

            TokenConstant resToken;

            if (resultType == ExpressionType.Bool)
                resToken = new TokenBool(boolRes);
            else if (resultType == ExpressionType.Int)
                resToken = new TokenInt(intRes);
            else if (resultType == ExpressionType.Double)
                resToken = new TokenDouble(doubleRes);
            else if (resultType == ExpressionType.Str)
                resToken = new TokenString(stringRes);
            else
            {
                throw new InvalidOperationException($"Invalid operation result type: {operation} {type1} {type2} {resultType} ");
            }

            return resToken;
        }


        /******
        public double ComputeString(string source)
        {
            Stack<double> operands = new();
            Stack<char> operators = new();

            for (var i = 0; i < source.Length; i++)
            {
                char suspect = source[i];
                if (suspect == ' ') continue;

                if (suspect == '(')
                {
                    operators.Push(suspect);
                }
                else if (IsDigit(suspect))
                {
                    double value = 0;

                    while (i < source.Length && IsDigit(source[i]))
                    {
                        value = value * 10 + (source[i] - '0');
                        i++;
                    }

                    operands.Push(value);
                    i--;
                }
                else if (suspect == ')')
                {
                    while (operators.Count != 0 && operators.Peek() != '(')
                    {
                        var val2 = operands.Pop();

                        var val1 = operands.Pop();

                        var op = operators.Pop();

                        operands.Push(Operate(val1, val2, op));
                    }

                    if (operators.Count != 0) operators.Pop();
                }
                else
                {
                    var currentOperatorPriority = GetOperationPriority(suspect);

                    while (operators.Count != 0 && GetOperationPriority(operators.Peek()) >= currentOperatorPriority)
                    {
                        var val2 = operands.Pop();

                        var val1 = operands.Pop();

                        char op = operators.Pop();

                        operands.Push(Operate(val1, val2, op));
                    }

                    operators.Push(suspect);
                }
            }


            while (operators.Count != 0 && operands.Count > 1)
            {
                var val2 = operands.Pop();

                var val1 = operands.Pop();

                char op = operators.Pop();

                operands.Push(Operate(val1, val2, op));
            }

            if (operators.Count != 0)
            {
                char op = operators.Pop();
                throw new InvalidOperationException($"operators stack is not empty at the end: {op}");
                //return double.NaN;
            }

            if (operands.Count > 1)
            {
                double op = operands.Pop();
                throw new InvalidOperationException($"operands stack is not empty at the end: {op}");
                //return double.NaN;
            }

            return operands.Pop();
        }

         private static double Operate(double left, double right, string op) =>
            op switch
            {
                "+" => left + right,
                "-" => left - right,
                "*" => left * right,
                "/" => right switch
                {
                    0 => throw new DivideByZeroException(),
                    _ => left / right
                },
                _ => throw new InvalidOperationException($"Invalid operator: {op}")
            };
         ***********/

        private static int GetOperationPriority(string operation) =>
        operation switch
        {
            "+" or "-" => 1,
            "*" or "/" => 2,
            _ => 0
        };

    /// <summary>
    /// Check if the given char is digit
    /// </summary>
    /// <param name="char">char to be determined</param>
    /// <returns>true if the given char is digit, otherwise false</returns>
    //private static bool IsDigit(char @char) => @char is >= '0' and <= '9';
}
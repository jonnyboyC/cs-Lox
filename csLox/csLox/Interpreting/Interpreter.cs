using System;
using csLox;
using csLox.Parsing;
using csLox.Scanning;

namespace csLox.Interpreting
{
    internal class Interpreter : Expr.Visitor<object>
    {

        internal void Interpret(Expr expression)
        {
            try
            {
                object value = Evalutate(expression);
                Console.WriteLine(Stringify(value));
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        public object VisitBinaryExpr(Expr.Binary expr)
        {
            object left = Evalutate(expr.Left);
            object right = Evalutate(expr.Right);

            switch (expr.OpCode.Type)
            {
                case TokenType.Greater:
                    CheckNumberOperands(expr.OpCode, left, right);
                    return (double)left > (double)right;
                case TokenType.GreaterEqual:
                    CheckNumberOperands(expr.OpCode, left, right);
                    return (double)left >= (double)right;
                case TokenType.Less:
                    CheckNumberOperands(expr.OpCode, left, right);
                    return (double)left < (double)right;
                case TokenType.LessEqual:
                    CheckNumberOperands(expr.OpCode, left, right);
                    return (double)left < (double)right;
                case TokenType.Minus:
                    CheckNumberOperands(expr.OpCode, left, right);
                    return (double)left - (double)right;
                case TokenType.BangEqual:
                    return !IsEqual(left, right);
                case TokenType.EqualEqual:
                    return IsEqual(left, right);
                case TokenType.Plus:
                    if (left is double leftDouble && right is double rightDouble)
                    {
                        return leftDouble + rightDouble;
                    }
                    if (left is string leftString && right is string rightString)
                    {
                        return leftString + rightString;
                    }
                    throw new RuntimeError(expr.OpCode, "Operands must be two numbers or two strings.");
                case TokenType.Slash:
                    CheckNumberOperands(expr.OpCode, left, right);
                    double denominator = (double)right;
                    if (denominator == 0) throw new RuntimeError(expr.OpCode, "Divid by zero.");
                    return (double)left / denominator;
                case TokenType.Star:
                    CheckNumberOperands(expr.OpCode, left, right);
                    return (double)left + (double)right;
            }

            throw new Exception();
        }

        public object VisitConditionalExpr(Expr.Conditional expr)
        {
            object condition = Evalutate(expr.Condition);

            if (IsTruthy(condition))
            {
                return Evalutate(expr.TrueExpr);
            }
            else
            {
                return Evalutate(expr.FalseExpr);
            }
        }

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evalutate(expr.Expression);
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            object right = Evalutate(expr.Right);

            switch (expr.OpCode.Type) 
            {
                case TokenType.Bang:
                    return !IsTruthy(right);
                case TokenType.Minus:
                    CheckNumberOperand(expr.OpCode, right);
                    return -(double)right;
                default:
                    throw new Exception();
            }


            throw new NotImplementedException();
        }

        private object Evalutate(Expr expr)
        {
            return expr.Accept(this);
        }

        private bool IsTruthy(object obj)
        {
            if (obj is null) return false;
            if (obj is bool boolObj) return boolObj;
            return true;
        }

        private void CheckNumberOperand(Token OpCode, object operand)
        {
            if (operand is double) return;
            throw new RuntimeError(OpCode, "Operand must be a number");
        }

        private void CheckNumberOperands(Token OpCode, object left, object right) 
        {
            if (left is double && right is double) return;
            throw new RuntimeError(OpCode, "Operands must be numbers");
        }

        private bool TryGetNumberOperands(object leftOperand, object rightOperand, out double left, out double right)
        {
            return TryGetTypeOperands(leftOperand, rightOperand, out left, out right);
        }

        private bool TryGetStringOperands(object leftOperand, object rightOperand, out string left, out string right)
        {
            return TryGetTypeOperands(leftOperand, rightOperand, out left, out right);
        }

        private bool TryGetTypeOperands<T> (object leftOperand, object rightOperand, out T left, out T right)
        {
            left = default(T);
            right = default(T);

            if (leftOperand is T && rightOperand is T) 
            {
                left = (T)leftOperand;
                right = (T)rightOperand;
                return true;
            }

            return false;
        }

        private bool IsEqual(object a, object b) 
        {
            if (a is null) return b is null;
            return a.Equals(b);
        }

        private string Stringify(object obj) 
        {
            if (obj is null) return "nil";

            if (obj is double loxDouble) {
                string text = loxDouble.ToString();
                if (text.EndsWith(".0")) {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }

            return obj.ToString();
        }
    }
}
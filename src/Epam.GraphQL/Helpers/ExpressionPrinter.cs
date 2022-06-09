// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using Epam.GraphQL.Extensions;

namespace Epam.GraphQL.Helpers
{
    internal class ExpressionPrinter : ExpressionVisitor
    {
        private static readonly Dictionary<ExpressionType, string> _binaryOperands = new()
        {
            [ExpressionType.Assign] = "=",
            [ExpressionType.Equal] = "==",
            [ExpressionType.NotEqual] = "!=",
            [ExpressionType.GreaterThan] = ">",
            [ExpressionType.GreaterThanOrEqual] = ">=",
            [ExpressionType.LessThan] = "<",
            [ExpressionType.LessThanOrEqual] = "<=",
            [ExpressionType.OrElse] = "||",
            [ExpressionType.AndAlso] = "&&",
            [ExpressionType.Coalesce] = "??",
            [ExpressionType.Add] = "+",
            [ExpressionType.Subtract] = "-",
            [ExpressionType.Multiply] = "*",
            [ExpressionType.Divide] = "/",
            [ExpressionType.Modulo] = "%",
            [ExpressionType.And] = "&",
            [ExpressionType.Or] = "|",
            [ExpressionType.ExclusiveOr] = "^",
        };

        private readonly StringBuilder _builder = new();
        private readonly List<ParameterExpression> _namelessParameters = new();
        private readonly Stack<MethodCallExpression> _calls = new();
        private readonly bool _noIndent;
        private int _indent;

        private ExpressionPrinter(bool noIndent)
        {
            _noIndent = noIndent;
        }

        public static string Print(Expression expression, bool noIndent = false)
        {
            var visitor = new ExpressionPrinter(noIndent);
            visitor.Visit(expression);
            return visitor._builder.ToString();
        }

        public override Expression Visit(Expression expression)
        {
            switch (expression)
            {
                case UnaryExpression unaryExpression:
                    VisitUnary(unaryExpression);
                    break;
                case BinaryExpression binaryExpression:
                    VisitBinary(binaryExpression);
                    break;
                case LambdaExpression lambdaExpression:
                    base.Visit(lambdaExpression);
                    break;
                case MethodCallExpression methodCallExpression:
                    VisitMethodCall(methodCallExpression);
                    break;
                case MemberInitExpression memberInitExpression:
                    VisitMemberInit(memberInitExpression);
                    break;
                case ConstantExpression constantExpression:
                    VisitConstant(constantExpression);
                    break;
                case MemberExpression memberExpression:
                    VisitMember(memberExpression);
                    break;
                case ParameterExpression parameterExpression:
                    VisitParameter(parameterExpression);
                    break;
                case ConditionalExpression conditionalExpression:
                    VisitConditional(conditionalExpression);
                    break;
                default:
                    Append(expression.ToString());
                    break;
            }

            return expression;
        }

        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            if (_binaryOperands.TryGetValue(binaryExpression.NodeType, out var op))
            {
                Visit(binaryExpression.Left);

                Append($" {op} ");

                Visit(binaryExpression.Right);
            }
            else
            {
                Append(binaryExpression.ToString());
            }

            return binaryExpression;
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            var thisArg = methodCallExpression.Object;

            var isExtension = methodCallExpression.Method.IsExtensionMethod();
            var args = (isExtension ? methodCallExpression.Arguments.Skip(1) : methodCallExpression.Arguments).ToArray();

            if (isExtension)
            {
                thisArg = methodCallExpression.Arguments[0];
            }

            var shouldIndent = false;

            if (thisArg != null)
            {
                if ((_calls.TryPeek(out var storedMethodCall) && storedMethodCall == methodCallExpression)
                    || thisArg is MethodCallExpression)
                {
                    shouldIndent = true;

                    if (thisArg is MethodCallExpression previousMethodCall)
                    {
                        _calls.Push(previousMethodCall);
                    }
                }

                Visit(thisArg);

                if (shouldIndent)
                {
                    Indent();
                    AppendLine();

                    if (thisArg is MethodCallExpression)
                    {
                        _calls.Pop();
                    }
                }
            }

            Append(".");
            Append(methodCallExpression.Method.Name);
            Append("(");

            for (var i = 0; i < args.Length; i++)
            {
                Visit(args[i]);
                if (i < args.Length - 1)
                {
                    Append(", ");
                }
            }

            Append(")");

            if (shouldIndent)
            {
                Unindent();
            }

            return methodCallExpression;
        }

        protected override Expression VisitConstant(ConstantExpression constantExpression)
        {
            Append(Print(constantExpression.Value));

            return constantExpression;
        }

        protected override Expression VisitUnary(UnaryExpression unaryExpression)
        {
            switch (unaryExpression.NodeType)
            {
                case ExpressionType.Convert:
                    if (unaryExpression.Type != unaryExpression.Operand.Type)
                    {
                        Append($"({unaryExpression.Type.HumanizedName()})");
                    }

                    Visit(unaryExpression.Operand);
                    break;
                case ExpressionType.Quote:
                    Visit(unaryExpression.Operand);
                    break;
                case ExpressionType.Not:
                    Append("!");
                    Visit(unaryExpression.Operand);
                    break;
                default:
                    Append(unaryExpression.ToString());
                    break;
            }

            return unaryExpression;
        }

        protected override Expression VisitMemberInit(MemberInitExpression memberInitExpression)
        {
            Append($"new {memberInitExpression.Type.HumanizedName()}");
            AppendLine();
            Append("{");
            Indent();
            AppendLine();

            for (var i = 0; i < memberInitExpression.Bindings.Count; i++)
            {
                if (memberInitExpression.Bindings[i] is MemberAssignment assignment)
                {
                    Append($"{assignment.Member.Name} = ");
                    Visit(assignment.Expression);

                    if (i < memberInitExpression.Bindings.Count - 1)
                    {
                        Append(",");
                        AppendLine();
                    }
                }
                else
                {
                    Append("Error");
                }
            }

            Unindent();
            AppendLine();
            Append("}");

            return memberInitExpression;
        }

        protected override Expression VisitMember(MemberExpression memberExpression)
        {
            if (memberExpression.Expression != null)
            {
                if ((memberExpression.Expression.NodeType == ExpressionType.Convert && memberExpression.Expression is UnaryExpression unaryExpression && memberExpression.Expression.Type != unaryExpression.Operand.Type)
                    || memberExpression.Expression is BinaryExpression)
                {
                    Append("(");
                    Visit(memberExpression.Expression);
                    Append(")");
                }
                else
                {
                    Visit(memberExpression.Expression);
                }
            }
            else
            {
                Append(memberExpression.Member.DeclaringType.Name);
            }

            Append("." + memberExpression.Member.Name);

            return memberExpression;
        }

        protected override Expression VisitLambda<T>(Expression<T> lambdaExpression)
        {
            if (lambdaExpression.Parameters.Count != 1)
            {
                Append("(");
            }

            for (var i = 0; i < lambdaExpression.Parameters.Count; i++)
            {
                if (i != 0)
                {
                    Append(", ");
                }

                Visit(lambdaExpression.Parameters[i]);
            }

            if (lambdaExpression.Parameters.Count != 1)
            {
                Append(")");
            }

            Append(" => ");

            Visit(lambdaExpression.Body);

            return lambdaExpression;
        }

        protected override Expression VisitParameter(ParameterExpression parameterExpression)
        {
            if (parameterExpression.Name == null)
            {
                if (!_namelessParameters.Contains(parameterExpression))
                {
                    _namelessParameters.Add(parameterExpression);
                }

                Append($"param{_namelessParameters.IndexOf(parameterExpression)}");
            }
            else
            {
                Append(parameterExpression.Name);
            }

            return parameterExpression;
        }

        protected override Expression VisitConditional(ConditionalExpression conditionalExpression)
        {
            Visit(conditionalExpression.Test);
            Append(" ? ");
            Visit(conditionalExpression.IfTrue);
            Append(" : ");
            Visit(conditionalExpression.IfFalse);

            return conditionalExpression;
        }

        private string Print(object value)
        {
            if (value == null)
            {
                return "null";
            }

            if (value is string)
            {
                return $@"""{value}""";
            }

            var valueType = value.GetType();

            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(EnumerableQuery<>))
            {
                return valueType.HumanizedName();
            }

            if (value is ITuple tuple)
            {
                var builder = new StringBuilder();

                builder.Append('(');

                for (var i = 0; i < tuple.Length; i++)
                {
                    if (i > 0)
                    {
                        builder.Append(", ");
                    }

                    builder.Append(Print(tuple[i]));
                }

                builder.Append(')');

                return builder.ToString();
            }

            if (value is IEnumerable enumerable and not IQueryable)
            {
                var builder = new StringBuilder();
                builder.Append(valueType.HumanizedName());
                builder.Append(" { ");

                var isFirst = true;
                var count = 0;

                foreach (var item in enumerable)
                {
                    if (!isFirst)
                    {
                        builder.Append(", ");
                    }

                    isFirst = false;
                    count += 1;

                    if (count > 5)
                    {
                        builder.Append("...");
                        break;
                    }

                    builder.Append(Print(item));
                }

                builder.Append(" }");

                return builder.ToString();
            }

            var stringValue = value.ToString();

            if (stringValue != valueType.ToString())
            {
                return stringValue;
            }

            return valueType.HumanizedName();
        }

        private void Append(string str)
        {
            _builder.Append(str);
        }

        private void AppendLine()
        {
            if (!_noIndent)
            {
                _builder.AppendLine();
            }

            _builder.Append(' ', _indent * 4);
        }

        private void Indent()
        {
            if (!_noIndent)
            {
                _indent += 1;
            }
        }

        private void Unindent()
        {
            if (!_noIndent)
            {
                _indent -= 1;
            }
        }
    }
}

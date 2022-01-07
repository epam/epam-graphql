// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

#nullable enable

namespace Epam.GraphQL.Helpers
{
    internal class ExpressionEqualityComparer : IEqualityComparer<Expression?>
    {
        public static ExpressionEqualityComparer Instance { get; } = new ExpressionEqualityComparer();

        public int GetHashCode(Expression? expression)
        {
            var hashCode = default(HashCode);

            AddHashCode(null, ref hashCode, expression);

            return hashCode.ToHashCode();
        }

        public bool Equals(Expression? x, Expression? y) => Equals(null, x, y);

        private static void AddHashCode(ParameterScope? parameterScope, ref HashCode hashCode, Expression? expression)
        {
            if (expression == null)
            {
                return;
            }

            hashCode.Add(expression.NodeType);
            hashCode.Add(expression.Type);

            switch (expression)
            {
                case BinaryExpression expr:
                    AddHashCode(parameterScope, ref hashCode, expr);
                    break;
                case ConditionalExpression expr:
                    AddHashCode(parameterScope, ref hashCode, expr);
                    break;
                case ConstantExpression expr:
                    AddHashCode(ref hashCode, expr);
                    break;
                case DefaultExpression expr:
                    AddHashCode(ref hashCode, expr);
                    break;
                case LambdaExpression expr:
                    AddHashCode(parameterScope, ref hashCode, expr);
                    break;
                case InvocationExpression expr:
                    AddHashCode(parameterScope, ref hashCode, expr);
                    break;
                case ListInitExpression expr:
                    AddHashCode(parameterScope, ref hashCode, expr);
                    break;
                case MemberExpression expr:
                    AddHashCode(parameterScope, ref hashCode, expr);
                    break;
                case MemberInitExpression expr:
                    AddHashCode(parameterScope, ref hashCode, expr);
                    break;
                case MethodCallExpression expr:
                    AddHashCode(parameterScope, ref hashCode, expr);
                    break;
                case NewArrayExpression expr:
                    AddHashCode(parameterScope, ref hashCode, expr);
                    break;
                case NewExpression expr:
                    AddHashCode(parameterScope, ref hashCode, expr);
                    break;
                case ParameterExpression expr:
                    AddHashCode(parameterScope, ref hashCode, expr);
                    break;
                case TypeBinaryExpression expr:
                    AddHashCode(parameterScope, ref hashCode, expr);
                    break;
                case UnaryExpression expr:
                    AddHashCode(parameterScope, ref hashCode, expr);
                    break;
                default:
                    throw new NotImplementedException($"Unhandled expression node type: {expression.NodeType}");
            }
        }

        private static bool Equals(ParameterScope? parameterScope, Expression? x, Expression? y)
        {
            if (x == y)
            {
                return true;
            }

            if (x == null || y == null || x.NodeType != y.NodeType || x.Type != y.Type)
            {
                return false;
            }

            return (x, y) switch
            {
                (BinaryExpression first, BinaryExpression second) => Equals(parameterScope, first, second),
                (ConditionalExpression first, ConditionalExpression second) => Equals(parameterScope, first, second),
                (ConstantExpression first, ConstantExpression second) => Equals(first, second),
                (DefaultExpression first, DefaultExpression second) => Equals(first, second),
                (LambdaExpression first, LambdaExpression second) => Equals(parameterScope, first, second),
                (InvocationExpression first, IndexExpression second) => Equals(parameterScope, first, second),
                (InvocationExpression first, InvocationExpression second) => Equals(parameterScope, first, second),
                (ListInitExpression first, ListInitExpression second) => Equals(parameterScope, first, second),
                (MethodCallExpression first, MethodCallExpression second) => Equals(parameterScope, first, second),
                (MemberExpression first, MemberExpression second) => Equals(parameterScope, first, second),
                (MemberInitExpression first, MemberInitExpression second) => Equals(parameterScope, first, second),
                (NewArrayExpression first, NewArrayExpression second) => Equals(parameterScope, first, second),
                (NewExpression first, NewExpression second) => Equals(parameterScope, first, second),
                (ParameterExpression first, ParameterExpression second) => Equals(parameterScope, first, second),
                (TypeBinaryExpression first, TypeBinaryExpression second) => Equals(parameterScope, first, second),
                (UnaryExpression first, UnaryExpression second) => Equals(parameterScope, first, second),
                _ => throw new NotImplementedException($"Unhandled expression node type: {x.NodeType}"),
            };
        }

        private static void AddHashCode(ParameterScope? parameterScope, ref HashCode hashCode, BinaryExpression expression)
        {
            hashCode.Add(expression.Method);
            hashCode.Add(expression.IsLifted);
            hashCode.Add(expression.IsLiftedToNull);
            AddHashCode(parameterScope, ref hashCode, expression.Left);
            AddHashCode(parameterScope, ref hashCode, expression.Right);
        }

        private static bool Equals(ParameterScope? parameterScope, BinaryExpression x, BinaryExpression y)
            => Equals(x.Method, y.Method)
                && x.IsLifted == y.IsLifted
                && x.IsLiftedToNull == y.IsLiftedToNull
                && Equals(parameterScope, x.Left, y.Left)
                && Equals(parameterScope, x.Right, y.Right);

        private static void AddHashCode(ParameterScope? parameterScope, ref HashCode hashCode, ConditionalExpression expression)
        {
            AddHashCode(parameterScope, ref hashCode, expression.Test);
            AddHashCode(parameterScope, ref hashCode, expression.IfTrue);
            AddHashCode(parameterScope, ref hashCode, expression.IfFalse);
        }

        private static bool Equals(ParameterScope? parameterScope, ConditionalExpression x, ConditionalExpression y)
            => Equals(parameterScope, x.Test, y.Test)
                && Equals(parameterScope, x.IfTrue, y.IfTrue)
                && Equals(parameterScope, x.IfFalse, y.IfFalse);

        private static void AddHashCode(ref HashCode hashCode, ConstantExpression expression)
        {
            hashCode.Add(expression.Value);
        }

        private static bool Equals(ConstantExpression x, ConstantExpression y)
            => Equals(x.Value, y.Value);

        private static void AddHashCode(ref HashCode hashCode, DefaultExpression expression)
        {
            hashCode.Add(expression.Type);
        }

        private static bool Equals(DefaultExpression x, DefaultExpression y)
            => Equals(x.Type, y.Type);

        private static void AddHashCode(ParameterScope? parameterScope, ref HashCode hashCode, InvocationExpression expression)
        {
            AddHashCode(parameterScope, ref hashCode, expression.Expression);
            AddHashCode(parameterScope, ref hashCode, expression.Arguments);
        }

        private static bool Equals(ParameterScope? parameterScope, InvocationExpression x, InvocationExpression y)
            => Equals(parameterScope, x.Expression, y.Expression)
                && Equals(parameterScope, x.Arguments, y.Arguments);

        private static void AddHashCode(ParameterScope? parameterScope, ref HashCode hashCode, LambdaExpression expression)
        {
            hashCode.Add(expression.Parameters.Count);
            hashCode.Add(expression.ReturnType);

            if (expression.Parameters.Count > 0)
            {
                parameterScope = new ParameterScope(parameterScope);
                for (var i = 0; i < expression.Parameters.Count; i++)
                {
                    parameterScope.Add(expression.Parameters[i], i);
                }
            }

            AddHashCode(parameterScope, ref hashCode, expression.Body);
        }

        private static bool Equals(ParameterScope? parameterScope, LambdaExpression x, LambdaExpression y)
        {
            if (x.Parameters.Count != y.Parameters.Count || x.ReturnType != y.ReturnType)
            {
                return false;
            }

            if (x.Parameters.Count > 0)
            {
                for (var i = 0; i < x.Parameters.Count; i++)
                {
                    if (x.Parameters[i].Type != y.Parameters[i].Type)
                    {
                        return false;
                    }
                }

                parameterScope = new ParameterScope(parameterScope);
                for (var i = 0; i < x.Parameters.Count; i++)
                {
                    parameterScope.Add(x.Parameters[i], y.Parameters[i], i);
                }
            }

            return Equals(parameterScope, x.Body, y.Body);
        }

        private static void AddHashCode(ParameterScope? parameterScope, ref HashCode hashCode, ListInitExpression expression)
        {
            AddHashCode(parameterScope, ref hashCode, expression.NewExpression);
            AddHashCode(parameterScope, ref hashCode, expression.Initializers);
        }

        private static bool Equals(ParameterScope? parameterScope, ListInitExpression x, ListInitExpression y)
            => Equals(parameterScope, x.NewExpression, y.NewExpression)
                && Equals(parameterScope, x.Initializers, y.Initializers);

        private static void AddHashCode(ParameterScope? parameterScope, ref HashCode hashCode, MethodCallExpression expression)
        {
            hashCode.Add(expression.Method);
            AddHashCode(parameterScope, ref hashCode, expression.Object);
            AddHashCode(parameterScope, ref hashCode, expression.Arguments);
        }

        private static bool Equals(ParameterScope? parameterScope, MethodCallExpression x, MethodCallExpression y)
            => Equals(x.Method, y.Method)
                && Equals(parameterScope, x.Object, y.Object)
                && Equals(parameterScope, x.Arguments, y.Arguments);

        private static void AddHashCode(ParameterScope? parameterScope, ref HashCode hashCode, MemberExpression expression)
        {
            hashCode.Add(expression.Member);
            AddHashCode(parameterScope, ref hashCode, expression.Expression);
        }

        private static bool Equals(ParameterScope? parameterScope, MemberExpression x, MemberExpression y)
            => Equals(x.Member, y.Member) && Equals(parameterScope, x.Expression, y.Expression);

        private static void AddHashCode(ParameterScope? parameterScope, ref HashCode hashCode, MemberInitExpression expression)
        {
            AddHashCode(parameterScope, ref hashCode, expression.NewExpression);
            AddHashCode(parameterScope, ref hashCode, expression.Bindings);
        }

        private static bool Equals(ParameterScope? parameterScope, MemberInitExpression x, MemberInitExpression y)
            => Equals(parameterScope, x.NewExpression, y.NewExpression)
                && Equals(parameterScope, x.Bindings, y.Bindings);

        private static void AddHashCode(ParameterScope? parameterScope, ref HashCode hashCode, NewArrayExpression expression)
        {
            AddHashCode(parameterScope, ref hashCode, expression.Expressions);
        }

        private static bool Equals(ParameterScope? parameterScope, NewArrayExpression x, NewArrayExpression y)
            => Equals(parameterScope, x.Expressions, y.Expressions);

        private static void AddHashCode(ParameterScope? parameterScope, ref HashCode hashCode, NewExpression expression)
        {
            hashCode.Add(expression.Constructor);
            AddHashCode(parameterScope, ref hashCode, expression.Arguments);
            AddHashCode(ref hashCode, expression.Members);
        }

        private static bool Equals(ParameterScope? parameterScope, NewExpression x, NewExpression y)
            => Equals(x.Constructor, y.Constructor)
                && Equals(parameterScope, x.Arguments, y.Arguments)
                && Equals(x.Members, y.Members);

        private static void AddHashCode(ParameterScope? parameterScope, ref HashCode hashCode, ParameterExpression expression)
        {
            if (parameterScope != null && parameterScope.TryGetValue(expression, out var scope))
            {
                hashCode.Add(scope);
            }
            else
            {
                hashCode.Add(expression.Name);
                hashCode.Add(expression.Type);
            }
        }

        private static bool Equals(ParameterScope? parameterScope, ParameterExpression x, ParameterExpression y)
        {
            if (parameterScope != null)
            {
                var firstScopeExists = parameterScope.TryGetValue(x, out var firstScope);
                var secondScopeExists = parameterScope.TryGetValue(y, out var secondScope);

                if (firstScopeExists != secondScopeExists)
                {
                    return false;
                }

                if (firstScopeExists)
                {
                    return firstScope == secondScope;
                }
            }

            return x.Name == y.Name && x.Type == y.Type;
        }

        private static void AddHashCode(ParameterScope? parameterScope, ref HashCode hashCode, TypeBinaryExpression expression)
        {
            hashCode.Add(expression.TypeOperand);
            AddHashCode(parameterScope, ref hashCode, expression.Expression);
        }

        private static bool Equals(ParameterScope? parameterScope, TypeBinaryExpression x, TypeBinaryExpression y)
            => x.TypeOperand == y.TypeOperand && Equals(parameterScope, x.Expression, y.Expression);

        private static void AddHashCode(ParameterScope? parameterScope, ref HashCode hashCode, UnaryExpression expression)
        {
            hashCode.Add(expression.Method);
            hashCode.Add(expression.IsLifted);
            hashCode.Add(expression.IsLiftedToNull);
            AddHashCode(parameterScope, ref hashCode, expression.Operand);
        }

        private static bool Equals(ParameterScope? parameterScope, UnaryExpression x, UnaryExpression y)
            => Equals(x.Method, y.Method)
                && x.IsLifted == y.IsLifted
                && x.IsLiftedToNull == y.IsLiftedToNull
                && Equals(parameterScope, x.Operand, y.Operand);

        private static void AddHashCode(ParameterScope? parameterScope, ref HashCode hashCode, IReadOnlyList<Expression> expressions)
        {
            for (var i = 0; i < expressions.Count; i++)
            {
                AddHashCode(parameterScope, ref hashCode, expressions[i]);
            }
        }

        private static bool Equals(ParameterScope? parameterScope, IReadOnlyList<Expression> x, IReadOnlyList<Expression> y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null || x.Count != y.Count)
            {
                return false;
            }

            for (var i = 0; i < x.Count; i++)
            {
                if (!Equals(parameterScope, x[i], y[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static void AddHashCode(ref HashCode hashCode, IReadOnlyList<MemberInfo> members)
        {
            if (members == null)
            {
                return;
            }

            for (var i = 0; i < members.Count; i++)
            {
                hashCode.Add(members[i]);
            }
        }

        private static bool Equals(IReadOnlyList<MemberInfo> x, IReadOnlyList<MemberInfo> y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null || x.Count != y.Count)
            {
                return false;
            }

            for (var i = 0; i < x.Count; i++)
            {
                if (!Equals(x[i], y[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static void AddHashCode(ParameterScope? parameterScope, ref HashCode hashCode, IReadOnlyList<MemberBinding> bindings)
        {
            for (var i = 0; i < bindings.Count; i++)
            {
                AddHashCode(parameterScope, ref hashCode, bindings[i]);
            }
        }

        private static bool Equals(ParameterScope? parameterScope, IReadOnlyList<MemberBinding> x, IReadOnlyList<MemberBinding> y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null || x.Count != y.Count)
            {
                return false;
            }

            for (var i = 0; i < x.Count; i++)
            {
                if (!Equals(parameterScope, x[i], y[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static void AddHashCode(ParameterScope? parameterScope, ref HashCode hashCode, MemberBinding binding)
        {
            switch (binding)
            {
                case MemberAssignment castedBinding:
                    AddHashCode(parameterScope, ref hashCode, castedBinding);
                    break;
                case MemberListBinding castedBinding:
                    AddHashCode(parameterScope, ref hashCode, castedBinding);
                    break;
                case MemberMemberBinding castedBinding:
                    AddHashCode(parameterScope, ref hashCode, castedBinding);
                    break;
                default:
                    throw new InvalidOperationException($"Unhandled member binding type: {binding.BindingType}");
            }
        }

        private static bool Equals(ParameterScope? parameterScope, MemberBinding x, MemberBinding y)
        {
            if (x == y)
            {
                return true;
            }

            if (x == null || y == null || x.BindingType != y.BindingType)
            {
                return false;
            }

            return (x, y) switch
            {
                (MemberAssignment firstBinding, MemberAssignment secondBinding) => Equals(parameterScope, firstBinding, secondBinding),
                (MemberListBinding firstBinding, MemberListBinding secondBinding) => Equals(parameterScope, firstBinding, secondBinding),
                (MemberMemberBinding firstBinding, MemberMemberBinding secondBinding) => Equals(parameterScope, firstBinding, secondBinding),
                _ => throw new InvalidOperationException($"Unhandled member binding type: {x.BindingType}"),
            };
        }

        private static void AddHashCode(ParameterScope? parameterScope, ref HashCode hashCode, IReadOnlyList<ElementInit> initializers)
        {
            for (var i = 0; i < initializers.Count; i++)
            {
                AddHashCode(parameterScope, ref hashCode, initializers[i]);
            }
        }

        private static bool Equals(ParameterScope? parameterScope, IReadOnlyList<ElementInit> x, IReadOnlyList<ElementInit> y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null || x.Count != y.Count)
            {
                return false;
            }

            for (var i = 0; i < x.Count; i++)
            {
                if (!Equals(parameterScope, x[i], y[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static void AddHashCode(ParameterScope? parameterScope, ref HashCode hashCode, ElementInit initializer)
        {
            hashCode.Add(initializer.AddMethod);
            AddHashCode(parameterScope, ref hashCode, initializer.Arguments);
        }

        private static bool Equals(ParameterScope? parameterScope, ElementInit x, ElementInit y) =>
            Equals(x.AddMethod, y.AddMethod)
            && Equals(parameterScope, x.Arguments, y.Arguments);

        private static void AddHashCode(ParameterScope? parameterScope, ref HashCode hashCode, MemberAssignment binding)
        {
            hashCode.Add(binding.Member);
            AddHashCode(parameterScope, ref hashCode, binding.Expression);
        }

        private static bool Equals(ParameterScope? parameterScope, MemberAssignment x, MemberAssignment y) =>
            Equals(x.Member, y.Member)
            && Equals(parameterScope, x.Expression, y.Expression);

        private static void AddHashCode(ParameterScope? parameterScope, ref HashCode hashCode, MemberListBinding binding)
        {
            hashCode.Add(binding.Member);
            AddHashCode(parameterScope, ref hashCode, binding.Initializers);
        }

        private static bool Equals(ParameterScope? parameterScope, MemberListBinding x, MemberListBinding y) =>
            Equals(x.Member, y.Member)
            && Equals(parameterScope, x.Initializers, y.Initializers);

        private static void AddHashCode(ParameterScope? parameterScope, ref HashCode hashCode, MemberMemberBinding binding)
        {
            hashCode.Add(binding.Member);
            AddHashCode(parameterScope, ref hashCode, binding.Bindings);
        }

        private static bool Equals(ParameterScope? parameterScope, MemberMemberBinding x, MemberMemberBinding y) =>
            Equals(x.Member, y.Member)
            && Equals(parameterScope, x.Bindings, y.Bindings);

        private class ParameterScope
        {
            private readonly ParameterScope? _previousScope;
            private readonly Dictionary<ParameterExpression, int> _map = new();

            public ParameterScope(ParameterScope? previousScope)
            {
                _previousScope = previousScope;
            }

            public void Add(ParameterExpression key1, ParameterExpression key2, int index)
            {
                _map.Add(key1, index);
                if (key1 != key2)
                {
                    _map.Add(key2, index);
                }
            }

            public void Add(ParameterExpression key, int index)
            {
                _map.Add(key, index);
            }

            public bool TryGetValue(ParameterExpression key, out (int Depth, int Index) value)
            {
                if (_map.TryGetValue(key, out var index))
                {
                    value = (0, index);
                    return true;
                }

                if (_previousScope != null && _previousScope.TryGetValue(key, out value))
                {
                    value.Depth++;
                    return true;
                }

                value = default;
                return false;
            }
        }
    }
}

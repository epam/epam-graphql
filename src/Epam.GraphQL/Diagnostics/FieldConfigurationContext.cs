// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Epam.GraphQL.Extensions;

namespace Epam.GraphQL.Diagnostics
{
    internal class FieldConfigurationContext
    {
        private readonly List<OperationArgument> _arguments = new();

        public FieldConfigurationContext(
            IConfigurationContext parentContext,
            FieldConfigurationContext? previous,
            string operation)
        {
            Operation = operation;
            Previous = previous;
            ParentContext = parentContext;
        }

        public IConfigurationContext ParentContext { get; }

        protected string Operation { get; }

        protected FieldConfigurationContext? Previous { get; }

        public FieldConfigurationContext NextOperation(string operation)
        {
            return new FieldConfigurationContext(ParentContext, this, operation);
        }

        public FieldConfigurationContext NextOperation<T>(string operation)
        {
            return NextOperation($"{operation}<{typeof(T).HumanizedName()}>");
        }

        public FieldConfigurationContext NextOperation<T1, T2>(string operation)
        {
            return NextOperation($"{operation}<{typeof(T1).HumanizedName()}, {typeof(T2).HumanizedName()}>");
        }

        public FieldConfigurationContext Argument(string arg)
        {
            _arguments.Add(new OperationArgument(arg, optional: false));
            return this;
        }

        public FieldConfigurationContext OptionalArgument(string? arg)
        {
            _arguments.Add(new OperationArgument(arg, optional: true));
            return this;
        }

        public FieldConfigurationContext Argument(Delegate arg)
        {
            _arguments.Add(new OperationArgument(arg, optional: false));
            return this;
        }

        public FieldConfigurationContext OptionalArgument(Delegate? arg)
        {
            _arguments.Add(new OperationArgument(arg, optional: true));
            return this;
        }

        public FieldConfigurationContext Argument(LambdaExpression arg)
        {
            _arguments.Add(new OperationArgument(arg, optional: false));
            return this;
        }

        public FieldConfigurationContext OptionalArgument(LambdaExpression? arg)
        {
            _arguments.Add(new OperationArgument(arg, optional: true));
            return this;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            if (Previous != null)
            {
                builder.Append(Previous);
                builder.Append('.');
            }

            builder.Append(Operation);
            builder.Append('(');

            var lastIndex = _arguments.Count - 1;

            while (lastIndex >= 0 && _arguments[lastIndex].IsDefault)
            {
                lastIndex -= 1;
            }

            for (var i = 0; i <= lastIndex; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(_arguments[i].Print());
            }

            builder.Append(')');

            return builder.ToString();
        }

        public void AddError(string message)
        {
            ParentContext.AddError($"Field: {this}\r\n{message}");
        }

        public string GetError(string message)
        {
            return ParentContext.GetError($"Field: {this}\r\n{message}");
        }

        public void AddErrorIf(bool condition, string message)
        {
            if (condition)
            {
                AddError(message);
            }
        }
    }
}

// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;

namespace Epam.GraphQL.Diagnostics
{
    internal class OperationArgument : IPrinter
    {
        private readonly IPrinter _printer;

        public OperationArgument(LambdaExpression? expression, bool optional)
            : this(new LambdaExpressionPrinter(expression), optional && expression == null)
        {
        }

        public OperationArgument(Delegate? func, bool optional)
            : this(new DelegatePrinter<Delegate>(func), optional && func == null)
        {
        }

        public OperationArgument(string? str, bool optional)
            : this(new StringPrinter(str), optional && str == null)
        {
        }

        private OperationArgument(IPrinter printer, bool isDefault)
        {
            _printer = printer;
            IsDefault = isDefault;
        }

        public bool IsDefault { get; }

        public string Print()
        {
            return _printer.Print();
        }
    }
}

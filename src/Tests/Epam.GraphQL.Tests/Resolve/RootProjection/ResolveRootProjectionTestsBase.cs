// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using GraphQLParser.AST;

namespace Epam.GraphQL.Tests.Resolve.RootProjection
{
    public class ResolveRootProjectionTestsBase
    {
        private readonly OperationType _operationType;

        public ResolveRootProjectionTestsBase(OperationType operationType)
        {
            _operationType = operationType;
        }

        protected void Test(
            Action<Query<TestUserContext>> queryBuilder,
            Action<Mutation<TestUserContext>> mutationBuilder,
            string query,
            string expected)
        {
            switch (_operationType)
            {
                case OperationType.Query:
                    TestHelpers.TestQuery(
                        builder: queryBuilder,
                        query: $"query {{ {query} }}",
                        expected: expected);

                    break;
                case OperationType.Mutation:
                    TestHelpers.TestMutation(
                        queryBuilder: query => query.Field("test").Resolve(_ => 0),
                        mutationBuilder: mutationBuilder,
                        dataContext: null,
                        query: $"mutation {{ {query} }}",
                        expected: expected);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public static class CustomObject
        {
            public static CustomObject<T> Create<T>(T fieldValue) => new() { TestField = fieldValue };

            public static CustomObject<T, T2> Create<T, T2>(T firstField, T2 secondField) => new() { FirstField = firstField, SecondField = secondField };
        }

        public class BaseCustomObject<TId>
        {
            public TId Id { get; set; }
        }

        public class CustomObject<T> : BaseCustomObject<int>
        {
            public T TestField { get; set; }
        }

        public class CustomObject<T, T2> : BaseCustomObject<int>
        {
            public T FirstField { get; set; }

            public T2 SecondField { get; set; }
        }

        public class Geometry
        {
            public int Id { get; set; }
        }

        public class Line : Geometry
        {
            public double Length { get; set; }
        }

        public class Circle : Geometry
        {
            public double Radius { get; set; }
        }

        public class OperationFixtureArgCollection : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new object[] { OperationType.Query };
                yield return new object[] { OperationType.Mutation };
            }
        }

        public class OperationAndArgumentFixtureArgCollection : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new object[] { OperationType.Query, ArgumentType.Argument };
                yield return new object[] { OperationType.Query, ArgumentType.PayloadField };
                yield return new object[] { OperationType.Mutation, ArgumentType.Argument };
                yield return new object[] { OperationType.Mutation, ArgumentType.PayloadField };
            }
        }
    }
}

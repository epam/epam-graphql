// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections;
using System.Linq;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;

namespace Epam.GraphQL.Tests.Resolve.Mutation
{
    public class ResolveMutationTestBase : BaseMutationTests
    {
        protected static Type CreatePersonLoader(Func<TestUserContext, IQueryable<Person>> getBaseQuery = null)
        {
            return GraphQLTypeBuilder.CreateMutableLoaderType(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName).Editable();
                },
                getBaseQuery: getBaseQuery ?? (context => context.DataContext.GetQueryable<Person>()));
        }

        public class FixtureArgCollection : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new object[] { ArgumentType.Argument };
                yield return new object[] { ArgumentType.PayloadField };
            }
        }
    }
}

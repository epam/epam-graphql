// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Mutation
{
    public partial class MutationTests
    {
        [Test(Description = "Empty mutation/should not emit SubmitInputType")]
        public void EmptyMutationShouldNotEmitSubmitInputType()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>());

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(personLoader, "people");
            }

            TestQuery(
                QueryBuilder,
                @"query {
                    __type(name: ""SubmitInputType"") {
                        name
                    }
                }",
                @"{
                    __type: null
                }");
        }

        [Test(Description = "Empty mutation/should not emit SubmitOutputType")]
        public void EmptyMutationShouldNotEmitSubmitOutputType()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>());

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(personLoader, "people");
            }

            TestQuery(
                QueryBuilder,
                @"query {
                    __type(name: ""SubmitOutputType"") {
                        name
                    }
                }",
                @"{
                    __type: null
                }");
        }

        [Test(Description = "Empty mutation/should not emit input type for entity")]
        public void EmptyMutationShouldNotEmitInputTypeForEntity()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>());

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(personLoader, "people");
            }

            TestQuery(
                QueryBuilder,
                @"query {
                    __type(name: ""InputPerson"") {
                        name
                    }
                }",
                @"{
                    __type: null
                }");
        }

        [Test(Description = "Empty mutation/should not emit submit output type for entity")]
        public void EmptyMutationShouldNotEmitSubmitOutputTypeForEntity()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>());

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(personLoader, "people");
            }

            TestQuery(
                QueryBuilder,
                @"query {
                    __type(name: ""PersonSubmitOutput"") {
                        name
                    }
                }",
                @"{
                    __type: null
                }");
        }
    }
}

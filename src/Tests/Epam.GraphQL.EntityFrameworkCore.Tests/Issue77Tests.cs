// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using Epam.GraphQL.Tests;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.EntityFrameworkCore.Tests
{
    [TestFixture]
    public class Issue77Tests : BaseTests
    {
        [Test]
        public void QueryFromNestedLoader()
        {
            Type personLoaderType = null;

            personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("children")
                        .FromLoader<Person, Person, TestUserContext>(personLoaderType, (p, c) => p.Id == c.ManagerId);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: context => context.DbContext.People.ToList().AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestQuery(
                Builder,
                @"
                    query {
                        people {
                            items {
                                id
                                children {
                                    id
                                }
                            }
                        }
                    }",
                @"{
                    people: {
                        items: [{
                            id: 1,
                            children: [{
                                id: 2
                            },{
                                id: 3
                            }]
                        },{
                            id: 2,
                            children: [{
                                id: 4
                            },{
                                id: 5
                            }]
                        },{
                            id: 3,
                            children: []
                        },{
                            id: 4,
                            children: []
                        },{
                            id: 5,
                            children: [{
                                id: 6
                            }]
                        },{
                            id: 6,
                            children: []
                        }]
                    }
                }");
        }
    }
}

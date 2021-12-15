// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.Contracts.Models;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Common
{
    [TestFixture]
    public abstract class BaseTests<TEntity, TQueryConfigurator, TFilterType, TLoaderTypeCreator, TLoaderConfigurator, TLoaderConfiguration> : BaseTests
        where TEntity : class, IHasId<int>
        where TQueryConfigurator : IQueryConfigurator<TFilterType>, new()
        where TLoaderTypeCreator : ILoaderTypeCreator<TEntity>, new()
        where TLoaderConfigurator : ILoaderConfigurator<TEntity, TLoaderConfiguration>, new()
    {
        private readonly TFilterType _filterType;
        private readonly LoaderType _loaderType;
        private readonly TLoaderConfiguration _loaderConfiguration;
        private Action<Query<TestUserContext>> _queryBuilder;

        protected BaseTests(TFilterType filterType, LoaderType loaderType, TLoaderConfiguration loaderConfiguration)
        {
            _filterType = filterType;
            _loaderType = loaderType;
            _loaderConfiguration = loaderConfiguration;
        }

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();

            var queryConfigurator = new TQueryConfigurator();
            var loaderTypeCreator = new TLoaderTypeCreator();
            var loaderConfigurator = new TLoaderConfigurator();
            Type loaderType = _loaderType switch
            {
                LoaderType.Loader => loaderTypeCreator.CreateLoaderType(FakeData.GetEntities<TEntity>(), loaderConfigurator.ConfigureLoader(_loaderConfiguration)),
                LoaderType.IdentifiableLoader => loaderTypeCreator.CreateIdentifiableLoaderType(FakeData.GetEntities<TEntity>(), loaderConfigurator.ConfigureIdentifiableLoader(_loaderConfiguration)),
                LoaderType.MutableLoader => loaderTypeCreator.CreateMutableLoaderType(FakeData.GetEntities<TEntity>(), loaderConfigurator.ConfigureMutableLoader(_loaderConfiguration)),
                _ => throw new NotSupportedException(),
            };
            _queryBuilder = query => queryConfigurator.ConfigureQuery(_filterType, query, loaderType);
        }

        protected void TestQuery(string query, string expected, Action checks = null, Action beforeExecute = null)
        {
            TestHelpers.TestQuery(_queryBuilder, query, expected, null, checks, beforeExecute);
        }

        protected void TestQueryError(Type exceptionType, string message, string query)
        {
            TestHelpers.TestQueryError(_queryBuilder, exceptionType, message, query);
        }
    }
}

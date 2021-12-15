// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.SystemTextJson;
using NUnit.Framework;
using PublicApiGenerator;
using Shouldly;

namespace Epam.GraphQL.Api.Tests
{
    [TestFixture]
    public class ApiTests
    {
        [TestCase(typeof(SchemaExecuter<,,>))]
        [TestCase(typeof(EntityFrameworkCoreSchemaExecutionOptionsBuilderExtensions))]
        [TestCase(typeof(MiniProfilerSchemaOptionsBuilderExtensions))]
        [TestCase(typeof(SystemTextJsonExecutionResultExtensions))]
        [TestCase(typeof(NewtonsoftJsonExecutionResultExtensions))]
        public void ApiDifferenceTests(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            string publicApi = type.Assembly.GeneratePublicApi(new ApiGeneratorOptions
            {
                IncludeAssemblyAttributes = false,
            }) + Environment.NewLine;

            publicApi.ShouldMatchApproved(options => options.WithFilenameGenerator((testMethodInfo, discriminator, fileType, fileExtension) => $"{type.Assembly.GetName().Name}.{fileType}.{fileExtension}"));
        }
    }
}

// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.GraphQL.Infrastructure;
using Epam.GraphQL.Savers;
using GraphQL;

namespace Epam.GraphQL.Loaders
{
    [InternalApi]
    public interface IMutableLoader<TExecutionContext>
    {
        Type EntityType { get; }

        ISaveResult<TExecutionContext> CreateSaveResultFromValues(Type mutationType, string fieldName, IEnumerable<IInputItem> values);

        ISaveResult<TExecutionContext> CreateSaveResultFromValues(Type mutationType, string fieldName, IEnumerable<object> values);

        Task<IEnumerable<ISaveResult<TExecutionContext>>> MutateAsync(IResolveFieldContext context, ISaveResult<TExecutionContext> previousSaveResult);

        Task ReloadAsync(IResolveFieldContext context, ISaveResult<TExecutionContext> saveResult, IEnumerable<string> fieldNames);

        bool IsFakeId(object id);

        object GetId(object entity);
    }
}

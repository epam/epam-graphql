// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Mutation;
using Epam.GraphQL.Builders.Mutation.Implementations;
using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Mutation;
using Epam.GraphQL.Savers;
using Epam.GraphQL.Types;
using GraphQL;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

#nullable enable

namespace Epam.GraphQL
{
    public abstract class Mutation<TExecutionContext> : RootProjection<TExecutionContext>
    {
        private const string GeneratedSubmitOutputTypeName = "SubmitOutput";
        private const string GeneratedMutationResultTypeName = "MutationResult";
        private const string SubmitName = "submit";
        private const string PayloadName = "payload";

        // TODO Consider defining true .NET type for this; it seems like there is no nececity for using reflection anymore
        private readonly Type _mutationResultType;
        private readonly PropertyInfo _mutationResultPayloadPropInfo;
        private readonly PropertyInfo _mutationResultDataPropInfo;
        private readonly Lazy<SubmitInputTypeRegistry<TExecutionContext>> _submitInputTypeRegistry;

        private Type _submitOutputType = null!; // Initialized in AfterConfigure method

        protected Mutation()
        {
            _mutationResultType = new Dictionary<string, Type>
            {
                ["Payload"] = typeof(object),
                ["Data"] = typeof(object),
            }.MakeType(GeneratedMutationResultTypeName);
            _mutationResultPayloadPropInfo = _mutationResultType.GetProperty("Payload");
            _mutationResultDataPropInfo = _mutationResultType.GetProperty("Data");
            _submitInputTypeRegistry = new Lazy<SubmitInputTypeRegistry<TExecutionContext>>(() => Registry.GetRequiredService<SubmitInputTypeRegistry<TExecutionContext>>());
        }

        private SubmitInputTypeRegistry<TExecutionContext> SubmitInputTypeRegistry => _submitInputTypeRegistry.Value;

        internal async Task<IEnumerable<object>> DoAfterSave(IResolveFieldContext context, IEnumerable<object> entities)
        {
            var profiler = context.GetProfiler();

            using (profiler.Step(nameof(DoAfterSave)))
            {
                return await AfterSaveAsync(context.GetUserContext<TExecutionContext>(), entities).ConfigureAwait(false);
            }
        }

        internal object CreateSubmitOutput(IDictionary<string, IList<ISaveResultItem>> source)
        {
            var myObject = _submitOutputType.CreateInstanceAndHoistBaseException();

            foreach (var field in source)
            {
                var prop = _submitOutputType.GetProperty(field.Key);
                myObject.SetPropertyValue(prop, field.Value);
            }

            return myObject;
        }

        internal object CreateSubmitOutput<TDataType>(IDictionary<string, IList<ISaveResultItem>> source, TDataType data)
        {
            var result = _mutationResultType.CreateInstanceAndHoistBaseException();
            result.SetPropertyValue(_mutationResultPayloadPropInfo, CreateSubmitOutput(source));
            result.SetPropertyValue(_mutationResultDataPropInfo, data);
            return result;
        }

        protected internal void SubmitField<TLoader, TEntity>(string fieldName)
            where TLoader : Projection<TEntity, TExecutionContext>, IMutableLoader<TExecutionContext>
            where TEntity : class
        {
            SubmitField(typeof(TLoader), fieldName);
        }

        protected internal void SubmitField<TLoader, TEntity, TId>(string fieldName)
            where TLoader : MutableLoader<TEntity, TId, TExecutionContext>
            where TEntity : class
            where TId : IEquatable<TId>
        {
            SubmitInputTypeRegistry.Register<TLoader, TEntity, TId>(GetType(), fieldName);
        }

        protected internal void SubmitField(Type loaderType, string fieldName)
        {
            var baseLoaderType = TypeHelpers.FindMatchingGenericBaseType(loaderType, typeof(MutableLoader<,,>));
            if (baseLoaderType == null)
            {
                throw new ArgumentException($"Cannot find the corresponding base type for loader: {loaderType}");
            }

            SubmitInputTypeRegistry.Register(GetType(), fieldName, loaderType, baseLoaderType.GetGenericArguments()[0], baseLoaderType.GetGenericArguments()[1]);
        }

        protected internal new IMutationFieldBuilder<TExecutionContext> Field(string name, string? deprecationReason = null)
        {
            var field = AddField(name, deprecationReason);
            return new MutationFieldBuilder<Field<object, TExecutionContext>, TExecutionContext>(field);
        }

        protected override void AfterConfigure()
        {
            var inputTypeMap = SubmitInputTypeRegistry.GetInputTypeMap(GetType());
            if (inputTypeMap.Any())
            {
                _submitOutputType = inputTypeMap
                    .Keys
                    .ToDictionary(key => key, value => typeof(object))
                    .MakeType(GeneratedSubmitOutputTypeName, typeof(Input));

                SubmitField(
                    SubmitName,
                    GraphTypeDescriptor.Create<TExecutionContext>(typeof(SubmitOutputGraphType<,>).MakeGenericType(GetType(), typeof(TExecutionContext))),
                    PayloadName,
                    typeof(SubmitInputGraphType<,>).MakeGenericType(GetType(), typeof(TExecutionContext)),
                    PerformResolve,
                    _submitOutputType);
            }
        }

        protected virtual Task<IEnumerable<object>> AfterSaveAsync(TExecutionContext context, IEnumerable<object> entities)
        {
            return Task.FromResult(Enumerable.Empty<object>());
        }

        private void SubmitField(string name, IGraphTypeDescriptor<TExecutionContext> returnGraphType, string argName, Type argGraphType, Func<IResolveFieldContext, Dictionary<string, object>, Task<object>> resolve, Type fieldType)
        {
            ThrowIfIsNotConfiguring();
            Configurator.AddSubmitField(name, returnGraphType, argName, argGraphType, resolve, fieldType);
        }

        private async Task<object> PerformResolve(IResolveFieldContext context, Dictionary<string, object> payload)
        {
            var inputItems = new Dictionary<string, IEnumerable<IInputItem>>();

            foreach (var kv in payload)
            {
                var entityType = SubmitInputTypeRegistry.GetEntityTypeByFieldName(GetType(), kv.Key);
                var loaderType = SubmitInputTypeRegistry.GetLoaderTypeByFieldName(GetType(), kv.Key);
                var graphType = (IGraphType)Registry.GetRequiredService(Registry.GetInputEntityGraphType(loaderType, entityType));

                inputItems.Add(
                    kv.Key,
                    (kv.Value as System.Collections.IEnumerable)
                        .Cast<IDictionary<string, object>>()
                        .Select(entity => InputItem.Create(entityType, entity.ToObject(entityType, graphType), entity)));
            }

            var updater = new DbContextSaver<TExecutionContext>(Registry, this, context, inputItems);
            var result = await updater.SaveEntitiesAsync().ConfigureAwait(false);

            var response = CreateSubmitOutput(result);
            return response;
        }
    }
}

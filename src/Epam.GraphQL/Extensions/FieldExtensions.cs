// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using GraphQLParser.AST;

namespace Epam.GraphQL.Extensions
{
    internal static class FieldExtensions
    {
        public static IEnumerable<string> GetSubFieldsNames(this GraphQLField? field, IEnumerable<GraphQLFragmentDefinition> fragments, Func<GraphQLField, bool> predicate)
        {
            if (field == null)
            {
                return Enumerable.Empty<string>();
            }

            var result = new List<string>();

            if (field.SelectionSet != null)
            {
                result.AddRange(field.SelectionSet.Selections.OfType<GraphQLField>().Where(f => predicate(f) && !f.IsMeta()).Select(field => field.Name.StringValue));
            }

            var fragmentNames = field.SelectionSet?.Selections.OfType<GraphQLFragmentSpread>().Select(fragment => fragment.FragmentName.Name.StringValue);
            var fragmentFields = fragments
                .Where(fragment => fragmentNames.Contains(fragment.FragmentName.Name.StringValue))
                .SelectMany(f => GetFragmentFields(fragments, f, predicate))
                .Select(f => f.Name.StringValue);
            result.AddRange(fragmentFields);

            return result;
        }

        public static bool IsMeta(this GraphQLField field) => field.Name.StringValue.Equals("__typename", StringComparison.Ordinal);

        private static IEnumerable<GraphQLField> GetFragmentFields(IEnumerable<GraphQLFragmentDefinition> fragments, GraphQLFragmentDefinition fragment, Func<GraphQLField, bool> predicate)
        {
            var result = fragment.SelectionSet.Selections.OfType<GraphQLField>().Where(f => predicate(f) && !f.IsMeta())
                .Concat(fragment.SelectionSet.Selections.OfType<GraphQLFragmentSpread>().SelectMany(f => GetFragmentFields(fragments, fragments.Single(d => d.FragmentName.Name.StringValue == f.FragmentName.Name.StringValue), predicate)));

            return result;
        }
    }
}

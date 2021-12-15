// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Language.AST;

#nullable enable

namespace Epam.GraphQL.Extensions
{
    internal static class FieldExtensions
    {
        public static IEnumerable<string> GetSubFieldsNames(this Field? field, Fragments fragments, Func<Field, bool> predicate)
        {
            if (field == null)
            {
                return Enumerable.Empty<string>();
            }

            var result = new List<string>();
            result.AddRange(field.SelectionSet.Children.OfType<Field>().Where(f => predicate(f) && !f.IsMeta()).Select(field => field.Name));

            var fragmentNames = field.SelectionSet.Children.OfType<FragmentSpread>().Select(fragment => fragment.Name);
            var fragmentFields = fragments
                .Where(fragment => fragmentNames.Contains(fragment.Name))
                .SelectMany(f => GetFragmentFields(fragments, f, predicate))
                .Select(f => f.Name);
            result.AddRange(fragmentFields);

            return result;
        }

        public static bool IsMeta(this Field field) => field.Name.Equals("__typename", StringComparison.Ordinal);

        private static IEnumerable<Field> GetFragmentFields(Fragments fragments, FragmentDefinition fragment, Func<Field, bool> predicate)
        {
            var result = fragment.SelectionSet.Children.OfType<Field>().Where(f => predicate(f) && !f.IsMeta())
                .Concat(fragment.SelectionSet.Children.OfType<FragmentSpread>().SelectMany(f => GetFragmentFields(fragments, fragments.Single(d => d.Name == f.Name), predicate)));

            return result;
        }
    }
}

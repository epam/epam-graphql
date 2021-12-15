// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Extensions;

#nullable enable

namespace Epam.GraphQL.Helpers
{
    /// <summary>
    /// Defines methods to support the comparison of objects of type <see cref="ICollection{TElement}"/> for equality.
    /// <see cref="IEqualityComparer{T}"/> for <see cref="ICollection{TElement}"/>.
    /// </summary>
    internal sealed class CollectionEqualityComparer<TElement> : IEqualityComparer<ICollection<TElement>?>
    {
        private readonly IEqualityComparer<TElement> _elementComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionEqualityComparer{TElement}"/> class.
        /// </summary>
        public CollectionEqualityComparer(IEqualityComparer<TElement>? elementComparer = null)
        {
            _elementComparer = elementComparer ?? EqualityComparer<TElement>.Default;
        }

        /// <summary>
        /// Gets a default equality comparer for <see cref="ICollection{TElement}"/> type.
        /// </summary>
        /// <returns>The default instance of the <see cref="CollectionEqualityComparer{TElement}"/> type.</returns>
        public static CollectionEqualityComparer<TElement> Default { get; } = new CollectionEqualityComparer<TElement>();

        /// <summary>
        /// Determines whether the specified objects of type <see cref="ICollection{TElement}"/> are equal.
        /// </summary>
        /// <param name="x">The first object of type <see cref="ICollection{TElement}"/> to compare.</param>
        /// <param name="y">The second object of type <see cref="ICollection{TElement}"/> to compare.</param>
        /// <returns>true if the specified objects <see cref="ICollection{TElement}"/> are equal; otherwise, false.</returns>
        public bool Equals(ICollection<TElement>? x, ICollection<TElement>? y)
        {
            if (x == null)
            {
                return y == null;
            }

            if (y == null)
            {
                return false;
            }

            return x.Count == y.Count && x.SequenceEqual(y, _elementComparer);
        }

        /// <summary>
        /// Returns a hash code for the specified object of <see cref="ICollection{TElement}"/> type.
        /// </summary>
        /// <param name="obj">The object of type <see cref="ICollection{TElement}"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object of type <see cref="ICollection{TElement}"/> .</returns>
        public int GetHashCode(ICollection<TElement>? obj)
        {
            var hashCode = default(HashCode);
            if (obj != null)
            {
                obj.ForEach(item => hashCode.Add(item, _elementComparer));
            }

            return hashCode.ToHashCode();
        }
    }
}

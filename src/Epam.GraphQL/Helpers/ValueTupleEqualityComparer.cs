// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;

namespace Epam.GraphQL.Helpers
{
#pragma warning disable SA1414 // Tuple types in signatures should have element names

    /// <summary>
    /// Defines methods to support the comparison of objects of type <see cref="ValueTuple{T1, T2}"/> for equality.
    /// <see cref="IEqualityComparer{T}"/> for <see cref="ValueTuple{T1, T2}"/>.
    /// </summary>
    internal sealed class ValueTupleEqualityComparer<T1> : IEqualityComparer<ValueTuple<T1>>
    {
        private readonly IEqualityComparer<T1> _firstItemComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTupleEqualityComparer{T1}"/> class.
        /// </summary>
        public ValueTupleEqualityComparer(IEqualityComparer<T1>? firstItemComparer = null)
        {
            _firstItemComparer = firstItemComparer ?? EqualityComparer<T1>.Default;
        }

        /// <summary>
        /// Gets a default equality comparer for <see cref="ValueTuple{T1}"/> type.
        /// </summary>
        /// <returns>The default instance of the <see cref="ValueTupleEqualityComparer{T1, T2}"/> type.</returns>
        public static ValueTupleEqualityComparer<T1> Default { get; } = new ValueTupleEqualityComparer<T1>();

        /// <summary>
        /// Determines whether the specified objects of type <see cref="ValueTuple{T1}"/> are equal.
        /// </summary>
        /// <param name="x">The first object of type <see cref="ValueTuple{T1}"/> to compare.</param>
        /// <param name="y">The second object of type <see cref="ValueTuple{T1}"/> to compare.</param>
        /// <returns>true if the specified objects <see cref="ValueTuple{T1}"/> are equal; otherwise, false.</returns>
        public bool Equals(ValueTuple<T1> x, ValueTuple<T1> y) => _firstItemComparer.Equals(x.Item1, y.Item1);

        /// <summary>
        /// Returns a hash code for the specified object of <see cref="ValueTuple{T1}"/> type.
        /// </summary>
        /// <param name="obj">The object of type <see cref="ValueTuple{T1}"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object of type <see cref="ValueTuple{T1}"/> .</returns>
        public int GetHashCode(ValueTuple<T1> obj) => _firstItemComparer.GetHashCode(obj.Item1);
    }

    /// <summary>
    /// Defines methods to support the comparison of objects of type <see cref="ValueTuple{T1, T2}"/> for equality.
    /// <see cref="IEqualityComparer{T}"/> for <see cref="ValueTuple{T1, T2}"/>.
    /// </summary>
    internal sealed class ValueTupleEqualityComparer<T1, T2> : IEqualityComparer<ValueTuple<T1, T2>>
    {
        private readonly IEqualityComparer<T1> _firstItemComparer;
        private readonly IEqualityComparer<T2> _secondItemComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTupleEqualityComparer{T1, T2}"/> class.
        /// </summary>
        public ValueTupleEqualityComparer(IEqualityComparer<T1>? firstItemComparer = null, IEqualityComparer<T2>? secondItemComparer = null)
        {
            _firstItemComparer = firstItemComparer ?? EqualityComparer<T1>.Default;
            _secondItemComparer = secondItemComparer ?? EqualityComparer<T2>.Default;
        }

        /// <summary>
        /// Gets a default equality comparer for <see cref="ValueTuple{T1, T2}"/> type.
        /// </summary>
        /// <returns>The default instance of the <see cref="ValueTupleEqualityComparer{T1, T2}"/> type.</returns>
        public static ValueTupleEqualityComparer<T1, T2> Default { get; } = new ValueTupleEqualityComparer<T1, T2>();

        /// <summary>
        /// Determines whether the specified objects of type <see cref="ValueTuple{T1, T2}"/> are equal.
        /// </summary>
        /// <param name="x">The first object of type <see cref="ValueTuple{T1, T2}"/> to compare.</param>
        /// <param name="y">The second object of type <see cref="ValueTuple{T1, T2}"/> to compare.</param>
        /// <returns>true if the specified objects <see cref="ValueTuple{T1, T2}"/> are equal; otherwise, false.</returns>
        public bool Equals((T1, T2) x, (T1, T2) y) => _firstItemComparer.Equals(x.Item1, y.Item1) && _secondItemComparer.Equals(x.Item2, y.Item2);

        /// <summary>
        /// Returns a hash code for the specified object of <see cref="ValueTuple{T1, T2}"/> type.
        /// </summary>
        /// <param name="obj">The object of type <see cref="ValueTuple{T1, T2}"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object of type <see cref="ValueTuple{T1, T2}"/> .</returns>
        public int GetHashCode((T1, T2) obj)
        {
            var result = default(HashCode);
            result.Add(obj.Item1, _firstItemComparer);
            result.Add(obj.Item2, _secondItemComparer);
            return result.ToHashCode();
        }
    }

    /// <summary>
    /// Defines methods to support the comparison of objects of type <see cref="ValueTuple{T1, T2, T3}"/> for equality.
    /// <see cref="IEqualityComparer{T}"/> for <see cref="ValueTuple{T1, T2, T3}"/>.
    /// </summary>
    internal sealed class ValueTupleEqualityComparer<T1, T2, T3> : IEqualityComparer<ValueTuple<T1, T2, T3>>
    {
        private readonly IEqualityComparer<T1> _firstItemComparer;
        private readonly IEqualityComparer<T2> _secondItemComparer;
        private readonly IEqualityComparer<T3> _thirdItemComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTupleEqualityComparer{T1, T2, T3}"/> class.
        /// </summary>
        public ValueTupleEqualityComparer(IEqualityComparer<T1>? firstItemComparer = null, IEqualityComparer<T2>? secondItemComparer = null, IEqualityComparer<T3>? thirdItemComparer = null)
        {
            _firstItemComparer = firstItemComparer ?? EqualityComparer<T1>.Default;
            _secondItemComparer = secondItemComparer ?? EqualityComparer<T2>.Default;
            _thirdItemComparer = thirdItemComparer ?? EqualityComparer<T3>.Default;
        }

        /// <summary>
        /// Gets a default equality comparer for <see cref="ValueTuple{T1, T2, T3}"/> type.
        /// </summary>
        /// <returns>The default instance of the <see cref="ValueTupleEqualityComparer{T1, T2, T3}"/> type.</returns>
        public static ValueTupleEqualityComparer<T1, T2, T3> Default { get; } = new ValueTupleEqualityComparer<T1, T2, T3>();

        /// <summary>
        /// Determines whether the specified objects of type <see cref="ValueTuple{T1, T2, T3}"/> are equal.
        /// </summary>
        /// <param name="x">The first object of type <see cref="ValueTuple{T1, T2, T3}"/> to compare.</param>
        /// <param name="y">The second object of type <see cref="ValueTuple{T1, T2, T3}"/> to compare.</param>
        /// <returns>true if the specified objects <see cref="ValueTuple{T1, T2, T3}"/> are equal; otherwise, false.</returns>
        public bool Equals((T1, T2, T3) x, (T1, T2, T3) y) => _firstItemComparer.Equals(x.Item1, y.Item1) && _secondItemComparer.Equals(x.Item2, y.Item2) && _thirdItemComparer.Equals(x.Item3, y.Item3);

        /// <summary>
        /// Returns a hash code for the specified object of <see cref="ValueTuple{T1, T2, T3}"/> type.
        /// </summary>
        /// <param name="obj">The object of type <see cref="ValueTuple{T1, T2, T3}"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object of type <see cref="ValueTuple{T1, T2, T3}"/> .</returns>
        public int GetHashCode((T1, T2, T3) obj)
        {
            var result = default(HashCode);
            result.Add(obj.Item1, _firstItemComparer);
            result.Add(obj.Item2, _secondItemComparer);
            result.Add(obj.Item3, _thirdItemComparer);
            return result.ToHashCode();
        }
    }

    /// <summary>
    /// Defines methods to support the comparison of objects of type <see cref="ValueTuple{T1, T2, T3, T4}"/> for equality.
    /// <see cref="IEqualityComparer{T}"/> for <see cref="ValueTuple{T1, T2, T3, T4}"/>.
    /// </summary>
    internal sealed class ValueTupleEqualityComparer<T1, T2, T3, T4> : IEqualityComparer<ValueTuple<T1, T2, T3, T4>>
    {
        private readonly IEqualityComparer<T1> _firstItemComparer;
        private readonly IEqualityComparer<T2> _secondItemComparer;
        private readonly IEqualityComparer<T3> _thirdItemComparer;
        private readonly IEqualityComparer<T4> _fourthItemComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTupleEqualityComparer{T1, T2, T3, T4}"/> class.
        /// </summary>
        public ValueTupleEqualityComparer(IEqualityComparer<T1>? firstItemComparer = null, IEqualityComparer<T2>? secondItemComparer = null, IEqualityComparer<T3>? thirdItemComparer = null, IEqualityComparer<T4>? fourthItemComparer = null)
        {
            _firstItemComparer = firstItemComparer ?? EqualityComparer<T1>.Default;
            _secondItemComparer = secondItemComparer ?? EqualityComparer<T2>.Default;
            _thirdItemComparer = thirdItemComparer ?? EqualityComparer<T3>.Default;
            _fourthItemComparer = fourthItemComparer ?? EqualityComparer<T4>.Default;
        }

        /// <summary>
        /// Gets a default equality comparer for <see cref="ValueTuple{T1, T2, T3, T4}"/> type.
        /// </summary>
        /// <returns>The default instance of the <see cref="ValueTupleEqualityComparer{T1, T2, T3, T4}"/> type.</returns>
        public static ValueTupleEqualityComparer<T1, T2, T3, T4> Default { get; } = new ValueTupleEqualityComparer<T1, T2, T3, T4>();

        /// <summary>
        /// Determines whether the specified objects of type <see cref="ValueTuple{T1, T2, T3, T4}"/> are equal.
        /// </summary>
        /// <param name="x">The first object of type <see cref="ValueTuple{T1, T2, T3, T4}"/> to compare.</param>
        /// <param name="y">The second object of type <see cref="ValueTuple{T1, T2, T3, T4}"/> to compare.</param>
        /// <returns>true if the specified objects <see cref="ValueTuple{T1, T2, T3, T4}"/> are equal; otherwise, false.</returns>
        public bool Equals((T1, T2, T3, T4) x, (T1, T2, T3, T4) y) => _firstItemComparer.Equals(x.Item1, y.Item1) && _secondItemComparer.Equals(x.Item2, y.Item2) && _thirdItemComparer.Equals(x.Item3, y.Item3) && _fourthItemComparer.Equals(x.Item4, y.Item4);

        /// <summary>
        /// Returns a hash code for the specified object of <see cref="ValueTuple{T1, T2, T3, T4}"/> type.
        /// </summary>
        /// <param name="obj">The object of type <see cref="ValueTuple{T1, T2, T3, T4}"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object of type <see cref="ValueTuple{T1, T2, T3, T4}"/> .</returns>
        public int GetHashCode((T1, T2, T3, T4) obj)
        {
            var result = default(HashCode);
            result.Add(obj.Item1, _firstItemComparer);
            result.Add(obj.Item2, _secondItemComparer);
            result.Add(obj.Item3, _thirdItemComparer);
            result.Add(obj.Item4, _fourthItemComparer);
            return result.ToHashCode();
        }
    }

    /// <summary>
    /// Defines methods to support the comparison of objects of type <see cref="ValueTuple{T1, T2, T3, T4, T5}"/> for equality.
    /// <see cref="IEqualityComparer{T}"/> for <see cref="ValueTuple{T1, T2, T3, T4, T5}"/>.
    /// </summary>
    internal sealed class ValueTupleEqualityComparer<T1, T2, T3, T4, T5> : IEqualityComparer<ValueTuple<T1, T2, T3, T4, T5>>
    {
        private readonly IEqualityComparer<T1> _firstItemComparer;
        private readonly IEqualityComparer<T2> _secondItemComparer;
        private readonly IEqualityComparer<T3> _thirdItemComparer;
        private readonly IEqualityComparer<T4> _fourthItemComparer;
        private readonly IEqualityComparer<T5> _fifthItemComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTupleEqualityComparer{T1, T2, T3, T4, T5}"/> class.
        /// </summary>
        public ValueTupleEqualityComparer(IEqualityComparer<T1>? firstItemComparer = null, IEqualityComparer<T2>? secondItemComparer = null, IEqualityComparer<T3>? thirdItemComparer = null, IEqualityComparer<T4>? fourthItemComparer = null, IEqualityComparer<T5>? fifthItemComparer = null)
        {
            _firstItemComparer = firstItemComparer ?? EqualityComparer<T1>.Default;
            _secondItemComparer = secondItemComparer ?? EqualityComparer<T2>.Default;
            _thirdItemComparer = thirdItemComparer ?? EqualityComparer<T3>.Default;
            _fourthItemComparer = fourthItemComparer ?? EqualityComparer<T4>.Default;
            _fifthItemComparer = fifthItemComparer ?? EqualityComparer<T5>.Default;
        }

        /// <summary>
        /// Gets a default equality comparer for <see cref="ValueTuple{T1, T2, T3, T4, T5}"/> type.
        /// </summary>
        /// <returns>The default instance of the <see cref="ValueTupleEqualityComparer{T1, T2, T3, T4, T5}"/> type.</returns>
        public static ValueTupleEqualityComparer<T1, T2, T3, T4, T5> Default { get; } = new ValueTupleEqualityComparer<T1, T2, T3, T4, T5>();

        /// <summary>
        /// Determines whether the specified objects of type <see cref="ValueTuple{T1, T2, T3, T4, T5}"/> are equal.
        /// </summary>
        /// <param name="x">The first object of type <see cref="ValueTuple{T1, T2, T3, T4, T5}"/> to compare.</param>
        /// <param name="y">The second object of type <see cref="ValueTuple{T1, T2, T3, T4, T5}"/> to compare.</param>
        /// <returns>true if the specified objects <see cref="ValueTuple{T1, T2, T3, T4, T5}"/> are equal; otherwise, false.</returns>
        public bool Equals((T1, T2, T3, T4, T5) x, (T1, T2, T3, T4, T5) y) => _firstItemComparer.Equals(x.Item1, y.Item1) && _secondItemComparer.Equals(x.Item2, y.Item2) && _thirdItemComparer.Equals(x.Item3, y.Item3) && _fourthItemComparer.Equals(x.Item4, y.Item4) && _fifthItemComparer.Equals(x.Item5, y.Item5);

        /// <summary>
        /// Returns a hash code for the specified object of <see cref="ValueTuple{T1, T2, T3, T4, T5}"/> type.
        /// </summary>
        /// <param name="obj">The object of type <see cref="ValueTuple{T1, T2, T3, T4, T5}"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object of type <see cref="ValueTuple{T1, T2, T3, T4, T5}"/> .</returns>
        public int GetHashCode((T1, T2, T3, T4, T5) obj)
        {
            var result = default(HashCode);
            result.Add(obj.Item1, _firstItemComparer);
            result.Add(obj.Item2, _secondItemComparer);
            result.Add(obj.Item3, _thirdItemComparer);
            result.Add(obj.Item4, _fourthItemComparer);
            result.Add(obj.Item5, _fifthItemComparer);
            return result.ToHashCode();
        }
    }

    /// <summary>
    /// Defines methods to support the comparison of objects of type <see cref="ValueTuple{T1, T2, T3, T4, T5, T6}"/> for equality.
    /// <see cref="IEqualityComparer{T}"/> for <see cref="ValueTuple{T1, T2, T3, T4, T5, T6}"/>.
    /// </summary>
    internal sealed class ValueTupleEqualityComparer<T1, T2, T3, T4, T5, T6> : IEqualityComparer<ValueTuple<T1, T2, T3, T4, T5, T6>>
    {
        private readonly IEqualityComparer<T1> _firstItemComparer;
        private readonly IEqualityComparer<T2> _secondItemComparer;
        private readonly IEqualityComparer<T3> _thirdItemComparer;
        private readonly IEqualityComparer<T4> _fourthItemComparer;
        private readonly IEqualityComparer<T5> _fifthItemComparer;
        private readonly IEqualityComparer<T6> _sixthItemComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTupleEqualityComparer{T1, T2, T3, T4, T5, T6}"/> class.
        /// </summary>
        public ValueTupleEqualityComparer(IEqualityComparer<T1>? firstItemComparer = null, IEqualityComparer<T2>? secondItemComparer = null, IEqualityComparer<T3>? thirdItemComparer = null, IEqualityComparer<T4>? fourthItemComparer = null, IEqualityComparer<T5>? fifthItemComparer = null, IEqualityComparer<T6>? sixthItemComparer = null)
        {
            _firstItemComparer = firstItemComparer ?? EqualityComparer<T1>.Default;
            _secondItemComparer = secondItemComparer ?? EqualityComparer<T2>.Default;
            _thirdItemComparer = thirdItemComparer ?? EqualityComparer<T3>.Default;
            _fourthItemComparer = fourthItemComparer ?? EqualityComparer<T4>.Default;
            _fifthItemComparer = fifthItemComparer ?? EqualityComparer<T5>.Default;
            _sixthItemComparer = sixthItemComparer ?? EqualityComparer<T6>.Default;
        }

        /// <summary>
        /// Gets a default equality comparer for <see cref="ValueTuple{T1, T2, T3, T4, T5, T6}"/> type.
        /// </summary>
        /// <returns>The default instance of the <see cref="ValueTupleEqualityComparer{T1, T2, T3, T4, T5, T6}"/> type.</returns>
        public static ValueTupleEqualityComparer<T1, T2, T3, T4, T5, T6> Default { get; } = new ValueTupleEqualityComparer<T1, T2, T3, T4, T5, T6>();

        /// <summary>
        /// Determines whether the specified objects of type <see cref="ValueTuple{T1, T2, T3, T4, T5, T6}"/> are equal.
        /// </summary>
        /// <param name="x">The first object of type <see cref="ValueTuple{T1, T2, T3, T4, T5, T6}"/> to compare.</param>
        /// <param name="y">The second object of type <see cref="ValueTuple{T1, T2, T3, T4, T5, T6}"/> to compare.</param>
        /// <returns>true if the specified objects <see cref="ValueTuple{T1, T2, T3, T4, T5, T6}"/> are equal; otherwise, false.</returns>
        public bool Equals((T1, T2, T3, T4, T5, T6) x, (T1, T2, T3, T4, T5, T6) y) => _firstItemComparer.Equals(x.Item1, y.Item1) && _secondItemComparer.Equals(x.Item2, y.Item2) && _thirdItemComparer.Equals(x.Item3, y.Item3) && _fourthItemComparer.Equals(x.Item4, y.Item4) && _fifthItemComparer.Equals(x.Item5, y.Item5) && _sixthItemComparer.Equals(x.Item6, y.Item6);

        /// <summary>
        /// Returns a hash code for the specified object of <see cref="ValueTuple{T1, T2, T3, T4, T5, T6}"/> type.
        /// </summary>
        /// <param name="obj">The object of type <see cref="ValueTuple{T1, T2, T3, T4, T5, T6}"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object of type <see cref="ValueTuple{T1, T2, T3, T4, T5, T6}"/> .</returns>
        public int GetHashCode((T1, T2, T3, T4, T5, T6) obj)
        {
            var result = default(HashCode);
            result.Add(obj.Item1, _firstItemComparer);
            result.Add(obj.Item2, _secondItemComparer);
            result.Add(obj.Item3, _thirdItemComparer);
            result.Add(obj.Item4, _fourthItemComparer);
            result.Add(obj.Item5, _fifthItemComparer);
            result.Add(obj.Item6, _sixthItemComparer);
            return result.ToHashCode();
        }
    }

#pragma warning restore SA1414 // Tuple types in signatures should have element names
}

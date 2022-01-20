// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Adapters;
using Epam.GraphQL.Infrastructure;
using Epam.GraphQL.Relay;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Infrastructure
{
    [TestFixture]
    public class ConnectionUtilsTests
    {
        private QueryExecuter _executer;

        public static IEnumerable<TestCaseData> HappyPathTestData
        {
            get
            {
                var allData = Items(0, 19).ToList();
                var emptyData = Items(0, -1).ToList();

                // No pagination
                yield return new TestCaseData(new TestData { Data = allData, ShouldComputeTotalCount = true })
                    .SetName("None({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = false, Items = allData, StartCursor = 0, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { Data = emptyData, ShouldComputeTotalCount = true })
                    .SetName("None({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = false, Items = emptyData, StartCursor = null, EndCursor = null, TotalCount = 0 });

                yield return new TestCaseData(new TestData { Data = allData, ShouldComputeTotalCount = false, ShouldComputeEndOffset = false })
                    .SetName("None({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = false, Items = allData, StartCursor = 0, EndCursor = null, TotalCount = null });

                yield return new TestCaseData(new TestData { Data = emptyData, ShouldComputeTotalCount = false, ShouldComputeEndOffset = false })
                    .SetName("None({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = false, Items = emptyData, StartCursor = 0, EndCursor = null, TotalCount = null });

                // First only
                yield return new TestCaseData(new TestData { First = 10, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("First({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = true, Items = Items(0, 9), StartCursor = 0, EndCursor = 9 });

                yield return new TestCaseData(new TestData { First = 20, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("First({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = false, Items = allData, StartCursor = 0, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { First = 30, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("First({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = false, Items = allData, StartCursor = 0, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { First = 0, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("First({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = true, Items = emptyData, StartCursor = 0, EndCursor = 0 });

                // Before only
                yield return new TestCaseData(new TestData { Before = 10, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("Before({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = true, Items = Items(0, 9), StartCursor = 0, EndCursor = 9 });

                yield return new TestCaseData(new TestData { Before = 0, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("Before({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = false, Items = emptyData, StartCursor = null, EndCursor = null });

                yield return new TestCaseData(new TestData { Before = 19, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("Before({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = true, Items = Items(0, 18), StartCursor = 0, EndCursor = 18 });

                yield return new TestCaseData(new TestData { Before = -10, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("Before({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = false, Items = allData, StartCursor = 0, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { Before = -1, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("Before({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = false, Items = allData, StartCursor = 0, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { Before = 20, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("Before({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = false, Items = allData, StartCursor = 0, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { Before = 30, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("Before({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = false, Items = allData, StartCursor = 0, EndCursor = 19, TotalCount = allData.Count });

                // Before and first
                yield return new TestCaseData(new TestData { First = 10, Before = 10, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = true, Items = Items(0, 9), StartCursor = 0, EndCursor = 9 });

                yield return new TestCaseData(new TestData { First = 25, Before = 25, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = false, Items = allData, StartCursor = 0, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { First = 0, Before = 0, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = false, Items = emptyData, StartCursor = null, EndCursor = null });

                yield return new TestCaseData(new TestData { First = 10, Before = 15, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = true, Items = Items(0, 9), StartCursor = 0, EndCursor = 9 });

                yield return new TestCaseData(new TestData { First = 10, Before = 25, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = true, Items = Items(0, 9), StartCursor = 0, EndCursor = 9 });

                yield return new TestCaseData(new TestData { First = 10, Before = 5, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = true, Items = Items(0, 4), StartCursor = 0, EndCursor = 4 });

                yield return new TestCaseData(new TestData { First = 25, Before = 5, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = true, Items = Items(0, 4), StartCursor = 0, EndCursor = 4 });

                yield return new TestCaseData(new TestData { First = 5, Before = -1, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = true, Items = Items(0, 4), StartCursor = 0, EndCursor = 4 });

                yield return new TestCaseData(new TestData { First = 0, Before = 10, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = true, Items = emptyData, StartCursor = 0, EndCursor = 0 });

                // After only
                yield return new TestCaseData(new TestData { After = 9, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = false, Items = Items(10, 19), StartCursor = 10, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { After = 0, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = false, Items = Items(1, 19), StartCursor = 1, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { After = 19, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = false, Items = emptyData, StartCursor = null, EndCursor = null, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { After = -10, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = false, Items = allData, StartCursor = 0, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { After = -1, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = false, Items = allData, StartCursor = 0, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { After = 20, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = false, Items = allData, StartCursor = 0, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { After = 30, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = false, Items = allData, StartCursor = 0, EndCursor = 19, TotalCount = allData.Count });

                // After and before
                yield return new TestCaseData(new TestData { After = 4, Before = 15, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(5, 14), StartCursor = 5, EndCursor = 14 });

                yield return new TestCaseData(new TestData { After = 9, Before = 19, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(10, 18), StartCursor = 10, EndCursor = 18 });

                yield return new TestCaseData(new TestData { After = 0, Before = 10, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(1, 9), StartCursor = 1, EndCursor = 9 });

                yield return new TestCaseData(new TestData { After = 9, Before = -1, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After/Before({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = false, Items = Items(10, 19), StartCursor = 10, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { After = 9, Before = 20, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After/Before({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = false, Items = Items(10, 19), StartCursor = 10, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { After = 15, Before = 25, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After/Before({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = false, Items = Items(16, 19), StartCursor = 16, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { After = -1, Before = 10, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = true, Items = Items(0, 9), StartCursor = 0, EndCursor = 9 });

                yield return new TestCaseData(new TestData { After = 20, Before = 10, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = true, Items = Items(0, 9), StartCursor = 0, EndCursor = 9 });

                // After and first
                yield return new TestCaseData(new TestData { First = 5, After = 9, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/First({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(10, 14), StartCursor = 10, EndCursor = 14 });

                yield return new TestCaseData(new TestData { First = 10, After = 9, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After/First({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = false, Items = Items(10, 19), StartCursor = 10, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { First = 20, After = 9, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After/First({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = false, Items = Items(10, 19), StartCursor = 10, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { First = 10, After = 15, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After/First({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = false, Items = Items(16, 19), StartCursor = 16, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { First = 10, After = -1, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/First({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = true, Items = Items(0, 9), StartCursor = 0, EndCursor = 9 });

                yield return new TestCaseData(new TestData { First = 10, After = 20, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/First({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = true, Items = Items(0, 9), StartCursor = 0, EndCursor = 9 });

                yield return new TestCaseData(new TestData { First = 10, After = 19, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After/First({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = false, Items = emptyData, StartCursor = null, EndCursor = null, TotalCount = allData.Count });

                // After, before and first
                yield return new TestCaseData(new TestData { After = 4, Before = 15, First = 5, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(5, 9), StartCursor = 5, EndCursor = 9 });

                yield return new TestCaseData(new TestData { After = 4, Before = 15, First = 15, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(5, 14), StartCursor = 5, EndCursor = 14 });

                yield return new TestCaseData(new TestData { After = 4, Before = 15, First = 0, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = emptyData, StartCursor = 5, EndCursor = 5 });

                yield return new TestCaseData(new TestData { After = 9, Before = 19, First = 5, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(10, 14), StartCursor = 10, EndCursor = 14 });

                yield return new TestCaseData(new TestData { After = 9, Before = 19, First = 15, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(10, 18), StartCursor = 10, EndCursor = 18 });

                yield return new TestCaseData(new TestData { After = 9, Before = 19, First = 0, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = emptyData, StartCursor = 10, EndCursor = 10 });

                yield return new TestCaseData(new TestData { After = 0, Before = 10, First = 5, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(1, 5), StartCursor = 1, EndCursor = 5 });

                yield return new TestCaseData(new TestData { After = 0, Before = 10, First = 15, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(1, 9), StartCursor = 1, EndCursor = 9 });

                yield return new TestCaseData(new TestData { After = 0, Before = 10, First = 0, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = emptyData, StartCursor = 1, EndCursor = 1 });

                yield return new TestCaseData(new TestData { After = 9, Before = -1, First = 5, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(10, 14), StartCursor = 10, EndCursor = 14 });

                yield return new TestCaseData(new TestData { After = 9, Before = 20, First = 5, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(10, 14), StartCursor = 10, EndCursor = 14 });

                yield return new TestCaseData(new TestData { After = 15, Before = 25, First = 3, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(16, 18), StartCursor = 16, EndCursor = 18 });

                yield return new TestCaseData(new TestData { After = -1, Before = 10, First = 5, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = true, Items = Items(0, 4), StartCursor = 0, EndCursor = 4 });

                yield return new TestCaseData(new TestData { After = 20, Before = 10, First = 5, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/First({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = true, Items = Items(0, 4), StartCursor = 0, EndCursor = 4 });

                // Last only
                yield return new TestCaseData(new TestData { Last = 10, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = false, Items = Items(10, 19), StartCursor = 10, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { Last = 20, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("Last({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = false, Items = allData, StartCursor = 0, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { Last = 30, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("Last({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = false, Items = allData, StartCursor = 0, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { Last = 0, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("Last({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = false, Items = emptyData, StartCursor = null, EndCursor = null, TotalCount = allData.Count });

                // After and last
                yield return new TestCaseData(new TestData { After = 0, Last = 10, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = false, Items = Items(10, 19), StartCursor = 10, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { After = -1, Last = 10, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = false, Items = Items(10, 19), StartCursor = 10, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { After = 9, Last = 10, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = false, Items = Items(10, 19), StartCursor = 10, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { After = 10, Last = 10, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = false, Items = Items(11, 19), StartCursor = 11, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { After = 10, Last = 0, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = false, Items = emptyData, StartCursor = null, EndCursor = null, TotalCount = allData.Count });

                // Before and last
                yield return new TestCaseData(new TestData { Before = 19, Last = 10, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("Before/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(9, 18), StartCursor = 9, EndCursor = 18 });

                yield return new TestCaseData(new TestData { Before = 20, Last = 10, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("Before/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = false, Items = Items(10, 19), StartCursor = 10, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { Before = 11, Last = 10, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("Before/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(1, 10), StartCursor = 1, EndCursor = 10 });

                yield return new TestCaseData(new TestData { Before = 10, Last = 10, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("Before/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = false, HasNextPage = true, Items = Items(0, 9), StartCursor = 0, EndCursor = 9 });

                yield return new TestCaseData(new TestData { Before = 10, Last = 5, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("Before/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(5, 9), StartCursor = 5, EndCursor = 9 });

                yield return new TestCaseData(new TestData { Before = 10, Last = 0, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("Before/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = emptyData, StartCursor = 10, EndCursor = 10 });

                // After, before and last
                yield return new TestCaseData(new TestData { After = 4, Before = 15, Last = 5, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(10, 14), StartCursor = 10, EndCursor = 14 });

                yield return new TestCaseData(new TestData { After = 4, Before = 15, Last = 15, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(5, 14), StartCursor = 5, EndCursor = 14 });

                yield return new TestCaseData(new TestData { After = 4, Before = 15, Last = 0, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = emptyData, StartCursor = 15, EndCursor = 15 });

                yield return new TestCaseData(new TestData { After = 9, Before = 19, Last = 5, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(14, 18), StartCursor = 14, EndCursor = 18 });

                yield return new TestCaseData(new TestData { After = 9, Before = 19, Last = 15, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(10, 18), StartCursor = 10, EndCursor = 18 });

                yield return new TestCaseData(new TestData { After = 9, Before = 19, Last = 0, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = emptyData, StartCursor = 19, EndCursor = 19 });

                yield return new TestCaseData(new TestData { After = 0, Before = 10, Last = 5, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(5, 9), StartCursor = 5, EndCursor = 9 });

                yield return new TestCaseData(new TestData { After = 0, Before = 10, Last = 15, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(1, 9), StartCursor = 1, EndCursor = 9 });

                yield return new TestCaseData(new TestData { After = 0, Before = 10, Last = 0, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = emptyData, StartCursor = 10, EndCursor = 10 });

                yield return new TestCaseData(new TestData { After = 9, Before = -1, Last = 5, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After/Before/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = false, Items = Items(15, 19), StartCursor = 15, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { After = 9, Before = 20, Last = 5, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After/Before/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = false, Items = Items(15, 19), StartCursor = 15, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { After = 15, Before = 25, Last = 3, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After/Before/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = false, Items = Items(17, 19), StartCursor = 17, EndCursor = 19, TotalCount = allData.Count });

                yield return new TestCaseData(new TestData { After = -1, Before = 10, Last = 5, Data = allData, ShouldComputeTotalCount = false })
                    .SetName("After/Before/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = true, Items = Items(5, 9), StartCursor = 5, EndCursor = 9 });

                yield return new TestCaseData(new TestData { After = 20, Before = 10, Last = 5, Data = allData, ShouldComputeTotalCount = true })
                    .SetName("After/Before/Last({0})")
                    .Returns(new TestResult { HasPreviousPage = true, HasNextPage = false, Items = Items(15, 19), StartCursor = 15, EndCursor = 19, TotalCount = allData.Count });
            }
        }

        public static IEnumerable<TestCaseData> ErroneousPathTestData
        {
            get
            {
                yield return new TestCaseData(
                    new TestData { First = -1 },
                    typeof(ArgumentOutOfRangeException),
                    "`first` is out of range. Must be non-negative or null. (Parameter 'first')")
                    .SetName("First({0})");

                yield return new TestCaseData(
                    new TestData { Last = -1 },
                    typeof(ArgumentOutOfRangeException),
                    "`last` is out of range. Must be non-negative or null. (Parameter 'last')")
                    .SetName("Last({0})");

                yield return new TestCaseData(
                    new TestData { First = 1, Last = 1 },
                    typeof(ArgumentException),
                    "Cannot use `first` in conjunction with `last`.")
                    .SetName("First/Last({0})");
            }
        }

        [SetUp]
        public void SetUp()
        {
            var logger = Substitute.For<ILogger>();
            var asyncEnumerableConverter = Substitute.For<IQueryableToAsyncEnumerableConverter>();
            var asNoTrackingConverter = Substitute.For<IQueryableToAsNoTrackingQueryableConverter>();

            asNoTrackingConverter.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<int>>(0));

            _executer = new QueryExecuter(logger, asyncEnumerableConverter, asNoTrackingConverter);
        }

        [TestCaseSource(typeof(ConnectionUtilsTests), nameof(HappyPathTestData))]
        public TestResult HappyPath(TestData arguments)
        {
            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            var connection = ConnectionUtils.ToConnection(
                arguments.Data.AsQueryable().OrderBy(p => p),
                () => string.Empty,
                _executer,
                arguments.First,
                arguments.Last,
                arguments.Before,
                arguments.After,
                arguments.ShouldComputeTotalCount,
                arguments.ShouldComputeEndOffset,
                true,
                true);

            return new TestResult(connection);
        }

        [TestCaseSource(typeof(ConnectionUtilsTests), nameof(ErroneousPathTestData))]
        public void ErroneousPath(TestData arguments, Type expectedExceptionType, string expectedMessage)
        {
            var dummyQuery = Enumerable.Empty<int>();
            Assert.Throws(
                Is.TypeOf(expectedExceptionType).And.Message.EqualTo(expectedMessage),
                () => ConnectionUtils.ToConnection(
                    dummyQuery.AsQueryable().OrderBy(p => p),
                    () => string.Empty,
                    _executer,
                    arguments.First,
                    arguments.Last,
                    arguments.Before,
                    arguments.After,
                    arguments.ShouldComputeTotalCount,
                    arguments.ShouldComputeEndOffset,
                    true,
                    true));
        }

        private static string Convert<T>(IEnumerable<T> items)
        {
            if (items == null)
            {
                return null;
            }

            if (items.Count() <= 4)
            {
                return $"[{string.Join(", ", items)}]";
            }

            return $"[{string.Join(", ", items.Take(3))}, ..., {items.Last()}]";
        }

        private static IEnumerable<int> Items(int first, int last)
        {
            return Enumerable.Range(first, last - first + 1);
        }

        public class TestData
        {
            public int? First { get; set; }

            public int? Last { get; set; }

            public int? Before { get; set; }

            public int? After { get; set; }

            public IEnumerable<int> Data { get; set; }

            public bool ShouldComputeEndOffset { get; set; } = true;

            public bool ShouldComputeTotalCount { get; set; }

            public override string ToString()
            {
                var result = string.Empty;

                void Add(string name, object value)
                {
                    if (value != null)
                    {
                        if (value.Equals(false))
                        {
                            return;
                        }

                        if (result.Length > 0)
                        {
                            result += ", ";
                        }

                        if (value.Equals(true))
                        {
                            result += name;
                            return;
                        }

                        result += $"{name}: {value}";
                    }
                }

                Add(nameof(After), After);
                Add(nameof(Before), Before);
                Add(nameof(First), First);
                Add(nameof(Last), Last);
                Add(nameof(Data), Convert(Data));
                Add(nameof(ShouldComputeTotalCount), ShouldComputeTotalCount);
                Add(nameof(ShouldComputeEndOffset), ShouldComputeEndOffset);

                return "{ " + result + " }";
            }
        }

        public class TestResult
        {
            public TestResult()
            {
            }

            internal TestResult(Connection<int> connection)
            {
                if (connection == null)
                {
                    throw new ArgumentNullException(nameof(connection));
                }

                HasPreviousPage = connection.PageInfo.HasPreviousPage;
                HasNextPage = connection.PageInfo.HasNextPage;
                Items = connection.Items;
                TotalCount = connection.TotalCount == -1 ? null : connection.TotalCount;

                if (int.TryParse(connection.PageInfo.StartCursor, out var startCursor))
                {
                    StartCursor = startCursor;
                }

                if (int.TryParse(connection.PageInfo.EndCursor, out var endCursor))
                {
                    EndCursor = endCursor;
                }
            }

            public bool HasPreviousPage { get; set; }

            public bool HasNextPage { get; set; }

            public int? StartCursor { get; set; }

            public int? EndCursor { get; set; }

            public IEnumerable<int> Items { get; set; }

            public int? TotalCount { get; set; }

            public override bool Equals(object obj) => Equals(obj as TestResult);

            public bool Equals(TestResult obj) => obj != null
                && obj.HasPreviousPage == HasPreviousPage
                && obj.HasNextPage == HasNextPage
                && obj.StartCursor == StartCursor
                && obj.EndCursor == EndCursor
                && obj.Items.SequenceEqual(Items)
                && obj.TotalCount == TotalCount;

            public override int GetHashCode() =>
                    HasPreviousPage.GetHashCode() ^
                    HasNextPage.GetHashCode() ^
                    StartCursor.GetHashCode() ^
                    EndCursor.GetHashCode() ^
                    Items.GetHashCode() ^
                    TotalCount.GetHashCode();

            public override string ToString() => $"HasPreviousPage = {HasPreviousPage}, HasNextPage = {HasNextPage}, Items = {Convert(Items)}, StartCursor = {StartCursor}, EndCursor = {EndCursor}, TotalCount = {TotalCount}";
        }
    }
}

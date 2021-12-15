// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;
using System.Globalization;
using Epam.Contracts.Models;

namespace Epam.GraphQL.Tests.TestData
{
    public class Unit : IHasId<int>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? HeadId { get; set; }

        public int? ParentId { get; set; }

        public Unit Parent { get; set; }

        public int? BranchId { get; set; }

        public Person Head { get; set; }

        public ICollection<Person> Employees { get; set; }

        public string StringId => Id.ToString(CultureInfo.InvariantCulture);
    }
}

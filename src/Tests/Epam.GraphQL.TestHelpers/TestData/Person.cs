// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Globalization;
using Epam.Contracts.Models;

namespace Epam.GraphQL.Tests.TestData
{
    public class Person : IHasId<int>, IPerson
    {
        private int? _unitId;

        public Person()
        {
        }

        public Person(int id, string fullName, decimal salary, int? managerId, int? unitId, DateTime hireDate, DateTime? terminationDate, bool isDeleted)
            : this()
        {
            Id = id;
            FullName = fullName;
            Salary = salary;
            ManagerId = managerId;
            UnitId = unitId;
            HireDate = hireDate;
            TerminationDate = terminationDate;
            IsDeleted = isDeleted;
            CountryId = 1;
        }

        public Person(Person person)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            Id = person.Id;
            FullName = person.FullName;
            Salary = person.Salary;
            ManagerId = person.ManagerId;
            UnitId = person.UnitId;
            HireDate = person.HireDate;
            TerminationDate = person.TerminationDate;
            IsDeleted = person.IsDeleted;
            CountryId = person.CountryId;
        }

        public decimal Salary { get; set; }

        public int Id { get; set; }

        public long LongId { get => Id; set => Id = Convert.ToInt32(value); }

        public string FullName { get; set; }

        public int? ManagerId { get; set; }

#pragma warning disable CA1305 // Specify IFormatProvider
        public long? LongManagerId { get => ManagerId; set => ManagerId = Convert.ToInt32(value); }
#pragma warning restore CA1305 // Specify IFormatProvider

        public int? UnitId
        {
            get => _unitId;
            set
            {
                _unitId = value;
                if (value == null)
                {
                    Unit = null;
                }
            }
        }

        public DateTime HireDate { get; set; }

        public DateTime? TerminationDate { get; set; }

        public Unit Unit { get; set; }

        public Unit IsInChargeOf { get; set; }

        public Person Manager { get; set; }

        public ICollection<Person> Subordinates { get; set; }

        public bool IsDeleted { get; set; }

        public int CountryId { get; set; }

        public int? NullableCountryId => CountryId;

        public PersonSettings Settings { get; set; }

        public string UnitStringId => UnitId?.ToString(CultureInfo.InvariantCulture);

        public override string ToString()
        {
            return $"Person: {FullName}";
        }
    }
}

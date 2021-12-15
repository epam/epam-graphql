// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.Tests
{
    public class BaseTests
    {
        public Data FakeData { get; private set; }

        public static Data CreateData() => new();

        [SetUp]
        public void SetUp()
        {
            FakeData = CreateData();
        }

        public class Data
        {
            public Data()
            {
                People = new List<Person>(CreatePeople());
                Units = new List<Unit>(CreateUnits());
                Branches = new List<Branch>(CreateBranches());
                Countries = new List<Country>(CreateCountries());
                PeopleSettings = new List<PersonSettings>();

                LinoelLivermore = People[0];
                SophieGandley = People[1];
                HannieEveritt = People[2];

                Alpha = Units[0];
                Beta = Units[1];

                UpdateRelations();
            }

            public List<Person> People { get; }

            public List<PersonSettings> PeopleSettings { get; }

            public List<Unit> Units { get; }

            public List<Branch> Branches { get; }

            public List<Country> Countries { get; }

            public Person LinoelLivermore { get; }

            public Person SophieGandley { get; }

            public Person HannieEveritt { get; }

            public Unit Alpha { get; }

            public Unit Beta { get; }

            public static IEnumerable<Unit> CreateUnits()
            {
                yield return new Unit { Id = 1, Name = "Alpha", HeadId = 1, BranchId = 2 };
                yield return new Unit { Id = 2, Name = "Beta", HeadId = 2, ParentId = 1, BranchId = 1 };
            }

            public static IEnumerable<Person> CreatePeople()
            {
                yield return new Person(1, "Linoel Livermore", 4015.69m, null, 1, new DateTime(2000, 1, 20), null, false);
                yield return new Person(2, "Sophie Gandley", 2381.91m, 1, 1, new DateTime(2010, 6, 14), null, false);
                yield return new Person(3, "Hannie Everitt", 1393.08m, 1, 1, new DateTime(2015, 3, 1), null, false);
                yield return new Person(4, "Florance Goodricke", 549.33m, 2, 2, new DateTime(2013, 9, 19), new DateTime(2019, 10, 1), true);
                yield return new Person(5, "Aldon Exley", 3389.21m, 2, 2, new DateTime(2015, 3, 21), new DateTime(2017, 2, 19), true);
                yield return new Person(6, "Walton Alvarez", 3436.75m, 5, 2, new DateTime(2011, 7, 29), new DateTime(2018, 5, 10), true);
            }

            public static IEnumerable<Branch> CreateBranches()
            {
                yield return new Branch { Id = 1, Name = "Branch 1" };
                yield return new Branch { Id = 2, Name = "Branch 2" };
            }

            public static IEnumerable<Country> CreateCountries()
            {
                yield return new Country { Id = 1 };
            }

            public IEnumerable<TEntity> GetEntities<TEntity>()
            {
                if (typeof(TEntity) == typeof(Person))
                {
                    return (IEnumerable<TEntity>)People;
                }

                if (typeof(TEntity) == typeof(Unit))
                {
                    return (IEnumerable<TEntity>)Units;
                }

                if (typeof(TEntity) == typeof(Branch))
                {
                    return (IEnumerable<TEntity>)Branches;
                }

                throw new NotImplementedException();
            }

            public void UpdateRelations()
            {
                foreach (var p in People)
                {
                    if (p.Id == 0)
                    {
                        p.Id = People.Max(person => person.Id) + 1;

                        foreach (var s in People)
                        {
                            if (s.Manager == p)
                            {
                                s.ManagerId = p.Id;
                            }
                        }

                        foreach (var s in Units)
                        {
                            if (s.Head == p)
                            {
                                s.HeadId = p.Id;
                            }
                        }
                    }
                }

                foreach (var u in Units)
                {
                    if (u.Id == 0)
                    {
                        u.Id = Units.Max(unit => unit.Id) + 1;

                        foreach (var s in People)
                        {
                            if (s.Unit == u)
                            {
                                s.UnitId = u.Id;
                            }
                        }
                    }
                }

                foreach (var p in People)
                {
                    p.Unit = Units.SingleOrDefault(u => u.Id == p.UnitId);
                    p.Manager = People.SingleOrDefault(u => u.Id == p.ManagerId);
                }

                foreach (var u in Units)
                {
                    u.Head = People.SingleOrDefault(p => p.Id == u.HeadId);
                }
            }

            public void AddPerson(Person person)
            {
                if (person == null || person.Id != 0)
                {
                    throw new InvalidOperationException();
                }

                People.Add(person);

                if (person.Unit != null)
                {
                    person.UnitId = person.Unit.Id;
                }

                if (person.Manager != null)
                {
                    person.ManagerId = person.Manager.Id;
                }
            }

            public void AddPersonSettings(PersonSettings settings)
            {
                if (settings == null)
                {
                    throw new InvalidOperationException();
                }

                PeopleSettings.Add(settings);

                if (settings.Person != null)
                {
                    settings.Id = settings.Person.Id;
                }
            }

            public void AddUnit(Unit unit)
            {
                if (unit == null || unit.Id != 0)
                {
                    throw new InvalidOperationException();
                }

                Units.Add(unit);

                if (unit.Head != null)
                {
                    unit.HeadId = unit.Head.Id;
                }
            }
        }
    }
}

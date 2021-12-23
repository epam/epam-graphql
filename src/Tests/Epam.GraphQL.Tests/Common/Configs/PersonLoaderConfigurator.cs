// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Globalization;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Tests.TestData;

namespace Epam.GraphQL.Tests.Common.Configs
{
    public class PersonLoaderConfigurator : ILoaderConfigurator<Person, PersonLoaderConfigurationType>
    {
        public Action<IdentifiableLoader<Person, int, TestUserContext>> ConfigureIdentifiableLoader(PersonLoaderConfigurationType configuration)
        {
            return configuration switch
            {
                PersonLoaderConfigurationType.Basic => BasicBuilder,
                PersonLoaderConfigurationType.PersonFilterable => PersonFilterableBuilder,
                PersonLoaderConfigurationType.PersonSortable => PersonSortableBuilder,
                PersonLoaderConfigurationType.PersonFilterableAndSortable => PersonFilterableAndSortableBuilder,
                _ => throw new NotImplementedException(),
            };

            static void BasicBuilder(IdentifiableLoader<Person, int, TestUserContext> loader)
            {
                loader
                    .Field(p => p.Id);
                loader
                    .Field("ctxId", (ctx, p) => p.Id + ctx.UserId);
                loader
                    .Field("ctxManagerId", (ctx, p) => p.ManagerId + ctx.UserId);
                loader
                    .Field("ctxFullName", (ctx, p) => p.FullName + ctx.UserId.ToString(CultureInfo.InvariantCulture));
                loader
                    .Field(p => p.ManagerId);
                loader
                    .Field(p => p.FullName);
                loader
                    .Field("loaderManagerName")
                    .FromLoader<Person>(loader.GetType(), (p, c) => p.ManagerId == c.Id)
                    .Select(c => c.FullName);
                loader
                    .Field("roundedSalary", p => Convert.ToInt32(p.Salary));
                loader
                    .Field("managerName", p => p.Manager != null ? p.Manager.FullName : null);
                loader
                    .Field(p => p.HireDate);
                loader
                    .Field(p => p.TerminationDate);
            }

            static void PersonFilterableBuilder(IdentifiableLoader<Person, int, TestUserContext> loader)
            {
                loader
                    .Field(p => p.Id)
                    .Filterable();
                loader
                    .Field("ctxId", (ctx, p) => p.Id + ctx.UserId)
                    .Filterable();
                loader
                    .Field("ctxManagerId", (ctx, p) => p.ManagerId + ctx.UserId)
                    .Filterable();
                loader
                    .Field("ctxFullName", (ctx, p) => p.FullName + ctx.UserId.ToString(CultureInfo.InvariantCulture))
                    .Filterable();
                loader
                    .Field(p => p.ManagerId)
                    .Filterable();
                loader
                    .Field(p => p.FullName)
                    .Filterable();
                loader
                    .Field("loaderManagerName")
                    .FromLoader<Person>(loader.GetType(), (p, c) => p.ManagerId == c.Id)
                    .Select(c => c.FullName);
                loader
                    .Field("roundedSalary", p => Convert.ToInt32(p.Salary))
                    .Filterable();
                loader
                    .Field("managerName", p => p.Manager != null ? p.Manager.FullName : null)
                    .Filterable();
                loader
                    .Field(p => p.HireDate)
                    .Filterable();
                loader
                    .Field(p => p.TerminationDate)
                    .Filterable();
                loader.Filter<string>("managerNameFilter", value => person => person.Manager != null && person.Manager.FullName == value);
            }

            static void PersonSortableBuilder(IdentifiableLoader<Person, int, TestUserContext> loader)
            {
                loader
                    .Field(p => p.Id)
                    .Sortable();
                loader
                    .Field("ctxId", (ctx, p) => p.Id + ctx.UserId)
                    .Sortable();
                loader
                    .Field("ctxManagerId", (ctx, p) => p.ManagerId + ctx.UserId)
                    .Sortable();
                loader
                    .Field("ctxFullName", (ctx, p) => p.FullName + ctx.UserId.ToString(CultureInfo.InvariantCulture))
                    .Sortable();
                loader
                    .Field(p => p.ManagerId)
                    .Sortable();
                loader
                    .Field(p => p.FullName)
                    .Sortable();
                loader
                    .Field("loaderManagerName")
                    .FromLoader<Person>(loader.GetType(), (p, c) => p.ManagerId == c.Id)
                    .Select(c => c.FullName);
                loader
                    .Field("roundedSalary", p => Convert.ToInt32(p.Salary))
                    .Sortable();
                loader
                    .Field("managerName", p => p.Manager != null ? p.Manager.FullName : null)
                    .Sortable();
                loader
                    .Field(p => p.HireDate)
                    .Sortable();
                loader
                    .Field(p => p.TerminationDate)
                    .Sortable();
                loader.Sorter("managerNameSorter", p => p.Manager != null ? p.Manager.FullName : null);
            }

            static void PersonFilterableAndSortableBuilder(IdentifiableLoader<Person, int, TestUserContext> loader)
            {
                loader
                    .Field(p => p.Id)
                    .Filterable()
                    .Sortable();
                loader
                    .Field("ctxId", (ctx, p) => p.Id + ctx.UserId)
                    .Filterable()
                    .Sortable();
                loader
                    .Field("ctxManagerId", (ctx, p) => p.ManagerId + ctx.UserId)
                    .Filterable()
                    .Sortable();
                loader
                    .Field("ctxFullName", (ctx, p) => p.FullName + ctx.UserId.ToString(CultureInfo.InvariantCulture))
                    .Filterable()
                    .Sortable();
                loader
                    .Field(p => p.ManagerId)
                    .Filterable()
                    .Sortable();
                loader
                    .Field(p => p.FullName)
                    .Filterable()
                    .Sortable();
                loader
                    .Field("loaderManagerName")
                    .FromLoader<Person>(loader.GetType(), (p, c) => p.ManagerId == c.Id)
                    .Select(c => c.FullName);
                loader
                    .Field("roundedSalary", p => Convert.ToInt32(p.Salary))
                    .Filterable()
                    .Sortable();
                loader
                    .Field("managerName", p => p.Manager != null ? p.Manager.FullName : null)
                    .Filterable()
                    .Sortable();
                loader
                    .Field(p => p.HireDate)
                    .Filterable()
                    .Sortable();
                loader
                    .Field(p => p.TerminationDate)
                    .Filterable()
                    .Sortable();
                loader.Filter<string>("managerNameFilter", value => person => person.Manager != null && person.Manager.FullName == value);
                loader.Sorter("managerNameSorter", p => p.Manager != null ? p.Manager.FullName : null);
            }
        }

        public Action<Loader<Person, TestUserContext>> ConfigureLoader(PersonLoaderConfigurationType configuration)
        {
            return configuration switch
            {
                PersonLoaderConfigurationType.Basic => BasicBuilder,
                PersonLoaderConfigurationType.PersonFilterable => PersonFilterableBuilder,
                PersonLoaderConfigurationType.PersonSortable => PersonSortableBuilder,
                PersonLoaderConfigurationType.PersonFilterableAndSortable => PersonFilterableAndSortableBuilder,
                _ => throw new NotImplementedException(),
            };

            static void BasicBuilder(Loader<Person, TestUserContext> loader)
            {
                loader
                    .Field(p => p.Id);
                loader
                    .Field("ctxId", (ctx, p) => p.Id + ctx.UserId);
                loader
                    .Field("ctxManagerId", (ctx, p) => p.ManagerId + ctx.UserId);
                loader
                    .Field("ctxFullName", (ctx, p) => p.FullName + ctx.UserId.ToString(CultureInfo.InvariantCulture));
                loader
                    .Field(p => p.ManagerId);
                loader
                    .Field(p => p.FullName);
                loader
                    .Field("loaderManagerName")
                    .FromLoader<Person>(loader.GetType(), (p, c) => p.ManagerId == c.Id)
                    .Select(c => c.FullName);
                loader
                    .Field("roundedSalary", p => Convert.ToInt32(p.Salary));
                loader
                    .Field("managerName", p => p.Manager != null ? p.Manager.FullName : null);
                loader
                    .Field(p => p.HireDate);
                loader
                    .Field(p => p.TerminationDate);
            }

            static void PersonFilterableBuilder(Loader<Person, TestUserContext> loader)
            {
                loader
                    .Field(p => p.Id)
                    .Filterable();
                loader
                    .Field("ctxId", (ctx, p) => p.Id + ctx.UserId)
                    .Filterable();
                loader
                    .Field("ctxManagerId", (ctx, p) => p.ManagerId + ctx.UserId)
                    .Filterable();
                loader
                    .Field("ctxFullName", (ctx, p) => p.FullName + ctx.UserId.ToString(CultureInfo.InvariantCulture))
                    .Filterable();
                loader
                    .Field(p => p.ManagerId)
                    .Filterable();
                loader
                    .Field(p => p.FullName)
                    .Filterable();
                loader
                    .Field("loaderManagerName")
                    .FromLoader<Person>(loader.GetType(), (p, c) => p.ManagerId == c.Id)
                    .Select(c => c.FullName);
                loader
                    .Field("roundedSalary", p => Convert.ToInt32(p.Salary))
                    .Filterable();
                loader
                    .Field("managerName", p => p.Manager != null ? p.Manager.FullName : null)
                    .Filterable();
                loader
                    .Field(p => p.HireDate)
                    .Filterable();
                loader
                    .Field(p => p.TerminationDate)
                    .Filterable();
                loader.Filter<string>("managerNameFilter", value => person => person.Manager != null && person.Manager.FullName == value);
            }

            static void PersonSortableBuilder(Loader<Person, TestUserContext> loader)
            {
                loader
                    .Field(p => p.Id)
                    .Sortable();
                loader
                    .Field("ctxId", (ctx, p) => p.Id + ctx.UserId)
                    .Sortable();
                loader
                    .Field("ctxManagerId", (ctx, p) => p.ManagerId + ctx.UserId)
                    .Sortable();
                loader
                    .Field("ctxFullName", (ctx, p) => p.FullName + ctx.UserId.ToString(CultureInfo.InvariantCulture))
                    .Sortable();
                loader
                    .Field(p => p.ManagerId)
                    .Sortable();
                loader
                    .Field(p => p.FullName)
                    .Sortable();
                loader
                    .Field("loaderManagerName")
                    .FromLoader<Person>(loader.GetType(), (p, c) => p.ManagerId == c.Id)
                    .Select(c => c.FullName);
                loader
                    .Field("roundedSalary", p => Convert.ToInt32(p.Salary))
                    .Sortable();
                loader
                    .Field("managerName", p => p.Manager != null ? p.Manager.FullName : null)
                    .Sortable();
                loader
                    .Field(p => p.HireDate)
                    .Sortable();
                loader
                    .Field(p => p.TerminationDate)
                    .Sortable();
                loader.Sorter("managerNameSorter", p => p.Manager != null ? p.Manager.FullName : null);
            }

            static void PersonFilterableAndSortableBuilder(Loader<Person, TestUserContext> loader)
            {
                loader
                    .Field(p => p.Id)
                    .Filterable()
                    .Sortable();
                loader
                    .Field("ctxId", (ctx, p) => p.Id + ctx.UserId)
                    .Filterable()
                    .Sortable();
                loader
                    .Field("ctxManagerId", (ctx, p) => p.ManagerId + ctx.UserId)
                    .Filterable()
                    .Sortable();
                loader
                    .Field("ctxFullName", (ctx, p) => p.FullName + ctx.UserId.ToString(CultureInfo.InvariantCulture))
                    .Filterable()
                    .Sortable();
                loader
                    .Field(p => p.ManagerId)
                    .Filterable()
                    .Sortable();
                loader
                    .Field(p => p.FullName)
                    .Filterable()
                    .Sortable();
                loader
                    .Field("loaderManagerName")
                    .FromLoader<Person>(loader.GetType(), (p, c) => p.ManagerId == c.Id)
                    .Select(c => c.FullName);
                loader
                    .Field("roundedSalary", p => Convert.ToInt32(p.Salary))
                    .Filterable()
                    .Sortable();
                loader
                    .Field("managerName", p => p.Manager != null ? p.Manager.FullName : null)
                    .Filterable()
                    .Sortable();
                loader
                    .Field(p => p.HireDate)
                    .Filterable()
                    .Sortable();
                loader
                    .Field(p => p.TerminationDate)
                    .Filterable()
                    .Sortable();
                loader.Filter<string>("managerNameFilter", value => person => person.Manager != null && person.Manager.FullName == value);
                loader.Sorter("managerNameSorter", p => p.Manager != null ? p.Manager.FullName : null);
            }
        }

        public Action<MutableLoader<Person, int, TestUserContext>> ConfigureMutableLoader(PersonLoaderConfigurationType configuration)
        {
            return configuration switch
            {
                PersonLoaderConfigurationType.Basic => BasicBuilder,
                PersonLoaderConfigurationType.PersonFilterable => PersonFilterableBuilder,
                PersonLoaderConfigurationType.PersonSortable => PersonSortableBuilder,
                PersonLoaderConfigurationType.PersonFilterableAndSortable => PersonFilterableAndSortableBuilder,
                _ => throw new NotImplementedException(),
            };

            static void BasicBuilder(MutableLoader<Person, int, TestUserContext> loader)
            {
                loader
                    .Field(p => p.Id);
                loader
                    .Field("ctxId", (ctx, p) => p.Id + ctx.UserId);
                loader
                    .Field("ctxManagerId", (ctx, p) => p.ManagerId + ctx.UserId);
                loader
                    .Field("ctxFullName", (ctx, p) => p.FullName + ctx.UserId.ToString(CultureInfo.InvariantCulture));
                loader
                    .Field(p => p.ManagerId);
                loader
                    .Field(p => p.FullName);
                loader
                    .Field("loaderManagerName")
                    .FromLoader<Person>(loader.GetType(), (p, c) => p.ManagerId == c.Id)
                    .Select(c => c.FullName);
                loader
                    .Field("roundedSalary", p => Convert.ToInt32(p.Salary));
                loader
                    .Field("managerName", p => p.Manager != null ? p.Manager.FullName : null);
                loader
                    .Field(p => p.HireDate);
                loader
                    .Field(p => p.TerminationDate);
            }

            static void PersonFilterableBuilder(MutableLoader<Person, int, TestUserContext> loader)
            {
                loader
                    .Field(p => p.Id)
                    .Filterable();
                loader
                    .Field("ctxId", (ctx, p) => p.Id + ctx.UserId)
                    .Filterable();
                loader
                    .Field("ctxManagerId", (ctx, p) => p.ManagerId + ctx.UserId)
                    .Filterable();
                loader
                    .Field("ctxFullName", (ctx, p) => p.FullName + ctx.UserId.ToString(CultureInfo.InvariantCulture))
                    .Filterable();
                loader
                    .Field(p => p.ManagerId)
                    .Filterable();
                loader
                    .Field(p => p.FullName)
                    .Filterable();
                loader
                    .Field("loaderManagerName")
                    .FromLoader<Person>(loader.GetType(), (p, c) => p.ManagerId == c.Id)
                    .Select(c => c.FullName);
                loader
                    .Field("roundedSalary", p => Convert.ToInt32(p.Salary))
                    .Filterable();
                loader
                    .Field("managerName", p => p.Manager != null ? p.Manager.FullName : null)
                    .Filterable();
                loader
                    .Field(p => p.HireDate)
                    .Filterable();
                loader
                    .Field(p => p.TerminationDate)
                    .Filterable();
                loader.Filter<string>("managerNameFilter", value => person => person.Manager != null && person.Manager.FullName == value);
            }

            static void PersonSortableBuilder(MutableLoader<Person, int, TestUserContext> loader)
            {
                loader
                    .Field(p => p.Id)
                    .Sortable();
                loader
                    .Field("ctxId", (ctx, p) => p.Id + ctx.UserId)
                    .Sortable();
                loader
                    .Field("ctxManagerId", (ctx, p) => p.ManagerId + ctx.UserId)
                    .Sortable();
                loader
                    .Field("ctxFullName", (ctx, p) => p.FullName + ctx.UserId.ToString(CultureInfo.InvariantCulture))
                    .Sortable();
                loader
                    .Field(p => p.ManagerId)
                    .Sortable();
                loader
                    .Field(p => p.FullName)
                    .Sortable();
                loader
                    .Field("loaderManagerName")
                    .FromLoader<Person>(loader.GetType(), (p, c) => p.ManagerId == c.Id)
                    .Select(c => c.FullName);
                loader
                    .Field("roundedSalary", p => Convert.ToInt32(p.Salary))
                    .Sortable();
                loader
                    .Field("managerName", p => p.Manager != null ? p.Manager.FullName : null)
                    .Sortable();
                loader
                    .Field(p => p.HireDate)
                    .Sortable();
                loader
                    .Field(p => p.TerminationDate)
                    .Sortable();
                loader.Sorter("managerNameSorter", p => p.Manager != null ? p.Manager.FullName : null);
            }

            static void PersonFilterableAndSortableBuilder(MutableLoader<Person, int, TestUserContext> loader)
            {
                loader
                    .Field(p => p.Id)
                    .Filterable()
                    .Sortable();
                loader
                    .Field("ctxId", (ctx, p) => p.Id + ctx.UserId)
                    .Filterable()
                    .Sortable();
                loader
                    .Field("ctxManagerId", (ctx, p) => p.ManagerId + ctx.UserId)
                    .Filterable()
                    .Sortable();
                loader
                    .Field("ctxFullName", (ctx, p) => p.FullName + ctx.UserId.ToString(CultureInfo.InvariantCulture))
                    .Filterable()
                    .Sortable();
                loader
                    .Field(p => p.ManagerId)
                    .Filterable()
                    .Sortable();
                loader
                    .Field(p => p.FullName)
                    .Filterable()
                    .Sortable();
                loader
                    .Field("loaderManagerName")
                    .FromLoader<Person>(loader.GetType(), (p, c) => p.ManagerId == c.Id)
                    .Select(c => c.FullName);
                loader
                    .Field("roundedSalary", p => Convert.ToInt32(p.Salary))
                    .Filterable()
                    .Sortable();
                loader
                    .Field("managerName", p => p.Manager != null ? p.Manager.FullName : null)
                    .Filterable()
                    .Sortable();
                loader
                    .Field(p => p.HireDate)
                    .Filterable()
                    .Sortable();
                loader
                    .Field(p => p.TerminationDate)
                    .Filterable()
                    .Sortable();
                loader.Filter<string>("managerNameFilter", value => person => person.Manager != null && person.Manager.FullName == value);
                loader.Sorter("managerNameSorter", p => p.Manager != null ? p.Manager.FullName : null);
            }
        }
    }
}

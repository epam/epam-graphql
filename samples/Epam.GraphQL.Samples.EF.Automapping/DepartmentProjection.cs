using System;
using System.Collections.Generic;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Samples.Data.Models;

namespace Epam.GraphQL.Samples.EF.Automapping
{
    public class DepartmentProjection : Projection<Department, GraphQLExecutionContext>
    {
        protected override void OnConfigure()
        {
            Field(unit => unit.Id).Filterable().Sortable();
            Field(unit => unit.Name).Filterable().Sortable();
        }
    }
}

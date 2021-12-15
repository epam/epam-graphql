using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Samples.Data.Models;

namespace Epam.GraphQL.Samples.EF.Mutation
{
    public class DepartmentLoader : MutableLoader<Department, int, GraphQLExecutionContext>
    {
        protected override Expression<Func<Department, int>> IdExpression => dep => dep.Id;

        public override bool IsFakeId(int id) => id < 0;

        protected override IQueryable<Department> GetBaseQuery(GraphQLExecutionContext context) =>
            context.DbContext.Departments;

        protected override void OnConfigure()
        {
            Field(unit => unit.Id).Filterable().Sortable();
            Field(unit => unit.Name).EditableIf(change => false).Filterable().Sortable();
            Field(unit => unit.ParentId).EditableIf(change => false);
            Field(unit => unit.ModifiedAt).EditableIf(change => false);
        }
    }
}

using Epam.GraphQL;

namespace Epam.GraphQL.Samples.EF.Automapping
{
    public class GraphQLQuery : Query<GraphQLExecutionContext>
    {
        protected override void OnConfigure()
        {
            // Populate all data from Departments data set. A model will be mapped to GraphQL type automatically.
            Field("departments")
                .FromIQueryable(context => context.DbContext.Departments);
        }
    }
}

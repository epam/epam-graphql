using Epam.GraphQL;

namespace Epam.GraphQL.Samples.EF.Mutation
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

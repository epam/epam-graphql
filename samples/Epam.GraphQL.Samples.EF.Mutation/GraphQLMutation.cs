using Epam.GraphQL;
using Epam.GraphQL.Samples.Data.Models;

namespace Epam.GraphQL.Samples.EF.Mutation
{
    public class GraphQLMutation : Mutation<GraphQLExecutionContext>
    {
        protected override void OnConfigure()
        {
            // Populate all data from Departments data set. A model will be mapped to GraphQL type automatically.
            SubmitField<DepartmentLoader, Department>("departments");
        }
    }
}

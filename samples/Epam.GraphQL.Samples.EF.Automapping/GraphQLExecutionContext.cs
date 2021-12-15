using Epam.GraphQL.Samples.Data;

namespace Epam.GraphQL.Samples.EF.Automapping
{
    public class GraphQLExecutionContext
    {
        /// <summary>
        /// DbContext should be declared here, because you need access to it for data querying.
        /// </summary>
        public GraphQLDbContext DbContext { get; set; }
    }
}

using Telerik.Blazor.Components;
using TelerikToStrawberryShake.Models;

namespace TelerikToStrawberryShake.Mappings;

internal sealed class GraphQlMapper(
    Type? rootFilterType,
    Type? rootSortType,
    IDictionary<string, GraphQlFilterDescriptor> graphQlFilterDescriptors,
    IDictionary<string, GraphQlSortDescriptor> graphQlSortDescriptors)
{
    public GraphQlInput Map(GridReadEventArgs args)
    {
        var dataSourceRequest = args.Request;

        var graphQlInput = GraphQlInput.Default();

        graphQlInput = new GraphQlPagingMapper(dataSourceRequest).Map(graphQlInput);

        if (rootFilterType != null)
        {
            graphQlInput = new GraphQlFilteringMapper(dataSourceRequest, rootFilterType, graphQlFilterDescriptors).Map(graphQlInput);
        }

        if (rootSortType != null)
        {
            graphQlInput = new GraphQlSortingMapper(dataSourceRequest, rootSortType, graphQlSortDescriptors).Map(graphQlInput);
        }

        return graphQlInput;
    }
}
using System.Text;
using Telerik.DataSource;
using TelerikToStrawberryShake.Models;

namespace TelerikToStrawberryShake.Mappings;

internal sealed class GraphQlPagingMapper(DataSourceRequest dataSourceRequest)
{
    public GraphQlInput Map(GraphQlInput graphQlInput)
    {
        graphQlInput = MapAfter(graphQlInput);
        graphQlInput = MapFirst(graphQlInput);

        return graphQlInput;
    }

    private static string Base64Encode(int plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes($"{plainText}");
        return Convert.ToBase64String(plainTextBytes);
    }

    private GraphQlInput MapAfter(GraphQlInput paging)
    {
        if (dataSourceRequest.Skip != 0)
        {
            paging = paging with
            {
                After = Base64Encode(dataSourceRequest.Skip)
            };
        }

        return paging;
    }

    private GraphQlInput MapFirst(GraphQlInput paging)
    {
        if (dataSourceRequest.PageSize != 0)
        {
            paging = paging with
            {
                First = dataSourceRequest.PageSize
            };
        }

        return paging;
    }
}
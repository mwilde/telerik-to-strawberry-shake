using Telerik.DataSource;
using TelerikToStrawberryShake.Exceptions;
using TelerikToStrawberryShake.Models;

namespace TelerikToStrawberryShake.Mappings;

internal sealed class GraphQlFilteringMapper(
    DataSourceRequest dataSourceRequest,
    Type rootFilterType,
    IDictionary<string, GraphQlFilterDescriptor> graphQlFilterDescriptors)
{
    private readonly DataSourceRequest _dataSourceRequest = dataSourceRequest;

    public GraphQlInput Map(GraphQlInput graphQlInput)
    {
        return _dataSourceRequest.Filters.Count switch
               {
                   1   => MapSingleFiltering(graphQlInput),
                   > 1 => MapMultipleFiltering(graphQlInput),
                   _   => graphQlInput
               };
    }

    private static bool IsCompositeFilterDescriptor(IFilterDescriptor input, out CompositeFilterDescriptor compositeFilterDescriptor)
    {
        if (input is CompositeFilterDescriptor cfd)
        {
            compositeFilterDescriptor = cfd;
            return true;
        }

        compositeFilterDescriptor = null!;
        return false;
    }

    private static bool IsFilterDescriptor(IFilterDescriptor input, out FilterDescriptor filterDescriptor)
    {
        if (input is FilterDescriptor fd)
        {
            filterDescriptor = fd;
            return true;
        }

        filterDescriptor = null!;
        return false;
    }

    private static object? MapFilter(FilterDescriptor filterDescriptor, GraphQlFilterDescriptor graphQlFilterDescriptor)
    {
        if (graphQlFilterDescriptor.SubFilterDescriptor == null)
        {
            if (graphQlFilterDescriptor.FieldType == typeof(string))
            {
                return GraphQlFilteringMapperHelper.MapStringFilter(filterDescriptor, graphQlFilterDescriptor);
            }

            if (graphQlFilterDescriptor.FieldType == typeof(int))
            {
                return GraphQlFilteringMapperHelper.MapIntFilter(filterDescriptor, graphQlFilterDescriptor);
            }

            if (graphQlFilterDescriptor.FieldType == typeof(int?))
            {
                return GraphQlFilteringMapperHelper.MapNullableIntFilter(filterDescriptor, graphQlFilterDescriptor);
            }

            if (graphQlFilterDescriptor.FieldType == typeof(bool) ||
                graphQlFilterDescriptor.FieldType == typeof(bool?))
            {
                return GraphQlFilteringMapperHelper.MapBoolFilter(filterDescriptor, graphQlFilterDescriptor);
            }

            if (graphQlFilterDescriptor.FieldType == typeof(Guid))
            {
                return GraphQlFilteringMapperHelper.MapGuidFilter(filterDescriptor, graphQlFilterDescriptor);
            }

            if (graphQlFilterDescriptor.FieldType == typeof(Guid?))
            {
                return GraphQlFilteringMapperHelper.MapNullableGuidFilter(filterDescriptor, graphQlFilterDescriptor);
            }

            if (graphQlFilterDescriptor.FieldType == typeof(DateTime))
            {
                return GraphQlFilteringMapperHelper.MapDateTimeFilter(filterDescriptor, graphQlFilterDescriptor);
            }

            if (graphQlFilterDescriptor.FieldType == typeof(DateTime?))
            {
                return GraphQlFilteringMapperHelper.MapNullableDateTimeFilter(filterDescriptor, graphQlFilterDescriptor);
            }

            if (graphQlFilterDescriptor.FieldType == typeof(DateTimeOffset))
            {
                return GraphQlFilteringMapperHelper.MapDateTimeOffsetFilter(filterDescriptor, graphQlFilterDescriptor);
            }

            if (graphQlFilterDescriptor.FieldType == typeof(DateTimeOffset?))
            {
                return GraphQlFilteringMapperHelper.MapNullableDateTimeOffsetFilter(filterDescriptor, graphQlFilterDescriptor);
            }

            throw new MissingGraphQlFilterMappingException(graphQlFilterDescriptor.FieldType);
        }

        var subFilter         = graphQlFilterDescriptor.SubFilterDescriptor;
        var subFilterInstance = MapFilter(filterDescriptor, subFilter);

        return GraphQlFilteringMapperHelper.CreateFilter(graphQlFilterDescriptor, subFilter.FieldName, subFilterInstance);
    }

    private static bool TryMapFilter(FilterDescriptor filterDescriptor, GraphQlFilterDescriptor graphQlFilterDescriptor, out object filter)
    {
        var mapped = MapFilter(filterDescriptor, graphQlFilterDescriptor);
        if (mapped != null)
        {
            filter = mapped;
            return true;
        }

        filter = null!;
        return false;
    }

    private GraphQlInput MapMultipleFiltering(GraphQlInput filtering)
    {
        var filters = ReflectionsHelper.CreateListOf(rootFilterType);

        foreach (var filterDescriptor in _dataSourceRequest.Filters)
        {
            if (IsCompositeFilterDescriptor(filterDescriptor, out var cfd))
            {
                if (IsFilterDescriptor(cfd.FilterDescriptors.First(), out var fd))
                {
                    if (graphQlFilterDescriptors.TryGetValue(fd.Member, out var gfd))
                    {
                        if (TryMapFilter(fd, gfd, out var filter))
                        {
                            var clone = ReflectionsHelper.CreateInstance(rootFilterType);
                            GraphQlFilteringMapperHelper.SetValue(clone, gfd.FieldName, filter);
                            filters.Add(clone);
                        }
                    }
                }
            }
        }

        if (filters.Count > 0)
        {
            var rootFilter = ReflectionsHelper.CreateInstance(rootFilterType);
            GraphQlFilteringMapperHelper.SetValue(rootFilter, "And", filters);
            filtering = filtering with
            {
                Where = rootFilter
            };
        }

        return filtering;
    }

    private GraphQlInput MapSingleFiltering(GraphQlInput filtering)
    {
        if (IsCompositeFilterDescriptor(_dataSourceRequest.Filters.Single(), out var cfd))
        {
            if (IsFilterDescriptor(cfd.FilterDescriptors.First(), out var fd))
            {
                if (graphQlFilterDescriptors.TryGetValue(fd.Member, out var gfd))
                {
                    if (TryMapFilter(fd, gfd, out var filter))
                    {
                        var rootFilter = ReflectionsHelper.CreateInstance(rootFilterType);
                        GraphQlFilteringMapperHelper.SetValue(rootFilter, gfd.FieldName, filter);
                        filtering = filtering with
                        {
                            Where = rootFilter
                        };
                    }
                }
            }
        }

        return filtering;
    }
}
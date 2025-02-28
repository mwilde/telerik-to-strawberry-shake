using Telerik.DataSource;
using TelerikToStrawberryShake.Models;

namespace TelerikToStrawberryShake.Mappings;

internal sealed class GraphQlSortingMapper(
    DataSourceRequest dataSourceRequest,
    Type rootSortType,
    IDictionary<string, GraphQlSortDescriptor> graphQlSortDescriptors)
{
    public GraphQlInput Map(GraphQlInput graphQlInput)
    {
        return dataSourceRequest.Sorts.Count switch
        {
            1 => MapSingleSorting(graphQlInput),
            > 1 => MapMultipleSorting(graphQlInput),
            _ => graphQlInput
        };
    }

    private static object? CreateSort(GraphQlSortDescriptor graphQlSortDescriptor, string? property, object? value)
    {
        if (property == null || value == null)
        {
            return null;
        }

        var instance = ReflectionsHelper.CreateInstance(graphQlSortDescriptor.SortType);
        SetValue(instance, property, value);

        return instance;
    }

    private static object? MapSort(SortDescriptor sortDescriptor, GraphQlSortDescriptor graphQlSortDescriptor)
    {
        if (graphQlSortDescriptor.SubSortDescriptor == null)
        {
            return sortDescriptor switch
            {
                { SortDirection: ListSortDirection.Descending } => SortEnumType.Desc,
                _ => SortEnumType.Asc
            };
        }

        var subSort = graphQlSortDescriptor.SubSortDescriptor;
        var subSortInstance = MapSort(sortDescriptor, subSort);

        return CreateSort(graphQlSortDescriptor, subSort.FieldName, subSortInstance);
    }

    private static void SetValue(object instance, string property, object value)
        => instance.GetType().GetProperty(property)!.SetValue(instance, value);

    private static bool TryMapSort(SortDescriptor sortDescriptor, GraphQlSortDescriptor graphQlSortDescriptor,
        out object sort)
    {
        var mapped = MapSort(sortDescriptor, graphQlSortDescriptor);
        if (mapped != null)
        {
            sort = mapped;
            return true;
        }

        sort = null!;
        return false;
    }

    private GraphQlInput MapMultipleSorting(GraphQlInput sorting)
    {
        var sorts = ReflectionsHelper.CreateListOf(rootSortType);

        foreach (var sd in dataSourceRequest.Sorts)
        {
            if (graphQlSortDescriptors.TryGetValue(sd.Member, out var gsd))
            {
                if (TryMapSort(sd, gsd, out var sort))
                {
                    var rootSort = ReflectionsHelper.CreateInstance(rootSortType);
                    SetValue(rootSort, gsd.FieldName, sort);
                    sorts.Add(rootSort);
                }
            }
        }

        if (sorts.Count > 0)
        {
            sorting = sorting with
            {
                Order = sorts
            };
        }

        return sorting;
    }

    private GraphQlInput MapSingleSorting(GraphQlInput sorting)
    {
        var sorts = ReflectionsHelper.CreateListOf(rootSortType);

        var sd = dataSourceRequest.Sorts.Single();
        if (graphQlSortDescriptors.TryGetValue(sd.Member, out var gsd))
        {
            if (TryMapSort(sd, gsd, out var sort))
            {
                var rootSort = ReflectionsHelper.CreateInstance(rootSortType);
                SetValue(rootSort, gsd.FieldName, sort);
                sorts.Add(rootSort);
                sorting = sorting with
                {
                    Order = sorts
                };
            }
        }

        return sorting;
    }
}
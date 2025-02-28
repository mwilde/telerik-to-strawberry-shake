using Telerik.DataSource;
using TelerikToStrawberryShake.Models;

namespace TelerikToStrawberryShake.Mappings;

internal static class GraphQlFilteringMapperHelper
{
    private const string contains = "Contains";

    private const string nContains = "Ncontains";

    private const string startsWith = "StartsWith";

    private const string endsWith = "EndsWith";

    private const string eq = "Eq";

    private const string neq = "Neq";

    private const string gt = "Gt";

    private const string gte = "Gte";

    private const string lt = "Lt";

    private const string lte = "Lte";

    private static readonly IEnumerable<Type> graphQlTypes = typeof(GraphQlApiClient).Assembly.GetTypes();

    public static object? CreateFilter(GraphQlFilterDescriptor graphQlFilterDescriptor, string? property, object? value)
    {
        if (property == null || value == null)
        {
            return null;
        }

        var instance = ReflectionsHelper.CreateInstance(graphQlTypes.Single(obj => obj.Name == graphQlFilterDescriptor.FilterType));
        SetValue(instance, property, value);

        return instance;
    }

    public static object? MapBoolFilter(FilterDescriptor filterDescriptor, GraphQlFilterDescriptor graphQlFilterDescriptor)
    {
        var     value     = filterDescriptor.Value;
        string? operation = null;

        switch (filterDescriptor)
        {
            case { Operator: FilterOperator.IsEqualTo }:
                operation = $"{eq}";
                break;
        }

        return CreateFilter(graphQlFilterDescriptor, operation, value);
    }

    public static object? MapDateTimeFilter(FilterDescriptor filterDescriptor, GraphQlFilterDescriptor graphQlFilterDescriptor)
    {
        var     value     = filterDescriptor.Value;
        string? operation = null;

        switch (filterDescriptor)
        {
            case { Operator: FilterOperator.IsEqualTo }:
                operation = $"{eq}";
                break;
            case { Operator: FilterOperator.IsNotEqualTo }:
                operation = $"{neq}";
                break;
            case { Operator: FilterOperator.IsGreaterThan }:
                operation = $"{gt}";
                break;
            case { Operator: FilterOperator.IsLessThan }:
                operation = $"{lt}";
                break;
            case { Operator: FilterOperator.IsGreaterThanOrEqualTo }:
                operation = $"{gte}";
                break;
            case { Operator: FilterOperator.IsLessThanOrEqualTo }:
                operation = $"{lte}";
                break;
        }

        return CreateFilter(graphQlFilterDescriptor, operation, value);
    }

    public static object? MapDateTimeOffsetFilter(FilterDescriptor filterDescriptor, GraphQlFilterDescriptor graphQlFilterDescriptor)
    {
        var     value     = filterDescriptor.Value;
        string? operation = null;

        switch (filterDescriptor)
        {
            case { Operator: FilterOperator.IsEqualTo }:
                operation = $"{eq}";
                break;
            case { Operator: FilterOperator.IsLessThan }:
                operation = $"{lt}";
                break;
            case { Operator: FilterOperator.IsLessThanOrEqualTo }:
                operation = $"{lte}";
                break;
            case { Operator: FilterOperator.IsGreaterThanOrEqualTo }:
                operation = $"{gte}";
                break;
            case { Operator: FilterOperator.IsGreaterThan }:
                operation = $"{gt}";
                break;
            case { Operator: FilterOperator.IsNotEqualTo }:
                operation = $"{neq}";
                break;
        }

        return CreateFilter(graphQlFilterDescriptor, operation, value);
    }

    public static object? MapGuidFilter(FilterDescriptor filterDescriptor, GraphQlFilterDescriptor graphQlFilterDescriptor)
    {
        var     value     = filterDescriptor.Value;
        string? operation = null;

        switch (filterDescriptor)
        {
            case { Operator: FilterOperator.IsEqualTo }:
                operation = $"{eq}";
                break;
            case { Operator: FilterOperator.IsNotEqualTo }:
                operation = $"{neq}";
                break;
            case { Operator: FilterOperator.IsEmpty }:
                operation = $"{eq}";
                value     = Guid.Empty;
                break;
            case { Operator: FilterOperator.IsNotEmpty }:
                operation = $"{neq}";
                value     = Guid.Empty;
                break;
        }

        return CreateFilter(graphQlFilterDescriptor, operation, value);
    }

    public static object? MapIntFilter(FilterDescriptor filterDescriptor, GraphQlFilterDescriptor graphQlFilterDescriptor)
    {
        var     value     = filterDescriptor.Value;
        string? operation = null;

        switch (filterDescriptor)
        {
            case { Operator: FilterOperator.IsEqualTo }:
                operation = $"{eq}";
                break;
            case { Operator: FilterOperator.IsNotEqualTo }:
                operation = $"{neq}";
                break;
            case { Operator: FilterOperator.IsGreaterThan }:
                operation = $"{gt}";
                break;
            case { Operator: FilterOperator.IsLessThan }:
                operation = $"{lt}";
                break;
            case { Operator: FilterOperator.IsLessThanOrEqualTo }:
                operation = $"{lte}";
                break;
            case { Operator: FilterOperator.IsGreaterThanOrEqualTo }:
                operation = $"{gte}";
                break;
        }

        return CreateFilter(graphQlFilterDescriptor, operation, value);
    }

    public static object? MapNullableDateTimeFilter(FilterDescriptor filterDescriptor, GraphQlFilterDescriptor graphQlFilterDescriptor)
    {
        var     value     = filterDescriptor.Value;
        string? operation = null;

        switch (filterDescriptor)
        {
            case { Operator: FilterOperator.IsEqualTo }:
                operation = $"{eq}";
                break;
            case { Operator: FilterOperator.IsNotEqualTo }:
                operation = $"{neq}";
                break;
            case { Operator: FilterOperator.IsGreaterThan }:
                operation = $"{gt}";
                break;
            case { Operator: FilterOperator.IsLessThan }:
                operation = $"{lt}";
                break;
            case { Operator: FilterOperator.IsGreaterThanOrEqualTo }:
                operation = $"{gte}";
                break;
            case { Operator: FilterOperator.IsLessThanOrEqualTo }:
                operation = $"{lte}";
                break;
        }

        return CreateFilter(graphQlFilterDescriptor, operation, value);
    }

    public static object? MapNullableDateTimeOffsetFilter(FilterDescriptor filterDescriptor, GraphQlFilterDescriptor graphQlFilterDescriptor)
    {
        var     value     = filterDescriptor.Value;
        string? operation = null;

        switch (filterDescriptor)
        {
            case { Operator: FilterOperator.IsEqualTo }:
                operation = $"{eq}";
                break;
            case { Operator: FilterOperator.IsLessThan }:
                operation = $"{lt}";
                break;
            case { Operator: FilterOperator.IsLessThanOrEqualTo }:
                operation = $"{lte}";
                break;
            case { Operator: FilterOperator.IsGreaterThanOrEqualTo }:
                operation = $"{gte}";
                break;
            case { Operator: FilterOperator.IsGreaterThan }:
                operation = $"{gt}";
                break;
            case { Operator: FilterOperator.IsNotEqualTo }:
                operation = $"{neq}";
                break;
        }

        return CreateFilter(graphQlFilterDescriptor, operation, value);
    }

    public static object? MapNullableGuidFilter(FilterDescriptor filterDescriptor, GraphQlFilterDescriptor graphQlFilterDescriptor)
    {
        var     value     = filterDescriptor.Value;
        string? operation = null;

        switch (filterDescriptor)
        {
            case { Operator: FilterOperator.IsEqualTo }:
                operation = $"{eq}";
                break;
            case { Operator: FilterOperator.IsNotEqualTo }:
                operation = $"{neq}";
                break;
            case { Operator: FilterOperator.IsEmpty }:
                operation = $"{eq}";
                value     = Guid.Empty;
                break;
            case { Operator: FilterOperator.IsNotEmpty }:
                operation = $"{neq}";
                value     = Guid.Empty;
                break;
        }

        return CreateFilter(graphQlFilterDescriptor, operation, value);
    }

    public static object? MapNullableIntFilter(FilterDescriptor filterDescriptor, GraphQlFilterDescriptor graphQlFilterDescriptor)
    {
        var     value     = filterDescriptor.Value;
        string? operation = null;

        switch (filterDescriptor)
        {
            case { Operator: FilterOperator.IsEqualTo }:
                operation = $"{eq}";
                break;
            case { Operator: FilterOperator.IsNotEqualTo }:
                operation = $"{neq}";
                break;
            case { Operator: FilterOperator.IsGreaterThan }:
                operation = $"{gt}";
                break;
            case { Operator: FilterOperator.IsLessThan }:
                operation = $"{lt}";
                break;
            case { Operator: FilterOperator.IsLessThanOrEqualTo }:
                operation = $"{lte}";
                break;
            case { Operator: FilterOperator.IsGreaterThanOrEqualTo }:
                operation = $"{gte}";
                break;
        }

        return CreateFilter(graphQlFilterDescriptor, operation, value);
    }

    public static object? MapStringFilter(FilterDescriptor filterDescriptor, GraphQlFilterDescriptor graphQlFilterDescriptor)
    {
        var     value     = filterDescriptor.Value;
        string? operation = null;

        switch (filterDescriptor)
        {
            case { Operator: FilterOperator.Contains }:
                operation = $"{contains}";
                break;
            case { Operator: FilterOperator.DoesNotContain }:
                operation = $"{nContains}";
                break;
            case { Operator: FilterOperator.StartsWith }:
                operation = $"{startsWith}";
                break;
            case { Operator: FilterOperator.EndsWith }:
                operation = $"{endsWith}";
                break;
            case { Operator: FilterOperator.IsEqualTo }:
                operation = $"{eq}";
                break;
            case { Operator: FilterOperator.IsNotEqualTo }:
                operation = $"{neq}";
                break;
            case { Operator: FilterOperator.IsEmpty }:
                operation = $"{eq}";
                value     = string.Empty;
                break;
            case { Operator: FilterOperator.IsNotEmpty }:
                operation = $"{neq}";
                value     = string.Empty;
                break;
        }

        return CreateFilter(graphQlFilterDescriptor, operation, value);
    }

    public static void SetValue(object instance, string property, object value)
        => instance.GetType().GetProperty(property)!.SetValue(instance, value);
}
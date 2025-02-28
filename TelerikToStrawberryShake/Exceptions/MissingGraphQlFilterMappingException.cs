namespace TelerikToStrawberryShake.Exceptions;

internal sealed class MissingGraphQlFilterMappingException(Type type) : Exception(GetMessage(type))
{
    private static string GetMessage(Type type) =>
        $"Please add code snippet below to map filter for <{type}> correctly:\n" +
        $"if (graphQlFilterDescriptor.FieldType == typeof(<{type}>))\n" +
        "{\n" +
        "    return GraphQlFilteringMapperHelper.Map<{type}>Filter(filterDescriptor, graphQlFilterDescriptor);\n" +
        "}";
}
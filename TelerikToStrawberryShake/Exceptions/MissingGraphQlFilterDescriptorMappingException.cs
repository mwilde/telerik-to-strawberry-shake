namespace TelerikToStrawberryShake.Exceptions;

internal sealed class MissingGraphQlFilterDescriptorMappingException(Type type) : Exception(GetMessage(type))
{
    private static string GetMessage(Type type) =>
        $"Please add code snippet below to map descriptor for <{type}> correctly:\n" +
        "if (fieldType == typeof(<type>))\n" +
        "{\n" +
        "   filterType = \"<type>OperationFilterInput\";\n" +
        "}";
}
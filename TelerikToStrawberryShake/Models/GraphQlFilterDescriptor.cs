namespace TelerikToStrawberryShake.Models;

internal sealed record GraphQlFilterDescriptor(string FieldName, Type FieldType, string FilterType, GraphQlFilterDescriptor? SubFilterDescriptor)
{
    public string Path() => SubFilterDescriptor != null ? $"{FieldName}.{SubFilterDescriptor.Path()}" : $"{FieldName}";
}
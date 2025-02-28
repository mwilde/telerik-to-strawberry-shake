namespace TelerikToStrawberryShake.Models;

internal sealed record GraphQlSortDescriptor(string FieldName, Type SortType, GraphQlSortDescriptor? SubSortDescriptor)
{
    public string Path() => SubSortDescriptor != null ? $"{FieldName}.{SubSortDescriptor.Path()}" : $"{FieldName}";
}
using System.Collections;

namespace TelerikToStrawberryShake.Models;

internal static class ReflectionsHelper
{
    public static object CreateInstance(Type type) =>
        Activator.CreateInstance(type) ?? throw new InvalidOperationException($"Unable to create instance of type '{type.Name}'");

    public static IList CreateListOf(Type t) => (IList)CreateInstance(typeof(List<>).MakeGenericType(t));
}
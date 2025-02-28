using System.Collections;

namespace TelerikToStrawberryShake.Models;

internal sealed record GraphQlInput(int? First, int? Last, string? After, string? Before, IList? Order, object? Where)
{
    public static GraphQlInput Default() => new(null, null, null, null, null, null);
}
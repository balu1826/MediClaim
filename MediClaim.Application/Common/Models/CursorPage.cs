namespace MediClaim.Application.Common.Models;

public class CursorPage<T>
{
    public List<T> Items { get; set; } = [];

    public Guid? NextCursor { get; set; }

    public bool HasMore { get; set; }
}
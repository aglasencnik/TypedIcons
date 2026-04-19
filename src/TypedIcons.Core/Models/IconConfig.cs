using System.Collections.Generic;
using System.Linq;

namespace TypedIcons.Core.Models;

public record struct IconConfig()
{
    public bool Equals(IconConfig other)
    {
        return Icons.SequenceEqual(other.Icons);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return Icons.Aggregate(0, (current, icon) => (current * 397) ^ icon.GetHashCode());
        }
    }

    public List<IconReference> Icons { get; set; } = [];
}
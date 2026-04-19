using System.Collections.Generic;
using System.Linq;

namespace TypedIcons.Core.Models;

public record struct IconCache()
{
    public bool Equals(IconCache other)
    {
        if (Icons.Count != other.Icons.Count) return false;

        foreach (var kvp in Icons)
        {
            if (!other.Icons.TryGetValue(kvp.Key, out var otherValue)) return false;
            if (!Equals(kvp.Value, otherValue)) return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            // Order-independent hash: XOR of per-entry hashes
            return Icons.Select(kvp => (kvp.Key?.GetHashCode() ?? 0) * 397 ^ (kvp.Value?.GetHashCode() ?? 0))
                .Aggregate(0, (current, entryHash) => current ^ entryHash);
        }
    }

    public Dictionary<string, string> Icons { get; set; } = [];
}
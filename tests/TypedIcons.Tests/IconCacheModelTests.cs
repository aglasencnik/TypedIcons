using TypedIcons.Core.Models;

namespace TypedIcons.Tests;

public class IconCacheModelTests
{
    [Fact]
    public void Equals_SameEntries_ReturnsTrue()
    {
        var a = new IconCache
        {
            Icons = new Dictionary<string, string>
            {
                ["check"] = "<svg>check</svg>",
                ["x-mark"] = "<svg>x</svg>"
            }
        };
        
        var b = new IconCache
        {
            Icons = new Dictionary<string, string>
            {
                ["check"] = "<svg>check</svg>",
                ["x-mark"] = "<svg>x</svg>"
            }
        };

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equals_SameEntriesDifferentInsertionOrder_ReturnsTrue()
    {
        var a = new IconCache
        {
            Icons = new Dictionary<string, string>
            {
                ["check"] = "<svg>check</svg>",
                ["x-mark"] = "<svg>x</svg>"
            }
        };
        
        var b = new IconCache
        {
            Icons = new Dictionary<string, string>
            {
                ["x-mark"] = "<svg>x</svg>",
                ["check"] = "<svg>check</svg>"
            }
        };

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        var a = new IconCache
        {
            Icons = new Dictionary<string, string> { ["check"] = "<svg>v1</svg>" }
        };
        
        var b = new IconCache
        {
            Icons = new Dictionary<string, string> { ["check"] = "<svg>v2</svg>" }
        };

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Equals_DifferentKey_ReturnsFalse()
    {
        var a = new IconCache
        {
            Icons = new Dictionary<string, string> { ["check"] = "<svg/>" }
        };
        
        var b = new IconCache
        {
            Icons = new Dictionary<string, string> { ["trash"] = "<svg/>" }
        };

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Equals_DifferentCounts_ReturnsFalse()
    {
        var a = new IconCache
        {
            Icons = new Dictionary<string, string> { ["check"] = "<svg/>" }
        };
        
        var b = new IconCache
        {
            Icons = new Dictionary<string, string>
            {
                ["check"] = "<svg/>",
                ["x-mark"] = "<svg/>"
            }
        };

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Equals_EmptyDictionaries_ReturnsTrue()
    {
        var a = new IconCache { Icons = new Dictionary<string, string>() };
        var b = new IconCache { Icons = new Dictionary<string, string>() };

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equals_UsableAsDictionaryKey()
    {
        // If Equals/GetHashCode are consistent, these should work as dictionary keys
        var a = new IconCache
        {
            Icons = new Dictionary<string, string> { ["check"] = "<svg/>" }
        };
        var b = new IconCache
        {
            Icons = new Dictionary<string, string> { ["check"] = "<svg/>" }
        };

        var dict = new Dictionary<IconCache, int> { [a] = 42 };
        Assert.True(dict.ContainsKey(b));
        Assert.Equal(42, dict[b]);
    }
}
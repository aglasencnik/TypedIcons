using TypedIcons.Core.Models;

namespace TypedIcons.Tests;

public class IconConfigModelTests
{
    [Fact]
    public void Equals_SameIconsSameOrder_ReturnsTrue()
    {
        var a = new IconConfig
        {
            Icons = new List<IconReference>
            {
                new() { Set = "heroicons", Name = "check" },
                new() { Set = "heroicons", Name = "x-mark" }
            }
        };
        
        var b = new IconConfig
        {
            Icons = new List<IconReference>
            {
                new() { Set = "heroicons", Name = "check" },
                new() { Set = "heroicons", Name = "x-mark" }
            }
        };

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }
    
    [Fact]
    public void Equals_DifferentOrder_ReturnsFalse()
    {
        var a = new IconConfig
        {
            Icons = new List<IconReference>
            {
                new() { Set = "heroicons", Name = "check" },
                new() { Set = "heroicons", Name = "x-mark" }
            }
        };
        
        var b = new IconConfig
        {
            Icons = new List<IconReference>
            {
                new() { Set = "heroicons", Name = "x-mark" },
                new() { Set = "heroicons", Name = "check" }
            }
        };

        Assert.NotEqual(a, b);
    }
    
    [Fact]
    public void Equals_DifferentIcons_ReturnsFalse()
    {
        var a = new IconConfig
        {
            Icons = new List<IconReference> { new() { Set = "heroicons", Name = "check" } }
        };
        var b = new IconConfig
        {
            Icons = new List<IconReference> { new() { Set = "heroicons", Name = "trash" } }
        };

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Equals_EmptyCollections_ReturnsTrue()
    {
        var a = new IconConfig { Icons = new List<IconReference>() };
        var b = new IconConfig { Icons = new List<IconReference>() };

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentLengths_ReturnsFalse()
    {
        var a = new IconConfig
        {
            Icons = new List<IconReference> { new() { Set = "lucide", Name = "home" } }
        };
        
        var b = new IconConfig
        {
            Icons = new List<IconReference>
            {
                new() { Set = "lucide", Name = "home" },
                new() { Set = "lucide", Name = "user" }
            }
        };

        Assert.NotEqual(a, b);
    }
}
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace TypedIcons.Sandbox.Components.Components;

public partial class TestIcon : ComponentBase
{
    [Parameter] public TestIconData? Value { get; set; }
    
    [Parameter] public string? Size { get; set; }
    
    [Parameter] public string? Width { get; set; }
    
    [Parameter] public string? Height { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Value is null) 
            return;
        
        var iconData = Value.Value;
        var (width, height) = CalculateDimensions(iconData);
        
        builder.OpenElement(0, "svg");
        builder.AddAttribute(1, "xmlns", "http://www.w3.org/2000/svg");
        builder.AddAttribute(2, "viewBox", iconData.ViewBox);
        builder.AddAttribute(3, "width", width);
        builder.AddAttribute(4, "height", height);
        builder.AddAttribute(5, "fill", "currentColor");

        builder.AddMarkupContent(6, iconData.Content);

        builder.CloseElement();
    }

    private (string, string) CalculateDimensions(TestIconData iconData)
    {
        string width, height;
        
        if (Size is not null)
        {
            width = height = Size;
        }
        else if (Width is not null && Height is not null)
        {
            width = Width;
            height = Height;
        }
        else if (Width is not null)
        {
            width = height = Width;
        }
        else if (Height is not null)
        {
            width = height = Height;
        }
        else
        {
            width = height = iconData.Width;
        }
        
        return (width, height);
    }
}
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace TypedIcons.Sandbox.Components.Components;

public partial class TestIcon : ComponentBase
{
    [Parameter] public TestIconData? Value { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Value is null) return;
        
        var iconData = Value.Value;
        
        builder.OpenElement(0, "svg");
        builder.AddAttribute(1, "xmlns", "http://www.w3.org/2000/svg");
        builder.AddAttribute(2, "viewBox", iconData.ViewBox);
        builder.AddAttribute(3, "width", iconData.Width);
        builder.AddAttribute(4, "height", iconData.Height);
        builder.AddAttribute(5, "fill", "currentColor");

        builder.AddMarkupContent(6, Value.Value.Content);

        builder.CloseElement();
    }
}
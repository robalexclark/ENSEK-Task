using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;

namespace MeterReadingsBlazorClient.BUnit;

public abstract class BlazoriseTestBase : TestContext
{
    protected BlazoriseTestBase()
    {
        Services
            .AddBlazorise()
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons();
        JSInterop.Mode = JSRuntimeMode.Loose;
        JSInterop.SetupModule("./_content/Blazorise/blazorise.js?v=1.8.0.0");
        JSInterop.SetupModule("./_content/Blazorise.Bootstrap5/bootstrap5.js?v=1.8.0.0");
        JSInterop.SetupModule("./_content/Blazorise.DataGrid/datagrid.js?v=1.8.0.0");
    }
}
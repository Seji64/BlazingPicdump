using BlazingPicdump;
using BlazingPicdump.Services;
using IndexedDB.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using Serilog;

namespace Company.WebApplication1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddHttpClient("Picdump", client => client.BaseAddress = new Uri("https://cors-proxy.tihoda.de/")).AddPolicyHandler(Helper.GetRetryPolicy());
            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;

                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 10000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Text;
            });
            builder.Services.AddSingleton<IIndexedDbFactory, IndexedDbFactory>();
            builder.Services.AddTransient<NetworkStateInterop>();
            builder.Services.AddSingleton<NetworkState>();

            Log.Logger = new LoggerConfiguration()
#if DEBUG
                            .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                            .WriteTo.BrowserConsole()
                            .CreateLogger();

            await builder.Build().RunAsync();
        }
    }
}
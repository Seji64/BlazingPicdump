using BlazingPicdump.Services;
using Microsoft.AspNetCore.Components;
#nullable disable

namespace BlazingPicdump.Components
{
    public partial class Network : System.IDisposable
    {
        [Inject]
        internal NetworkState State { get; set; }

        [Parameter]
        public RenderFragment Online { get; set; }

        [Parameter]
        public RenderFragment Offline { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            State.StatusChanged += StateHasChanged;
        }

        public void Dispose()
        {
            State.StatusChanged -= StateHasChanged;
        }
    }
}

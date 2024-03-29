﻿using Microsoft.JSInterop;
#nullable disable

namespace BlazingPicdump.Services
{
    public class NetworkStateInterop
    {
        private readonly IJSRuntime jsRuntime;
        private Action<bool> handler;

        public NetworkStateInterop(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        public ValueTask InitializeAsync(Action<bool> handler)
        {
            this.handler = handler;

            return jsRuntime.InvokeVoidAsync("Network.Initialize", DotNetObjectReference.Create(this));
        }

        [JSInvokable("Network.StatusChanged")]
        public void OnStatusChanged(bool isOnline)
            => handler?.Invoke(isOnline);
    }
}

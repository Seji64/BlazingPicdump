#nullable disable
using Serilog;

namespace BlazingPicdump.Services
{
    public class NetworkState
    {
        public event Action StatusChanged;

        public NetworkState(NetworkStateInterop interop)
        {
            _ = interop.InitializeAsync(OnStatusChanged);
        }

        private void OnStatusChanged(bool isOnline)
        {
            if (IsOnline != isOnline)
            {
                Log.Debug($"IsOnline changed {IsOnline} => '{isOnline}'.");
                IsOnline = isOnline;
                StatusChanged?.Invoke();
            }
        }

        public bool IsOnline { get; protected set; } = true;
    }
}

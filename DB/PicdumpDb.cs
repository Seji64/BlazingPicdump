using IndexedDB.Blazor;
using Microsoft.JSInterop;

namespace BlazingPicdump.DB
{
    public class PicdumpDb : IndexedDb
    {
        public PicdumpDb(IJSRuntime jSRuntime, string name, int version) : base(jSRuntime, name, version) { }
        public IndexedSet<BlazingPicdump.Models.Picdump> Picdumps { get; set; }
        public IndexedSet<BlazingPicdump.Models.Image> Images { get; set; }
    }
}

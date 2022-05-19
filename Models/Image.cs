namespace BlazingPicdump.Models
{
    public class Image
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public long? ParentPicdumpId { get; set; }
    }
}

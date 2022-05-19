namespace BlazingPicdump.Models
{
    public class Picdump
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long Id { get; set; }
        public string? Hash { get; set; }
        public string? Name { get; set; }
        public string? BaseURL { get; set; }
        public DateTime PublishDate { get; set; }
        public Image? Thumbnail { get; set; }
        public string? Description { get; set; }

    }
}

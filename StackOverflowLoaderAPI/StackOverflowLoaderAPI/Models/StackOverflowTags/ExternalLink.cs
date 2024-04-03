using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StackOverflowLoaderAPI.Models.StackOverflowTags;

    public class ExternalLink
    {
        public string type { get; set; }
        public string link { get; set; }

        [Key]
        [JsonIgnore]
        public int Id { get; set; }
    }



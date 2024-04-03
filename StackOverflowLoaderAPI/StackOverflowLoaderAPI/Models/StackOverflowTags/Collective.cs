using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace StackOverflowLoaderAPI.Models.StackOverflowTags;

    public class Collective
    {
        [NotMapped]
        public List<string>? tags { get; set; }
        public List<ExternalLink> external_links { get; set; }
        public string description { get; set; }
        public string link { get; set; }
        public string name { get; set; }
        public string slug { get; set; }

        [Key]
        [JsonIgnore]
        public int Id { get; set; }
    }


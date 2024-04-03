using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackOverflowLoaderAPI.Models.StackOverflowTags;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StackOverflowLoaderAPI.Models.StackOverflowTags;

    [Index(nameof(name))]
    public class Item
    {
        // [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Collective>? collectives { get; set; }
        // [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        // public virtual ICollection<Collective>? collectives { get; } = new List<Collective>();
        public int count { get; set; }
        public bool has_synonyms { get; set; }
        public bool is_moderator_only { get; set; }
        public bool is_required { get; set; }

        public string name { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? user_id { get; set; }

        [Key]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public int Id { get; set; }
      //  [Newtonsoft.Json.JsonIgnore]
      //  [System.Text.Json.Serialization.JsonIgnore]
        public double? share { get; set; }
    }


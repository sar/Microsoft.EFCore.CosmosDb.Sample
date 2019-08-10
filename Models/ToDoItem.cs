namespace Microsoft.EFCore.CosmosDb.Sample.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;
    using Newtonsoft.Json;

    public class ToDoItem
    {
        [Key]
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("complete")]
        public bool Completed { get; set; }
    }
}
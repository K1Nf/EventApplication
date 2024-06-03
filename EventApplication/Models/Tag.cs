using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EventApplication.Models
{
    public class Tag
    {
        [Key]

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }



        [Required(ErrorMessage = "Укажите название тэга")]
        [Display(Name = "Тэг")]
        public string Name { get; set; }


        [NotMapped]
        [JsonIgnore]
        public int? UserId { get; set; }
        [NotMapped]
        [JsonIgnore]
        public List<User>? Users { get; set; } = [];

        [NotMapped]
        [JsonIgnore]
        public int EventId { get; set; }
        [NotMapped]
        [JsonIgnore]
        public List<Event>? Events { get; set; } = [];
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PIV.Models
{
    public class Weather
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public DateTime InfoDate { get; set; }
        [Required]
        public float Precipitation { get; set; }

        public float Humidity { get; set; }

        [Required]
        [MaxLength(4)]
        public float TemperatureC { get; set; }
    }
}

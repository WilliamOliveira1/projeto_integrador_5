using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PIV.Models
{
    public class SensorData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public DateTime InfoDate { get; set; }
        [Required]
        public string SensorId { get; set; }
        [Required]
        public float Precipitation { get; set; }

        public float Humidity { get; set; }

        [Required]
        [MaxLength(4)]
        public float TemperatureC { get; set; }
    }
}

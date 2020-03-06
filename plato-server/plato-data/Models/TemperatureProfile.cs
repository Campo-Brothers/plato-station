using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace plato.data.Models
{
    [Table("temperature_profile")]
    public class TemperatureProfile
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [NotMapped]
        public bool Current
        {
            get => _current != "0";
            set => _current = value ? "1" : "0";
        }

        [Column("image")]
        public string Image { get; set; }

        public virtual ICollection<TemperatureSchedule> Schedules { get; set; }

        [Column("current")]
        private string _current { get; set; }
    }
}

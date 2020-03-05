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

        [Column("current")]
        public string Current { get; set; }

        [Column("image")]
        public string Image { get; set; }

        public virtual ICollection<TemperatureSchedule> Schedules { get; set; }
    }
}

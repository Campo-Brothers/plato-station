using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace plato.data.Models
{
    [Table("temperature_schedule")]
    public class TemperatureSchedule
    {
        [Column("TemperatureProfileId")]
        public TemperatureProfile Profile { get; set; }

        [Column("Id")]
        public int Id { get; set; }

        [Column("day_of_week")]
        public int DayOfWeek { get; set; }

        [Column("time_start")]
        public TimeSpan Start { get; set; }

        [Column("time_end")]
        public TimeSpan End { get; set; }

        [Column("temperature")]
        public double Temperature { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace plato.data.Models
{
    [Table("temperature_current")]
    public class TemperatureCurrent
    {
        [Column("room")]
        public string Room { get; set; }

        [Column("device_id")]
        public string DeviceId { get; set; }

        [NotMapped]
        public bool MainControl
        {
            get => _mainControl.CompareTo("Y") == 0;
            set => _mainControl = value ? "Y" : "N";
        }

        [NotMapped]
        public bool ReleStatus
        {
            get => _releStatus.CompareTo("Y") == 0;
            set => _releStatus = value ? "Y" : "N";
        }

        [Column("treshold")]
        public double Treshold { get; set; }

        [Column("treshold_end")]
        public TimeSpan TresholdEnd { get; set; }

        [Column("curr_temperature")]
        public double CurrentTemperature { get; set; }

        [Column("humidity")]
        public double CurrentHumidity { get; set; }

        #region Private fields

        [Column("main_control")]
        private string _mainControl { get; set; }

        [Column("rele_status")]
        private string _releStatus { get; set; }

        #endregion
    }
}

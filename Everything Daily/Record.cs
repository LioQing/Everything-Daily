using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everything_Daily
{
    public class Record
    {
        public string Id { get; set; }
        public DateTime Time { get; set; }
        public string Duration { get; set; }
        public string Remarks { get; set; }
        public Record(string Id, DateTime Time, string Duration, string Remarks)
        {
            this.Id = Id;
            this.Time = Time;
            this.Duration = Duration;
            this.Remarks = Remarks;
        }
    }
}

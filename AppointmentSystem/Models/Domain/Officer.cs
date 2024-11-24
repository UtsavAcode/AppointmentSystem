using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppointmentSystem.Models.Domain
{
    public class Officer
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public int PostId { get; set; }
        public bool Status { get; set; }
        public string WorkStartTime { get; set; }

        public string WorkEndTime { get; set; }

        public virtual Post Post { get; set; }
    }
}

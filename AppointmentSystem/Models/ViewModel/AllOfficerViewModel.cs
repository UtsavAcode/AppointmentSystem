using System.ComponentModel.DataAnnotations;

namespace AppointmentSystem.Models.ViewModel
{
    public class AllOfficerViewModel
    {
        public int Id { get; set; }


       
        public string Name { get; set; }


        public int PostId { get; set; }

        public bool Status { get; set; }



        public string WorkStartTime { get; set; }


        public string WorkEndTime { get; set; }
        public string PostName { get; set; }

    }
}

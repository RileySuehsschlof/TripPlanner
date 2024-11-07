using System.ComponentModel.DataAnnotations;

namespace TripPlanner.Models
{
    public class Trip
    {
        [Key]
        public int TripId { get; set; }


        [Required(ErrorMessage = "Destination is required")]
        public string? Destination { get; set; }


        [Required(ErrorMessage = "Start date is required")]
        public DateOnly StartDate { get; set; }


        [Required(ErrorMessage = "End date is required")]

        public DateOnly EndDate { get; set; }

        
        public string? Accomodation { get; set; }
        public string? AccomodationPhone { get; set; }
        public string? AccomodationEmail { get; set; }

        public string? ThingsToDo1 { get; set; }
        public string? ThingsToDo2 { get; set; }

        public string? ThingsToDo3 { get; set; }


    }
}

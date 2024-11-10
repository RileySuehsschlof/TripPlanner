namespace TripPlanner.Models
{
    public class TripViewModel
    {
        
            public string? Destination { get; set; }
            public DateOnly? StartDate { get; set; }
            public DateOnly? EndDate { get; set; }
            public string? Accomodation { get; set; }
            public string? AccomodationPhone { get; set; }
            public string? AccomodationEmail { get; set; }
            public string? ThingsToDo1 { get; set; }
            public string? ThingsToDo2 { get; set; }
            public string? ThingsToDo3 { get; set; }
    }
    
}

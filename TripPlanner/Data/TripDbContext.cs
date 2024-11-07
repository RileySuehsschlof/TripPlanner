using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TripPlanner.Models;

namespace TripPlanner.Data
{
    public class TripDbContext : DbContext
    {
        public TripDbContext(DbContextOptions<TripDbContext> options) : base(options) { }
        public DbSet<Trip> Trips { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trip>().HasData(
                new Trip
                {
                    TripId = 1,
                    Destination = "Banff",
                    StartDate = DateOnly.Parse("11 Nov 2024"),
                    EndDate = DateOnly.Parse(" 19 Dec 2024"),
                    Accomodation = "Banff Springs Hotel",
                    AccomodationPhone = "403-403-4033",
                    AccomodationEmail = "BanffSprings@Hotel.com",
                    ThingsToDo1 = "Ski",
                    ThingsToDo2 = "Go to a resturant"
                });
        }
    }
}

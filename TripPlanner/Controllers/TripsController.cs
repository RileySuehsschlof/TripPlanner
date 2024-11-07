using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Data;
using TripPlanner.Models;

namespace TripPlanner.Controllers
{
    public class TripsController : Controller
    {
        private readonly TripDbContext _context;

        public TripsController(TripDbContext context)
        {
            _context = context;
        }

        // GET: Trips
        public async Task<IActionResult> Index()
        {
            return View(await _context.Trips.ToListAsync());
        }

        // GET: Trips/Create - First step
        public IActionResult Create()
        {
            return View("TripBasicInfo");
        }

        // POST: Trips/AddBasicInfo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddBasicInfo([Bind("Destination,StartDate,EndDate")] Trip trip)
        {
            if (ModelState.IsValid)
            {
                TempData["TripBasicInfo"] = JsonSerializer.Serialize(trip);
                return RedirectToAction(nameof(AddAccommodation));
            }
            return View("TripBasicInfo", trip);
        }

        // GET: Trips/AddAccommodation - Second step
        public IActionResult AddAccommodation()
        {
            var tripJson = TempData["TripBasicInfo"] as string;
            if (tripJson == null)
            {
                return RedirectToAction(nameof(Create));
            }

            var trip = JsonSerializer.Deserialize<Trip>(tripJson);
            TempData.Keep("TripBasicInfo"); // Keep the data for the next step
            return View("AccomodationInfo", trip);
        }

        // POST: Trips/AddAccommodation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAccommodation([Bind("Accomodation,AccomodationPhone,AccomodationEmail")] Trip tripAccom)
        {
           
            if (ModelState.IsValid)
            {
                var tripJson = TempData["TripBasicInfo"] as string;
                var trip = JsonSerializer.Deserialize<Trip>(tripJson);
                TempData.Keep("TripBasicInfo");
                if (string.IsNullOrEmpty(tripJson))
                {
                    // Handle the case where TempData is empty or null.
                    ModelState.AddModelError(string.Empty, "TripBasicInfo data is missing.");
                    return View("TripBasicInfo");
                }

                // Combine basic info with accommodation info
                trip.Accomodation = tripAccom.Accomodation;
                trip.AccomodationPhone = tripAccom.AccomodationPhone;
                trip.AccomodationEmail = tripAccom.AccomodationEmail;
                trip.TripId = 2;
                TempData["TripAccomodation"] = JsonSerializer.Serialize(trip);
                if (tripJson == null)
                {
                    Console.WriteLine("No data in TempData.");
                    return RedirectToAction(nameof(Create)); // Redirect if no data found
                }
                return RedirectToAction(nameof(AddThingsToDo));
            }
            return View("AccomodationInfo", tripAccom);
        }

     


        // POST: Trips/AddThingsToDo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddThingsToDo([Bind("ThingsToDo1,ThingsToDo2,ThingsToDo3")] Trip tripThings)
        {
            Console.WriteLine("Here");
            if (ModelState.IsValid)
            {
                var tripJson = TempData["TripAccomodation"] as string;
                var trip = JsonSerializer.Deserialize<Trip>(tripJson);

                // Add things to do to the complete trip
                trip.ThingsToDo1 = tripThings.ThingsToDo1;
                trip.ThingsToDo2 = tripThings.ThingsToDo2;
                trip.ThingsToDo3 = tripThings.ThingsToDo3;
                Console.WriteLine(trip.Destination);

                _context.Add(trip);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("ThingsToDo", tripThings);
        }
        // GET: Trips/AddThingsToDo - Third step
        public IActionResult AddThingsToDo()
        {
            var tripJson = TempData["TripAccomodation"] as string;
            if (tripJson == null)
            {
                return RedirectToAction(nameof(Create)); // Redirect if no data found
            }


            var trip = JsonSerializer.Deserialize<Trip>(tripJson);
            TempData.Keep("TripAccomodation"); // Keep data for the post action
            return View("ThingsToDo", trip);
        }

        // GET: Trips/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .FirstOrDefaultAsync(m => m.TripId == id);
            if (trip == null)
            {
                return NotFound();
            }

            return View(trip);
        }

        // GET: Trips/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }
            return View(trip);
        }

        // POST: Trips/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TripId,Destination,StartDate,EndDate,Accomodation,AccomodationPhone,AccomodationEmail,ThingsToDo1,ThingsToDo2,ThingsToDo3")] Trip trip)
        {
            if (id != trip.TripId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TripExists(trip.TripId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(trip);
        }

        // GET: Trips/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .FirstOrDefaultAsync(m => m.TripId == id);
            if (trip == null)
            {
                return NotFound();
            }

            return View(trip);
        }

        // POST: Trips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip != null)
            {
                _context.Trips.Remove(trip);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TripExists(int id)
        {
            return _context.Trips.Any(e => e.TripId == id);
        }
    }
}
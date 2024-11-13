using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Newtonsoft.Json;
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
            //set the location for if you had just completed creating a trip
            ViewData["Location"] = TempData["Location"];

            //clear all the temp data for the second time they create trip
            TempData.Clear();
            return View(await _context.Trips.ToListAsync());
        }


        // GET: Trip/TripBasicInfo
        [HttpGet]
        public IActionResult TripBasicInfo()
        {
            return View();
        }

        [HttpPost]
        public IActionResult TripBasicInfo(TripViewModel model)
        {


            List<string> errorMessages = new List<string>();

            // Validation checks
            if (model.Destination == null)
            {
                errorMessages.Add("Destination is required");
            }
            if (model.StartDate == null)
            {
                errorMessages.Add("Start date is required");
            }
            if (model.EndDate == null)
            {
                errorMessages.Add("End date is required");
            }
            if (model.StartDate > model.EndDate)
            {
                errorMessages.Add("End date cannot be before start date");
            }

            // If there are validation errors, store them in ViewBag and return the same view
            if (errorMessages.Any())
            {
                ViewBag.ErrorMessages = errorMessages;
                return View(model); // Return the same view to show the errors
            }
            if (ModelState.IsValid && ViewBag.ErrorMessage == null)
            {
                
                // Serialize model and store in TempData
                TempData["TripData"] = JsonConvert.SerializeObject(model);
                TempData.Keep("TripData");



                

                if (model.Accomodation == null)
                {
                    return RedirectToAction("ThingsToDo");
                }

                // Redirect to AccomodationInfo action
                
                return RedirectToAction("AccomodationInfo");
            }

            // Return to the same view if model is invalid
            return View(model);
        }

        [HttpGet]
        public IActionResult AccomodationInfo()
        {
            var model = new TripViewModel();

            // Retrieve the Trip data from TempData
            var tripData = TempData.Peek("TripData") as string;
            TempData.Keep("TripData"); // Keep TempData for the next request


            ViewData["Accomodation"] = model.Accomodation;
            //getting the accomodation from the last form
            if (!string.IsNullOrEmpty(tripData))
            {
                model = JsonConvert.DeserializeObject<TripViewModel>(tripData);
                ViewData["Accomodation"] = model.Accomodation;
            }
            
            return View();
        }

        [HttpPost]
        public IActionResult AccomodationInfo(TripViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the existing trip data from TempData and update it with new accommodation info
                var tripData = TempData.Peek("TripData") as string;
                TempData.Keep("TripData");


                //an arry to store our errormessages
                List<string> errorMessages = new List<string>();

                //the pattern we are using for checking the phone number
                const string phonePattern = @"^(\d{3}[-\s]?)?(\d{3}[-\s]?)?\d{4}$";


                if (model.AccomodationPhone != null && !Regex.IsMatch(model.AccomodationPhone, phonePattern))
                {
                    errorMessages.Add("Phone number is incorrect");
                }

                //trying to see if we can turn it into a MailAddress object to test if it is a valid email
                if (model.AccomodationEmail != null && !MailAddress.TryCreate(model.AccomodationEmail, out _))
                {
                    errorMessages.Add("Email is incorrect");
                }
                

                if (errorMessages.Any())
                {
                    ViewBag.ErrorMessages = errorMessages;
           

                }


                if (!string.IsNullOrEmpty(tripData) && ViewBag.ErrorMessages ==null)
                {
                    var existingTrip = JsonConvert.DeserializeObject<TripViewModel>(tripData);
                    existingTrip.AccomodationPhone = model.AccomodationPhone;
                    existingTrip.AccomodationEmail = model.AccomodationEmail;

                    // Store the updated data back into TempData
                    TempData["TripData"] = JsonConvert.SerializeObject(existingTrip);
                    TempData.Keep("TripData");

                    // Redirect to the next step (Things to do)

                    return RedirectToAction("ThingsToDo");
                }
                //specifically needed to ensure that accomodation will persist even when validation errors occur
                else
                {
                    var existingTrip = JsonConvert.DeserializeObject<TripViewModel>(tripData);
                    TempData["acc"] = existingTrip.Accomodation;
                }
                
            }// Return to the same view if model validation fails
            return View(model);
        }


        [HttpGet]
        public IActionResult ThingsToDo()
        {
            // Retrieve the Trip data from TempData
            var tripData = TempData.Peek("TripData") as string;

            // If there is trip data, deserialize it and set the location in ViewData
            if (!string.IsNullOrEmpty(tripData))
            {
                var model = JsonConvert.DeserializeObject<TripViewModel>(tripData);
                ViewData["Location"] = model.Destination;
            }

            return View(); 
        }



        // POST: Trip/ThingsToDo
        [HttpPost]
        public async Task<IActionResult> ThingsToDo(TripViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the trip data from TempData
                var tripData = TempData.Peek("TripData") as string;
                TempData.Keep("TripData");

                if (!string.IsNullOrEmpty(tripData))
                {
                    // Deserialize the trip data into the Trip
                    var existingTrip = JsonConvert.DeserializeObject<TripViewModel>(tripData);

                    // Update the existing trip with the new values (ThingsToDo)
                    existingTrip.ThingsToDo1 = model.ThingsToDo1;
                    existingTrip.ThingsToDo2 = model.ThingsToDo2;
                    existingTrip.ThingsToDo3 = model.ThingsToDo3;

                    
                    // Now save the trip to the database
                    var trip = new Trip
                    {
                        Destination = existingTrip.Destination,
                        StartDate = (DateOnly)existingTrip.StartDate,
                        EndDate = (DateOnly)existingTrip.EndDate,
                        AccomodationPhone = existingTrip.AccomodationPhone,
                        AccomodationEmail = existingTrip.AccomodationEmail,
                        Accomodation = existingTrip.Accomodation,
                        ThingsToDo1 = existingTrip.ThingsToDo1,
                        ThingsToDo2 = existingTrip.ThingsToDo2,
                        ThingsToDo3 = existingTrip.ThingsToDo3
                    };

                    // Add the trip to the database and save it
                    _context.Trips.Add(trip);
                    await _context.SaveChangesAsync();

                    
                    TempData["Location"] = existingTrip.Destination;

                    // Redirect back to the Index page after saving the data
                    return RedirectToAction("Index");
                }
            }

            // Return to the same view if the model is invalid
            return View(model);
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
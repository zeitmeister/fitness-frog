using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;
using static Treehouse.FitnessFrog.Models.Entry;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        public ActionResult Add(string Date)
        {
            var entry = new Entry()
            {
                Date = DateTime.Today
            };
            var newDate = entry.Date.ToString("m, d, yyyy");

            SetupActivitiesSelectListItems();

            return View(entry);
        }

       

        [HttpPost]
        public ActionResult Add(Entry entry)
        {
            // If there aren't any "Duration" field validation errors
            // then make sure that the duration is greater than 0.
            ValidateEntry(entry);

            if (ModelState.IsValid)
            {
                _entriesRepository.AddEntry(entry);

                //TODO Display the Entries list page

                return RedirectToAction("Index");
            }

            SetupActivitiesSelectListItems();

            return View(entry);
        }

    

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entry entry = _entriesRepository.GetEntry((int)id);
            
            if(entry == null)
            {
                return HttpNotFound();
            }
            
            SetupActivitiesSelectListItems();
            return View(entry);
        }
        [HttpPost]
        public ActionResult Edit(Entry entry)
        {
            //TODO Validate the entry

            ValidateEntry(entry);
            if (ModelState.IsValid)
            {
                _entriesRepository.UpdateEntry(entry);
                return RedirectToAction("Index");
            }
            
            SetupActivitiesSelectListItems();
            return View(entry);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //TODO Retrieve entry for the provided if parameter value.
            Entry entry = _entriesRepository.GetEntry((int)id);

            //TODO Reutrn "not found" if an entry wasn't found.
            if (entry == null)
            {
                return HttpNotFound();
            }
            //TODO Pass the entry to the view.

            return View(entry);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            // TODO Delete the entry
            _entriesRepository.DeleteEntry(id);

            //Redirect the user the "Entries" list page
            return RedirectToAction("Index");
        }

        private void ValidateEntry(Entry entry)
        {
            if (ModelState.IsValidField("Duration") && entry.Duration <= 0)
            {
                ModelState.AddModelError("Duration", "The duration field value must be greater than '0'");
            }
        }

        private void SetupActivitiesSelectListItems()
        {
            ViewBag.ActivitiesSelectListItems = new SelectList(
                            Data.Data.Activities, "Id", "Name");
        }
    }
}
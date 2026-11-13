using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;
namespace RaceDay.Web.Controllers;
public class EventsController(ApiClient api):Controller
{
    private string? Role=>HttpContext.Session.GetString("Role");
    private IActionResult? Require(string role)=>HttpContext.Session.GetInt32("UserId")==null?RedirectToAction("Login","Account"):Role!=role?Forbid():null;
    public async Task<IActionResult> Index(){var r=await api.GetAsync<List<HomeController.EventCard>>("api/events");ViewBag.Error=r.Error;return View(r.Data??new());}
    [HttpGet] public IActionResult Create(){var guard=Require("Organiser");return guard??View(new EventVm());}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Create(EventVm vm){var guard=Require("Organiser");if(guard!=null)return guard;if(!ModelState.IsValid)return View(vm);var r=await api.PostAsync<object>("api/events",vm);if(!r.Success){ModelState.AddModelError("",r.Error??"Could not create event.");return View(vm);}TempData["Success"]="Event created.";return RedirectToAction(nameof(Index));}
    [HttpGet] public async Task<IActionResult> Edit(int id){var guard=Require("Organiser");if(guard!=null)return guard;var r=await api.GetAsync<EventVm>($"api/events/{id}");if(!r.Success||r.Data==null)return NotFound();r.Data.EventID=id;return View(r.Data);}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Edit(int id,EventVm vm){var guard=Require("Organiser");if(guard!=null)return guard;if(!ModelState.IsValid)return View(vm);var r=await api.PutAsync<object>($"api/events/{id}",vm);if(!r.Success){ModelState.AddModelError("",r.Error??"Could not update event.");return View(vm);}TempData["Success"]="Event updated.";return RedirectToAction(nameof(Index));}

    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> UploadBanner(int id, IFormFile file){var guard=Require("Organiser");if(guard!=null)return guard;if(file==null||file.Length==0){TempData["Error"]="Choose an image first.";return RedirectToAction(nameof(Edit),new{id});}var r=await api.PostFileAsync<object>($"api/uploads/events/{id}/banner",file);TempData[r.Success?"Success":"Error"]=r.Success?"Event banner uploaded.":r.Error;return RedirectToAction(nameof(Edit),new{id});}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Delete(int id){var guard=Require("Organiser");if(guard!=null)return guard;var r=await api.DeleteAsync<object>($"api/events/{id}");TempData[r.Success?"Success":"Error"]=r.Success?"Event deleted.":r.Error;return RedirectToAction(nameof(Index));}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Enrol(int categoryId){var guard=Require("Participant");if(guard!=null)return guard;var r=await api.PostAsync<object>("api/enrolments",new{CategoryID=categoryId});TempData[r.Success?"Success":"Error"]=r.Success?"Your enrolment has been recorded.":r.Error;return RedirectToAction("Index","Home");}
}

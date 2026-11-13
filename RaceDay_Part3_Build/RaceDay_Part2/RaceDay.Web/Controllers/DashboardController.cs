using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
namespace RaceDay.Web.Controllers;
public class DashboardController(ApiClient api):Controller
{
    private bool SignedIn=>HttpContext.Session.GetInt32("UserId")!=null;
    public async Task<IActionResult> Index(){if(!SignedIn)return RedirectToAction("Login","Account");var role=HttpContext.Session.GetString("Role");if(role=="Organiser"){var r=await api.GetAsync<List<EventItem>>("api/events");var owned=(r.Data??new()).Where(x=>x.OrganiserID==HttpContext.Session.GetInt32("UserId")).ToList();foreach(var ev in owned){var enrolments=await api.GetAsync<List<object>>($"api/enrolments/events/{ev.EventID}");ev.EnrolmentCount=enrolments.Data?.Count??0;}ViewBag.Events=owned;ViewBag.Error=r.Error;return View("Organiser");}var mine=await api.GetAsync<List<object>>("api/enrolments/me");ViewBag.Enrolments=mine.Data??new();ViewBag.Error=mine.Error;return View("Participant");}
    public sealed class EventItem{public int EventID{get;set;}public int OrganiserID{get;set;}public string EventName{get;set;}="";public DateTime EventDate{get;set;}public string Location{get;set;}="";public string Status{get;set;}="";public int EnrolmentCount{get;set;}}
}

using Microsoft.AspNetCore.Mvc;using RaceDay.Web.Services;
namespace RaceDay.Web.Controllers;
public class ResultsController(ApiClient api):Controller
{ public async Task<IActionResult> Index(){if(HttpContext.Session.GetString("Role")!="Participant")return HttpContext.Session.GetInt32("UserId")==null?RedirectToAction("Login","Account"):Forbid();var r=await api.GetAsync<List<ResultRow>>("api/results/me");ViewBag.Error=r.Error;return View(r.Data??new());}
 public sealed class ResultRow{public int ResultID{get;set;}public int? FinishPosition{get;set;}public TimeSpan? FinishTime{get;set;}public string ResultStatus{get;set;}="";public EnrolmentInfo Enrolment{get;set;}=new();}public sealed class EnrolmentInfo{public CategoryInfo Category{get;set;}=new();}public sealed class CategoryInfo{public string CategoryName{get;set;}="";public EventInfo Event{get;set;}=new();}public sealed class EventInfo{public string EventName{get;set;}="";public DateTime EventDate{get;set;}}
}

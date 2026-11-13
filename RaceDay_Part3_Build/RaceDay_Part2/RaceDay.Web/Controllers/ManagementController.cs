using Microsoft.AspNetCore.Mvc;using RaceDay.Web.Models;using RaceDay.Web.Services;
namespace RaceDay.Web.Controllers;
public class ManagementController(ApiClient api):Controller
{
 private bool Organiser=>HttpContext.Session.GetString("Role")=="Organiser"&&HttpContext.Session.GetInt32("UserId")!=null;
 public async Task<IActionResult> Categories(int eventId){if(!Organiser)return HttpContext.Session.GetInt32("UserId")==null?RedirectToAction("Login","Account"):Forbid();ViewBag.EventId=eventId;var r=await api.GetAsync<List<CategoryRow>>($"api/events/{eventId}/categories");ViewBag.Error=r.Error;return View(r.Data??new());}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> AddCategory(int eventId,CategoryVm vm){if(!Organiser)return Forbid();var r=await api.PostAsync<object>($"api/events/{eventId}/categories",vm);TempData[r.Success?"Success":"Error"]=r.Success?"Category added.":r.Error;return RedirectToAction(nameof(Categories),new{eventId});}
 public async Task<IActionResult> Enrolments(int eventId){if(!Organiser)return Forbid();var r=await api.GetAsync<List<EnrolmentRow>>($"api/enrolments/events/{eventId}");ViewBag.EventId=eventId;ViewBag.Error=r.Error;return View(r.Data??new());}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> SaveResult(int enrolmentId,int? finishPosition,TimeSpan? finishTime,string resultStatus="Finished",int eventId=0){if(!Organiser)return Forbid();var r=await api.PostAsync<object>("api/results",new{EnrolmentID=enrolmentId,FinishPosition=finishPosition,FinishTime=finishTime,ResultStatus=resultStatus});TempData[r.Success?"Success":"Error"]=r.Success?"Result saved.":r.Error;return RedirectToAction(nameof(Enrolments),new{eventId});}
 public sealed class CategoryRow{public int CategoryID{get;set;}public string CategoryName{get;set;}="";public string? AgeGroup{get;set;}public decimal EntryFee{get;set;}public int? MaxParticipants{get;set;}}
 public sealed class EnrolmentRow{public int EnrolmentID{get;set;}public string ParticipantName{get;set;}="";public string CategoryName{get;set;}="";public DateTime EnrolmentDate{get;set;}public string Status{get;set;}="";}
}

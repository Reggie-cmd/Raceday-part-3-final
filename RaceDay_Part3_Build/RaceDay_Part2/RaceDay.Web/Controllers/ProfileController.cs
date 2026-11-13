using Microsoft.AspNetCore.Mvc;using RaceDay.Web.Models;using RaceDay.Web.Services;
namespace RaceDay.Web.Controllers;
public class ProfileController(ApiClient api):Controller
{
 [HttpGet] public async Task<IActionResult> Index(){if(HttpContext.Session.GetInt32("UserId")==null)return RedirectToAction("Login","Account");var r=await api.GetAsync<ProfileData>("api/users/me");ViewBag.Error=r.Error;ViewBag.ProfilePictureUrl=r.Data?.Profile?.ProfilePictureUrl;return View(new ProfileVm{PhoneNumber=r.Data?.Profile?.PhoneNumber,Address=r.Data?.Profile?.Address,EmergencyContactName=r.Data?.Profile?.EmergencyContactName,EmergencyContactPhone=r.Data?.Profile?.EmergencyContactPhone});}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Index(ProfileVm vm){if(HttpContext.Session.GetInt32("UserId")==null)return RedirectToAction("Login","Account");if(!ModelState.IsValid)return View(vm);var r=await api.PutAsync<object>("api/users/me",vm);TempData[r.Success?"Success":"Error"]=r.Success?"Profile updated.":r.Error;return View(vm);}

 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> UploadPicture(IFormFile file){if(HttpContext.Session.GetInt32("UserId")==null)return RedirectToAction("Login","Account");if(file==null||file.Length==0){TempData["Error"]="Choose an image first.";return RedirectToAction(nameof(Index));}var r=await api.PostFileAsync<object>("api/uploads/profile-picture",file);TempData[r.Success?"Success":"Error"]=r.Success?"Profile picture uploaded.":r.Error;return RedirectToAction(nameof(Index));}
 public sealed class ProfileData{public ProfileDetails? Profile{get;set;}} public sealed class ProfileDetails{public string? ProfilePictureUrl{get;set;}public string? PhoneNumber{get;set;}public string? Address{get;set;}public string? EmergencyContactName{get;set;}public string? EmergencyContactPhone{get;set;}}
}

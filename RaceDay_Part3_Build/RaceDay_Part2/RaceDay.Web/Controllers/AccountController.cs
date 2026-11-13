using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;
namespace RaceDay.Web.Controllers;
public class AccountController(ApiClient api) : Controller
{
    [HttpGet] public IActionResult Login()=>View(new LoginVm());
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Login(LoginVm vm)
    {
        if(!ModelState.IsValid)return View(vm);
        var r=await api.SendAsync<UserDto>(HttpMethod.Post,"api/auth/login",new{vm.Email,vm.Password},true);
        if(!r.Success||r.Data is null){ModelState.AddModelError("",r.Error??"Login failed.");return View(vm);}
        HttpContext.Session.SetInt32("UserId",r.Data.UserID);HttpContext.Session.SetString("UserName",$"{r.Data.FirstName} {r.Data.LastName}");HttpContext.Session.SetString("Role",r.Data.Role);HttpContext.Session.SetString("Email",r.Data.Email);
        return RedirectToAction("Index", "Dashboard");
    }
    [HttpGet] public IActionResult Register()=>View(new RegisterVm());
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Register(RegisterVm vm)
    {
        if(!ModelState.IsValid)return View(vm);
        var r=await api.PostAsync<object>("api/auth/register",vm);
        if(!r.Success){ModelState.AddModelError("",r.Error??"Registration failed.");return View(vm);}
        TempData["Success"]="Account created. Please log in.";return RedirectToAction(nameof(Login));
    }
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Logout()
    { if(HttpContext.Session.GetString("ApiCookie") is not null) await api.SendAsync<object>(HttpMethod.Post,"api/auth/logout");HttpContext.Session.Clear();return RedirectToAction("Index","Home"); }
    public sealed record UserDto(int UserID,string FirstName,string LastName,string Email,string Role,DateTime CreatedAt);
}

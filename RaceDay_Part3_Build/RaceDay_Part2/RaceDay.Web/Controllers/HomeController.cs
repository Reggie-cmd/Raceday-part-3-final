using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
namespace RaceDay.Web.Controllers;
public class HomeController(ApiClient api):Controller
{
    public async Task<IActionResult> Index(){var r=await api.GetAsync<List<EventCard>>("api/events");ViewBag.Error=r.Error;return View(r.Data??new());}
    public async Task<IActionResult> Details(int id){var r=await api.GetAsync<EventCard>($"api/events/{id}");if(!r.Success)return NotFound();return View(r.Data);}
    public sealed class EventCard { public int EventID{get;set;} public string EventName{get;set;}="";public string? Description{get;set;}public DateTime EventDate{get;set;}public TimeSpan StartTime{get;set;}public string Location{get;set;}="";public decimal DistanceKm{get;set;}public string EventType{get;set;}="";public string Status{get;set;}="";public int OrganiserID{get;set;}public string? BannerImageUrl{get;set;}public List<CategoryCard>? Categories{get;set;} }
    public sealed class CategoryCard {public int CategoryID{get;set;}public string CategoryName{get;set;}="";public string? AgeGroup{get;set;}public decimal EntryFee{get;set;} }
}

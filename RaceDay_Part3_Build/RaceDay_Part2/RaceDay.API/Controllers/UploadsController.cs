using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Storage;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[ApiController, Route("api/uploads"), Authorize]
public sealed class UploadsController(RaceDayDbContext db, BlobStorageService storage) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("events/{eventId:int}/banner"), Authorize(Policy = "OrganiserOnly"), RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadBanner(int eventId, IFormFile file, CancellationToken ct)
    {
        var item = await db.Events.FindAsync(new object[] { eventId }, ct);
        if (item is null) return NotFound();
        if (item.OrganiserID != UserId) return Forbid();
        try { item.BannerImageUrl = await storage.UploadAsync(file, "event-banners", ct); await db.SaveChangesAsync(ct); return Ok(new { item.EventID, item.BannerImageUrl }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPost("profile-picture"), Authorize(Policy = "ParticipantOnly"), RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadProfilePicture(IFormFile file, CancellationToken ct)
    {
        var profile = await db.UserProfiles.SingleOrDefaultAsync(x => x.UserID == UserId, ct);
        if (profile is null) { profile = new UserProfile { UserID = UserId }; db.UserProfiles.Add(profile); }
        try { profile.ProfilePictureUrl = await storage.UploadAsync(file, "profile-pictures", ct); await db.SaveChangesAsync(ct); return Ok(new { profile.ProfilePictureUrl }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }
}

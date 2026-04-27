using backend.Contexts;
using backend.Models;
using backend.Services;
using backend.VisibilityFiltering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly Context _context;
    private readonly AuthService _authSrv;

    public AdminController(Context context, AuthService authSrv)
    {
        _context = context;
        _authSrv = authSrv;
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("User")]
    public async Task<IActionResult> GetAllUsers()
    {
        var authUser = await _authSrv.GetUser(User);
        
        if (authUser == null) return Unauthorized();
        if (authUser.Role != UserRole.Administrator) return Forbid();

        return Ok(
            (await _context.Users.ToListAsync())
                .FilterSerialize(authUser)
        );
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("Vehicle")]
    public async Task<IActionResult> GetAllVehicles()
    {
        var authUser = await _authSrv.GetUser(User);
        
        if (authUser == null) return Unauthorized();
        if (authUser.Role != UserRole.Administrator) return Forbid();

        return Ok(
            (await _context.Vehicles
                .Include(x => x.Owner)
                .Include(x => x.Images)
                .ToListAsync())
            .FilterSerialize(authUser)
        );
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("Rental")]
    public async Task<IActionResult> GetAllRentals(
        [FromQuery] bool active = true
    )
    {
        var authUser = await _authSrv.GetUser(User);

        if (authUser == null) return Unauthorized();
        if (authUser.Role != UserRole.Administrator) return Forbid();

        return Ok(
            (await _context.Rentals
                .Include(x => x.Renter)
                .Include(x => x.Vehicle)
                .ThenInclude(x => x.Owner)
                .Where(x => active ? x.Status < RentalStatus.Finished : true)
                .ToListAsync())
            .FilterSerialize(authUser)
        );
    }
}
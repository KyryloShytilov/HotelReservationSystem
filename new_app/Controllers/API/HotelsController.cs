using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using new_app.DTOs;
using new_app.Models;
using new_app.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_app.Controllers.API;

[Route("api/[controller]")]
[ApiController]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;

    public HotelsController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    // GET: api/Hotels
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels()
    {
        return await _hotelService.GetHotelsAsync();
    }

    // GET: api/Hotels/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<HotelDto>> GetHotel(int id)
    {
        try
        {
            return await _hotelService.GetHotelAsync(id);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    // POST: api/Hotels
    [HttpPost]
    [Authorize(Roles = RoleName.Admin)]
    public async Task<ActionResult<HotelDto>> CreateHotel(HotelDto hotelDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _hotelService.CreateHotelAsync(hotelDto);
            return CreatedAtAction(nameof(GetHotel), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT: api/Hotels/5
    [HttpPut("{id}")]
    [Authorize(Roles = RoleName.Admin)]
    public async Task<IActionResult> UpdateHotel(int id, HotelDto hotelDto)
    {
        if (id != hotelDto.Id)
        {
            return BadRequest("Id mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _hotelService.UpdateHotelAsync(id, hotelDto);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    // DELETE: api/Hotels/5
    [HttpDelete("{id}")]
    [Authorize(Roles = RoleName.Admin)]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        try
        {
            await _hotelService.DeleteHotelAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}
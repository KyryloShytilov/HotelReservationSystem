using AutoMapper;
using Microsoft.EntityFrameworkCore;
using new_app.Data;
using new_app.DTOs;
using new_app.Models;
using new_app.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_app.Services;

public class HotelService : IHotelService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public HotelService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<HotelDto>> GetHotelsAsync()
    {
        var hotels = await _context.Hotels
            .Include(c => c.Country)
            .ToListAsync();
            
        return _mapper.Map<List<HotelDto>>(hotels);
    }

    public async Task<HotelDto> GetHotelAsync(int id)
    {
        var hotel = await _context.Hotels
            .Include(h => h.Country)
            .SingleOrDefaultAsync(c => c.Id == id);

        if (hotel == null)
            throw new Exception($"Hotel with ID {id} not found");

        return _mapper.Map<HotelDto>(hotel);
    }

    public async Task<HotelDto> CreateHotelAsync(HotelDto hotelDto)
    {
        var hotel = _mapper.Map<Hotel>(hotelDto);

        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        hotelDto.Id = hotel.Id;
        return hotelDto;
    }

    public async Task<bool> UpdateHotelAsync(int id, HotelDto hotelDto)
    {
        var hotelInDb = await _context.Hotels.SingleOrDefaultAsync(c => c.Id == id);

        if (hotelInDb == null)
            throw new Exception($"Hotel with ID {id} not found");

        _mapper.Map(hotelDto, hotelInDb);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> DeleteHotelAsync(int id)
    {
        var hotel = await _context.Hotels.SingleOrDefaultAsync(c => c.Id == id);

        if (hotel == null)
            throw new Exception($"Hotel with ID {id} not found");

        _context.Hotels.Remove(hotel);
        await _context.SaveChangesAsync();
        
        return true;
    }
}
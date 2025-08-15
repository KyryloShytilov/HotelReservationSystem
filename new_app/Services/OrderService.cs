using Microsoft.EntityFrameworkCore;
using new_app.Data;
using new_app.DTOs;
using new_app.Models;
using new_app.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_app.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Order>> GetOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Hotel)
                .ThenInclude(h => h.Country)
            .ToListAsync();
    }

    public async Task<Order> GetOrderAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Hotel)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            throw new Exception($"Order with ID {id} not found");

        return order;
    }

    public async Task<Order> CreateOrderAsync(NewOrderDto newOrderDto)
    {
        var customer = await _context.Customers.FindAsync(newOrderDto.CustomerId);
        if (customer == null)
            throw new Exception($"Customer with ID {newOrderDto.CustomerId} not found");

        var hotel = await _context.Hotels.FindAsync(newOrderDto.HotelId);
        if (hotel == null)
            throw new Exception($"Hotel with ID {newOrderDto.HotelId} not found");

        var numOfDays = (int)(newOrderDto.EndDate - newOrderDto.StartDate).TotalDays;
        if (numOfDays <= 0)
            throw new Exception("End date must be after start date");

        var fullPrice = Math.Round((hotel.PricePerNight * numOfDays), 2);

        var order = new Order
        {
            Customer = customer,
            Hotel = hotel,
            DateOrdered = DateTime.Now,
            StartDate = newOrderDto.StartDate,
            EndDate = newOrderDto.EndDate,
            NumberOfDays = numOfDays,
            FullPrice = fullPrice
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return order;
    }

    public async Task DeleteOrderAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        
        if (order == null)
            throw new Exception($"Order with ID {id} not found");

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
    }
}
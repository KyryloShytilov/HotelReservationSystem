using Microsoft.EntityFrameworkCore;
using new_app.Data;
using new_app.Models;
using new_app.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_app.Services;

public class CustomerService : ICustomerService
{
    private readonly ApplicationDbContext _context;

    public CustomerService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Customer>> GetCustomersAsync()
    {
        return await _context.Customers.ToListAsync();
    }

    public async Task<Customer> GetCustomerAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
            throw new Exception($"Customer with ID {id} not found");

        return customer;
    }

    public async Task<bool> DeleteCustomerAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        
        if (customer == null)
            throw new Exception($"Customer with ID {id} not found");

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        
        return true;
    }
}
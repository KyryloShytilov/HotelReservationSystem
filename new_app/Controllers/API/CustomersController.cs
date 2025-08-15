using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using new_app.Models;
using new_app.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_app.Controllers.API;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    // GET: api/Customers
    [HttpGet]
    [Authorize(Roles = RoleName.Admin)]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
    {
        return await _customerService.GetCustomersAsync();
    }

    // GET: api/Customers/5
    [HttpGet("{id}")]
    [Authorize(Roles = RoleName.Admin)]
    public async Task<ActionResult<Customer>> GetCustomer(int id)
    {
        try
        {
            return await _customerService.GetCustomerAsync(id);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    // DELETE: api/Customers/5
    [HttpDelete("{id}")]
    [Authorize(Roles = RoleName.Admin)]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        try
        {
            await _customerService.DeleteCustomerAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}
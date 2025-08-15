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
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // GET: api/Orders
    [HttpGet]
    [Authorize(Roles = RoleName.Admin)]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    {
        return await _orderService.GetOrdersAsync();
    }

    // GET: api/Orders/5
    [HttpGet("{id}")]
    [Authorize(Roles = RoleName.Admin)]
    public async Task<ActionResult<Order>> GetOrder(int id)
    {
        try
        {
            return await _orderService.GetOrderAsync(id);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    // POST: api/Orders
    [HttpPost]
    [Authorize(Roles = RoleName.Admin)]
    public async Task<IActionResult> CreateOrder(NewOrderDto newOrderDto)
    {
        try
        {
            await _orderService.CreateOrderAsync(newOrderDto);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE: api/Orders/5
    [HttpDelete("{id}")]
    [Authorize(Roles = RoleName.Admin)]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        try
        {
            await _orderService.DeleteOrderAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}
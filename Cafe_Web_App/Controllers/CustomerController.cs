using Microsoft.AspNetCore.Mvc;
using Repository.IRepository;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Services.IServices;

namespace Cafe_Web_App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAll()
        {
            try
            {
                var customers = await _customerService.GetCustomers();
                if (customers != null)
                {
                    var response = new TResponse<IEnumerable<Customer>>
                    (
                        "lấy danh sách khách hàng thành công",
                        customers
                    );
                    return Ok(response);
                }
                return BadRequest("lấy danh sách khách hàng thất bại");
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(Guid Id)
        {
            try { 
            var customer = await _customerService.GetCustomerById(Id);
                if (customer != null)
                { 
                    var response = new TResponse<Customer>(
                        "lấy thông tin khách hàng thành công",
                        customer
                        );
                    return Ok(response);
                }
                return BadRequest("lấy danh sách khách hàng thất bại");

            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<ActionResult<CustomerResponse>> CreateCustomer([FromBody ] AddCustomerRequest customer)
        {
            try
            {
                var newCustomer = await _customerService.AddCustomer(customer);
                if(customer != null)
                {
                    var response = new TResponse<CustomerResponse>(
                        "khách hàng đã được tạo thành công",
                        newCustomer
                        );
                    return Ok(response);
                }
                return BadRequest("tạo khách hàng thất bại");

            }catch(Exception ex ) { 
                return BadRequest(ex);
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Customer>> UpdateCustomer(Guid Id,[FromBody] UpdateCustomerRequest customer)
        {
            try
            {
                var updateCustomer = await _customerService.UpdateCustomer(Id,customer);
                if(updateCustomer  != null)
                {
                    var response = new TResponse<Customer>(
                        "cập nhật khách hàng thành công",
                        updateCustomer
                        );
                    return Ok(response);
                }
                return BadRequest("cập nhật khác hàng thất bại");

            }catch (Exception ex) {
            return BadRequest(ex.Message);
            }

        }

    }
}

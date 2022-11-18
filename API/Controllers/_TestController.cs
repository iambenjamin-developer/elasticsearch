using API.Models.Tests;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class _TestController : ControllerBase
    {
        [HttpPost]
        public void Post([FromBody] RequestBody request)
        {
        }        
    }
}

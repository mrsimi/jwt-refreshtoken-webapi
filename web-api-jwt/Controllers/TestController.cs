using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace web_api_jwt.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {        
        [HttpGet()]
        public IActionResult Test()
        {
            return Ok("Super secret content, I hope you've got clearance for this...");
        }
    }
}

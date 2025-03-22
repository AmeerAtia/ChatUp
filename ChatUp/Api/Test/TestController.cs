using ChatUp.Api.Authorization;

namespace ChatUp.Api.Test;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpPost, Authorizer]
    public async Task<IActionResult> SendMessage()
    {

        return Ok("done");
    }
}
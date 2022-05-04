using Microsoft.AspNetCore.Mvc;

namespace TelegramApi.Controllers;

public class WebhookController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromServices] HandleUpdateService)
}
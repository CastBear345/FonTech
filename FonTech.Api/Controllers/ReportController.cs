using Microsoft.AspNetCore.Mvc;

namespace FonTech.Api.Controllers;

public class ReportController : ControllerBase
{
    public IActionResult Index()
    {
        return Ok();
    }
}

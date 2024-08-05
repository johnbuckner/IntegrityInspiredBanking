using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Base;

public class BaseController : ControllerBase
{
    [Route("/error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult HandleError() => Problem();
}

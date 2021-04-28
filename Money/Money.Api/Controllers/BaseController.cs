using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Money.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<T> : ControllerBase
    {
        protected readonly ILogger<T> Logger;

        protected BaseController(ILogger<T> logger)
        {
            Logger = logger;
        }
    }
}

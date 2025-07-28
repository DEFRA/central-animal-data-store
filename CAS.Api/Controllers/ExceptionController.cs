using CAS.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
#if DEBUG 
namespace CAS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExceptionController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping() => Ok("api running");

        [HttpGet("not-found")]
        public IActionResult ThrowDomain() => throw new NotFoundException("Sheep", 42);

        [HttpGet("non-domain")]
        public IActionResult ThrowOther() => throw new InvalidOperationException("internal error");

        [HttpGet("validation")]
        public IActionResult ThrowValidation() => throw new ValidationException("test validation exception message");

        [HttpGet("permission")]
        public IActionResult ThrowPermission() => throw new PermissionDeniedException("You are not authorized to perform this action.");

        [HttpGet("conflict")]
        public IActionResult ThrowConflict() => throw new ConflictException("conflict exception message");

        [HttpGet("domain")]
        public IActionResult ThrowDomainGeneric() => throw new DomainException("domain exception message");

        [HttpGet("domain-without-title")]
        public IActionResult ThrowDomainWithoutTitle() => throw new DomainException("Domain error without title"); 

        [HttpGet("null-message")]
        public IActionResult ThrowNullMessage() => throw new NullMessageException();

        [HttpGet("empty-message")]
        public IActionResult ThrowEmptyMessage() => throw new InvalidOperationException(string.Empty);
    }
}
#endif

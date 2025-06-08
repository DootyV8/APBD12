using APBD12.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD12.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpDelete("{idClient}")]
        public async Task<IActionResult> DeleteClient(int idClient)
        {
            var (success, message) = await _clientService.DeleteClientAsync(idClient);
            if (!success) return BadRequest(new { message });
            return NoContent();
        }
    }
}
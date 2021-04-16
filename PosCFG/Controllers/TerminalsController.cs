using PosCFG.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PosCFG.Dto;



namespace PosCFG.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [Authorize(Policy = "ApiScope")]
    [Authorize(Roles = "TerminalsFull")]
    public class TerminalsController : Controller
    {

        private readonly ITerminalService _terminalservice;

        public TerminalsController(ITerminalService terminalservice)
        {
            _terminalservice = terminalservice;
        }


        // GET /api/Terminals
        [HttpGet]
        public async Task<IActionResult> GetTerminals()

        {
            return Ok(await _terminalservice.GetTerminals());

        }


        // GET /api/Terminal/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Terminal>> GetTerminalById(string id)
        {

            var response = await _terminalservice.GetTerminalByID(id);
            if (response.Data == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(response);
            }

        }


        //  POST /api/Terminal      
        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult> AddTerminal([FromBody] GetTerminalDto terminal)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _terminalservice.AddTerminal(terminal);

            if (response.Data == null)
                return NotFound(response);
            return Ok(response);
        }

        //UpdateTerminals pasando DTO
        [HttpPut()]
        [Consumes("application/json")]
        public async Task<ActionResult> UpdateTerminals([FromBody] GetTerminalDto updateTerminalDto)
        {
            var response = await _terminalservice.GetTerminalByID(updateTerminalDto.TerminalID);
            if (response.Data == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(await _terminalservice.UpdateTerminal(updateTerminalDto));
            }

        }

        // DELETE /api/Terminal/id
        [HttpDelete("{terminal_id}")]
        public async Task<IActionResult> DeleteTerminal(string terminal_id)
        {
            if (!this.ModelState.IsValid)
                return BadRequest();
            else
            {
                var response = await _terminalservice.DeleteTerminal(terminal_id);
                if (response.Data == null)
                    return Ok(response);
                else
                {
                    return NotFound(response);
                }
            }
        }
    }
}





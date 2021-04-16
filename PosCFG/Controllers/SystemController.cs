using System.Collections.Generic;
using System.Linq;
using PosCFG.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using PosCFG.Dto;
using Microsoft.AspNetCore.Authorization;



namespace PosCFG.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [Authorize(Policy = "ApiScope")]
    [Authorize(Roles="TerminalsFull")]
    public class SystemController : Controller
    {
       
       private readonly ISystemPOSService _systemPOSService;

       public SystemController(ISystemPOSService systemPOSService)
       {
           _systemPOSService = systemPOSService;
       }

        [HttpGet]
        public async Task<IActionResult> GetSystemPOS()
        {

            return Ok(await _systemPOSService.GetSystemPOS());
        }



    //Search by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<SystemPOS>> GetSystemPOSById(string id){
            
            var response = await _systemPOSService.GetSystemPOSByID(id);
                if(response.Data == null){
                    return NotFound();
                }else{
                    return Ok(response);
                }
            }

        // POST: api/system
        //AddSystemPOS
        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult> AddSystemPOS([FromBody] AddSystemPOSDto systemPOS){
            
            // AGREGA AL ARCHIVO
           // _csvfile.AppendSystemLine(systemPOS,"test_system.txt");

            var response = await _systemPOSService.AddSystemPOS(systemPOS);

            if(response.Data == null)
                return NotFound(response);
            return Ok(response);
        } 

         //UpdateSystemPOS pasando DTO
        [HttpPut()]
        public async Task<ActionResult> UpdateSystemPOS([FromBody] AddSystemPOSDto updateSystempos)
        {
            var response = await _systemPOSService.GetSystemPOSByID(updateSystempos.TerminalID);
                if(response.Data == null){
                    return NotFound();
                }else{
                    return Ok(await _systemPOSService.UpdateSystemPOS(updateSystempos));
                }
            
        }

        [HttpPut("test/")]
        public async Task<ActionResult> UpdateSystemPOSFromCSV()
        {
           /* var response = await _systemPOSService.GetSystemPOSByID(updateSystempos.TerminalID);
                if(response.Data == null){
                    return NotFound();
                }else{
                    return Ok(await _systemPOSService.UpdateSystemPOS(updateSystempos));
                }*/
            return Ok(await _systemPOSService.UpdateSystemPOSFromCSV());
        }

        
        //Delete SystemPOS
        [HttpDelete("{terminal_id}")]
        public async Task<ActionResult> DeleteSystemPOS(string terminal_id){
            
            var response = await _systemPOSService.GetSystemPOSByID(terminal_id);
                
            if(response.Data == null)
                return NotFound(response);
            else
            {
                return Ok(await _systemPOSService.DeleteSystemPOS(terminal_id));
            }
            
            
        }
            
    } 
}

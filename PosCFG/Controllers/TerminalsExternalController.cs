using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PosCFG.Dto;
using PosCFG.Service;
using Microsoft.AspNetCore.Authorization;


namespace PosCFG.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "ApiScope")]
    [Authorize(Roles = "TerminalsExternal")]
    public class TerminalsExternalController : ControllerBase
    {
        private readonly ITerminalsExternalService _terminalsexternalservice;
        public TerminalsExternalController(ITerminalsExternalService terminalsexternalservice)
        {
                _terminalsexternalservice = terminalsexternalservice;
        }

        [HttpGet("client/")]
        public async Task<IActionResult> GetAllTerminalsExternalList([FromQuery] bool? terminalEnabled)
        {
            return Ok(await _terminalsexternalservice.GetAllTerminalsExternalList(terminalEnabled));
        }
        
        
        

             //Search by ID
        [HttpGet("client/{id}")]
        public async Task<ActionResult<GetTerminalExternalClientDto>> GetTerminalExternalClientById(string id){
            
            var response = await _terminalsexternalservice.GetTerminalExternalClientById(id);
                if(response.Data == null){
                    response.Message = "No existe registro con ese Termianl ID ";
                    return NotFound(response);
                }else{
                    return Ok(response);
                }
        }

        
        [HttpPut("client/")]
        [Consumes("application/json")]
        public async Task<ActionResult> UpdateTerminalsExternalClient([FromBody] UpdateTerminalExternalClientDto updateTerminalsExternal)
        {
            if(ModelState.IsValid)
            {
                var response = await _terminalsexternalservice.GetTerminalExternalClientById(updateTerminalsExternal.TerminalID);
                if(response.Data == null){
                    response.Message = "No existe registro con ese Termianl ID ";
                    response.Success = false;
                    return NotFound(response);
                }
                else{
                    if(updateTerminalsExternal.TerminalID != null && (updateTerminalsExternal.TerminalID).Length > 8)
                    {   
                        response.Message = "Terminal ID supera la cantidad de caracteres (8) permitidos";
                        response.Data = null;
                        response.Success = false;
                        return BadRequest(response);
                    }
                    ////else{
                    if(updateTerminalsExternal.nombreComercial !=null && (updateTerminalsExternal.nombreComercial).Length > 24)
                    {
                        response.Message = "Nombre Comercial supera la cantidad de caracteres (24) permitidos";
                        response.Data = null;
                        response.Success = false;
                        return BadRequest(response);
                    }
                    ////    else{
                    if(updateTerminalsExternal.direccionFiscal != null && (updateTerminalsExternal.direccionFiscal).Length > 24)
                    {
                        response.Message = "Direccion Fiscal supera la cantidad de caracteres (24) permitidos";
                        response.Data = null;
                        response.Success = false;
                        return BadRequest(response);
                    }
                    ////        else{
                    if(updateTerminalsExternal.rut != null && (updateTerminalsExternal.rut).Length > 12)
                    {
                        response.Message = "RUT supera la cantidad de caracteres (12) permitidos";
                        response.Data = null;
                        response.Success = false;
                        return BadRequest(response);
                    }
                    ////            else{
                    if(updateTerminalsExternal.terminalEnabled != null && updateTerminalsExternal.terminalEnabled !=0 && updateTerminalsExternal.terminalEnabled !=1 )
                    {
                        response.Message = "Terminal Enabled debe ser 0 o 1";
                        response.Data = null;
                        response.Success = false;
                        return BadRequest(response);
                    }
                    if(updateTerminalsExternal.ca_name != null && (updateTerminalsExternal.ca_name).Length != 40 )
                    {
                        response.Message = "El largo del parametro ca_name es mayor o menor que 40";
                        response.Data = null;
                        response.Success = false;
                        return BadRequest(response);
                    }
                    if(updateTerminalsExternal.mcc != null && (updateTerminalsExternal.mcc).Length != 4 )
                    {
                        response.Message = "El largo del parametro mcc es mayor o menor que 4";
                        response.Data = null;
                        response.Success = false;
                        return BadRequest(response);
                    }
                    if(updateTerminalsExternal.pf_id != null && (updateTerminalsExternal.pf_id).Length != 11 )
                    {
                        response.Message = "El pf_id debe ser de 11 caracteres.";
                        response.Data = null;
                        response.Success = false;
                        return BadRequest(response);
                    }
                    if(updateTerminalsExternal.visa_spnsrd_mercht != null && (updateTerminalsExternal.visa_spnsrd_mercht).Length != 15 )
                    {
                        response.Message = "El largo del parametro visa_spnsrd_mercht es mayor o menor a 15";
                        response.Data = null;
                        response.Success = false;
                        return BadRequest(response);
                    }
                    if(updateTerminalsExternal.amex_id_comercio != null && (updateTerminalsExternal.amex_id_comercio).Length != 20 )
                    {
                        response.Message = "El largo del parametro amex_id_comercio es mayor o menor a 20";
                        response.Data = null;
                        response.Success = false;
                        return BadRequest(response);
                    }
                    ////                else{
                    response=await _terminalsexternalservice.UpdateTerminalExternalClient(updateTerminalsExternal);
                    if (response.Success){

                        return Ok(response);
                    }
                    else{
                        
                        return BadRequest();
                    }
                                        
                                    ////}
                                ////}
                            ////}    
                        ////}   
                    ////}
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

    }
}
using PosCFG.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PosCFG.Dto;


public interface ITerminalService
{
    Task<ServiceResponse<List<GetTerminalDto>>> GetTerminals();
    Task<ServiceResponse<GetTerminalDto>> GetTerminalByID(string terminalID);
    Task<ServiceResponse<GetTerminalDto>> AddTerminal(GetTerminalDto terminal);
    Task<ServiceResponse<GetTerminalDto>> UpdateTerminal(GetTerminalDto updateTerminalDto);
    Task<ServiceResponse<Terminal>> DeleteTerminal(string terminal_id);



}
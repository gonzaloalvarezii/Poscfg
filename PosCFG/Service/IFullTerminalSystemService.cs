using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PosCFG.Dto;
using PosCFG.Models;

public interface IFullTerminalSystemService
{
    //bool existeTerminal(string terminalID);
    Task<ServiceResponse<GetTerminalDto>> GetTerminalByID(string terminal_id);
    Task<ServiceResponse<GetTerminalDto>> InsertFullTerminalSystem(AddFullTerminalSystemDto fullTerminalSystem);
    Task<ServiceResponse<GetFullTerminalSystemDto>> GetFullTerminalSystemByID(string terminal_id);
    Task<ServiceResponse<List<GetTerminalDto>>> GetTerminalSystem();
    Task<ServiceResponse<GetFullTerminalSystemDto>> UpdateFulTerminalSystem(GetFullTerminalSystemDto updateFullTerminalSystem);
    Task<ServiceResponse<Terminal>> DeleteFullTerminalSystem(string terminal_id);
    Task<ServiceResponse<CargaMasivaAux>> InsertTerminalCargaMasivaAux(AddFullTerminalSystemDto fullTerminalSystemDto);
    Task<ServiceResponse<List<GetCargaMasivaDto>>> GetTerminalAux();
    Task<ServiceResponse<GetCargaMasivaDto>> GetTerminalAuxByID(int id);
    Task<ServiceResponse<GetTerminalDto>> InsertNewFullTerminalFromCargaMasivaAux(GetCargaMasivaDto terminal_aux);
    Task<ServiceResponse<GetCargaMasivaDto>> DeleteTerminalCargaMasivaAux(int terminal_id);


}
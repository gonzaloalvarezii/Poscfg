using System.Collections.Generic;
using System.Threading.Tasks;
using PosCFG.Dto;

namespace PosCFG.Service
{
    public interface ITerminalsExternalService
    {


/*        Task<ServiceResponse<List<GetTerminalsExternalDto>>> GetAllTerminalsExternal();
        Task<ServiceResponse<GetTerminalsExternalDto>> GetTerminalExternalById(string terminalID);
        Task<ServiceResponse<GetTerminalsExternalDto>> UpdateTerminalExternal(UpdateTerminalsExternalDto updateTerminalExternal);
*/
        Task<ServiceResponse<List<GetTerminalsExternalClientListDto>>> GetAllTerminalsExternalList(bool? terminalEnabled = null);
        Task<ServiceResponse<GetTerminalExternalClientDto>> GetTerminalExternalClientById(string terminalID);
        Task<ServiceResponse<GetTerminalExternalClientDto>> UpdateTerminalExternalClient(UpdateTerminalExternalClientDto updateTerminalExternal);
        

    }
}
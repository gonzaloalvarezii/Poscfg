using System.Collections.Generic;
using System.Threading.Tasks;
using PosCFG.Models;
using Microsoft.EntityFrameworkCore;
using PosCFG.Dto;
using PosCFG.Service;

public interface ISystemPOSService
{
    Task<ServiceResponse<List<GetSystemPOSDto>>>  GetSystemPOS();
    Task<ServiceResponse<GetSystemPOSDto>> GetSystemPOSByID(string id);
    Task<ServiceResponse<List<GetSystemPOSDto>>> AddSystemPOS(AddSystemPOSDto systempos);
    Task<ServiceResponse<List<GetSystemPOSDto>>> DeleteSystemPOS(string id);
    Task<ServiceResponse<Dictionary<string,CSVService.TerminalChecksums>>> UpdateSystemPOSFromCSV();
    Task<ServiceResponse<GetSystemPOSDto>> UpdateSystemPOS(AddSystemPOSDto updatesystempos);


}
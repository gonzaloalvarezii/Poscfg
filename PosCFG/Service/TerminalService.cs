using System.Collections.Generic;
using System.Linq;
using PosCFG.Models;
using PosCFG.Dto;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using AutoMapper;
using PosCFG.Service;




public class TerminalService : ITerminalService
{

    private readonly PosCFGDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICSVService _csvfile;
    
    public TerminalService(PosCFGDbContext context, IMapper mapper, ICSVService csvfile)
    {
        _context = context; 
        _mapper = mapper;
        _csvfile = csvfile;
    }
    public async Task<ServiceResponse<List<GetTerminalDto>>> GetTerminals()
    {
        ServiceResponse<List<GetTerminalDto>> serviceResponse = new ServiceResponse<List<GetTerminalDto>>();
        try{
            var terminals =  await _context.Terminals.ToListAsync();
            serviceResponse.Data = _mapper.Map<List<GetTerminalDto>>(terminals);
        }
        catch (Exception ex)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = ex.Message;  
        }
        return serviceResponse;
            
    }
    public async Task<ServiceResponse<GetTerminalDto>> GetTerminalByID(string terminal_id)
    {
        ServiceResponse<GetTerminalDto> serviceResponse = new ServiceResponse<GetTerminalDto>();
        try{
            //Terminal terminal = new Terminal();
            var terminal = await _context.Terminals.FirstOrDefaultAsync(ct => ct.TerminalID == terminal_id);
            
            serviceResponse.Data = _mapper.Map<GetTerminalDto>(terminal);
           
        }
        catch (Exception ex)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = ex.Message;  
        }
        return serviceResponse;
        
    }

    public async Task<ServiceResponse<GetTerminalDto>> AddTerminal(GetTerminalDto terminal_dto)
    {
        ServiceResponse<GetTerminalDto> serviceResponse = new ServiceResponse<GetTerminalDto>();
        try
        {
            var ter = await _context.Terminals.FirstOrDefaultAsync(t => t.TerminalID.Equals(terminal_dto.TerminalID));
            if(ter == null)
            {
               // AGREGA AL ARCHIVO
               _csvfile.AppendTerminalsLine(terminal_dto);

                var terminal = _mapper.Map<Terminal>(terminal_dto);
                
                var newTerminal =  await _context.Terminals.AddAsync(terminal);
                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetTerminalDto>(terminal);
            }
            else 
            {
                serviceResponse.Message = " Terminal ID ya existe. ";
            }
        
        }
        catch (Exception ex)
        {   if (ex is DbUpdateException)
            {
            
            serviceResponse.Success = false;
            serviceResponse.Message = "DbUpdateException" + ex.Message;
            }
            else 
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            
        }
        return serviceResponse;
    }
    
    public async Task<ServiceResponse<GetTerminalDto>> UpdateTerminal(GetTerminalDto updateTerminalDto)
        {
            ServiceResponse<GetTerminalDto> serviceResponse = new ServiceResponse<GetTerminalDto>();

            try
            {
                // ACTUALIZA EL ARCHIVO
                //_csvfile.updateTerminalsLine(updateTerminalDto.TerminalID, updateTerminalDto);
                _csvfile.UpdateTerminalsLine(updateTerminalDto.TerminalID, updateTerminalDto);

                Terminal terminal = await _context.Terminals.FirstOrDefaultAsync(ct => ct.TerminalID.Equals(updateTerminalDto.TerminalID));

                terminal.TerminalID = updateTerminalDto.TerminalID;
                terminal.SerialNumber = updateTerminalDto.SerialNumber;
                terminal.Suspend = updateTerminalDto.Suspend;
                //terminal.HeaderLine1 = updateTerminalDto.HeaderLine1;
                //terminal.HeaderLine2 = updateTerminalDto.HeaderLine2;
                //terminal.HeaderLine3 = updateTerminalDto.HeaderLine3;
                terminal.ParmConnectTime = updateTerminalDto.ParmConnectTime;
                terminal.Custom1 = updateTerminalDto.Custom1;
                terminal.Custom2 = updateTerminalDto.Custom2;
                terminal.Custom3 = updateTerminalDto.Custom3;
                terminal.Custom4 = updateTerminalDto.Custom4;
                terminal.Custom5 = updateTerminalDto.Custom5;
                terminal.Custom6 = updateTerminalDto.Custom6;
                terminal.Custom7 = updateTerminalDto.Custom7;
                terminal.Custom8 = updateTerminalDto.Custom8;
                terminal.Custom9 = updateTerminalDto.Custom9;
                terminal.Custom10 = updateTerminalDto.Custom10;
                terminal.Custom11 = updateTerminalDto.Custom11;
                terminal.Custom12 = updateTerminalDto.Custom12;
                terminal.Custom13 = updateTerminalDto.Custom13;
                terminal.Custom14 = updateTerminalDto.Custom14;
                terminal.Custom15 = updateTerminalDto.Custom15;
                terminal.Custom16 = updateTerminalDto.Custom16;
                terminal.Custom17 = updateTerminalDto.Custom17;
                terminal.Custom18 = updateTerminalDto.Custom18;
                terminal.Custom19 = updateTerminalDto.Custom19;
                terminal.Custom20 = updateTerminalDto.Custom20;
                terminal.TranConnectTime = updateTerminalDto.TranConnectTime;




                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetTerminalDto>(terminal);


            }
            catch (Exception ex)
            { 
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
         return serviceResponse;
    }



    public async Task<ServiceResponse<Terminal>> DeleteTerminal(string terminal_id)
    {
        ServiceResponse<Terminal> serviceResponse = new ServiceResponse<Terminal>();
        try{
            var terminal = await _context.Terminals.FirstOrDefaultAsync(ct => ct.TerminalID.Equals(terminal_id));
            _context.Terminals.Remove(terminal);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
        return serviceResponse;


    }
}

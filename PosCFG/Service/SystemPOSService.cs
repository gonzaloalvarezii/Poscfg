using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PosCFG.Models;
using AutoMapper;
using PosCFG.Dto;
using System.Linq;
using PosCFG.Service;

public class SystemPOSService : ISystemPOSService
{
        private readonly PosCFGDbContext _context; 
        private readonly IMapper _mapper;
        private readonly ICSVService _csvfile;

        public SystemPOSService(PosCFGDbContext context, IMapper mapper, ICSVService csvfile)
        {
            _context = context; 
            _mapper = mapper;
            _csvfile = csvfile;
        }
       
        public async Task<ServiceResponse<List<GetSystemPOSDto>>> GetSystemPOS()
        {
            ServiceResponse<List<GetSystemPOSDto>> serviceResponse = new ServiceResponse<List<GetSystemPOSDto>>();
            try
            {
                var systempos = await _context.SystemPOSs.ToListAsync();
                serviceResponse.Data = _mapper.Map<List<GetSystemPOSDto>>(systempos);
                
            }catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetSystemPOSDto>> GetSystemPOSByID(string id)
        {
            ServiceResponse<GetSystemPOSDto> serviceResponse = new ServiceResponse<GetSystemPOSDto>();
            try
            {
                var systempos = await _context.SystemPOSs.FirstOrDefaultAsync(ct=> ct.TerminalID.Equals(id));
                serviceResponse.Data = _mapper.Map<GetSystemPOSDto>(systempos);

            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetSystemPOSDto>>> AddSystemPOS(AddSystemPOSDto systempos)
        {
            ServiceResponse<List<GetSystemPOSDto>> serviceResponse = new ServiceResponse<List<GetSystemPOSDto>>();
            try
            {
                
                //validar que exista Terminal
                var term = await _context.Terminals
                .FirstOrDefaultAsync(t => t.TerminalID.Equals(systempos.TerminalID));
                await _context.SaveChangesAsync();

                    if (term != null)
                    {
                        var sys = await _context.SystemPOSs.FirstOrDefaultAsync(s => s.TerminalID.Equals(systempos.TerminalID));
                        if (sys == null)
                        {
                            // APPEND AL ARCHIVO
                            _csvfile.AppendSystemLine(systempos);
                            
                            SystemPOS newSystemPOS = _mapper.Map<SystemPOS>(systempos);
                            await _context.SystemPOSs.AddAsync(newSystemPOS);
                            await _context.SaveChangesAsync();
                            var listsystempos = await _context.SystemPOSs.ToListAsync();
                            serviceResponse.Data = _mapper.Map<List<GetSystemPOSDto>>(listsystempos);
                        }
                        else
                        {
                            serviceResponse.Message = " SystemPOS already exists. ";
                        }

                    }
                    else
                    {
                        serviceResponse.Message = " First, Add Terminal. TerminalID no exist. ";
                    }

            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetSystemPOSDto>>> DeleteSystemPOS(string id)
        {
            ServiceResponse<List<GetSystemPOSDto>> serviceResponse = new ServiceResponse<List<GetSystemPOSDto>>();
            try
            {
                SystemPOS systemPOS = await _context.SystemPOSs.FirstAsync(c => c.TerminalID.Equals(id));
                _context.SystemPOSs.Remove(systemPOS);
                await _context.SaveChangesAsync();
                var listSystemPOS = await _context.SystemPOSs.ToListAsync();
                serviceResponse.Data = _mapper.Map<List<GetSystemPOSDto>>(listSystemPOS);

            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<Dictionary<string,CSVService.TerminalChecksums>>> UpdateSystemPOSFromCSV()
        {
            ServiceResponse<Dictionary<string,CSVService.TerminalChecksums>> serviceResponse = new ServiceResponse<Dictionary<string,CSVService.TerminalChecksums>>();
            try 
            {
                Dictionary<string,CSVService.TerminalChecksums> dTerminalChecksums=_csvfile.checksumsUpdate();

                foreach( KeyValuePair<string, CSVService.TerminalChecksums> kvp in dTerminalChecksums )
                {
                    Console.WriteLine("TerminalID = {0}, TerminalChecksum = {1}, ControlChecksum = {2}, ParmConnChecksum = {3}, TranConnChecksum = [{4}]", 
                        kvp.Key, kvp.Value.TerminalChecksum, kvp.Value.ControlChecksum, kvp.Value.ParmConnChecksum, string.Join(", ", kvp.Value.TranConnChecksum) );
                }

                ///PRUEBA
                serviceResponse.Data=dTerminalChecksums;
            }
            catch (Exception ex)
            { 
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            await Task.Delay(1000);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetSystemPOSDto>> UpdateSystemPOS(AddSystemPOSDto updatesystempos)
        {
            ServiceResponse<GetSystemPOSDto> serviceResponse = new ServiceResponse<GetSystemPOSDto>();
            try
            {
                //ACTUALIZAR SYSTEM.TXT
                bool rstTerminalChecksum=false;
                bool rstControlChecksum=false;
                bool rstTranConnChecksum=false;
                bool newPackage=false;
                _csvfile.UpdateSystemLine(updatesystempos.TerminalID,updatesystempos,rstTerminalChecksum,rstTranConnChecksum,rstControlChecksum, newPackage);

                SystemPOS systempos = await _context.SystemPOSs.FirstOrDefaultAsync(ct => ct.TerminalID.Equals(updatesystempos.TerminalID));

                systempos.TerminalChecksum = updatesystempos.TerminalChecksum;
                systempos.ControlGroup = updatesystempos.ControlGroup;
                systempos.ControlCheckSum = updatesystempos.ControlCheckSum;
                systempos.ParameterGroup = updatesystempos.ParameterGroup;
                systempos.ParameterReload = updatesystempos.ParameterReload;
                systempos.ParameterVersion = updatesystempos.ParameterVersion;
                systempos.ProgramID = updatesystempos.ProgramID;
                systempos.ProgramReload = updatesystempos.ProgramReload;
                systempos.ProgramVersion = updatesystempos.ParameterVersion;
                systempos.Paquete = updatesystempos.Paquete;
                systempos.ConnectGroup = updatesystempos.ConnectGroup;
                systempos.ParmConnChecksum = updatesystempos.ParmConnChecksum;
                systempos.TranConnChecksum1 = updatesystempos.TranConnChecksum1;
                systempos.TranConnChecksum2 = updatesystempos.TranConnChecksum2;
                
                await _context.SaveChangesAsync();
           
                serviceResponse.Data = _mapper.Map<GetSystemPOSDto>(systempos);


            }
            catch (Exception ex)
            { 
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
         return serviceResponse;
    }

    
}
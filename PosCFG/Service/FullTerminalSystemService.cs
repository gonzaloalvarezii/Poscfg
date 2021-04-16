using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PosCFG.Dto;
using PosCFG.Models;
using System;
using System.Threading.Tasks;
using PosCFG.Service;
using System.Collections.Generic;
using System.Linq;
using PosCFG.JPOS;


namespace PosCFG.Service
{
    public class FullTerminalSystemService : IFullTerminalSystemService
    {
        private readonly PosCFGDbContext _context;
        private readonly JPOSDbContext _MySqlcontext;
        private readonly IMapper _mapper;
        private readonly ICSVService _csvfile;
        private string addpft = "pft.";
        private string addca = "ca.";
        private string addRUT = "RUT:";
        ////char pad = ' ';

        public FullTerminalSystemService(PosCFGDbContext context, JPOSDbContext mysqlcontext, IMapper mapper, ICSVService csvfile)
        {
            _context = context;
            _MySqlcontext = mysqlcontext;
            _mapper = mapper;
            _csvfile = csvfile;
        }

        public async Task<ServiceResponse<GetTerminalDto>> InsertFullTerminalSystem(AddFullTerminalSystemDto fullTerminalSystem)
        {

            ServiceResponse<GetTerminalDto> serviceResponse = new ServiceResponse<GetTerminalDto>();

        
            try
            {
                serviceResponse= await GetTerminalByID(fullTerminalSystem.TerminalID);
              
               
                //AGREGO TERMINAL SI NO EXISTE
                if (serviceResponse.Data == null)
                {

                    Terminal terminal = new Terminal();

                    terminal.TerminalID = fullTerminalSystem.TerminalID;
                    terminal.SerialNumber = fullTerminalSystem.SerialNumber;
                    //rst-69
                    terminal.ParmConnectTime = GetConnectTime.getConnectTime(_context);

                    terminal.Custom1 = fullTerminalSystem.HeaderLine1;
                    terminal.Custom2 = fullTerminalSystem.HeaderLine2;
                    //RUT
                    terminal.Custom3 = addRUT + fullTerminalSystem.HeaderLine3;
                    terminal.Custom4 = fullTerminalSystem.Custom4;
                    terminal.Custom5 = fullTerminalSystem.Custom5;
                    terminal.Custom6 = fullTerminalSystem.Custom6;
                    terminal.Custom7 = fullTerminalSystem.Custom7;
                    terminal.Custom8 = fullTerminalSystem.Custom8;
                    terminal.Custom9 = fullTerminalSystem.Custom9;
                    terminal.Custom10 = fullTerminalSystem.Custom10;
                    terminal.Custom11 = fullTerminalSystem.Custom11;
                    terminal.Custom12 = fullTerminalSystem.Custom12;
                    terminal.Custom13 = fullTerminalSystem.Custom13;
                    terminal.Custom14 = fullTerminalSystem.Custom14;
                    terminal.Custom15 = fullTerminalSystem.Custom15;
                    terminal.Custom16 = fullTerminalSystem.Custom16;
                    //cabal u$s
                    terminal.Custom19 = fullTerminalSystem.Custom19;
                    await _context.Terminals.AddAsync(terminal);

                    //AGREGO SYSTEM
                    SystemPOS systempos = new SystemPOS();
                    systempos.TerminalID = fullTerminalSystem.TerminalID;
                    systempos.ControlGroup = fullTerminalSystem.ControlGroup;
                    //bool tiene true (checked) por defecto
                    //RST-20
                    if (fullTerminalSystem.ControlCheckSum)
                    {
                        systempos.ControlCheckSum = 0;
                    }

                    systempos.ParameterGroup = fullTerminalSystem.ParameterGroup;
                    //bool tiene true por defecto
                    //RST-20
                    if (fullTerminalSystem.ParameterReload)
                    {
                        systempos.ParameterReload = 1;
                    }

                    systempos.ParameterVersion = fullTerminalSystem.ParameterVersion;
                    systempos.ProgramID = fullTerminalSystem.ProgramID;
                    //bool Cargar Programa (Program Reload)
                    //RST-20
                    if (fullTerminalSystem.ProgramReload)
                    {
                        systempos.ProgramReload = 1;
                    }

                    systempos.ProgramVersion = fullTerminalSystem.ProgramVersion;
                    systempos.Paquete = fullTerminalSystem.Paquete;
                    systempos.ConnectGroup = fullTerminalSystem.ConnectGroup;
                    //bool tiene true (checked) por defecto
                    //RST-20
                    if (fullTerminalSystem.ParmConnChecksum)
                    {
                        systempos.ParmConnChecksum = 0;
                    }

                    //bool viene true por defecto
                    //RST-20
                    if (fullTerminalSystem.TerminalChecksum)
                    {
                        systempos.TerminalChecksum = 0;
                    }


                    await _context.SystemPOSs.AddAsync(systempos);
                    //await _context.SaveChangesAsync();



                    if (fullTerminalSystem.enabled_JPOS)
                    {
                        //AGREGO TerminalStatus 1
                        TerminalStatus term = new TerminalStatus();
                        term.TerminalID = fullTerminalSystem.TerminalID;
                        term.status = 1;
                        await _context.TerminalsStatus.AddAsync(term);


                        jpos jpos_value = new jpos();
                        char pad = '0';
                        string[] parameters = new string[]{
                                fullTerminalSystem.ca == "" ? null : fullTerminalSystem.ca,
                                fullTerminalSystem.mcc == "" ? null : fullTerminalSystem.mcc,
                                fullTerminalSystem.pf_id == "" || fullTerminalSystem.pf_id == null ? null : fullTerminalSystem.pf_id.PadLeft(11, pad),
                                fullTerminalSystem.visa_spnsrd_mercht == "" ? null : fullTerminalSystem.visa_spnsrd_mercht,
                                fullTerminalSystem.amex_id_comercio == "" ? null : fullTerminalSystem.amex_id_comercio,
                        };

                        Sysconfig sys = await _MySqlcontext.sysconfig.FirstOrDefaultAsync(j => j.id.Equals(addca + fullTerminalSystem.TerminalID));
                        //Existe terminal en jpos con ca.
                        if (sys != null)
                        {

                            if (sys.value != null)
                            {
                                jpos_value.setSysconfigValue_CA(sys.value);
                            }
                        }
                        else
                        {

                            sys = new Sysconfig();
                            sys.id = addca + fullTerminalSystem.TerminalID;
                            await _MySqlcontext.sysconfig.AddAsync(sys);

                            //AGREGA pft.
                            Sysconfig syspft = new Sysconfig();
                            syspft.id = addpft + fullTerminalSystem.TerminalID;
                            syspft.value = "handy";
                            await _MySqlcontext.sysconfig.AddAsync(syspft);
                        }

                        jpos_value.updateSysconfigValue_CA(parameters);
                        sys.value = jpos_value.genSysconfigValue_CA();

                    }



                    //commit luego de todos los cambios
                    await _context.SaveChangesAsync();
                    await _MySqlcontext.SaveChangesAsync();

                    //AGREGO LINEA AL ARCHIVO TERMINALS
                    _csvfile.AppendTerminalsLine(fullTerminalSystem);

                    //AGREGO LINEA AL ARCHIVO SYSTEM
                    _csvfile.AppendSystemLine(fullTerminalSystem);

                    serviceResponse = await GetTerminalByID(fullTerminalSystem.TerminalID);

                }
                else {

                    serviceResponse.Success = false;
                    serviceResponse.Message = "Terminal Existente.";

                }
              


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

        public async Task<ServiceResponse<GetFullTerminalSystemDto>> GetFullTerminalSystemByID(string terminal_id)
        {
            ServiceResponse<GetFullTerminalSystemDto> serviceResponse = new ServiceResponse<GetFullTerminalSystemDto>();
            try
            {
                var terminal = await _context.Terminals.FirstOrDefaultAsync(t => t.TerminalID.Equals(terminal_id));
                var system = await _context.SystemPOSs.FirstOrDefaultAsync(t => t.TerminalID.Equals(terminal_id));

                GetFullTerminalSystemDto fullTerminalSystem = new GetFullTerminalSystemDto();
                fullTerminalSystem.TerminalID = terminal.TerminalID;
                fullTerminalSystem.SerialNumber = terminal.SerialNumber;
                //rst-69
                fullTerminalSystem.ParmConnectTime = terminal.ParmConnectTime;
                fullTerminalSystem.HeaderLine1 = terminal.Custom1;
                fullTerminalSystem.HeaderLine2 = terminal.Custom2;
                
                //remove RUT:
                fullTerminalSystem.HeaderLine3 = terminal.Custom3.Remove(0,4);
                fullTerminalSystem.Custom4 = terminal.Custom4;
                fullTerminalSystem.Custom5 = terminal.Custom5;
                fullTerminalSystem.Custom6 = terminal.Custom6;
                fullTerminalSystem.Custom7 = terminal.Custom7;
                fullTerminalSystem.Custom8 = terminal.Custom8;
                fullTerminalSystem.Custom9 = terminal.Custom9;
                fullTerminalSystem.Custom10 = terminal.Custom10;
                fullTerminalSystem.Custom11 = terminal.Custom11;
                fullTerminalSystem.Custom12 = terminal.Custom12;
                fullTerminalSystem.Custom13 = terminal.Custom13;
                fullTerminalSystem.Custom14 = terminal.Custom14;
                fullTerminalSystem.Custom15 = terminal.Custom15;
                fullTerminalSystem.Custom16 = terminal.Custom16;
                fullTerminalSystem.Custom19 = terminal.Custom19;
                fullTerminalSystem.merchanType = system.ControlGroup;
                
                //actualizar datos terminal
                if(system.TerminalChecksum == 0)
                    fullTerminalSystem.TerminalChecksum = true;
                else
                    fullTerminalSystem.TerminalChecksum = false;
                
                //actualizar parametros de conexion
                if(system.ParmConnChecksum == 0)
                    fullTerminalSystem.ParmConnChecksum = true;
                else
                    fullTerminalSystem.ParmConnChecksum = false;

                //actualizar tipo de comercio - controlchecksum
                if(system.ControlCheckSum == 0)
                    fullTerminalSystem.ControlCheckSum = true;
                else
                    fullTerminalSystem.ControlCheckSum = false;

                //cargar parametros de conexion
                if(system.ParameterReload == 1)
                    fullTerminalSystem.ParameterReload = true;
                else
                    fullTerminalSystem.ParameterReload = false;

                //cargar programa - program reload
                if(system.ProgramReload == 1)
                    fullTerminalSystem.ProgramReload = true;
                else
                    fullTerminalSystem.ProgramReload = false;

                fullTerminalSystem.ParameterGroup = system.ParameterGroup;
                fullTerminalSystem.ParameterVersion = system.ParameterVersion;
                fullTerminalSystem.ProgramID = system.ProgramID;
                fullTerminalSystem.ProgramVersion = system.ProgramVersion;
                fullTerminalSystem.Paquete = system.Paquete;
                fullTerminalSystem.ConnectGroup = system.ConnectGroup;
                
                
                //parametros de JPOS falta definir cuales son
                var jpos = await _MySqlcontext.sysconfig.FirstOrDefaultAsync(j => j.id.Equals(addca+fullTerminalSystem.TerminalID));
                if(jpos != null )
                {
                    if ( jpos.value != null ){

                        jpos jp=new jpos();
                        jp.setSysconfigValue_CA(jpos.value);
                        
                        string ca=jp.getCA();
                        string mcc =jp.getMcc();
                        string pf_id=jp.getPf_id();
                        string visa_spnsrd_mercht=jp.getVisa_spnsrd_mercht();
                        string amex_id_comercio=jp.getAmex_id_comercio();

                        fullTerminalSystem.ca = ca.Length == 0 ? null : ca; 
                        fullTerminalSystem.mcc = mcc.Length == 0 ? null : mcc; 
                        fullTerminalSystem.pf_id = pf_id.Length == 0 ? null : pf_id; 
                        fullTerminalSystem.visa_spnsrd_mercht = visa_spnsrd_mercht.Length == 0 ? null : visa_spnsrd_mercht; 
                        fullTerminalSystem.amex_id_comercio = amex_id_comercio.Length == 0 ? null : amex_id_comercio; 
                    }
                    
                    var status = await _context.TerminalsStatus.FirstOrDefaultAsync(s => s.TerminalID.Equals(terminal_id));
                    if(status != null)
                    {
                        if(status.status == 1)
                            fullTerminalSystem.enabled_JPOS = true;
                        else
                            fullTerminalSystem.enabled_JPOS = false;
                    }
                    else
                    {
                    fullTerminalSystem.enabled_JPOS = false;
                    }
                    
                }
                serviceResponse.Data = fullTerminalSystem;

            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;  
            }

            return serviceResponse;
        }


        public async Task<ServiceResponse<List<GetTerminalDto>>> GetTerminalSystem()
        {
            ServiceResponse<List<GetTerminalDto>> serviceResponse = new ServiceResponse<List<GetTerminalDto>>();
            try
            {
                //Traigo las tuplas que estan en terminal y system
                List<Terminal> lists = await _context.Terminals
                                    .Include(s => s.SystemPOS)
                                    .Where(s => s.SystemPOS != null)
                                    .ToListAsync();

                serviceResponse.Data = _mapper.Map<List<GetTerminalDto>>(lists);
            

        
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            
            return serviceResponse;

        }

        public async Task<ServiceResponse<GetFullTerminalSystemDto>> UpdateFulTerminalSystem(GetFullTerminalSystemDto updateFullTerminalSystem)
        {
            ServiceResponse<GetFullTerminalSystemDto> serviceResponse = new ServiceResponse<GetFullTerminalSystemDto>();
            try
            {
                bool rstTerminalChecksum=false;
                bool rstTranConnChecksum=false;
                bool rstControlChecksum=false;
                bool newPackage=false;
                Terminal terminal = await _context.Terminals.FirstOrDefaultAsync(ct => ct.TerminalID.Equals(updateFullTerminalSystem.TerminalID));
                SystemPOS systempos = await _context.SystemPOSs.FirstOrDefaultAsync(ct => ct.TerminalID.Equals(updateFullTerminalSystem.TerminalID));

                if ( terminal.TerminalID != updateFullTerminalSystem.TerminalID ||
                terminal.SerialNumber != updateFullTerminalSystem.SerialNumber ||
                //rst-69
                terminal.ParmConnectTime != updateFullTerminalSystem.ParmConnectTime ||
                terminal.Custom1 != updateFullTerminalSystem.HeaderLine1 ||
                terminal.Custom2 != updateFullTerminalSystem.HeaderLine2 ||
                //addRUT
                terminal.Custom3 != addRUT+updateFullTerminalSystem.HeaderLine3 ||
                terminal.Custom4 != updateFullTerminalSystem.Custom4 ||
                terminal.Custom5 != updateFullTerminalSystem.Custom5 ||
                terminal.Custom6 != updateFullTerminalSystem.Custom6 ||
                terminal.Custom7 != updateFullTerminalSystem.Custom7 ||
                terminal.Custom8 != updateFullTerminalSystem.Custom8 ||
                terminal.Custom9 != updateFullTerminalSystem.Custom9 ||
                terminal.Custom10 != updateFullTerminalSystem.Custom10 ||
                terminal.Custom11 != updateFullTerminalSystem.Custom11 ||
                terminal.Custom12 != updateFullTerminalSystem.Custom12 ||
                terminal.Custom13 != updateFullTerminalSystem.Custom13 ||
                terminal.Custom14 != updateFullTerminalSystem.Custom14 ||
                terminal.Custom15 != updateFullTerminalSystem.Custom15 ||
                terminal.Custom16 != updateFullTerminalSystem.Custom16 ||
                terminal.Custom19 != updateFullTerminalSystem.Custom19 ||
                updateFullTerminalSystem.TerminalChecksum){
                    
                    systempos.TerminalChecksum = 0;
                    rstTerminalChecksum=true;

                }

                terminal.TerminalID = updateFullTerminalSystem.TerminalID;
                terminal.SerialNumber = updateFullTerminalSystem.SerialNumber;
                //rst-69
                terminal.ParmConnectTime = updateFullTerminalSystem.ParmConnectTime;
                terminal.Custom1 = updateFullTerminalSystem.HeaderLine1;
                terminal.Custom2 = updateFullTerminalSystem.HeaderLine2;
                //addRUT
                terminal.Custom3 = addRUT+updateFullTerminalSystem.HeaderLine3;
                terminal.Custom4 = updateFullTerminalSystem.Custom4;
                terminal.Custom5 = updateFullTerminalSystem.Custom5;
                terminal.Custom6 = updateFullTerminalSystem.Custom6;
                terminal.Custom7 = updateFullTerminalSystem.Custom7;
                terminal.Custom8 = updateFullTerminalSystem.Custom8;
                terminal.Custom9 = updateFullTerminalSystem.Custom9;
                terminal.Custom10 = updateFullTerminalSystem.Custom10;
                terminal.Custom11 = updateFullTerminalSystem.Custom11;
                terminal.Custom12 = updateFullTerminalSystem.Custom12;
                terminal.Custom13 = updateFullTerminalSystem.Custom13;
                terminal.Custom14 = updateFullTerminalSystem.Custom14;
                terminal.Custom15 = updateFullTerminalSystem.Custom15;
                terminal.Custom16 = updateFullTerminalSystem.Custom16;
                terminal.Custom19 = updateFullTerminalSystem.Custom19;
                                    
                
                if (systempos.ControlGroup != updateFullTerminalSystem.merchanType || updateFullTerminalSystem.ControlCheckSum ){
                    
                    systempos.ControlGroup = updateFullTerminalSystem.merchanType;
                    systempos.ControlCheckSum = 0;
                    rstControlChecksum=true;
                }


                if (systempos.ConnectGroup != updateFullTerminalSystem.ConnectGroup || updateFullTerminalSystem.ParmConnChecksum){
                    
                    systempos.ConnectGroup = updateFullTerminalSystem.ConnectGroup;
                    systempos.ParmConnChecksum = 0;
                    systempos.TranConnChecksum1 = 0;
                    systempos.TranConnChecksum2 = 0;
                    rstTranConnChecksum=true;
                }
                
                //rst-70
                if(updateFullTerminalSystem.ParameterReload)
                    systempos.ParameterReload = 1;
                //rst70 inicio
                else
                    systempos.ParameterReload = 0;
                //rst70 fin

                if(updateFullTerminalSystem.ProgramReload)
                    systempos.ProgramReload = 1; 
                //rst70 inicio
                else
                    systempos.ProgramReload = 0;   
                //rst70 fin

                if ( systempos.Paquete != updateFullTerminalSystem.Paquete )
                    newPackage = true ;
                
                systempos.ParameterGroup = updateFullTerminalSystem.ParameterGroup;
                systempos.ParameterVersion = updateFullTerminalSystem.ParameterVersion;
                systempos.ProgramID = updateFullTerminalSystem.ProgramID;
                systempos.ProgramVersion = updateFullTerminalSystem.ProgramVersion;
                systempos.Paquete = updateFullTerminalSystem.Paquete;
                
                
                TerminalStatus term = await _context.TerminalsStatus.FirstOrDefaultAsync(te => te.TerminalID.Equals(updateFullTerminalSystem.TerminalID));
                if ( term == null )
                {
                    term = new TerminalStatus();
                    term.TerminalID = updateFullTerminalSystem.TerminalID;
                    term.status = updateFullTerminalSystem.enabled_JPOS ? 1 : 0 ;
                    await _context.TerminalsStatus.AddAsync(term);
                }
                else
                {
                    term.status = updateFullTerminalSystem.enabled_JPOS ? 1 : 0 ;

                }

                jpos jpos_value=new jpos();
                char pad = '0';
                string[] parameters = new string[]{
                        updateFullTerminalSystem.ca == "" ? null : updateFullTerminalSystem.ca,
                        updateFullTerminalSystem.mcc == "" ? null : updateFullTerminalSystem.mcc,
                        updateFullTerminalSystem.pf_id == "" || updateFullTerminalSystem.pf_id == null ? null : updateFullTerminalSystem.pf_id.PadLeft(11, pad),
                        updateFullTerminalSystem.visa_spnsrd_mercht == "" ? null : updateFullTerminalSystem.visa_spnsrd_mercht,
                        updateFullTerminalSystem.amex_id_comercio == "" ? null : updateFullTerminalSystem.amex_id_comercio,
                };
    

                Sysconfig sys = await _MySqlcontext.sysconfig.FirstOrDefaultAsync(j => j.id.Equals(addca+updateFullTerminalSystem.TerminalID));

                if (sys == null)
                {
                    sys = new Sysconfig();
                    sys.id = addca+updateFullTerminalSystem.TerminalID;
                    await _MySqlcontext.sysconfig.AddAsync(sys);
                }

                jpos_value.updateSysconfigValue_CA(parameters);
                sys.value=jpos_value.genSysconfigValue_CA();
                        
                await _context.SaveChangesAsync();
                await _MySqlcontext.SaveChangesAsync();
              
                //ACTUALIZA LINEA AL ARCHIVO TERMINALS
                _csvfile.UpdateTerminalsLine(updateFullTerminalSystem.TerminalID,updateFullTerminalSystem);

                //ACTUALIZA LINEA AL ARCHIVO SYSTEM

                _csvfile.UpdateSystemLine(updateFullTerminalSystem.TerminalID,updateFullTerminalSystem,rstTerminalChecksum, rstTranConnChecksum, rstControlChecksum, newPackage);

                serviceResponse = await GetFullTerminalSystemByID(updateFullTerminalSystem.TerminalID);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            
            return serviceResponse;
        }


        public async Task<ServiceResponse<Terminal>> DeleteFullTerminalSystem(string terminal_id)
        {
            ServiceResponse<Terminal> serviceResponse = new ServiceResponse<Terminal>();
            
            try
            {
                SystemPOS systemPOS = await _context.SystemPOSs.FirstAsync(c => c.TerminalID.Equals(terminal_id));
                if(systemPOS != null)
                {
                    _context.SystemPOSs.Remove(systemPOS);
                    _csvfile.DeleteSystemLine(terminal_id);
                }
                
                TerminalStatus status = await _context.TerminalsStatus.FirstOrDefaultAsync(s => s.TerminalID.Equals(terminal_id));
                if(status != null)
                {
                    _context.TerminalsStatus.Remove(status);
                }

                
                Terminal terminal = await _context.Terminals.FirstOrDefaultAsync(t => t.TerminalID.Equals(terminal_id));
                if (terminal != null){

                _context.Terminals.Remove(terminal);
                _csvfile.DeleteTerminalsLine(terminal_id);

                }

                Sysconfig sys_ca = await _MySqlcontext.sysconfig.FirstOrDefaultAsync(j => j.id.Equals(addca+terminal_id));
                if (sys_ca!=null){
                    _MySqlcontext.sysconfig.Remove(sys_ca);
                }
                Sysconfig sys_pft = await _MySqlcontext.sysconfig.FirstOrDefaultAsync(j => j.id.Equals(addpft+terminal_id));
                if (sys_pft!=null){
                    _MySqlcontext.sysconfig.Remove(sys_pft);
                }

                await _MySqlcontext.SaveChangesAsync();
                await _context.SaveChangesAsync();
                
                serviceResponse.Data = terminal;

            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<CargaMasivaAux>> InsertTerminalCargaMasivaAux(AddFullTerminalSystemDto fullTerminalSystemDto)
        {
            ServiceResponse<CargaMasivaAux> serviceResponse = new ServiceResponse<CargaMasivaAux>();
            try
            {
                CargaMasivaAux terminales = new CargaMasivaAux();
                
                terminales.Custom1 = fullTerminalSystemDto.HeaderLine1 ?? null;
                terminales.Custom2 = fullTerminalSystemDto.HeaderLine2 ?? null;
                terminales.Custom3 = fullTerminalSystemDto.HeaderLine3 ?? null;
                terminales.Custom4 = fullTerminalSystemDto.Custom4;
                terminales.Custom5 = fullTerminalSystemDto.Custom5;
                terminales.Custom6 = fullTerminalSystemDto.Custom6;
                terminales.Custom7 = fullTerminalSystemDto.Custom7;
                terminales.Custom8 = fullTerminalSystemDto.Custom8;
                terminales.Custom9 = fullTerminalSystemDto.Custom9;
                terminales.Custom10 = fullTerminalSystemDto.Custom10;
                terminales.Custom11 = fullTerminalSystemDto.Custom11;
                terminales.Custom12 = fullTerminalSystemDto.Custom12;
                terminales.Custom13 = fullTerminalSystemDto.Custom13;
                terminales.Custom14 = fullTerminalSystemDto.Custom14;
                terminales.Custom15 = fullTerminalSystemDto.Custom15;
                terminales.Custom16 = fullTerminalSystemDto.Custom16;
                terminales.Custom19 = fullTerminalSystemDto.Custom19;
                
                if(fullTerminalSystemDto.TerminalChecksum)
                {
                    terminales.TerminalChecksum = 0;
                }
                
                terminales.ConnectGroup = fullTerminalSystemDto.ConnectGroup;
                
                if(fullTerminalSystemDto.ParmConnChecksum)
                {
                    terminales.ParmConnChecksum = 0; 
                }
                terminales.ControlGroup = fullTerminalSystemDto.ControlGroup;
                
                if(fullTerminalSystemDto.ControlCheckSum)
                {
                    terminales.ControlCheckSum = 0;
                }
                    
                terminales.ParameterGroup = fullTerminalSystemDto.ParameterGroup;
                terminales.ParameterVersion = fullTerminalSystemDto.ParameterVersion;
                
                if(fullTerminalSystemDto.ParameterReload)
                {
                    terminales.ParameterReload = 1;
                }
                
                terminales.ProgramID = fullTerminalSystemDto.ProgramID;
                terminales.ProgramVersion = fullTerminalSystemDto.ProgramVersion;
                
                if(fullTerminalSystemDto.ProgramReload)
                {
                    terminales.ProgramReload = 1;    
                }
                terminales.Paquete = fullTerminalSystemDto.Paquete;

                
                //prueba cargando todos los parametros
                terminales.TerminalID = "";
                terminales.SerialNumber = "";
                terminales.id_jpos = "";
                terminales.value = "";

                
                    //listaAux.Add(terminales);   
                await _context.CargaMasivaAux.AddAsync(terminales);
                await _context.SaveChangesAsync();
                
                
                //await _context.SaveChangesAsync();
                serviceResponse.Data = terminales;
        
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;

        }

        public async Task<ServiceResponse<List<GetCargaMasivaDto>>> GetTerminalAux()
        {
            ServiceResponse<List<GetCargaMasivaDto>> serviceResponse = new ServiceResponse<List<GetCargaMasivaDto>>();
            try
            {
                //Traigo las tuplas que estan en terminal y system
                List<CargaMasivaAux> lists = await _context.CargaMasivaAux.ToListAsync();
                
                serviceResponse.Data = _mapper.Map<List<GetCargaMasivaDto>>(lists);
        
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            
            return serviceResponse;

        }

        public async Task<ServiceResponse<GetCargaMasivaDto>> GetTerminalAuxByID(int id)
        {
            ServiceResponse<GetCargaMasivaDto> serviceResponse = new ServiceResponse<GetCargaMasivaDto>();
            try{
                //Terminal terminal = new Terminal();
                var terminal = await _context.CargaMasivaAux.FirstOrDefaultAsync(ct => ct.id == id);
                
                serviceResponse.Data = _mapper.Map<GetCargaMasivaDto>(terminal);
                //serviceResponse.Data = _mapper.Map<GetTerminalDto>(terminal);
                                        
            
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;  
            }
            return serviceResponse;

        }

        public async Task<ServiceResponse<GetTerminalDto>> InsertNewFullTerminalFromCargaMasivaAux(GetCargaMasivaDto terminal_aux)
        {
            ServiceResponse<GetTerminalDto> serviceResponse = new ServiceResponse<GetTerminalDto>();
            try
            {
                AddFullTerminalSystemDto aux = new AddFullTerminalSystemDto();
                aux.TerminalID = terminal_aux.TerminalID;
                aux.SerialNumber = terminal_aux.SerialNumber;
                aux.HeaderLine1 = terminal_aux.Custom1 ?? null;
                aux.HeaderLine2 = terminal_aux.Custom2 ?? null;
                aux.HeaderLine3 = terminal_aux.Custom3 ?? null;
                aux.Custom4 = terminal_aux.Custom4;
                aux.Custom5 = terminal_aux.Custom5;
                aux.Custom6 = terminal_aux.Custom6;
                aux.Custom7 = terminal_aux.Custom7;
                aux.Custom8 = terminal_aux.Custom8;
                aux.Custom9 = terminal_aux.Custom9;
                aux.Custom10 = terminal_aux.Custom10;
                aux.Custom11 = terminal_aux.Custom11;
                aux.Custom12 = terminal_aux.Custom12;
                aux.Custom13 = terminal_aux.Custom13;
                aux.Custom14 = terminal_aux.Custom14;
                aux.Custom15 = terminal_aux.Custom15;
                aux.Custom16 = terminal_aux.Custom16;
                aux.Custom19 = terminal_aux.Custom19;
                 
                if(terminal_aux.TerminalChecksum == 0)
                    aux.TerminalChecksum = true;
                else
                    aux.TerminalChecksum = false;
                
                //actualizar parametros de conexion
                if(terminal_aux.ParmConnChecksum == 0)
                    aux.ParmConnChecksum = true;
                else
                    aux.ParmConnChecksum = false;

                //actualizar tipo de comercio - controlchecksum
                if(terminal_aux.ControlCheckSum == 0)
                    aux.ControlCheckSum = true;
                else
                    aux.ControlCheckSum = false;

                //cargar parametros de conexion
                if(terminal_aux.ParameterReload == 1)
                    aux.ParameterReload = true;
                else
                    aux.ParameterReload = false;

                //cargar programa - program reload
                if(terminal_aux.ProgramReload == 1)
                    aux.ProgramReload = true;
                else
                    aux.ProgramReload = false;

                aux.ControlGroup = terminal_aux.ControlGroup;
                aux.ParameterGroup = terminal_aux.ParameterGroup;
                aux.ParameterVersion = terminal_aux.ParameterVersion;
                aux.ProgramID = terminal_aux.ProgramID;
                aux.ProgramVersion = terminal_aux.ProgramVersion;
                aux.Paquete = terminal_aux.Paquete;
                aux.ConnectGroup = terminal_aux.ConnectGroup;

                
                serviceResponse = await InsertFullTerminalSystem(aux);
                
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;

        }

        public async Task<ServiceResponse<GetCargaMasivaDto>> DeleteTerminalCargaMasivaAux(int terminal_id)
        {
            ServiceResponse<GetCargaMasivaDto> serviceResponse = new ServiceResponse<GetCargaMasivaDto>();
            
            try
            {
                CargaMasivaAux terminalAux = await _context.CargaMasivaAux.FirstAsync(c => c.id == terminal_id);
                if(terminalAux != null)
                {
                    _context.CargaMasivaAux.Remove(terminalAux);
                }
                
                await _context.SaveChangesAsync();
                
                serviceResponse.Data = _mapper.Map<GetCargaMasivaDto>(terminalAux);

            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        

        


    }
}
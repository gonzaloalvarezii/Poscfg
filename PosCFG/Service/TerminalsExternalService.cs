using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PosCFG.Dto;
using PosCFG.Models;
using PosCFG.Service;
using PosCFG.JPOS;

namespace PosCFG.Service
{
    public class TerminalsExternalService : ITerminalsExternalService
    {
        private readonly PosCFGDbContext _context;
        private readonly JPOSDbContext _MySqlcontext;
        private readonly IMapper _mapper;
        private readonly ICSVService _csvfile;
        private string addpft = "pft.";
        private string addca = "ca.";
        //private int TerminalStatusEnabled = 1;
        private int TerminalStatusDisabled = 0;
        private string addRUT = "RUT:";
        //char pad = ' ';

        public TerminalsExternalService (PosCFGDbContext context, IMapper mapper, JPOSDbContext MySqlcontext, ICSVService csvfile)
        {
            _context = context;
            _mapper = mapper;
            _MySqlcontext = MySqlcontext;
            _csvfile = csvfile;
        }


        public async Task<bool> existTerminalJPOS(string terminalid)
        {
            bool existe = false;
            Sysconfig sys1 = await _MySqlcontext.sysconfig.FirstOrDefaultAsync(t => t.id.Equals(addpft+terminalid));
            Sysconfig sys2 = await _MySqlcontext.sysconfig.FirstOrDefaultAsync(t => t.id.Equals(addca+terminalid));
            if(sys1!=null || sys2!=null)
            {
                existe = true;
            }
            return existe;
        }

        public async Task<bool> existeTerminalsStatus(string terminalid)
        {
            bool existe = false;
            TerminalStatus ter = await _context.TerminalsStatus.FirstOrDefaultAsync(t => t.TerminalID.Equals(terminalid));
            if(ter != null)
                existe = true;
            return existe;
        }
        

    public  async Task<ServiceResponse<List<GetTerminalsExternalClientListDto>>> GetAllTerminalsExternalList(bool? terminalEnabled)
        {
            ServiceResponse<List<GetTerminalsExternalClientListDto>> serviceResponse = new ServiceResponse<List<GetTerminalsExternalClientListDto>>();
            try{
                
                //Traigo las tuplas que estan en terminal y system
               // List<Terminal> lists = 
              
                    var query  = from t in _context.Terminals
                                 join s in _context.SystemPOSs on t.TerminalID equals s.TerminalID
                                 join ts in _context.TerminalsStatus on t.TerminalID equals ts.TerminalID
                                 select new
                                 {
                                     TerminalID = t.TerminalID,
                                     serialNumber = t.SerialNumber,
                                     status = ts.status

                                 };
                    var lists = await Task.FromResult(query);
                                      
 
                List<GetTerminalsExternalClientListDto> listTerminalExternal = new List<GetTerminalsExternalClientListDto>();
            
                foreach (var item in lists)
                {
                    GetTerminalsExternalClientListDto terextDto = new GetTerminalsExternalClientListDto();
                    terextDto.TerminalID = item.TerminalID;
                    terextDto.SerialNumber = item.serialNumber;
                    terextDto.terminalEnabled = item.status;

                    
                    
                    //Si existe en terminal Status - traer las que tienen status = 1 
                    //OJO existe terminalJPOS chequea que este en sysconfig ca.terminalID y pft.temrinalID
                        if(existTerminalJPOS(item.TerminalID).Result)
                            {
                                if(terminalEnabled==true & item.status == 1)
                                    listTerminalExternal.Add(terextDto);
                                else if (terminalEnabled==false & item.status == 0)
                                    listTerminalExternal.Add(terextDto);
                                else if (terminalEnabled is null)
                                    listTerminalExternal.Add(terextDto);

                            }
                        }
                serviceResponse.Data = listTerminalExternal;
                
            }
            catch (Exception ex)
            {
                
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;  
            }
            return serviceResponse;
                
        }

    public async Task<ServiceResponse<GetTerminalExternalClientDto>> GetTerminalExternalClientById(string terminalID)
        {
            ServiceResponse<GetTerminalExternalClientDto> serviceResponse = new ServiceResponse<GetTerminalExternalClientDto>();
            try{
                
                Terminal terminalExternal = await _context.Terminals
                    .Include(s => s.SystemPOS)
                    .Where(t => t.TerminalID.Equals(terminalID))
                    .FirstAsync();
                //var terminalExternalDto = _mapper.Map<List<TerminalsExternalDto>>(terminalsExternal);

                GetTerminalExternalClientDto terextDto = new GetTerminalExternalClientDto();

                
                AcquirerDto acq = new AcquirerDto();
                terextDto.TerminalID = terminalExternal.TerminalID;
                terextDto.SerialNumber = terminalExternal.SerialNumber;
                terextDto.nombreComercial = terminalExternal.Custom1;
                terextDto.direccionFiscal = terminalExternal.Custom2;
                terextDto.rut = terminalExternal.Custom3;
                

                
                acq.acqVisanet = terminalExternal.Custom4;
                acq.acqFirstdata = terminalExternal.Custom5;
                acq.acqOca = terminalExternal.Custom6;
                acq.acqCreditosDirectos = terminalExternal.Custom7;
                acq.acqCabal = terminalExternal.Custom8;
                acq.acqCreditel = terminalExternal.Custom9; 
                acq.acqPasscard = terminalExternal.Custom10;
                acq.acqEdenred = terminalExternal.Custom11;
                acq.acqAnda = terminalExternal.Custom12;
                acq.acqAMEX = terminalExternal.Custom13;
                acq.acqClubDelEste = terminalExternal.Custom14;
                acq.acqMides = terminalExternal.Custom15;
                acq.acqCabal_usd = terminalExternal.Custom19;
                              
                terextDto.acquirer = acq;
                terextDto.ConnectGroup = terminalExternal.SystemPOS.ConnectGroup;
                terextDto.terminalInit = terminalExternal.SystemPOS.TerminalChecksum;
                terextDto.merchantType = terminalExternal.SystemPOS.ControlGroup;
                

                TerminalStatus ter = await _context.TerminalsStatus.FirstOrDefaultAsync(t => t.TerminalID.Equals(terminalID));
                if(ter != null)
                {
                    terextDto.terminalEnabled = ter.status; 
                }
                else
                {
                    terextDto.terminalEnabled = TerminalStatusDisabled;
                }
                

                if(existTerminalJPOS(terminalID).Result)
                {
                //ver cual tomamos del value si el campo que está en ptf o ca
                //string term = addca+terminalID;
                //Sysconfig sysconfig = await _MySqlcontext.sysconfig.FirstOrDefaultAsync(t => t.id.Equals(term));
                //parametros de JPOS falta definir cuales son
                    var jpos = await _MySqlcontext.sysconfig.FirstOrDefaultAsync(j => j.id.Equals(addca+terminalID));
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

                            terextDto.ca_name = ca.Length == 0 ? null : ca; 
                            terextDto.mcc = mcc.Length == 0 ? null : mcc; 
                            terextDto.pf_id = pf_id.Length == 0 ? null : pf_id; 
                            terextDto.visa_spnsrd_mercht = visa_spnsrd_mercht.Length == 0 ? null : visa_spnsrd_mercht; 
                            terextDto.amex_id_comercio= amex_id_comercio.Length == 0 ? null : amex_id_comercio; 
                        }
                    }
                    
                    
                    ///terextDto.id = sysconfig.id;
                    ///terextDto.value = sysconfig.value;

                    //RESPETAR ORDEN COMO EN FullTerminalSystemService
                    //fullTerminalSystem.ca+"."+fullTerminalSystem.mcc+"."+fullTerminalSystem.pf_id+
                    //"."+fullTerminalSystem.visa_spnsrd_mercht+"."+fullTerminalSystem.amex_id_comercio

                    //se agrega ',' (coma) segun req.
                    //RST-64
                    /*char [] splitchar = {','};
                    string [] strArr = null;
                    string value = sysconfig.value;
                    //tomo los 40 caracteres primero y los agrego en caname2
                    if(value.TrimEnd() != "0")
                    {
                        string caname2 = value.Substring(0,40);
                        //tring[] jpos_values= sysconfig.value.Split('.');
                        //me quedo con el string luego del caname
                        string aux = value.Substring(41);
                        //separo por '.' el string restante
                        strArr = aux.Split(splitchar);
                        
                        if(strArr.Length > 1)
                        {
                            //Quita los espacios en blanco del final del String
                            terextDto.ca_name=caname2.TrimEnd();
                            terextDto.mcc= strArr[0];
                            terextDto.pf_id=strArr[1];
                            terextDto.visa_spnsrd_mercht=strArr[2];
                            terextDto.amex_id_comercio=strArr[3];
                        }
                        else
                        {
                            terextDto.ca_name=caname2.TrimEnd();
                            terextDto.mcc=null;
                            terextDto.pf_id=null;
                            terextDto.visa_spnsrd_mercht=null;
                            terextDto.amex_id_comercio=null;
                            
                        }
                    }else
                    {
                        terextDto.ca_name=value.TrimEnd();
                        terextDto.mcc=null;
                        terextDto.pf_id=null;
                        terextDto.visa_spnsrd_mercht=null;
                        terextDto.amex_id_comercio=null;
                    }*/
                }

                terextDto.terminalInit = _csvfile.TerminalInitialized(terminalID) ? 1 : 0 ;
               
                serviceResponse.Data = terextDto;
                
            }
                 
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;  
            }
            return serviceResponse;
                
        }

    public async Task<ServiceResponse<GetTerminalExternalClientDto>> UpdateTerminalExternalClient(UpdateTerminalExternalClientDto updateTerminalExternal)
        {

            ServiceResponse<GetTerminalExternalClientDto> serviceResponse = new ServiceResponse<GetTerminalExternalClientDto>();
            try
            {
                Terminal terminal = await _context.Terminals.FirstOrDefaultAsync(ct => ct.TerminalID.Equals(updateTerminalExternal.TerminalID));

                //SI ES NULL MANTIENE EL VALOR ACTUAL ?.
                terminal.Custom1 = updateTerminalExternal.nombreComercial?.ToUpper() ?? terminal.Custom1;
                terminal.Custom2 = updateTerminalExternal.direccionFiscal?.ToUpper() ?? terminal.Custom2;
                if( updateTerminalExternal.rut != null ){
                    terminal.Custom3 = addRUT+updateTerminalExternal.rut;
                }

                //AcquirerDto acq = new AcquirerDto();
                
                if(updateTerminalExternal.acquirer != null)
                {
                    terminal.Custom4 = updateTerminalExternal.acquirer.acqVisanet ?? terminal.Custom4;
                    
                    terminal.Custom6 = updateTerminalExternal.acquirer.acqOca ?? terminal.Custom6;
                    
                    terminal.Custom8 = updateTerminalExternal.acquirer.acqCabal ?? terminal.Custom8;
                    
                    terminal.Custom12 = updateTerminalExternal.acquirer.acqAnda ?? terminal.Custom12;
                    
                    terminal.Custom13 = updateTerminalExternal.acquirer.acqAMEX ?? terminal.Custom13;

                    terminal.Custom19 = updateTerminalExternal.acquirer.acqCabal_usd ?? terminal.Custom19;

                    //rst-64
                    //acqFirstdata, acqCreditel, acqPasscard, acqEdenred, acqClubDelEste
                    //CAMBIOS GONZALO
                    ////terminal.Custom5 = updateTerminalExternal.acquirer.acqFirstdata ?? terminal.Custom5;
                    ////terminal.Custom7 = updateTerminalExternal.acquirer.acqCreditosDirectos ?? terminal.Custom7;
                    ////terminal.Custom9 = updateTerminalExternal.acquirer.acqCreditel ?? terminal.Custom9;
                    ////terminal.Custom10 = updateTerminalExternal.acquirer.acqPasscard ?? terminal.Custom10;
                    terminal.Custom11 = updateTerminalExternal.acquirer.acqEdenred ?? terminal.Custom11;
                    terminal.Custom14 = updateTerminalExternal.acquirer.acqClubDelEste ?? terminal.Custom14;
                    //rst-64
                }

                SystemPOS systempos=await _context.SystemPOSs.FirstOrDefaultAsync(ct => ct.TerminalID.Equals(updateTerminalExternal.TerminalID));
                bool rstTerminalChecksum=false;
                bool rstTranConnChecksum=false;
                bool rstControlChecksum=false;
                bool newPackage=false;
                if (updateTerminalExternal.nombreComercial != null ||
                    updateTerminalExternal.direccionFiscal != null ||
                    updateTerminalExternal.rut != null || 
                    updateTerminalExternal.acquirer != null
                ){
                    if (systempos!= null)
                    {
                        systempos.TerminalChecksum = 0;
                        rstTerminalChecksum = true;
                    }
                   
                }
                
                if ( updateTerminalExternal.merchantType != null ){

                    if (systempos!= null)
                    {
                        systempos.ControlGroup = (int)updateTerminalExternal.merchantType;
                        rstControlChecksum = true;
                    }


                }
                    
                    //terminalEnabled -> Activar o desactivar Terminal en el Switch JPOS. 1:Activo 0:Inactivo
                    TerminalStatus term = await _context.TerminalsStatus.FirstOrDefaultAsync(t => t.TerminalID.Equals(updateTerminalExternal.TerminalID));
                    bool termStatus = term?.status != null && term.status == 1 ;
                    if(updateTerminalExternal.terminalEnabled == 1 || updateTerminalExternal.terminalEnabled == 0)
                    {
                        if (term != null )
                        {
                            term.status = (int)updateTerminalExternal.terminalEnabled;
                        }
                        else
                        {
                            term = new TerminalStatus();
                            term.TerminalID = updateTerminalExternal.TerminalID;
                            term.status = (int)updateTerminalExternal.terminalEnabled;
                            await _context.AddAsync(term);
                        }

                        

                    }
                    
                    // SI ALGUN PARAMETRO NO ES NULO
                    if( updateTerminalExternal.ca_name != null 
                        ||  updateTerminalExternal.mcc != null 
                        ||  updateTerminalExternal.pf_id != null 
                        ||  updateTerminalExternal.visa_spnsrd_mercht != null 
                        ||  updateTerminalExternal.amex_id_comercio != null 
                    ){
                        // SI LA TERMINAL ESTA HABILITADA
                        if (termStatus || updateTerminalExternal.terminalEnabled == 1){
                            jpos jpos_value=new jpos();

                            string[] parameters = new string[]{
                                    updateTerminalExternal.ca_name,
                                    updateTerminalExternal.mcc,
                                    updateTerminalExternal.pf_id,
                                    updateTerminalExternal.visa_spnsrd_mercht,
                                    updateTerminalExternal.amex_id_comercio,
                            };

                            Sysconfig sys = await _MySqlcontext.sysconfig.FirstOrDefaultAsync(s => s.id.Equals(addca+updateTerminalExternal.TerminalID));
                            if(sys != null)
                            {
                                
                                if (sys.value != null)
                                {
                                    jpos_value.setSysconfigValue_CA(sys.value);
                                }
                                
                            }
                            else
                            {
                                sys = new Sysconfig();
                                sys.id = addca+updateTerminalExternal.TerminalID;
                                await _MySqlcontext.sysconfig.AddAsync(sys);

                            }

                            jpos_value.updateSysconfigValue_CA(parameters);
                            sys.value=jpos_value.genSysconfigValue_CA();


                        }

                    }


                await _context.SaveChangesAsync();
                await _MySqlcontext.SaveChangesAsync();

                serviceResponse = await GetTerminalExternalClientById(updateTerminalExternal.TerminalID);
                serviceResponse.Success = true;


                //ACTUALIZA LINEA AL ARCHIVO TERMINALS
                _csvfile.UpdateTerminalsLine(updateTerminalExternal.TerminalID,updateTerminalExternal);

                //ACTUALIZA LINEA AL ARCHIVO SYSTEM
                _csvfile.UpdateSystemLine(updateTerminalExternal.TerminalID,updateTerminalExternal, rstTerminalChecksum, rstTranConnChecksum, rstControlChecksum, newPackage);
                              
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
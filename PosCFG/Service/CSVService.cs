using System;
using System.IO;
using System.Data;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Collections.Generic;
using PosCFG.Dto;
using Microsoft.Extensions.Options;
using PosCFG.Models;
using System.Reflection;
using System.Text.RegularExpressions;


namespace PosCFG.Service{

    public class CSVService : ICSVService    
    {
        private readonly IOptions<TDSFileConfig> _config;
        
        private readonly string _path;

        private List<SystemLine> _systemtxt;

        private List<TerminalsLine> _terminalstxt;

        private DataTable _controltxt;

        private List<String> _ConnectGroups;

        private List<TipoComercio> _ControlGroups;

        private List<string> _ProgramIDs;

        private List<string> _ParameterGroups;

        private DateTime _LastWrite;

        private string _levels_path;
        
        //NUEVO PARA ACTUALIZAR CHECKSUMS EN BD
        private Dictionary<string, bool> _SystemChecksums;


        //CONSTURCTOR
        public CSVService(IOptions<TDSFileConfig> config) {
            _config = config;
            _path = GetPath(_config.Value.tds_path);
            _levels_path = getLevelsFolders();
            _systemtxt=ReadSystemFile();
            _terminalstxt=ReadTerminalsFile();
            _controltxt=getControlTxtTable();
            _LastWrite=getLastWriteFile(_config.Value.system_fname);
            //Para listas desplegables
            _ConnectGroups=getConnectGroups();
            _ControlGroups=getControlGroups();
            _ParameterGroups=getParameterGroups();
            _ProgramIDs=getProgramIDs();
            _SystemChecksums = new Dictionary<string, bool>();
            //getAcquirersByGroupID("41");
            //Console.WriteLine(_path);
        }

        public class TerminalChecksums{

            public string TerminalChecksum {get; set;} = null;
            public string ControlChecksum {get; set;}  = null;
            public string ParmConnChecksum {get; set;} = null ;     
            public List<string> TranConnChecksum { get; set; } = null;


        }

        //NUEVO PARA ACTUALIZAR CHECKSUMS EN BD
        public Dictionary<string,TerminalChecksums> checksumsUpdate(){

            Dictionary<string,TerminalChecksums> chks=new Dictionary<string,TerminalChecksums>();
            foreach( KeyValuePair<string, bool> kvp in _SystemChecksums )
            {
                if (kvp.Value){

                    string id=kvp.Key;
                    var index=_systemtxt.FindIndex(x => x.TerminalID.Equals(id));

                    if ( index != -1 && _systemtxt[index].ParmConnChecksum != "0" )
                    {   
                        chks.Add(id,new TerminalChecksums());

                        chks[id].TerminalChecksum=_systemtxt[index].TerminalChecksum;
                        chks[id].ControlChecksum=_systemtxt[index].ControlChecksum;
                        chks[id].ParmConnChecksum=_systemtxt[index].ParmConnChecksum;
                        chks[id].TranConnChecksum=_systemtxt[index].TranConnChecksum;

                        //EN DUDA, PORQUE SI FALLA EL UPDATE EN LA BD NO SE VUELVE A HACER
                        //SI ESTO ESTA MARCADO NO FUNCA
                        ///_SystemChecksums[id]=false;
                    }
                }
                //Console.WriteLine("TerminalID = {0}, TerminalChecksum = {1}, ControlChecksum = {2}, ParmConnChecksum = {3}, TranConnChecksum = [{4}]", 
                    //kvp.Key, kvp.Value.TerminalChecksum, kvp.Value.ControlChecksum, kvp.Value.ParmConnChecksum, string.Join(", ", kvp.Value.TranConnChecksum) );
            }

            return chks;
        }


        public string GetPath(string p)
        {
            string pPath = p;

            if (pPath.Length >= 1)
            {
                //si termina con barras, se la quito
                if (pPath.Substring(pPath.Length - 1, 1) == "\\")
                {
                    pPath = pPath.Substring(0, pPath.Length - 2);
                }

            }

            return pPath;
        }

        // SYSTEM.TXT
        public List<SystemLine> GetSystemTxt(){
            return _systemtxt;
        }
        public List<SystemLine> ReadSystemFile(){

            //"TDS/Parameters/system.txt"
            string filename=_config.Value.system_fname;
            string filepath=$"{_path}/{filename}";

            using (var reader = new StreamReader(filepath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.RegisterClassMap<SystemLineMap>();
                csv.Configuration.MissingFieldFound=null;
                var records = csv.GetRecords<SystemLine>();                

                return new List<SystemLine>(records);
            }

            //return null;
        }

        //Convierte DTOs EN SystemLine
        //private SystemLine DTO2System(AddSystemPOSDto newline){
        private SystemLine DTO2System(object newline){

            SystemLine nl=null;
            Type objType=newline.GetType();

            if (objType.Name == "AddSystemPOSDto"){
                nl=new SystemLine{
                        TerminalID=Convert.ToString(objType.GetProperty("TerminalID").GetValue(newline)),
                        TerminalChecksum=Convert.ToString(objType.GetProperty("TerminalChecksum").GetValue(newline)),
                        ControlGroup=Convert.ToString(objType.GetProperty("ControlGroup").GetValue(newline)),
                        ControlChecksum=Convert.ToString(objType.GetProperty("ControlCheckSum").GetValue(newline)),
                        ParameterGroup=Convert.ToString(objType.GetProperty("ParameterGroup").GetValue(newline)),
                        ParameterReload=Convert.ToString(objType.GetProperty("ParameterReload").GetValue(newline)),
                        ParameterVersion=Convert.ToString(objType.GetProperty("ParameterVersion").GetValue(newline)),
                        ProgramID=Convert.ToString(objType.GetProperty("ProgramID").GetValue(newline)),
                        ProgramReload=Convert.ToString(objType.GetProperty("ProgramReload").GetValue(newline)),
                        ProgramVersion=Convert.ToString(objType.GetProperty("ProgramVersion").GetValue(newline)),
                        PackageName=Convert.ToString(objType.GetProperty("Paquete").GetValue(newline)),
                        ConnectGroup=Convert.ToString(objType.GetProperty("ConnectGroup").GetValue(newline)),
                        ParmConnChecksum=Convert.ToString(objType.GetProperty("ParmConnChecksum").GetValue(newline)),
                        TranConnChecksum=new List<string>{
                            //Convert.ToString(objType.GetProperty("TranConnChecksum1").GetValue(newline, null)),
                            //Convert.ToString(objType.GetProperty("TranConnChecksum2").GetValue(newline, null)),
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0"
                        }
                };
            }

            if (objType.Name == "AddFullTerminalSystemDto"){
                
                nl=new SystemLine{
                        TerminalID=Convert.ToString(objType.GetProperty("TerminalID").GetValue(newline)).Trim(),
                        //bool
                        TerminalChecksum=Convert.ToString((bool)objType.GetProperty("TerminalChecksum").GetValue(newline) ? 0 : 1 ).Trim(),
                        ControlGroup=Convert.ToString(objType.GetProperty("ControlGroup").GetValue(newline)).Trim(),
                        //bool
                        ControlChecksum=Convert.ToString((bool)objType.GetProperty("ControlCheckSum").GetValue(newline) ? 0 : 1 ).Trim(),
                        ParameterGroup=Convert.ToString(objType.GetProperty("ParameterGroup").GetValue(newline)).Trim(),
                        //bool
                        ParameterReload=Convert.ToString((bool)objType.GetProperty("ParameterReload").GetValue(newline) ? 1 : 0 ).Trim(),
                        ParameterVersion=Convert.ToString(objType.GetProperty("ParameterVersion").GetValue(newline)).Trim(),
                        ProgramID=Convert.ToString(objType.GetProperty("ProgramID").GetValue(newline)).Trim(),
                        //bool
                        ProgramReload=Convert.ToString((bool)objType.GetProperty("ProgramReload").GetValue(newline) ? 1 : 0 ).Trim(),
                        ProgramVersion=Convert.ToString(objType.GetProperty("ProgramVersion").GetValue(newline)).Trim(),
                        PackageName=Convert.ToString(objType.GetProperty("Paquete").GetValue(newline)).Trim(),
                        ConnectGroup=Convert.ToString(objType.GetProperty("ConnectGroup").GetValue(newline)).Trim(),
                        //bool
                        ParmConnChecksum=Convert.ToString((bool)objType.GetProperty("ParmConnChecksum").GetValue(newline) ? 0 : 1 ).Trim(),
                        TranConnChecksum=new List<string>{
                            //Convert.ToString(objType.GetProperty("TranConnChecksum1").GetValue(newline, null)),
                            //Convert.ToString(objType.GetProperty("TranConnChecksum2").GetValue(newline, null)),
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0"
                        }
                };

            }

            if (objType.Name == "GetFullTerminalSystemDto"){
                nl=new SystemLine{
                        TerminalID=Convert.ToString(objType.GetProperty("TerminalID").GetValue(newline)).Trim(),
                        //bool
                        TerminalChecksum=Convert.ToString((bool)objType.GetProperty("TerminalChecksum").GetValue(newline) ? 0 : 1 ).Trim(),
                        ControlGroup=Convert.ToString(objType.GetProperty("merchanType").GetValue(newline)).Trim(),
                        //bool
                        ControlChecksum=Convert.ToString((bool)objType.GetProperty("ControlCheckSum").GetValue(newline) ? 0 : 1 ).Trim(),
                        ParameterGroup=Convert.ToString(objType.GetProperty("ParameterGroup").GetValue(newline)).Trim(),
                        //bool
                        ParameterReload=Convert.ToString((bool)objType.GetProperty("ParameterReload").GetValue(newline) ? 1 : 0 ).Trim(),
                        ParameterVersion=Convert.ToString(objType.GetProperty("ParameterVersion").GetValue(newline)).Trim(),
                        ProgramID=Convert.ToString(objType.GetProperty("ProgramID").GetValue(newline)).Trim(),
                        //bool
                        ProgramReload=Convert.ToString((bool)objType.GetProperty("ProgramReload").GetValue(newline) ? 1 : 0 ).Trim(),
                        ProgramVersion=Convert.ToString(objType.GetProperty("ProgramVersion").GetValue(newline)).Trim(),
                        PackageName=Convert.ToString(objType.GetProperty("Paquete").GetValue(newline)).Trim(),
                        ConnectGroup=Convert.ToString(objType.GetProperty("ConnectGroup").GetValue(newline)).Trim(),
                        //bool
                        ParmConnChecksum=Convert.ToString((bool)objType.GetProperty("ParmConnChecksum").GetValue(newline) ? 0 : 1 ).Trim(),
                        TranConnChecksum=null
                        ///////FALTA DEFINIR EN DTO
                        /*TranConnChecksum=new List<string>{
                            //Convert.ToString(objType.GetProperty("TranConnChecksum1").GetValue(newline, null)),
                            //Convert.ToString(objType.GetProperty("TranConnChecksum2").GetValue(newline, null)),
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            ""
                        }*/
                };

            }

            if (objType.Name == "UpdateTerminalExternalClientDto"){
                nl=new SystemLine{
                        TerminalID=Convert.ToString(objType.GetProperty("TerminalID")?.GetValue(newline) ?? null ),
                        TerminalChecksum=null,
                        ControlGroup=objType.GetProperty("merchantType")?.GetValue(newline) != null ? Convert.ToString(objType.GetProperty("merchantType")?.GetValue(newline)) : null,
                        ControlChecksum=objType.GetProperty("merchantType")?.GetValue(newline) != null ? "0" : null,
                        ParameterGroup=null,
                        ParameterReload=null,
                        ParameterVersion=null,
                        ProgramID=null,
                        ProgramReload=null,
                        ProgramVersion=null,
                        PackageName=null,
                        ConnectGroup=null,
                        ParmConnChecksum=null,
                        TranConnChecksum=null
                };

            }
   
            return nl;
        }


        private void WriteSystemFile(List<SystemLine> records){

            string filename=_config.Value.system_fname;
            string filepath=$"{_path}/{filename}";

            using (var writer = new StreamWriter(filepath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Configuration.RegisterClassMap<SystemLineMap>();
                csv.WriteRecords(records);
            }

            _LastWrite=getLastWriteFile(filename);
        }

        //public void UpdateSystemLine(string id, AddSystemPOSDto sline){
        public void UpdateSystemLine(string id, object sline, bool rstTerminalChecksum, bool rstTranConnChecksum, bool rstControlChecksum, bool newPackage){

            DateTime LastWrite = getLastWriteFile(_config.Value.system_fname);
            if (oldFile(LastWrite))
            {
                _systemtxt=ReadSystemFile();
                _LastWrite=LastWrite;
            }

            var index=_systemtxt.FindIndex(x => x.TerminalID.Equals(id));
            if (index != -1)
            {
                SystemLine sl=DTO2System(sline);
                Type slType = sl.GetType();
                PropertyInfo[] properties = _systemtxt[index].GetType().GetProperties();
                foreach (PropertyInfo pi in properties){

                    PropertyInfo psl = slType.GetProperty(pi.Name);
                    if ( psl != null ){
                        var pslValue=psl.GetValue(sl);
                        if ( pslValue != null )
                        {
                            if ( pi.Name == "PackageName" )
                            {
                                if ( newPackage || _systemtxt[index].PackageName != "Done" ){

                                    pi.SetValue(_systemtxt[index],pslValue);

                                }
                            }
                            else
                            {
                                pi.SetValue(_systemtxt[index],pslValue);
                            }
                            
                        }
                    }

                }
                if (rstTerminalChecksum){
                    _systemtxt[index].TerminalChecksum="0";
                }

                if (rstTranConnChecksum){
                    _systemtxt[index].TranConnChecksum = new List<string>{
                        "0",
                        "0",
                        "0",
                        "0",
                        "0",
                        "0",
                        "0",
                        "0",
                        "0",
                        "0"

                    };
                    _systemtxt[index].ParmConnChecksum="0";
                }

            if (rstControlChecksum){

                _systemtxt[index].ControlChecksum="0";
            }

            //NUEVO PARA ACTUALIZAR CHECKSUMS EN BD
            if ( rstTerminalChecksum || rstTranConnChecksum || rstControlChecksum ){   
                _SystemChecksums[id]=true;
            }

                WriteSystemFile(_systemtxt);
                //_LastWrite=getLastWriteFile(_config.Value.system_fname);
            }
            
        }


        //public void AppendSystemLine(AddSystemPOSDto newline){
        public void AppendSystemLine(object newline){

            string filename=_config.Value.system_fname;
            string filepath=$"{_path}/{filename}";

            SystemLine nl=DTO2System(newline);
            var records = new List<SystemLine>
            {
                nl
            };
            // APPEND AL ARCHIVO
            using (var stream = File.Open(filepath, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                // Don't write the header again.
                csv.Configuration.HasHeaderRecord = false;
                csv.WriteRecords(records);
            }
            _systemtxt.Add(nl);

            //NUEVO PARA ACTUALIZAR CHECKSUM EN BD
            _SystemChecksums[nl.TerminalID]=true;
        }


        public void DeleteSystemLine(string id){

            DateTime LastWrite = getLastWriteFile(_config.Value.system_fname);
            if (oldFile(LastWrite))
            {
                _systemtxt=ReadSystemFile();
                _LastWrite=LastWrite;
            }

            var index=_systemtxt.FindIndex(x => x.TerminalID.Equals(id));
            if (index != -1)
            {
                _systemtxt.RemoveAt(index);
                WriteSystemFile(_systemtxt);
                
                _SystemChecksums[id]=false;
                //_LastWrite=getLastWriteFile(_config.Value.system_fname);
            }

        }

        public bool TerminalInitialized(string id){

            DateTime LastWrite=getLastWriteFile(_config.Value.system_fname);
            if (oldFile(LastWrite))
            {
                _systemtxt=ReadSystemFile();
                _LastWrite=LastWrite;
                
            }

            var index=_systemtxt.FindIndex(x => x.TerminalID.Equals(id));
            if (index != -1)
            {
                SystemLine sl=_systemtxt[index];
                return  (   
                            sl.TerminalChecksum != "0" && 
                            sl.ControlChecksum != "0" && 
                            sl.ParmConnChecksum != "0" 
                        );    
            }else{
                return false;
            }
            
             
                
        }
        

        public class SystemLine
        {
                public string TerminalID { get; set; }
                public string TerminalChecksum { get; set; }
                public string ControlGroup { get; set; }
                public string ControlChecksum { get; set; }
                public string ParameterGroup { get; set; }
                public string ParameterReload { get; set; }
                public string ParameterVersion { get; set; }
                public string ProgramID { get; set; }
                public string ProgramReload { get; set; }
                public string ProgramVersion { get; set; }
                public string PackageName { get; set; }
                public string ConnectGroup { get; set; }
                public string ParmConnChecksum { get; set; }
                public List<string> TranConnChecksum { get; set; }

        }


        public sealed class SystemLineMap : ClassMap<SystemLine>
        {
            public SystemLineMap()
            {
                Map(m => m.TerminalID).Name("TerminalID");
                Map(m => m.TerminalChecksum).Name("TerminalChecksum");
                Map(m => m.ControlGroup).Name("ControlGroup");
                Map(m => m.ControlChecksum).Name("ControlChecksum");
                Map(m => m.ParameterGroup).Name("ParameterGroup");
                Map(m => m.ParameterReload).Name("ParameterReload");
                Map(m => m.ParameterVersion).Name("ParameterVersion");
                Map(m => m.ProgramID).Name("ProgramID");
                Map(m => m.ProgramReload).Name("ProgramReload");
                Map(m => m.ProgramVersion).Name("ProgramVersion");
                Map(m => m.PackageName).Name("PackageName");
                Map(m => m.ConnectGroup).Name("ConnectGroup");
                Map(m => m.ParmConnChecksum).Name("ParmConnChecksum");
                Map(m => m.TranConnChecksum).Name("TranConnChecksum(1..10)").ConvertUsing(row => new List<string> { 
                    row.GetField<string>(13), 
                    row.GetField<string>(14), 
                    row.GetField<string>(15), 
                    row.GetField<string>(16), 
                    row.GetField<string>(17), 
                    row.GetField<string>(18), 
                    row.GetField<string>(19),
                    row.GetField<string>(20),
                    row.GetField<string>(21),
                    row.GetField<string>(22) 
                    });


            }
        }
        //FIN SYSTEM.TXT


        //TERMINALS.TXT
        public List<TerminalsLine> GetTerminalsTxt(){
            return _terminalstxt;
        }

        public List<TerminalsLine> ReadTerminalsFile(){
            //"TDS/Parameters/system.txt"
            string filename=_config.Value.terminals_fname;
            string filepath=$"{_path}/{filename}";
            
            using (var reader = new StreamReader(filepath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.RegisterClassMap<TerminalsLineMap>();
                csv.Configuration.MissingFieldFound=null;
                var records = csv.GetRecords<TerminalsLine>();                

                return new List<TerminalsLine>(records);
            }

        }

        private void WriteTerminalsFile(List<TerminalsLine> records){

            string filename=_config.Value.terminals_fname;
            string filepath=$"{_path}/{filename}";

            using (var writer = new StreamWriter(filepath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Configuration.RegisterClassMap<TerminalsLineMap>();
                csv.WriteRecords(records);
            }
        }

        public void UpdateTerminalsLine(string id, object tline){

            var index=_terminalstxt.FindIndex(x => x.TerminalID.Equals(id));
            if (index != -1)
            {
                TerminalsLine tl=DTO2Terminals(tline);
                //_terminalstxt[index]=tl;
                Type tlType = tl.GetType();
                PropertyInfo[] properties = _terminalstxt[index].GetType().GetProperties();
                foreach (PropertyInfo pi in properties){

                    PropertyInfo ptl = tlType.GetProperty(pi.Name);
                    if ( ptl != null ){
                        var ptlValue=ptl.GetValue(tl);
                        if ( ptlValue != null )
                            pi.SetValue(_terminalstxt[index],ptlValue);
                    }

                }
                WriteTerminalsFile(_terminalstxt);
            }
            
        }

        public void DeleteTerminalsLine(string id){

            var index=_terminalstxt.FindIndex(x => x.TerminalID.Equals(id));
            if (index != -1)
            {
                _terminalstxt.RemoveAt(index);
                WriteTerminalsFile(_terminalstxt);
            }
            
        }

        private TerminalsLine DTO2Terminals(object newline){
            
            TerminalsLine nl=null;
            Type objType=newline.GetType();

            if (objType.Name == "GetTerminalDto"){
                nl=new TerminalsLine{
                    TerminalID=Convert.ToString(objType.GetProperty("TerminalID").GetValue(newline)),
                    SerialNumber=Convert.ToString(objType.GetProperty("SerialNumber").GetValue(newline)),
                    Suspend=Convert.ToString(objType.GetProperty("Suspend").GetValue(newline)),
                    ConnectTime0=Convert.ToString(objType.GetProperty("ParmConnectTime").GetValue(newline)),
                    Custom1=Convert.ToString(objType.GetProperty("Custom1").GetValue(newline)),
                    Custom2=Convert.ToString(objType.GetProperty("Custom2").GetValue(newline)),
                    Custom3="RUT:"+Convert.ToString(objType.GetProperty("Custom3").GetValue(newline)),
                    Custom4=Convert.ToString(objType.GetProperty("Custom4").GetValue(newline)),
                    Custom5=Convert.ToString(objType.GetProperty("Custom5").GetValue(newline)),
                    Custom6=Convert.ToString(objType.GetProperty("Custom6").GetValue(newline)),
                    Custom7=Convert.ToString(objType.GetProperty("Custom7").GetValue(newline)),
                    Custom8=Convert.ToString(objType.GetProperty("Custom8").GetValue(newline)),
                    Custom9=Convert.ToString(objType.GetProperty("Custom9").GetValue(newline)),
                    Custom10=Convert.ToString(objType.GetProperty("Custom10").GetValue(newline)),
                    Custom11=Convert.ToString(objType.GetProperty("Custom11").GetValue(newline)),
                    Custom12=Convert.ToString(objType.GetProperty("Custom12").GetValue(newline)),
                    Custom13=Convert.ToString(objType.GetProperty("Custom13").GetValue(newline)),
                    Custom14=Convert.ToString(objType.GetProperty("Custom14").GetValue(newline)),
                    Custom15=Convert.ToString(objType.GetProperty("Custom15").GetValue(newline)),
                    Custom16=Convert.ToString(objType.GetProperty("Custom16").GetValue(newline)),
                    Custom17=Convert.ToString(objType.GetProperty("Custom17").GetValue(newline)),
                    Custom18=Convert.ToString(objType.GetProperty("Custom18").GetValue(newline)),
                    Custom19=Convert.ToString(objType.GetProperty("Custom19").GetValue(newline)),
                    Custom20=Convert.ToString(objType.GetProperty("Custom20").GetValue(newline)),
                    TermText=Convert.ToString(objType.GetProperty("TranConnectTime"))
                };
            }

            if (objType.Name == "AddFullTerminalSystemDto"){
                nl=new TerminalsLine{
                    TerminalID=Convert.ToString(objType.GetProperty("TerminalID").GetValue(newline)),
                    SerialNumber=Convert.ToString(objType.GetProperty("SerialNumber").GetValue(newline)),
                    Suspend="0", //NO DEFINIDO EN DTO
                    //rst-69
                    //ConnectTime0="0200", //NO DEFINIDO EN DTO
                    ConnectTime0=Convert.ToString(objType.GetProperty("ParmConnectTime").GetValue(newline)),
                    Custom1=Convert.ToString(objType.GetProperty("HeaderLine1").GetValue(newline)),
                    Custom2=Convert.ToString(objType.GetProperty("HeaderLine2").GetValue(newline)),
                    Custom3="RUT:"+Convert.ToString(objType.GetProperty("HeaderLine3").GetValue(newline)),
                    Custom4=Convert.ToString(objType.GetProperty("Custom4").GetValue(newline)),
                    Custom5=Convert.ToString(objType.GetProperty("Custom5").GetValue(newline)),
                    Custom6=Convert.ToString(objType.GetProperty("Custom6").GetValue(newline)),
                    Custom7=Convert.ToString(objType.GetProperty("Custom7").GetValue(newline)),
                    Custom8=Convert.ToString(objType.GetProperty("Custom8").GetValue(newline)),
                    Custom9=Convert.ToString(objType.GetProperty("Custom9").GetValue(newline)),
                    Custom10=Convert.ToString(objType.GetProperty("Custom10").GetValue(newline)),
                    Custom11=Convert.ToString(objType.GetProperty("Custom11").GetValue(newline)),
                    Custom12=Convert.ToString(objType.GetProperty("Custom12").GetValue(newline)),
                    Custom13=Convert.ToString(objType.GetProperty("Custom13").GetValue(newline)),
                    Custom14=Convert.ToString(objType.GetProperty("Custom14").GetValue(newline)),
                    Custom15=Convert.ToString(objType.GetProperty("Custom15").GetValue(newline)),
                    Custom16=Convert.ToString(objType.GetProperty("Custom16").GetValue(newline)),
                    Custom17=Convert.ToString(objType.GetProperty("Custom17")?.GetValue(newline)), //NO DEFINIDO EN DTO
                    Custom18=Convert.ToString(objType.GetProperty("Custom18")?.GetValue(newline)), //NO DEFINIDO EN DTO
                    Custom19=Convert.ToString(objType.GetProperty("Custom19").GetValue(newline)),
                    Custom20=Convert.ToString(objType.GetProperty("Custom20")?.GetValue(newline)), //NO DEFINIDO EN DTO
                    TermText="" //NO DEFINIDO EN DTO
                };
            }


            if (objType.Name == "GetFullTerminalSystemDto"){
                nl=new TerminalsLine{
                    TerminalID=Convert.ToString(objType.GetProperty("TerminalID").GetValue(newline)),
                    SerialNumber=Convert.ToString(objType.GetProperty("SerialNumber").GetValue(newline)),
                    Suspend=null, //NO DEFINIDO EN DTO
                    //rst-69
                    //ConnectTime0=null, //NO DEFINIDO EN DTO
                    ConnectTime0=Convert.ToString(objType.GetProperty("ParmConnectTime").GetValue(newline)),
                    Custom1=Convert.ToString(objType.GetProperty("HeaderLine1").GetValue(newline)),
                    Custom2=Convert.ToString(objType.GetProperty("HeaderLine2").GetValue(newline)),
                    Custom3="RUT:"+Convert.ToString(objType.GetProperty("HeaderLine3").GetValue(newline)),
                    Custom4=Convert.ToString(objType.GetProperty("Custom4").GetValue(newline)),
                    Custom5=Convert.ToString(objType.GetProperty("Custom5").GetValue(newline)),
                    Custom6=Convert.ToString(objType.GetProperty("Custom6").GetValue(newline)),
                    Custom7=Convert.ToString(objType.GetProperty("Custom7").GetValue(newline)),
                    Custom8=Convert.ToString(objType.GetProperty("Custom8").GetValue(newline)),
                    Custom9=Convert.ToString(objType.GetProperty("Custom9").GetValue(newline)),
                    Custom10=Convert.ToString(objType.GetProperty("Custom10").GetValue(newline)),
                    Custom11=Convert.ToString(objType.GetProperty("Custom11").GetValue(newline)),
                    Custom12=Convert.ToString(objType.GetProperty("Custom12").GetValue(newline)),
                    Custom13=Convert.ToString(objType.GetProperty("Custom13").GetValue(newline)),
                    Custom14=Convert.ToString(objType.GetProperty("Custom14").GetValue(newline)),
                    Custom15=Convert.ToString(objType.GetProperty("Custom15").GetValue(newline)),
                    Custom16=Convert.ToString(objType.GetProperty("Custom16").GetValue(newline)),
                    Custom17=Convert.ToString(objType.GetProperty("Custom17")?.GetValue(newline)), //NO DEFINIDO EN DTO
                    Custom18=Convert.ToString(objType.GetProperty("Custom18")?.GetValue(newline)), //NO DEFINIDO EN DTO
                    Custom19=Convert.ToString(objType.GetProperty("Custom19").GetValue(newline)),
                    Custom20=Convert.ToString(objType.GetProperty("Custom20")?.GetValue(newline)), //NO DEFINIDO EN DTO
                    TermText=null //NO DEFINIDO EN DTO
                };
            }
            
            if (objType.Name == "UpdateTerminalExternalClientDto"){

                Type acqType=null;
                var acqObj = objType.GetProperty("acquirer")?.GetValue(newline);

                if ( acqObj != null )
                {
                    acqType = acqObj.GetType();
                }

                nl=new TerminalsLine{
                    TerminalID=Convert.ToString(objType.GetProperty("TerminalID").GetValue(newline)),
                    SerialNumber=null, //NO DEFINIDO EN DTO
                    Suspend=null, //NO DEFINIDO EN DTO
                    ConnectTime0=null, //NO DEFINIDO EN DTO
                    Custom1=objType.GetProperty("nombreComercial").GetValue(newline)?.ToString().Trim(),
                    Custom2=objType.GetProperty("direccionFiscal").GetValue(newline)?.ToString().Trim(),
                    Custom3=objType.GetProperty("rut").GetValue(newline)?.ToString().Trim() != null ? "RUT:"+objType.GetProperty("rut").GetValue(newline)?.ToString().Trim() : null,
                    Custom4=acqType?.GetProperty("acqVisanet")?.GetValue(acqObj)?.ToString().Trim(),
                    Custom5=acqType?.GetProperty("acqFirstdata")?.GetValue(acqObj)?.ToString().Trim(),
                    Custom6=acqType?.GetProperty("acqOca")?.GetValue(acqObj)?.ToString().Trim(),
                    Custom7=acqType?.GetProperty("acqCreditosDirectos")?.GetValue(acqObj)?.ToString().Trim(),
                    Custom8=acqType?.GetProperty("acqCabal")?.GetValue(acqObj)?.ToString().Trim(),
                    Custom9=acqType?.GetProperty("acqCreditel")?.GetValue(acqObj)?.ToString().Trim(),
                    Custom10=acqType?.GetProperty("acqPasscard")?.GetValue(acqObj)?.ToString().Trim(),
                    Custom11=acqType?.GetProperty("acqEdenred")?.GetValue(acqObj)?.ToString().Trim(),
                    Custom12=acqType?.GetProperty("acqAnda")?.GetValue(acqObj)?.ToString().Trim(),
                    Custom13=acqType?.GetProperty("acqAMEX")?.GetValue(acqObj)?.ToString().Trim(),
                    Custom14=acqType?.GetProperty("acqClubDelEste")?.GetValue(acqObj)?.ToString().Trim(),
                    Custom15=acqType?.GetProperty("acqMides")?.GetValue(acqObj)?.ToString().Trim(),
                    Custom16=acqType?.GetProperty("Custom16")?.GetValue(acqObj)?.ToString().Trim(),
                    Custom17=acqType?.GetProperty("Custom17")?.GetValue(acqObj)?.ToString().Trim(), //NO DEFINIDO EN DTO
                    Custom18=acqType?.GetProperty("Custom18")?.GetValue(acqObj)?.ToString().Trim(), //NO DEFINIDO EN DTO
                    Custom19=acqType?.GetProperty("acqCabal_usd")?.GetValue(acqObj)?.ToString().Trim(), //NO DEFINIDO EN DTO
                    Custom20=acqType?.GetProperty("Custom20")?.GetValue(acqObj)?.ToString().Trim(), //NO DEFINIDO EN DTO
                    TermText=null //NO DEFINIDO EN DTO
                };
            }


            return nl;
        }

        public void AppendTerminalsLine(object newline){

            string filename=_config.Value.terminals_fname;
            string filepath=$"{_path}/{filename}";

            TerminalsLine nl=DTO2Terminals(newline);
            var records = new List<TerminalsLine>
            {
                nl
            };
            // Append to the file.
            using (var stream = File.Open(filepath, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                //NO ESCRIBE EL HEADER NUEVAMENTE
                csv.Configuration.HasHeaderRecord = false;
                csv.WriteRecords(records);
            }
            _terminalstxt.Add(nl);
        }
        public class TerminalsLine{
            public string TerminalID { get; set; }
            public string SerialNumber { get; set; }
            public string Suspend { get; set; }
            public string ConnectTime0 { get; set; }
            //HeaderLine1
            public string Custom1 {get; set;}
            //HeaderLine2
            public string Custom2 {get; set;}
            //HeaderLine3
            public string Custom3 {get; set;}
            public string Custom4 {get; set;}
            public string Custom5 {get; set;}
            public string Custom6 {get; set;}
            public string Custom7 {get; set;}
            public string Custom8 {get; set;}
            public string Custom9 {get; set;}
            public string Custom10 {get; set;}
            public string Custom11 {get; set;}
            public string Custom12 {get; set;}
            public string Custom13 {get; set;}
            public string Custom14 {get; set;}
            public string Custom15 {get; set;}
            public string Custom16 {get; set;}
            public string Custom17 {get; set;}
            public string Custom18 {get; set;}
            public string Custom19 {get; set;}
            public string Custom20 {get; set;}
            public string TermText {get; set;}

        }

        public sealed class TerminalsLineMap : ClassMap<TerminalsLine>
        {
            public TerminalsLineMap()
            {
                Map(m => m.TerminalID).Name("TerminalID");
                Map(m => m.SerialNumber).Name("SerialNumber");
                Map(m => m.Suspend).Name("Suspend");
                Map(m => m.ConnectTime0).Name("ConnectTime0");
                Map(m => m.Custom1).Name("HeaderLine1");
                Map(m => m.Custom2).Name("HeaderLine2");
                Map(m => m.Custom3).Name("HeaderLine3");
                Map(m => m.Custom4).Name("Visanet");
                Map(m => m.Custom5).Name("First Data");
                Map(m => m.Custom6).Name("OCA");
                Map(m => m.Custom7).Name("Creditos Directos");
                Map(m => m.Custom8).Name("CABAL");
                Map(m => m.Custom9).Name("CREDITEL");
                Map(m => m.Custom10).Name("PASSCARD");
                Map(m => m.Custom11).Name("EDENRED");
                Map(m => m.Custom12).Name("ANDA");
                Map(m => m.Custom13).Name("AMEX");
                Map(m => m.Custom14).Name("Club del Este");
                Map(m => m.Custom15).Name("MIDES");
                Map(m => m.Custom16).Name("Custom16");
                Map(m => m.Custom17).Name("Custom17");
                Map(m => m.Custom18).Name("Custom18");
                Map(m => m.Custom19).Name("CABAL_US$");
                Map(m => m.Custom20).Name("Custom20");
                Map(m => m.TermText).Name("TermText");

            }
        }

        //FIN TERMINALS.TXT


        // ConnectGroup

        public List<string> getConnectGroups(){

            List<string> ConnectGroups=null;

            try
            {
                string[] groups = Directory.GetFiles(_path, "connect_*.txt");
                if ( groups.Length > 0 ){
                    ConnectGroups=new List<string>();
                } 
                //string pattern = @"^\w+/\w+/\w+_(\d+)\.txt"; //LINUX
                //string pattern = @"^\w+\\\w+\\\w+_(\d+)\.txt$"; //WINDOWS
               string pattern = @"^\w:" + _levels_path + @"\\\w+_(\d+)\.txt$"; //WINDOWS
                foreach (string group in groups)
                {
                    foreach (Match match in Regex.Matches(group, pattern, RegexOptions.IgnoreCase))
                            ConnectGroups.Add(match.Groups[1].Value);
                            //match.Value, match.Groups[1].Value, match.Index
                            //Console.WriteLine(group);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }

            return ConnectGroups;
        }

        public List<string> getProgramIDs(){

            List<string> ProgramIDs=null;

            try
            {
                string[] programs = Directory.GetFiles(_path, "program_*_*.txt");
                if ( programs.Length > 0 ){
                    ProgramIDs=new List<string>();
                } 
                //string pattern = @"^\w+/\w+/\w+_(\d+)_\d+\.txt$"; //LINUX
              //  string pattern = @"^\w+\\\w+\\\w+_(\d+)_\d+\.txt$"; //WINDOWS
              string pattern = @"^\w:" + _levels_path + @"\\\w+_(\d+)_\d+\.txt$"; //WINDOWS
                foreach (string program in programs)
                {
                    foreach (Match match in Regex.Matches(program, pattern, RegexOptions.IgnoreCase)){

                        string id=match.Groups[1].Value;

                        if ( !ProgramIDs.Contains(id) ){

                            ProgramIDs.Add(id);
                        
                        }
                            

                    }
                            
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }

            return ProgramIDs;
        }

        public List<string> getProgramVersionsByID(string id){

            List<string> ProgramVersions=null;
            try
            {
                string[] programs = Directory.GetFiles(_path, "program_*_*.txt");
                if ( programs.Length > 0 ){
                    ProgramVersions=new List<string>();
                } 
                //string pattern = @"^\w+/\w+/\w+_(\d+)_(\d+)\.txt$"; //LINUX
               // string pattern = @"^\w+\\\w+\\\w+_(\d+)_(\d+)\.txt$"; //WINDOWS
              string pattern = @"^\w:" + _levels_path + @"\\\w+_(\d+)_(\d+)\.txt$"; //WINDOWS
                foreach (string program in programs)
                {
                    foreach (Match match in Regex.Matches(program, pattern, RegexOptions.IgnoreCase)){
                        
                        string pv=match.Groups[2].Value;
                        //Console.WriteLine(id);
                        if ( id == match.Groups[1].Value){

                            if ( !ProgramVersions.Contains(pv) ){
                                ProgramVersions.Add(pv);
                                //Console.WriteLine(pv);
                            }

                        }    

                    }
                            
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }

            return ProgramVersions;

        }

        public List<string> getParameterGroups(){

            List<string> ParameterGroups=null;

            try
            {
                string[] files = Directory.GetFiles(_path, "*_*_*.txt");
                if ( files.Length > 0 ){
                    //Console.WriteLine(files[0]);
                    ParameterGroups=new List<string>();
                } 
                //string pattern = @"^\w+/\w+/((?!program).)+_(\d+)_\d+\.txt$"; //LINUX
              //  string pattern = @"^\w+\\\w+\\((?!program).)+_(\d+)_\d+\.txt$"; //WINDOWS
             string pattern = @"^\w:" + _levels_path + @"\\((?!program).)+_(\d+)_\d+\.txt$"; //WINDOWS
                foreach (string file in files)
                {
                    foreach (Match match in Regex.Matches(file, pattern, RegexOptions.IgnoreCase)){

                        string id=match.Groups[2].Value;

                        if ( !ParameterGroups.Contains(id) ){

                            ParameterGroups.Add(id);
                        
                        }
                            

                    }
                            
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }

            return ParameterGroups;
        }

        public List<string> getParameterVersionsByID(string id){

            List<string> ParameterVersions=null;
            try
            {
                string[] files = Directory.GetFiles(_path, "*_*_*.txt");
                if ( files.Length > 0 ){
                    ParameterVersions=new List<string>();
                } 
                //string pattern = @"^\w+/\w+/((?!program).)+_(\d+)_(\d+)\.txt$"; //LINUX
               // string pattern = @"^\w+\\\w+\\((?!program).)+_(\d+)_(\d+)\.txt$"; //WINDOWS
                string pattern = @"^\w:" + _levels_path + @"\\((?!program).)+_(\d+)_(\d+)\.txt$"; //WINDOWS
                foreach (string file in files)
                {
                    foreach (Match match in Regex.Matches(file, pattern, RegexOptions.IgnoreCase)){
                        
                        string pv=match.Groups[3].Value;
                        //Console.WriteLine(id);
                        if ( id == match.Groups[2].Value){

                            if ( !ParameterVersions.Contains(pv) ){
                                ParameterVersions.Add(pv);
                                //Console.WriteLine(pv);
                            }

                        }    

                    }
                            
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }

            return ParameterVersions;

        }


        // CONTROL.TXT
        public DataTable getControlTxtTable(){

            string filename=_config.Value.control_fname;
            string filepath=$"{_path}/{filename}";

            using (var reader = new StreamReader(filepath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // Do any configuration to `CsvReader` before creating CsvDataReader.
                using (var dr = new CsvDataReader(csv))
                {        
                    var dt = new DataTable();
                    dt.Load(dr);

                    return dt;
                }
            }
        }

        public class TipoComercio{

            public string GroupID {get;set;}
            public string GroupName {get;set;}

        }


        public List<TipoComercio> getControlGroups(){

           var rows = from row in _controltxt.AsEnumerable()
                        //where row.Field<string>("GroupID") == groupid
                        //select row.Field<string>("GroupID");
                        //select row;
                        select new TipoComercio
                        {
                            GroupID = row.Field<string>("GroupID"),
                            GroupName = row.Field<string>("GroupName")
                        };


           /*foreach(var item in rows)
           {
   
                Console.WriteLine(item.GroupID);
                Console.WriteLine(item.GroupName);
           }*/

           return rows.ToList();
        }

        //FIN CONTROL.TXT

        //AUXILIARES
        public SystemLine GetLineByTerminalID(string terminalID){
            //"TDS/Parameters/system.txt"
            string filename=_config.Value.system_fname;
            string filepath=$"{_path}/{filename}";

            SystemLine row=null;

            using (var reader = new StreamReader(filepath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<SystemLine>();                
                foreach ( var r in records ){

                    if (String.Equals(row.TerminalID, terminalID))
                    {
                        row=r;
                        
                    }

                }
            }

            return row;
        }


        public void getAcquirersByGroupID(string groupid){

            string[] columnNames = _controltxt.Columns.Cast<DataColumn>()
                                        .Select(x => x.ColumnName)
                                        //.Where(x => x.Contains("Acquirer"))
                                        .ToArray();
            foreach ( string col in columnNames){
                Console.WriteLine(col);
            }

           var rows = from row in _controltxt.AsEnumerable()
                        where row.Field<string>("GroupID") == groupid
                        //select row.Field<string>("GroupID");
                        select row;


           //Console.WriteLine(rows);
           int i=0;
           foreach(DataRow dataRow in rows)
           {
                foreach(var item in dataRow.ItemArray)
                {
                    if (columnNames[i].Contains("Acquirer"))
                    {
                        Console.Write(item+ " ");
                        Console.WriteLine();
                    }     
                    i++;
                    
                }
                
           }

        }

        private DateTime getLastWriteFile(string fname)
        {
            DateTime dt=DateTime.Now;
            try
            {
                string filepath=$"{_path}/{fname}";
                // Get the creation time of a well-known directory.
                dt = File.GetLastWriteTime(filepath);
                Console.WriteLine("The last write time for this file was {0}.", dt);
                
                // Update the last write time.
                //File.SetLastWriteTime(path, DateTime.Now);
                //dt = File.GetLastWriteTime(path);
                //Console.WriteLine("The last write time for this file was {0}.", dt);
            }

            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }

            return dt;
            
        }

        private bool oldFile(DateTime currentWriteTime)
        {
            int result = -1;
            try
            {
                result = DateTime.Compare(_LastWrite, currentWriteTime);
                //if (result < 0)
                //    _LastWrite  is earlier than currentWriteTime
                //else if (result == 0)
                //     _LastWriteis the same time as currentWriteTime
                //else
                //    _LastWrite is later than currentWriteTime;
            }

            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }

            return result < 0;
            
        }

        public string getLevelsFolders() {

 

            string[] folders_path = _path.Split('\\');

            string folder_levels =string.Empty;

 

            for (int i = 0; i < folders_path.Length - 1; i++)

            {

                folder_levels = folder_levels + @"\\\w+";

            }

 

            return folder_levels;

 

        }

    }

}
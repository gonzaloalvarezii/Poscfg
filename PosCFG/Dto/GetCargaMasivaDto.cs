namespace PosCFG.Dto
{
    public class GetCargaMasivaDto
    {
        public int id {get; set;}   
        public string TerminalID { get; set; }
        public string SerialNumber { get; set; }
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
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
        public string Custom19 {get; set;}
        public int TerminalChecksum { get; set; }
        public int ControlGroup{get; set;}
        public int ControlCheckSum{get; set;}
        public int ParameterGroup{get; set;}
        public int ParameterReload{get; set;}
        public int ParameterVersion{get; set;}
        public int ProgramID{get; set;}
        public int ProgramReload{get; set;}
        public int ProgramVersion{get; set;}
        public string Paquete{get; set;}
        public int ConnectGroup{get; set;}
        public int ParmConnChecksum{get; set;}
        public string id_jpos {get; set;}
        public string value {get;set;}
        
    }
}
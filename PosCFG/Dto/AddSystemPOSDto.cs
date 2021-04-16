using PosCFG.Models;

namespace PosCFG.Dto
{
    public class AddSystemPOSDto
    {
        public string TerminalID { get; set; }
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
        public int TranConnChecksum1{get; set;}
        public int TranConnChecksum2{get; set;}
    }
}
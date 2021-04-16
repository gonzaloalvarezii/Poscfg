
using System.Collections.Generic;
namespace PosCFG.Dto

{
    public class GetTerminalExternalClientDto
    {
        //Campos de Terminal
        public string TerminalID { get; set; }
        public string SerialNumber { get; set; }
        public string nombreComercial { get; set; }
        public string direccionFiscal { get; set; }
        public string rut { get; set; }

        public AcquirerDto acquirer {get; set;}
       
        //Campos de System
        //[JsonIgnore]
        //public  SystemPOS SystemPOS{get; set;}
        
        public int ConnectGroup{get; set;}
        //terminalChecksum de System
        public int terminalInit {get;set;}
        //controlGroup de system
        public int merchantType{get; set;}
        
        //field of TerminalsStatus, status 1 o 0 => 1=Activo , 0=Inactivo
        public int terminalEnabled {get;set;}
        //Campos JPOS
        ///public string id {get; set;}
        ///public string value {get; set;}

        public string ca_name {get; set;}

        public string mcc {get; set;}

        public string pf_id {get; set;}

        public string visa_spnsrd_mercht {get; set;}

        public string amex_id_comercio {get; set;}
    }
}
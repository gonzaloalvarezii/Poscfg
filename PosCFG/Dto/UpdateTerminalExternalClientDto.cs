using System.ComponentModel.DataAnnotations;

namespace PosCFG.Dto
{
    public class UpdateTerminalExternalClientDto
    {
        //Campos de Terminal
        public string TerminalID { get; set; }
        public string nombreComercial {get;set;}
        public string direccionFiscal {get;set;}
        public string rut {get; set;}

        public AcquirerDto acquirer {get; set;}



        //es conntrolGroup que está en System        
        public int? merchantType {get; set;}
        //se obtiene de tabla TerminalStatus
        public int? terminalEnabled {get; set;}
        //Campos de System
        //[JsonIgnore]
        //public  SystemPOS SystemPOS{get; set;}
        //public int ControlGroup{get; set;}

        //Campos JPOS
        //[RST-34]
        /*
        CARD ACCEPTOR. Ver documento de formato de campo 43 de VISANET  
        (1-25 card acceptor name; 26-38 city name; 39-40 country code).
        Formato. Ej: "Merceria Katty           Montevideo   UY"
        */
        public string ca_name {get; set;}
        /*
        MCC. Se debe enviar en el campo 18 de cada transacción el MCC correspondiente al sub-merchant.
        */
        public string mcc {get;set;}
        /*
        Los Payment Facilitators (PF) deben identificar las transacciones en el campo 104 
        del mensaje ISO el ID asignadopor VISA ( PF ID ) en el registro como Third Party Agent así como los datos del sub-merchant.
        */
        public string pf_id {get;set;}
        /*
        RUT (“Tipo DOC” + “ “ + “12345678910”)
        */
        public string visa_spnsrd_mercht {get;set;}
        /*
        Id comercion asociado AMEX
        */
        public string amex_id_comercio {get;set;}
     
    }
}
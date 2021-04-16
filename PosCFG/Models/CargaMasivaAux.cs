using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FileHelpers;

namespace PosCFG.Models
{
    public class CargaMasivaAux
    {
        public int id {get; set;}   
        //TERMINAL
        public string TerminalID { get; set; }
        //Nro de Serie (HW)
        public string SerialNumber { get; set; }
        //Nombre del Comercio
        //custom1
        public string Custom1 { get; set; }
        //custom2
        //Direccion
        public string Custom2 { get; set; }
        //RUT
        //custom3
        public string Custom3 { get; set; }
        //visanet
        //custom4
        public string Custom4 {get; set;}
        //first data
        //custom5
        public string Custom5 {get; set;}
        //oca
        //custom6
        public string Custom6 {get; set;}
        //creditos directos
        //custom7
        public string Custom7 {get; set;}
        //cabal
        //custom8
        public string Custom8 {get; set;}
        //creditel
        //custom9
        public string Custom9 {get; set;}
        //passcard
        //custom10
        public string Custom10 {get; set;}
        //edenred
        //custom11
        public string Custom11 {get; set;}
        //anda
        //custom12
        public string Custom12 {get; set;}
        //amex
        //custom13
        public string Custom13 {get; set;}
        //club del este
        //custom14
        public string Custom14 {get; set;}
        //mides
        //custom15
        public string Custom15 {get; set;}
        //brou 
        //custom16
        public string Custom16 {get; set;}
        //cabal u$s
        //custom19
        public string Custom19 {get; set;}
        
        //SYSTEM
        //system
        //Tipo de comercio
        //public int merchanType {get; set;}
        //actualizar datos de Terminal
        public int TerminalChecksum { get; set; }
        //simcard?
        public int ControlGroup{get; set;}
        //actualizar tipo comercio
        public int ControlCheckSum{get; set;}
        //Parametergroup
        public int ParameterGroup{get; set;}
        //cargar parametro de conexion
        public int ParameterReload{get; set;}
        //parameter version
        public int ParameterVersion{get; set;}
        //Program ID
        public int ProgramID{get; set;}
        //Cargar programa
        public int ProgramReload{get; set;}
        //Program Version
        public int ProgramVersion{get; set;}
        //Package Name
        public string Paquete{get; set;}
        //Connect Group
        public int ConnectGroup{get; set;}
        //Actualizar parametros de conexion
        public int ParmConnChecksum{get; set;}

        //Campos JPOS
        //JPOS
        
        public string id_jpos {get; set;}
        public string value {get;set;}
        
    }
}
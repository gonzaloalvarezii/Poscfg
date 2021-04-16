using System;
using System.Text.Json.Serialization;
using PosCFG.Models;

namespace PosCFG.Dto
{
    public class GetTerminalsExternalDto
    {
        //Campos de Terminal
        public string TerminalID { get; set; }
        public string SerialNumber { get; set; }
        //public string HeaderLine1 { get; set; }
        //public string HeaderLine2 { get; set; }
        //public string HeaderLine3 { get; set; }
        public string Custom1 {get; set;}
        public string Custom2 {get; set;}
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
        //Campos de System
        //[JsonIgnore]
        //public  SystemPOS SystemPOS{get; set;}
        public int ControlGroup{get; set;}
        public int ConnectGroup{get; set;}

        //Campos JPOS
        public string id_jpos {get; set;}
        public string value {get; set;}
        
    }
}
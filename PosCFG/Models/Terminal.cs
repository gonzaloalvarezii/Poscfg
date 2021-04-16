using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FileHelpers;
using Microsoft.EntityFrameworkCore;

namespace PosCFG.Models {
[DelimitedRecord(",")]
[Table("Terminals")]
public class Terminal{
//[Key]
//public int id {get; set;}
[Required]
public string TerminalID { get; set; }
[Required]
public string SerialNumber { get; set; }
[FieldHidden]
public SystemPOS SystemPOS { get; set; }
//public string HeaderLine1 { get; set; }
//public string HeaderLine2 { get; set; }
//public string HeaderLine3 { get; set; }
public string Suspend{get; set;}
public string ParmConnectTime{get; set;}
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

public DateTime TranConnectTime {get; set; }

}
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FileHelpers;
using Microsoft.EntityFrameworkCore;


namespace PosCFG.Models {
[DelimitedRecord(",")]
[Table("SystemPOSs")]
public class SystemPOS{
[FieldHidden]
[Key]
public int ID { get; set; }

[Required]
[ForeignKey("TerminalID")]
public string TerminalID { get; set; }
[FieldHidden]
public Terminal Terminal { get; set; }
[Required]
public int TerminalChecksum { get; set; }
[Required]
public int ControlGroup{get; set;}
[Required]
public int ControlCheckSum{get; set;}
public int ParameterGroup{get; set;}
public int ParameterReload{get; set;}
public int ParameterVersion{get; set;}
public int ProgramID{get; set;}
public int ProgramReload{get; set;}
public int ProgramVersion{get; set;}
[MaxLength(100)]
public string Paquete{get; set;}
public int ConnectGroup{get; set;}
public int ParmConnChecksum{get; set;}
public int TranConnChecksum1{get; set;}
public int TranConnChecksum2{get; set;}

    
    }
}
using System.ComponentModel.DataAnnotations;
using FileHelpers;

namespace PosCFG.Models
{
    public class TerminalStatus
    {
        [FieldHidden]
        [Key]
        public int id { get; set; }
        public string TerminalID { get; set;}
        public int status { get; set;}

    }
}
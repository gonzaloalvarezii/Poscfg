using System.ComponentModel.DataAnnotations;

namespace PosCFG.Models
{
    public class Sysconfig
    {   
        [Key]
        public string id {get; set;}
        public string value {get; set;}
    }
}
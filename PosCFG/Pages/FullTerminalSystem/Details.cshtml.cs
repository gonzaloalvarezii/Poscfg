using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PosCFG.Dto;

namespace PosCFG.Pages.FullTerminalSystem
{
    public class DetailsModel : BaseModel
    {
        private readonly IFullTerminalSystemService _fullterminalsystemservice;

        public DetailsModel(IFullTerminalSystemService fullterminalsystemservice)
        {
            _fullterminalsystemservice = fullterminalsystemservice;
        }
        public GetFullTerminalSystemDto Terminals{ get; set; }

        public async Task<IActionResult> OnGetAsync(string Terminalid)
        {
            if (Terminalid == null)
            {
                return NotFound();
            }

            var sys = await _fullterminalsystemservice.GetFullTerminalSystemByID(Terminalid);
            Terminals = sys.Data;
            
            if (Terminals == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}

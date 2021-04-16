using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PosCFG.Dto;

namespace PosCFG.Pages
{
    public class DetailsModel : BaseModel
    {
        private readonly ITerminalService _terminalservice;

        public DetailsModel(ITerminalService terminalservice)
        {
            _terminalservice = terminalservice;
        }
        public GetTerminalDto Terminals{ get; set; }

        public async Task<IActionResult> OnGetAsync(string Terminalid)
        {
            if (Terminalid == null)
            {
                return NotFound();
            }

            var sys = await _terminalservice.GetTerminalByID(Terminalid);
            Terminals = sys.Data;
            
            if (Terminals == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}

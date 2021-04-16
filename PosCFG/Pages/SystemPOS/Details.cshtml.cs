using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PosCFG.Dto;

namespace PosCFG.Pages.SystemPOS
{
    public class DetailsModel : BaseModel
    {
        private readonly ISystemPOSService _systemposservice;

        public DetailsModel(ISystemPOSService systemposservice)
        {
            _systemposservice = systemposservice;
        }

        public GetSystemPOSDto SystemPosS{ get; set; }
        public async Task<IActionResult> OnGetAsync(string Terminalid)
        {
            if (Terminalid == null)
            {
                return NotFound();
            }

            var sys = await _systemposservice.GetSystemPOSByID(Terminalid);
            SystemPosS = sys.Data;
            
            if (SystemPosS == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}

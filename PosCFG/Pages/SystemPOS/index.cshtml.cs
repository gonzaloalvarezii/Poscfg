using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PosCFG.Dto;

namespace PosCFG.Pages.SystemPOS
{
    public class IndexModel : BaseModel
        {
            private readonly ISystemPOSService _systemposservice;
            public IndexModel(ISystemPOSService systemposservice)
            {
                _systemposservice = systemposservice;
            }
            public IList<GetSystemPOSDto> SystemPosS{ get; set; }

            public async Task OnGetAsync(string searchString)
            {
                var response = await _systemposservice.GetSystemPOS();
                SystemPosS = response.Data;
                var sys = from m in response.Data select m;

                if(!String.IsNullOrEmpty(searchString))
                {
                    sys = sys.Where(s => s.TerminalID.Contains(searchString));
                }

                SystemPosS = sys.ToList();
            }

            /*
            public async Task OnGetAsync(string searchString)
            {
            var response = await _terminalservice.GetTerminals();
            var terminals = from m in response.Data select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                terminals = terminals.Where(s => s.TerminalID.Contains(searchString));
            }
            
            
            Terminals = terminals.ToList();

            }
            */
            public async Task<IActionResult> OnPostDeleteAsync(GetSystemPOSDto sys)
            {
                var _sys = await _systemposservice.GetSystemPOSByID(sys.TerminalID);
            

                if (_sys.Data != null)
                {
                    await _systemposservice.DeleteSystemPOS(_sys.Data.TerminalID);
                    
                }

                return RedirectToPage("./Index");
            }

    }

}
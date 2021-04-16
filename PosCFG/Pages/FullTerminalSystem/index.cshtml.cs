using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PosCFG.Dto;

namespace PosCFG.Pages.FullTerminalSystem
{
    public class indexModel : BaseModel
    {
        private readonly IFullTerminalSystemService _fullterminalsystem;
        private readonly ITerminalService _terminalservice;
        public indexModel(IFullTerminalSystemService fullterminalsystem, ITerminalService temrinalservice)
        {
            _fullterminalsystem = fullterminalsystem;
            _terminalservice = temrinalservice;
        }

        
        public IList<GetTerminalDto> listTerminals;

        public async Task OnGetAsync(string searchString)
            {
                var response = await _fullterminalsystem.GetTerminalSystem();
                
                listTerminals = response.Data;
                var sys = from m in response.Data select m;

                if(!String.IsNullOrEmpty(searchString))
                {
                    sys = sys.Where(s => s.TerminalID.Contains(searchString));
                }

                listTerminals = sys.ToList();
            }

        public async Task<IActionResult> OnPostDeleteAsync(GetFullTerminalSystemDto fullTerminalSystem)
        {
            var response = await _fullterminalsystem.DeleteFullTerminalSystem(fullTerminalSystem.TerminalID);

            if (response.Data != null)
            {
                await _fullterminalsystem.DeleteFullTerminalSystem(response.Data.TerminalID);
                
            }

            return RedirectToPage(); 

        }

        
    }
}

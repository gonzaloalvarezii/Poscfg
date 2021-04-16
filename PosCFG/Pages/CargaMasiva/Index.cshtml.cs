using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PosCFG.Dto;
using PosCFG.Models;

namespace PosCFG.Pages.CargaMasiva
{
    public class IndexModel : BaseModel
    {
        private readonly IFullTerminalSystemService _fullTerminalSystem;
        public IndexModel(IFullTerminalSystemService fullTerminalSystem)
        {
            _fullTerminalSystem = fullTerminalSystem;
        }
        [BindProperty]
        public AddFullTerminalSystemDto fullTerminalSystemAUX {get; set;}
        [BindProperty]
        public IList<GetCargaMasivaDto> listTerminalsAux {get; set;}
        public async Task OnGetAsync(string searchString)
            {
                var response = await _fullTerminalSystem.GetTerminalAux();
                
                listTerminalsAux = response.Data;
                var sys = from m in response.Data select m;

                if(!String.IsNullOrEmpty(searchString))
                {
                    sys = sys.Where(s => s.TerminalID.Contains(searchString));
                }

                listTerminalsAux = sys.ToList();
            }

        public async Task<IActionResult> OnPostSaveAsync(int id)
        {
            
            var term = await _fullTerminalSystem.GetFullTerminalSystemByID(fullTerminalSystemAUX.TerminalID);
            GetFullTerminalSystemDto TerminalAux = term.Data;
            if(TerminalAux==null)
            {
                var response = await _fullTerminalSystem.GetTerminalAuxByID(id);
                response.Data.SerialNumber = fullTerminalSystemAUX.SerialNumber;
                response.Data.TerminalID = fullTerminalSystemAUX.TerminalID;
                
                await _fullTerminalSystem.InsertNewFullTerminalFromCargaMasivaAux(response.Data);
                await _fullTerminalSystem.DeleteTerminalCargaMasivaAux(id);
                    
                
            }

            return RedirectToPage(); 

        }


    }
}

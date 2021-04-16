using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PosCFG.Dto;

namespace PosCFG.Pages.CargaMasiva
{
    public class createModel : BaseModel
    {
        private readonly IFullTerminalSystemService _fullterminalsystem;
        
        
        public createModel(IFullTerminalSystemService fullterminalsystem)
        {
            _fullterminalsystem = fullterminalsystem;
        }
        
        [BindProperty]
        public AddFullTerminalSystemDto fullTerminalSystemAUX {get; set;}
        [BindProperty]
        public int cantTerminalesACargar {get; set;}
        

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                for (int i = 0; i < cantTerminalesACargar; i++)
                {
                    await _fullterminalsystem.InsertTerminalCargaMasivaAux(fullTerminalSystemAUX);
                }
                
            }
            catch (System.Exception)
            {
                
                throw;
            }
            
            return RedirectToPage("./Index");
        }

    }
}

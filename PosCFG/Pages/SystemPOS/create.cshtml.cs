using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PosCFG.Dto;

namespace PosCFG.Pages.SystemPOS
{
    public class createModel : BaseModel
    {
        private readonly ISystemPOSService _systemposservice;
        
        public createModel(ISystemPOSService systemposservice)
        {
            _systemposservice = systemposservice;
        }
    

        [BindProperty]
        public AddSystemPOSDto SystemPosS{ get; set; }
        
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
                await _systemposservice.AddSystemPOS(SystemPosS);
            }
            catch (System.Exception)
            {
                
                throw;
            }
            
            return RedirectToPage("./Index");
        }


    }
}

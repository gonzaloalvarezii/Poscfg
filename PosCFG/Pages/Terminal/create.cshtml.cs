using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using PosCFG.Dto;
using PosCFG.Controllers;

namespace PosCFG.Pages
{
    
    public class CreateModel : BaseModel
    {
        private readonly ITerminalService _terminalservice;
        public CreateModel(ITerminalService terminalservice)
        {
                _terminalservice = terminalservice;
        }
        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public GetTerminalDto Terminal {get; set;}

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _terminalservice.AddTerminal(Terminal);

            return RedirectToPage("./Index");
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using PosCFG.Dto;
using PosCFG.Controllers;
using System.Collections.Generic;

namespace PosCFG.Pages
{
    
    public class EditModel : BaseModel
    {
        private readonly ITerminalService _terminalservice;
        public EditModel(ITerminalService terminalservice )
        {
            _terminalservice = terminalservice;
            
        }
        [TempData]
        public string Message { get; set; }


        [BindProperty]
        public GetTerminalDto Terminal { get; set; }

        public async Task<IActionResult> OnGetAsync(string terminalID)
        {
            var _terminal = await _terminalservice.GetTerminalByID(terminalID);
            Terminal = _terminal.Data;
            if (_terminal.Data == null)
            {
                return RedirectToPage("./Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var _terminal = await _terminalservice.GetTerminalByID(Terminal.TerminalID);        
            var result = await _terminalservice.UpdateTerminal(Terminal);
            Message = $"{result.Message}";
            return RedirectToPage("./Index");
        }

    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using PosCFG.Dto;
using PosCFG.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PosCFG.Pages.Terminal
{

    public class TerminalModel : BaseModel

    {
        private readonly ITerminalService _terminalservice;
        [TempData]
        public string Message { get; set; }
        public string TerminalIDSort { get; set; }
        public string NombreSort { get; set; }
        public string DireccionSort { get; set; }
        public string RutSort { get; set; }

        public TerminalModel(ITerminalService terminalservice)
        {
            _terminalservice = terminalservice;
        }

        public IList<GetTerminalDto> Terminals{ get; set; }

        public async Task OnGetAsync(string searchString, string sortOrder)
        {
            var response = await _terminalservice.GetTerminals();
            var terminals = from m in response.Data select m;
            DireccionSort = String.IsNullOrEmpty(sortOrder) ? "Direccion_desc" : "";
            NombreSort = String.IsNullOrEmpty(sortOrder) ? "Nombre_desc" : "";
            

            if (!String.IsNullOrEmpty(searchString))
            {
                terminals = terminals.Where(s => s.TerminalID.Contains(searchString));
            }
            switch(sortOrder)
            {
                case "Nombre_desc":
                    terminals = terminals.OrderByDescending(s => s.Custom1);
                    break;
                case "DireccionSort_desc":
                    terminals = terminals.OrderByDescending(s => s.Custom2);
                    break;
                
                default:
                    terminals = terminals.OrderByDescending(s => s.TerminalID);
                    break;

            }
            
            Terminals = terminals.ToList();

        }

        public async Task<IActionResult> OnPostDeleteAsync(GetTerminalDto terminal)
        {
            var _terminal = await _terminalservice.GetTerminalByID(terminal.TerminalID);
            

            if (_terminal.Data != null)
            {
                await _terminalservice.DeleteTerminal(_terminal.Data.TerminalID);
                
            }

            return RedirectToPage();
        }
    }
}
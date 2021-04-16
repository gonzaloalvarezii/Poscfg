using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PosCFG.Dto;

namespace PosCFG.Pages.SystemPOS
{
    public class editModel : BaseModel
    {
        private readonly ISystemPOSService _systemposservice;
        
        public editModel(ISystemPOSService systemposservice)
        {
            _systemposservice = systemposservice;
        }
    

        [BindProperty]
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

        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                AddSystemPOSDto sys = new AddSystemPOSDto();
                sys.TerminalID = SystemPosS.TerminalID;
                sys.TerminalChecksum = SystemPosS.TerminalChecksum;
                sys.ControlGroup = SystemPosS.ControlGroup;
                sys.ControlCheckSum = SystemPosS.ControlCheckSum;
                sys.ParameterGroup = SystemPosS.ParameterGroup;
                sys.ParameterReload = SystemPosS.ParameterReload;
                sys.ParameterVersion = SystemPosS.ParameterVersion;
                sys.ProgramID = SystemPosS.ProgramID;
                sys.ProgramReload = SystemPosS.ProgramReload;
                sys.ProgramVersion = SystemPosS.ProgramVersion;
                sys.Paquete = SystemPosS.Paquete;
                sys.ConnectGroup = SystemPosS.ConnectGroup;
                sys.ParmConnChecksum = SystemPosS.ParmConnChecksum;
                sys.TranConnChecksum1 = SystemPosS.TranConnChecksum1;
                sys.TranConnChecksum2 = SystemPosS.TranConnChecksum2;

                
                await _systemposservice.UpdateSystemPOS(sys);

            }
            catch (DbUpdateConcurrencyException)
            {
                
                throw;
            }
        
        return RedirectToPage("./Index");
        }
    }
}

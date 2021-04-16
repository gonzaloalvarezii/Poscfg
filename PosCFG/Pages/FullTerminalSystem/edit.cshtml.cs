using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PosCFG.Dto;
using PosCFG.Service;

namespace PosCFG.Pages.FullTerminalSystem
{
    public class editModel : BaseModel
    {
        private readonly IFullTerminalSystemService _fullterminalsystemservice;
        private readonly ICSVService _csvfile;

        /*
        //MENU DESPLEGABLES
        //Connect Group
        public IEnumerable<SelectListItem> GetConnectGroups()
        {       
                List<SelectListItem> listaCG = new List<SelectListItem>();

                foreach (string cg in _csvfile.getConnectGroups())
                {
                            listaCG.Add(
                                new SelectListItem
                                {
                                    Value = cg,
                                    Text = cg
                                }
                            );
                    
                }

                return new SelectList(listaCG, "Value", "Text");

        }

        //Parameter Group
        public IEnumerable<SelectListItem> GetParameterGroups()
        {       
                List<SelectListItem> listaPG = new List<SelectListItem>();

                foreach (string pg in _csvfile.getParameterGroups())
                {
                            listaPG.Add(
                                new SelectListItem
                                {
                                    Value = pg,
                                    Text = pg
                                }
                            );
                    
                }

                return new SelectList(listaPG, "Value", "Text");

        }

        //ControlGroups
        public IEnumerable<SelectListItem> GetControlGroups()
        {       
                List<SelectListItem> listaCG = new List<SelectListItem>();

                foreach (var cg in _csvfile.getControlGroups())
                {
                            listaCG.Add(
                                new SelectListItem
                                {
                                    Value = cg.GroupID,
                                    Text = cg.GroupName
                                }
                            );
                    
                }

                return new SelectList(listaCG, "Value", "Text");

        }

        //ProgramIDs
        public IEnumerable<SelectListItem> GetProgramIDs()
        {       
                List<SelectListItem> listaPGID = new List<SelectListItem>();

                foreach (var pid in _csvfile.getProgramIDs())
                {
                            listaPGID.Add(
                                new SelectListItem
                                {
                                    Value = pid,
                                    Text = pid
                                }
                            );
                    
                }

                return new SelectList(listaPGID, "Value", "Text", listaPGID.FirstOrDefault());

        }

        //ProgramVersions//No Necesario Por Dropdown Cascade
        /*public IEnumerable<SelectListItem> GetProgramVersions(string id)
        {       
                List<SelectListItem> listaPGVER = new List<SelectListItem>();

                foreach (var pv in _csvfile.getProgramVersionsByID(id))
                {
                            listaPGVER.Add(
                                new SelectListItem
                                {
                                    Value = pv,
                                    Text = pv
                                }
                            );
                    
                }

                return new SelectList(listaPGVER, "Value", "Text");

        }*/

        
        public editModel(IFullTerminalSystemService fullterminalsystemservice, ICSVService csvfile)
        {
            _fullterminalsystemservice = fullterminalsystemservice;
            _csvfile = csvfile;
        }

        [BindProperty(SupportsGet = true)]
        public GetFullTerminalSystemDto fullTerminalSystem { get; set; }

        /*
        //DropDownList Cascada AJAX
        public JsonResult OnGetProgramVersions()
        {
            //Console.WriteLine(pid);
            return new JsonResult(_csvfile.getProgramVersionsByID(Convert.ToString(fullTerminalSystem.ProgramID)));
        }

        public JsonResult OnGetParameterVersions()
        {
            //Console.WriteLine(pver);
            return new JsonResult(_csvfile.getParameterVersionsByID(Convert.ToString(fullTerminalSystem.ParameterGroup)));
        }
        */

        public async Task<IActionResult> OnGetAsync(string terminalID)
        {
            var _terminal = await _fullterminalsystemservice.GetFullTerminalSystemByID(terminalID);
            fullTerminalSystem = _terminal.Data;
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
            //var _terminal = await _fullterminalsystemservice.GetFullTerminalSystemByID(fullTerminalSystem.TerminalID);        
            var result = await _fullterminalsystemservice.UpdateFulTerminalSystem(fullTerminalSystem);
            //Message = $"{result.Message}";
            return RedirectToPage("./Index");
        }

        
    }
}

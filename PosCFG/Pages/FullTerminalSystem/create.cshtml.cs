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
    public class createModel : BaseModel
    {
        private readonly IFullTerminalSystemService _fullterminalsystem;

        private readonly ICSVService _csvfile;


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

        public createModel(IFullTerminalSystemService fullterminalsystem, ICSVService csvfile)
        {
            _fullterminalsystem = fullterminalsystem;
            _csvfile = csvfile;
        }

        [BindProperty(SupportsGet = true)]
        //[BindProperty]
        public AddFullTerminalSystemDto fullTerminalSystemPOS { get; set; }


        //DropDownList Cascada AJAX
        public JsonResult OnGetProgramVersions()
        {
            //Console.WriteLine(pid);
            return new JsonResult(_csvfile.getProgramVersionsByID(Convert.ToString(fullTerminalSystemPOS.ProgramID)));
        }

        public JsonResult OnGetParameterVersions()
        {
            //Console.WriteLine(pver);
            return new JsonResult(_csvfile.getParameterVersionsByID(Convert.ToString(fullTerminalSystemPOS.ParameterGroup)));
        }

     

        public IEnumerable<SelectListItem> GetDefaultsTypes()
        {
          
            SelectList DefaultsTypes = new SelectList(new List<SelectListItem>{new SelectListItem {Text ="Ninguno", Value = "0"},
                                                                                 new SelectListItem {Text = "Handy", Value = "1"},
                                                                                 new SelectListItem {Text = "Resonance", Value = "2"},},


                       "Value", "Text");


            return DefaultsTypes;

        }


        public IEnumerable<SelectListItem> GetParameterVersion(string id) {

            IEnumerable<SelectListItem> lista = (IEnumerable<SelectListItem>)_csvfile.getParameterVersionsByID(id).Select(i => new SelectListItem{Text = i.ToString(),Value = i});

            return lista;
        }


        public IEnumerable<SelectListItem> GetProgramVersion(string id)
        {

            IEnumerable<SelectListItem> lista = (IEnumerable<SelectListItem>)_csvfile.getProgramVersionsByID(id).Select(i => new SelectListItem { Text = i.ToString(), Value = i });

            return lista;
        }
        public IActionResult OnGet(string Parameter_Default)
        {


            if (Parameter_Default != null)
            {
                if (Parameter_Default != "0")
                {
                    if (Parameter_Default == "1")
                    {
                        AddFullTerminalSystemDto _defaultHandy = new AddDefaultHandyTerminalSystemDto();
                        this.fullTerminalSystemPOS = _defaultHandy;
                    }
                    else if (Parameter_Default == "2")
                    {
                        AddFullTerminalSystemDto _defaultResonance = new AddDefaultResonanceTerminalSystemDto();
                        this.fullTerminalSystemPOS = _defaultResonance;
                    }

                }
            }
            return Page();
        }

        public async Task<JsonResult> OnGetCheckTerminal(string id)
        {
            ServiceResponse<GetTerminalDto> serviceResponse = new ServiceResponse<GetTerminalDto>();
            List<string> respuesta = new List<string>();

            serviceResponse = await  _fullterminalsystem.GetTerminalByID(id);

            if (serviceResponse.Data != null)
            {
                respuesta.Add("Terminal Existente.");
            }
            else
            {
                respuesta.Add("");
            }

            return new JsonResult(respuesta);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _fullterminalsystem.InsertFullTerminalSystem(fullTerminalSystemPOS);
            }
            catch (System.Exception)
            {

                throw;
            }

            return RedirectToPage("./Index");
        }


    }
}

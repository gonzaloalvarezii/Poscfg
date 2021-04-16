using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
public class BaseModel : PageModel
{
    public string CurrentFilter { get; set; }
}
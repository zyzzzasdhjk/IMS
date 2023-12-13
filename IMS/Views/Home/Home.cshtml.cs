using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IMS.Views.Home;

public class Home : PageModel
{
    [BindProperty] public string Today { get; private set; } = DateTime.Now.ToString("yyyy-MM-dd");

    public void OnGet()
    {
        Today = DateTime.Now.ToString("yyyy-MM-dd");
    }
}
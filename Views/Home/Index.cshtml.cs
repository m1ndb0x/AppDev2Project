using AppDev2Project.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppDev2Project.Pages;

public class IndexModel : PageModel
{
    public bool IsDbConnected { get; private set; }

    private readonly DatabaseConnectionStatus _dbStatus;

    public IndexModel(DatabaseConnectionStatus dbStatus)
    {
        _dbStatus = dbStatus;
    }

    public void OnGet()
    {
        IsDbConnected = _dbStatus.IsConnected;
    }
}
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MezunBurada.Web.Pages.Test;

public class BolumModel : PageModel
{
    public string SelectedDeptKey { get; } = "DeptComputerEngineering";

    public IReadOnlyList<string> DepartmentKeys { get; } = new List<string>
    {
        "DeptComputerEngineering",
        "DeptSoftwareEngineering",
        "DeptElectricalEngineering",
        "DeptIndustrialEngineering",
        "DeptMechanicalEngineering",
        "DeptArchitecture",
        "DeptBusiness",
        "DeptEconomics",
    };

    public void OnGet()
    {
    }
}

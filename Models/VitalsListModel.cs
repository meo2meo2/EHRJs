namespace EHRJs.Models;

public class VitalsListModel
{
    public required bool isPost { get; set; }
    public List<VitalsModel> vitals { get; set; }
}
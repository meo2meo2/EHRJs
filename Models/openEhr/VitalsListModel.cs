namespace EHRJs.Models;

public class VitalsListModel
{
    public required bool isPost { get; set; }
    public List<VitalsModel> vitals { get; set; }

    public required bool isError { get; set; }

    public required bool isSuccess { get; set; }

    public required string SuccessMessage { get; set; }

    public required string ErrorMessage { get; set; }
}
namespace EHRJs.Models;

public class FhirListModel
{
    public required bool isPost { get; set; }
    public List<VitalsModel> vitals { get; set; }

    public required bool isError { get; set; }

    public required bool isSuccess { get; set; }

    public required string SuccessMessage { get; set; }

    public required string ErrorMessage { get; set; }

    public List<PatientModel> Patients { get; set; }
    public string NextLink { get; set; }
    public string PreviousLink { get; set; }
}
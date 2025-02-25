using EHRJs.Utils.Fhir;

namespace EHRJs.Models;

public class PatientModel
{
    public string PatientID { get; set; } = string.Empty;
    public string DOB { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public Gender Gender { get; set; }

    public int Age => DateTime.Now.Year - Convert.ToDateTime(DOB).Year;
}
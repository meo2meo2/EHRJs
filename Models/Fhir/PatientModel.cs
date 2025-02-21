using EHRJs.Utils.Fhir;

namespace EHRJs.Models;

public class PatientModel
{
    public string PatientID { get; set; }
    public DateTime DOB { get; set; }
    public string FirstName { get; set; }
    public string  LastName { get; set; }
    public string  PhoneNumber { get; set; }
    public Gender Gender { get; set; }

    public int Age => DateTime.Now.Year - DOB.Year;
}
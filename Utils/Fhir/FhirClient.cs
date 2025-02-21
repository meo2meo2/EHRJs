using EHRJs.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using RestSharp;

namespace EHRJs.Utils.Fhir;

using Hl7.Fhir.Rest;

public class FHIRUtility
{
    private string serverLocation;
    private FhirClient fhirClient;
    public FHIRUtility()
    {
        serverLocation = FhirAppConstants.IsLocal
            ? FhirAppConstants.LocalFhirServerLocation
            : FhirAppConstants.RemoteFhirServerLocation;
        var httpClient = new HttpClient();
        
       fhirClient = new FhirClient(serverLocation, httpClient,new FhirClientSettings(){ Timeout = 0,
           PreferredFormat = ResourceFormat.Json,
           VerifyFhirVersion = true,
           // PreferredReturn can take Prefer.ReturnRepresentation or Prefer.ReturnMinimal to return the full resource or an empty payload
           PreferredReturn = Prefer.ReturnRepresentation});
    }

    public List<Patient> GetPatients()
    {
        List<Patient> patients = new List<Patient>();
        Bundle bund = fhirClient.Search<Patient>();
        for (int i=0; i< bund.Entry.Count; i++)
        {
           patients.Add((Patient)bund.Entry[i].Resource); 
        }
        return patients;
    }
    
    public bool DeletePatient(string id)
    {
        Patient p = (Patient)fhirClient.Get("Patient" +"/" + id);
        fhirClient.Delete(p);
        return true;
    }

    public bool AddPatient(PatientModel model)
    {
        Patient p = new Patient();
        p.Gender = model.Gender.ToString() == "Male" ? AdministrativeGender.Male : AdministrativeGender.Female;
        p.BirthDate = model.DOB.ToString("yyyy-MM-dd");
        p.Name = new List<HumanName>(){new HumanName().WithGiven(model.FirstName)};
        p.Name[0].Family = model.LastName;
        p.Telecom = new List<ContactPoint>();
        ContactPoint point = new ContactPoint(ContactPoint.ContactPointSystem.Phone,
            ContactPoint.ContactPointUse.Mobile, model.PhoneNumber);
        p.Telecom.Add(point); 
        p = fhirClient.Create(p);
        return p.Id.Length > 0 ? true : false;
    }
}
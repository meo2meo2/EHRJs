using System.Text.RegularExpressions;
using EHRJs.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;

namespace EHRJs.Utils.Fhir;

public class FHIRUtility
{
    private readonly FhirClient fhirClient;
    private readonly string serverLocation;

    public FHIRUtility()
    {
        serverLocation = FhirAppConstants.IsLocal
            ? FhirAppConstants.LocalFhirServerLocation
            : FhirAppConstants.RemoteFhirServerLocation;
        var httpClient = new HttpClient();

        fhirClient = new FhirClient(serverLocation, httpClient, new FhirClientSettings
        {
            Timeout = 0,
            PreferredFormat = ResourceFormat.Json,
            VerifyFhirVersion = true,
            // PreferredReturn can take Prefer.ReturnRepresentation or Prefer.ReturnMinimal to return the full resource or an empty payload
            PreferredReturn = Prefer.ReturnRepresentation
        });
    }

    public List<Patient> GetPatients(ref string next, ref string before)
    {
        List<Patient> patients = new List<Patient>();
        var searchParam = new SearchParameter();

        Bundle? bund = null;
        try
        {
            bund = fhirClient.Search(new SearchParams().OrderBy("_lastUpdated", SortOrder.Descending), "Patient");

        }
        catch (Exception e)
        {
            bund = fhirClient.Search(new SearchParams(), "Patient");

        }
       
        for (var i = 0; i < bund.Entry.Count; i++)
        {
            try
            {
                patients.Add((Patient)bund.Entry[i].Resource);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }

        if (!string.IsNullOrEmpty(bund.PreviousLink?.ToString())) before = bund.PreviousLink.ToString();

        if (!string.IsNullOrEmpty(bund.NextLink?.ToString())) next = bund.NextLink.ToString();
        return patients;
    }

    public bool DeletePatient(string id)
    {
        var p = (Patient)fhirClient.Get("Patient" + "/" + id);
        fhirClient.Delete(p);
        return true;
    }

    public bool AddPatient(PatientModel model)
    {
        var p = new Patient();
        p.Gender = model.Gender.ToString() == "Male" ? AdministrativeGender.Male : AdministrativeGender.Female;
        p.BirthDate = model.DOB;
        p.Name = new List<HumanName> { new HumanName().WithGiven(model.FirstName) };
        p.Name[0].Family = model.LastName;
        p.Telecom = new List<ContactPoint>();
        var point = new ContactPoint(ContactPoint.ContactPointSystem.Phone,
            ContactPoint.ContactPointUse.Mobile,FormatPhoneNumber(model.PhoneNumber));
        p.Telecom.Add(point);
        p = fhirClient.Create(p);
        return p.Id.Length > 0 ? true : false;
    }

    public void UpdatePatient(PatientModel model)
    {
        var p = (Patient)fhirClient.Get("Patient" + "/" + model.PatientID);
        p.Name[0].Family = model.LastName;
        p.Name[0].Given = new List<string> { model.FirstName };
        p.Gender = model.Gender.ToString() == "Male" ? AdministrativeGender.Male : AdministrativeGender.Female;
        p.BirthDate = model.DOB;
        p.Telecom = new List<ContactPoint>();
        var point = new ContactPoint(ContactPoint.ContactPointSystem.Phone,
            ContactPoint.ContactPointUse.Mobile, FormatPhoneNumber(model.PhoneNumber));
        p.Telecom.Add(point);
        fhirClient.Update(p);
    }
    
    public static string FormatPhoneNumber(string phone)
    {
        Regex regex = new Regex(@"[^\d]");
        phone = regex.Replace(phone, "");
        phone = Regex.Replace(phone, @"(\d{3})(\d{3})(\d{4})", "$1-$2-$3");
        return phone;
    }


    public Bundle GetPaginatedResources(int page, int pageSize)
    {
        // Dummy data - Replace with actual database or FHIR server query
        var totalRecords = 100; // Assume 100 total resources
        var resources = GetResources(page, pageSize); // Fetch paginated resources

        var bundle = new Bundle
        {
            Type = Bundle.BundleType.Searchset,
            Total = totalRecords,
            Entry = resources.Select(r => new Bundle.EntryComponent { Resource = r }).ToList(),
            Link = new List<Bundle.LinkComponent>
            {
                new() { Relation = "self", Url = $"{serverLocation}?_page={page}&_count={pageSize}" }
            }
        };

        // Add Next Link
        if (page * pageSize < totalRecords)
            bundle.Link.Add(new Bundle.LinkComponent
            {
                Relation = "next",
                Url = $"{serverLocation}?_page={page + 1}&_count={pageSize}"
            });

        // Add Previous Link
        if (page > 1)
            bundle.Link.Add(new Bundle.LinkComponent
            {
                Relation = "previous",
                Url = $"{serverLocation}?_page={page - 1}&_count={pageSize}"
            });

        return bundle;
    }

    private List<Resource> GetResources(int page, int pageSize)
    {
        // Simulated resources - Replace with actual FHIR resource fetching
        return Enumerable.Range(1, pageSize).Select(i => new Patient
        {
            Id = Guid.NewGuid().ToString(),
            Name = new List<HumanName> { new HumanName().WithGiven("John").AndFamily($"Doe {i}") }
        }).Cast<Resource>().ToList();
    }

    public FhirListModel getBundle(string page)
    {
        List<Patient> patients = new List<Patient>();
        var model = new FhirListModel
        {
            isPost = false,
            isError = false,
            isSuccess = false,
            SuccessMessage = null,
            ErrorMessage = null
        };
        var bund = (Bundle)fhirClient.Get(page);
        for (var i = 0; i < bund.Entry.Count; i++) patients.Add((Patient)bund.Entry[i].Resource);

        model.Patients = GetPatientModel(patients);

        if (!string.IsNullOrEmpty(bund.PreviousLink?.ToString())) model.PreviousLink = bund.PreviousLink.ToString();

        if (!string.IsNullOrEmpty(bund.NextLink?.ToString())) model.NextLink = bund.NextLink.ToString();

        return model;
    }


    
    /// <summary>
    /// To refactor
    /// </summary>
    /// <param name="patients"></param>
    /// <returns></returns>
    public static List<PatientModel> GetPatientModel(List<Patient> patients)
    {
        return patients.Select(p => new PatientModel
        {
            PatientID = p.Id,
            FirstName =   p.Name.FirstOrDefault().Given == null ? "" : p.Name.FirstOrDefault().Given.FirstOrDefault()?.ToString(),
            LastName = p.Name.FirstOrDefault().Family == null ? "" : p.Name.FirstOrDefault()?.Family.ToString(),
            Gender = Enum.Parse<Gender>(p.Gender?.ToString()),
            DOB = Convert.ToDateTime(p.BirthDate).ToString("yyyy-MM-dd"),
            PhoneNumber = p.Telecom.FirstOrDefault() == null ? "" : p.Telecom.FirstOrDefault(t => t.System == ContactPoint.ContactPointSystem.Phone)?.Value
        }).ToList();
    }
    
    public FhirListModel getBundle(string searchParam, string searchValue)
    {
        List<Patient> patients = new List<Patient>();
        var model = new FhirListModel
        {
            isPost = false,
            isError = false,
            isSuccess = false,
            SuccessMessage = null,
            ErrorMessage = null
        };
        SearchParams param = null;
        if (searchParam == null)
        {
            searchParam = "f";
            searchValue = "";
        }

        if (searchValue == null)
        {
            searchValue = "";
        }
        if (searchParam.ToString().ToLower() == "p")
        {
            param=  new SearchParams().Add("telecom:contains", searchValue);
        }else if (searchParam.ToString().ToLower() == "f")
        {
            param=  new SearchParams().Add("given:contains", searchValue);
        }else if (searchParam.ToString().ToLower() == "l")
        {
            param = new SearchParams().Add("family:contains", searchValue);

        }
        else
        {
            param = new SearchParams();
        }
        
        var bund = (Bundle)fhirClient.Search(param, "Patient");
        
        for (var i = 0; i < bund.Entry.Count; i++) patients.Add((Patient)bund.Entry[i].Resource);

        model.Patients = GetPatientModel(patients);

        if (!string.IsNullOrEmpty(bund.PreviousLink?.ToString())) model.PreviousLink = bund.PreviousLink.ToString();

        if (!string.IsNullOrEmpty(bund.NextLink?.ToString())) model.NextLink = bund.NextLink.ToString();

        return model;
    }

}
using EHRJs.Models;
using EHRJs.Utils.Fhir;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EHRJs.Controllers;

public class FhirController  : Controller
{
    private readonly ILogger<FhirController> _logger;
    private FHIRUtility _fhirUtility;

    public FhirController()
    {
        _fhirUtility = new FHIRUtility();
        _logger = new Logger<FhirController>(new LoggerFactory());
    }
    
    
    [HttpGet]

    public IActionResult Index()
    {
       List<Patient> patients =  _fhirUtility.GetPatients();
        FhirListModel model = new FhirListModel() { isPost = false, isError = false, isSuccess = false, ErrorMessage = "", SuccessMessage = ""};
        model.Patients = patients.Select(p => new PatientModel()
        {
            PatientID = p.Id,
            FirstName = p.Name.FirstOrDefault().Given.FirstOrDefault().ToString(),
            LastName = p.Name.FirstOrDefault().Family.ToString(),
            Gender =  Enum.Parse<Gender>( p.Gender.Value.ToString()),
            DOB = Convert.ToDateTime(p.BirthDate),
            PhoneNumber = p.Telecom.FirstOrDefault(t => t.System == ContactPoint.ContactPointSystem.Phone).Value
        }).ToList();
        return View(model);
    }

    [HttpPost]

    public IActionResult Index(FhirListModel model)
    {
        return View();
    }

    [HttpPost]
    public IActionResult UpdatePatient(PatientModel model)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public IActionResult AddPatient(PatientModel model)
    { 
        _fhirUtility.AddPatient(model);
        return RedirectToAction("Index");
    }

    public IActionResult DeletePatient(string id)
    {
        _fhirUtility.DeletePatient(id);
        return RedirectToAction("Index");
    }
}
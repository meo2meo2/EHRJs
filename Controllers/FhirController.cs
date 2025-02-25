using System.Web;
using EHRJs.Models;
using EHRJs.Utils.Fhir;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;

namespace EHRJs.Controllers;

public class FhirController : Controller
{
    private readonly ILogger<FhirController> _logger;
    private readonly FHIRUtility _fhirUtility;
    private PageState pageState;

    public FhirController()
    {
        _fhirUtility = new FHIRUtility();
        _logger = new Logger<FhirController>(new LoggerFactory());
    }


    [HttpGet]
    public IActionResult Index(PageState pageState)
    {
        string next = "", before = "";
        List<Patient> patients = _fhirUtility.GetPatients(ref next, ref before);
        var model = new FhirListModel
            { isPost = false, isError = false, isSuccess = false, ErrorMessage = "", SuccessMessage = "" };
        model.Patients = FHIRUtility.GetPatientModel(patients);
        model.PreviousLink = before;
        model.NextLink = next;

        if (pageState == PageState.Add)
        {
            model.isSuccess = true;
            model.SuccessMessage = "Patient Record was added successfully";
        }
        else if (pageState == PageState.Delete)
        {
            model.isSuccess = true;
            model.SuccessMessage = "Patient Record was deleted successfully";
        }
        else if (pageState == PageState.Edit)
        {
            model.isSuccess = true;
            model.SuccessMessage = "Patient Record was updated successfully";
        }

        return View(model);
    }
    
   


    [HttpPost]
    public IActionResult Index(FhirListModel model)
    {
        return View();
    }

    [HttpPost(Name = "update")]
    public IActionResult UpdatePatient(PatientModel model)
    {
        _fhirUtility.UpdatePatient(model);
        return RedirectToAction("Index", PageState.Edit);
    }

    [HttpPost]
    public IActionResult AddPatient(PatientModel model)
    {
        _fhirUtility.AddPatient(model);
        return RedirectToAction("Index", PageState.Add);
    }

    public IActionResult DeletePatient(string id)
    {
        _fhirUtility.DeletePatient(id);
        return RedirectToAction("Index", PageState.Delete);
    }

    public IActionResult PreviousPage(string page)
    {
        var uri = new Uri(page);
        var queryParams = HttpUtility.ParseQueryString(HttpUtility.HtmlDecode(uri.Query));
        var currentOffset = queryParams.Get("_getpagesoffset");
        var offset = Convert.ToInt16(currentOffset) - 20;
        queryParams.Set("_getpagesoffset", offset.ToString());

        var uriBuilder = new UriBuilder(uri);
        uriBuilder.Query = queryParams.ToString();
        var newUri = uriBuilder.Uri;
        var model = _fhirUtility.getBundle(newUri.ToString());
        model.NextLink = page;

        return View("index", model);
    }

    public IActionResult NextPage(string page)
    {
        var uri = new Uri(page);
        var queryParams = HttpUtility.ParseQueryString(HttpUtility.HtmlDecode(uri.Query));
        var currentOffset = queryParams.Get("_getpagesoffset");
        queryParams.Set("_getpagesoffset", (Convert.ToInt16(currentOffset) + 20).ToString());

        var uriBuilder = new UriBuilder(uri);
        uriBuilder.Query = queryParams.ToString();
        var newUri = uriBuilder.Uri;
        var model = _fhirUtility.getBundle(newUri.ToString());
        model.PreviousLink = page;

        return View("index", model);
    }

    public IActionResult SearchPatient(string searchParam, string txtSearch)
    {
        var model = _fhirUtility.getBundle( searchParam , txtSearch);
        return View("index", model);
    }
}
using System.Diagnostics;
using EHRJs.Models;
using EHRJs.Utils;
using Microsoft.AspNetCore.Mvc;

namespace EHRJs.Controllers;

public class HomeController(ILogger<HomeController> logger) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;

    public IActionResult Index(VitalsModel model)
    {
        var builder = new AQLBuilder();
        var modelNew = builder.BuildQuery();
        modelNew.isError = model.isError;
        modelNew.ErrorMessage = model.ErrorMessage;
        modelNew.isSuccess = model.isSuccess;
        modelNew.SuccessMessage = model.SuccessMessage;
        return View(modelNew);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost(Name = "add")]
    public IActionResult PostVitals(VitalsModel model)
    {
        var utility = new CompositionUtility();
        if (!utility.BuildComposition(model))
        {
            model.isError = true;
            model.ErrorMessage = "Error in creating composition";
        }
        else
        {
            model.isSuccess = true;
            model.SuccessMessage = "Composition created successfully";
        }

        return RedirectToAction("Index", model);
    }

    [HttpPost(Name = "update")]
    public IActionResult UpdateVitals(VitalsModel model)
    {
        var utility = new CompositionUtility();
        if (!utility.UpdateComposition(model))
        {
            model.isError = true;
            model.ErrorMessage = "Error in updating composition";
        }
        else
        {
            model.isSuccess = true;
            model.SuccessMessage = "Composition updated successfully";
        }

        return RedirectToAction("Index", model);
    }

    [HttpGet(Name = "delete")]
    public IActionResult DeleteVitals(string id)
    {
        var utility = new CompositionUtility();
        utility.DeleteComposition(id);
        return RedirectToAction("Index", new VitalsModel
        {
            isSuccess = true,
            SuccessMessage = $"Composition with id = {id} deleted successfully",
            isError = false
        });
    }
}
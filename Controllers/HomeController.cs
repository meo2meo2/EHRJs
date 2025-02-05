using System.Diagnostics;
using EHRJs.Models;
using EHRJs.Utils;
using Microsoft.AspNetCore.Mvc;

namespace EHRJs.Controllers;

public class HomeController(ILogger<HomeController> logger) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;

    public IActionResult Index()
    {
        AQLBuilder builder = new AQLBuilder();
        var model = builder.BuildQuery();
        return View(model);
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
        CompositionBuilder builder = new CompositionBuilder();
        if (builder.BuildComposition(model))
        {
            builder.GetCompositionResponse();
            return View("Index", new VitalsListModel(){isPost = true});
        }
        
        return View("Index");
    }
}
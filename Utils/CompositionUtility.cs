using System.Net;
using System.Resources;
using System.Text;
using EHRJs.Models;
using Newtonsoft.Json;
using RestSharp;

namespace EHRJs.Utils;

public class CompositionUtility
{
    private RestClient client;
    private RestClientOptions options;
    public readonly String compositionLocation = "/assets/composition.xml";
    public string TemplateContent { get; private set; }

    public CompositionUtility()
    {
        options = !AppConstants.IsLocal ? new RestClientOptions(AppConstants.BaseUrl) : new RestClientOptions(AppConstants.BaseUrlLocal);
        client = new RestClient(options);
        TemplateContent = "";
    }

    public  void GetCompositionResponse()
    {
        var compositionOptions = AppConstants.IsLocal ?
            AppConstants.OpenEhrBaseCompositionPostLocal.Replace("{{ehrId}}", AppConstants.EhrId) :
            AppConstants.OpenEhrBaseCompositionPost.Replace("{{ehrId}}", AppConstants.EhrId);
        StringBuilder sb = new StringBuilder(compositionOptions);
        var request = new RestRequest(sb.ToString(), Method.Get);
        request.AddHeader("Accept", "application/openehr.wt+json");
        request.AddHeader("Prefer", "return=representation");
        RestResponse response = client.Execute(request);
        TemplateContent = response.Content.ToString();
    }
    
    public bool BuildComposition(VitalsModel model)
    {
        DirectoryInfo info = new DirectoryInfo(Environment.CurrentDirectory);
        String location = info.FullName + compositionLocation;
        StringBuilder template = new StringBuilder( File.ReadAllText(location));
        
        model.Systolic = Convert.ToDouble( model.BloodPressure.Split("/")[0]);
        model.Diastolic = Convert.ToDouble( model.BloodPressure.Split("/")[1]);
        
        template.Replace("{{HEIGHT_VALUE}}", model.Height.ToString());
        template.Replace("{{WEIGHT_VALUE}}", model.Weight.ToString());
        template.Replace("{{SYSTOLIC_VALUE}}", model.Systolic.ToString());
        template.Replace("{{DIASTOLIC_VALUE}}", model.Diastolic.ToString());
        template.Replace("{{SPO_VALUE}}", model.Spo2.ToString());
        template.Replace("{{TEMPLATE_ID}}", AppConstants.TemplateId);
        template.Replace("{{DATE_TIME_VALUE}}", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        var contents = template.ToString();

        var requestParms = AppConstants.IsLocal
            ? AppConstants.OpenEhrBaseCompositionPostLocal.Replace("{{ehrId}}", AppConstants.EhrId)
            : AppConstants.OpenEhrBaseCompositionPost.Replace("{{ehrId}}", AppConstants.EhrId);
        StringBuilder sb = new StringBuilder(requestParms);
        
        var request = new RestRequest(sb.ToString(), Method.Post);
        request.AddHeader("Content-Type", "application/xml");
        request.AddHeader("Prefer", "return=representation");
        request.AddHeader("ehr_id", AppConstants.EhrId);
        request.AddHeader("templateId", AppConstants.TemplateId);
       
        request.AddBody(contents);
        RestResponse response = client.Execute(request);
        TemplateContent = response.Content.ToString();
        return response.StatusCode  == HttpStatusCode.Created;
    }

    public bool DeleteComposition(string id)
    {
        var compositionOptions = AppConstants.IsLocal ?
            AppConstants.OpenEhrBaseCompositionDeleteLocal.Replace("{{ehrId}}", AppConstants.EhrId).Replace("{{composition_id}}", id) :
            AppConstants.OpenEhrBaseCompositionDelete.Replace("{{ehrId}}", AppConstants.EhrId).Replace("{{composition_id}}", id);
        StringBuilder sb = new StringBuilder(compositionOptions);
        var request = new RestRequest(sb.ToString(), Method.Delete);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Prefer", "return=representation");
        RestResponse response = client.Execute(request);
        TemplateContent = response.Content.ToString();
        return true;
    }

    public bool UpdateComposition(VitalsModel model)
    {
        DirectoryInfo info = new DirectoryInfo(Environment.CurrentDirectory);
        String location = info.FullName + compositionLocation;
        StringBuilder template = new StringBuilder( File.ReadAllText(location));
        
        model.Systolic = Convert.ToDouble( model.BloodPressure.Split("/")[0]);
        model.Diastolic = Convert.ToDouble( model.BloodPressure.Split("/")[1]);
        
        template.Replace("{{HEIGHT_VALUE}}", model.Height.ToString());
        template.Replace("{{WEIGHT_VALUE}}", model.Weight.ToString());
        template.Replace("{{SYSTOLIC_VALUE}}", model.Systolic.ToString());
        template.Replace("{{DIASTOLIC_VALUE}}", model.Diastolic.ToString());
        template.Replace("{{SPO_VALUE}}", model.Spo2.ToString());
        template.Replace("{{TEMPLATE_ID}}", AppConstants.TemplateId);
        template.Replace("{{DATE_TIME_VALUE}}", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        var contents = template.ToString();

        var requestParms = AppConstants.IsLocal
            ? AppConstants.OpenEhrBaseCompositionUpdateLocal.Replace("{{ehrId}}", AppConstants.EhrId).Replace("{{composition_id}}", model.Uid.Split(":")[0])
            : AppConstants.OpenEhrBaseCompositionUpdate.Replace("{{ehrId}}", AppConstants.EhrId).Replace("{{composition_id}}", model.Uid.Split(":")[0]);
        StringBuilder sb = new StringBuilder(requestParms);
        
        var request = new RestRequest(sb.ToString(), Method.Put);
        request.AddHeader("Content-Type", "application/xml");
        request.AddHeader("Prefer", "return=representation");
        request.AddHeader("If-Match", model.Uid);
       
        
       
        request.AddBody(contents);
        RestResponse response = client.Execute(request);
        TemplateContent = response.Content.ToString();
        return response.StatusCode  == HttpStatusCode.OK;
    }
}
using System.Net;
using System.Text;
using EHRJs.Models;
using RestSharp;

namespace EHRJs.Utils;

public class CompositionUtility
{
    public readonly string compositionLocation = "/assets/composition.xml";
    private readonly RestClient client;
    private readonly RestClientOptions options;

    public CompositionUtility()
    {
        options = !OpenEhrAppConstants.IsLocal
            ? new RestClientOptions(OpenEhrAppConstants.BaseUrl)
            : new RestClientOptions(OpenEhrAppConstants.BaseUrlLocal);
        client = new RestClient(options);
        TemplateContent = "";
    }

    public string TemplateContent { get; private set; }

    public void GetCompositionResponse()
    {
        var compositionOptions = OpenEhrAppConstants.IsLocal
            ? OpenEhrAppConstants.OpenEhrBaseCompositionPostLocal.Replace("{{ehrId}}", OpenEhrAppConstants.EhrId)
            : OpenEhrAppConstants.OpenEhrBaseCompositionPost.Replace("{{ehrId}}", OpenEhrAppConstants.EhrId);
        var sb = new StringBuilder(compositionOptions);
        var request = new RestRequest(sb.ToString());
        request.AddHeader("Accept", "application/openehr.wt+json");
        request.AddHeader("Prefer", "return=representation");
        var response = client.Execute(request);
        TemplateContent = response.Content;
    }

    public bool BuildComposition(VitalsModel model)
    {
        var info = new DirectoryInfo(Environment.CurrentDirectory);
        var location = info.FullName + compositionLocation;
        var template = new StringBuilder(File.ReadAllText(location));

        model.Systolic = Convert.ToDouble(model.BloodPressure.Split("/")[0]);
        model.Diastolic = Convert.ToDouble(model.BloodPressure.Split("/")[1]);

        template.Replace("{{HEIGHT_VALUE}}", model.Height.ToString());
        template.Replace("{{WEIGHT_VALUE}}", model.Weight.ToString());
        template.Replace("{{SYSTOLIC_VALUE}}", model.Systolic.ToString());
        template.Replace("{{DIASTOLIC_VALUE}}", model.Diastolic.ToString());
        template.Replace("{{SPO_VALUE}}", model.Spo2.ToString());
        template.Replace("{{TEMPLATE_ID}}", OpenEhrAppConstants.TemplateId);
        template.Replace("{{DATE_TIME_VALUE}}", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        var contents = template.ToString();

        var requestParms = OpenEhrAppConstants.IsLocal
            ? OpenEhrAppConstants.OpenEhrBaseCompositionPostLocal.Replace("{{ehrId}}", OpenEhrAppConstants.EhrId)
            : OpenEhrAppConstants.OpenEhrBaseCompositionPost.Replace("{{ehrId}}", OpenEhrAppConstants.EhrId);
        var sb = new StringBuilder(requestParms);

        var request = new RestRequest(sb.ToString(), Method.Post);
        request.AddHeader("Content-Type", "application/xml");
        request.AddHeader("Prefer", "return=representation");
        request.AddHeader("ehr_id", OpenEhrAppConstants.EhrId);
        request.AddHeader("templateId", OpenEhrAppConstants.TemplateId);

        request.AddBody(contents);
        var response = client.Execute(request);
        TemplateContent = response.Content;
        return response.StatusCode == HttpStatusCode.Created;
    }

    public bool DeleteComposition(string id)
    {
        var compositionOptions = OpenEhrAppConstants.IsLocal
            ? OpenEhrAppConstants.OpenEhrBaseCompositionDeleteLocal.Replace("{{ehrId}}", OpenEhrAppConstants.EhrId)
                .Replace("{{composition_id}}", id)
            : OpenEhrAppConstants.OpenEhrBaseCompositionDelete.Replace("{{ehrId}}", OpenEhrAppConstants.EhrId)
                .Replace("{{composition_id}}", id);
        var sb = new StringBuilder(compositionOptions);
        var request = new RestRequest(sb.ToString(), Method.Delete);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Prefer", "return=representation");
        var response = client.Execute(request);
        TemplateContent = response.Content;
        return true;
    }

    public bool UpdateComposition(VitalsModel model)
    {
        var info = new DirectoryInfo(Environment.CurrentDirectory);
        var location = info.FullName + compositionLocation;
        var template = new StringBuilder(File.ReadAllText(location));

        model.Systolic = Convert.ToDouble(model.BloodPressure.Split("/")[0]);
        model.Diastolic = Convert.ToDouble(model.BloodPressure.Split("/")[1]);

        template.Replace("{{HEIGHT_VALUE}}", model.Height.ToString());
        template.Replace("{{WEIGHT_VALUE}}", model.Weight.ToString());
        template.Replace("{{SYSTOLIC_VALUE}}", model.Systolic.ToString());
        template.Replace("{{DIASTOLIC_VALUE}}", model.Diastolic.ToString());
        template.Replace("{{SPO_VALUE}}", model.Spo2.ToString());
        template.Replace("{{TEMPLATE_ID}}", OpenEhrAppConstants.TemplateId);
        template.Replace("{{DATE_TIME_VALUE}}", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        var contents = template.ToString();

        var requestParms = OpenEhrAppConstants.IsLocal
            ? OpenEhrAppConstants.OpenEhrBaseCompositionUpdateLocal.Replace("{{ehrId}}", OpenEhrAppConstants.EhrId)
                .Replace("{{composition_id}}", model.Uid.Split(":")[0])
            : OpenEhrAppConstants.OpenEhrBaseCompositionUpdate.Replace("{{ehrId}}", OpenEhrAppConstants.EhrId)
                .Replace("{{composition_id}}", model.Uid.Split(":")[0]);
        var sb = new StringBuilder(requestParms);

        var request = new RestRequest(sb.ToString(), Method.Put);
        request.AddHeader("Content-Type", "application/xml");
        request.AddHeader("Prefer", "return=representation");
        request.AddHeader("If-Match", model.Uid);


        request.AddBody(contents);
        var response = client.Execute(request);
        TemplateContent = response.Content;
        return response.StatusCode == HttpStatusCode.OK;
    }
}
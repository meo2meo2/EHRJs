using System.Net;
using System.Text;
using EHRJs.Models;
using RestSharp;

namespace EHRJs.Utils;


public class AQLBuilder
{
    private RestClient client;
    private RestClientOptions options;
    public readonly String QueryLocation = "/assets/query.txt";

    public AQLBuilder()
    {
        options = !AppConstants.IsLocal ? new RestClientOptions(AppConstants.BaseUrl) : new RestClientOptions(AppConstants.BaseUrlLocal);
        client = new RestClient(options);
    }
    public VitalsListModel BuildQuery()
    {
        DirectoryInfo info = new DirectoryInfo(Environment.CurrentDirectory);
        String location = info.FullName + QueryLocation;
        StringBuilder template = new StringBuilder( File.ReadAllText(location));
        
        template.Replace("{{TEMPLATE_ID}}", AppConstants.TemplateId);
        template.Replace("{{EHR_ID}}",  AppConstants.EhrId);
        var contents = template.ToString();

        var requestParms = AppConstants.IsLocal
            ? AppConstants.OpenEhrBaseQueryPostLocal.Replace("{{}}", AppConstants.EhrId)
            : AppConstants.OpenEhrBaseQueryPost.Replace("{{}}", AppConstants.EhrId);
        StringBuilder sb = new StringBuilder(requestParms);
        
        var request = new RestRequest(sb.ToString(), Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Prefer", "return=representation");
       
        request.AddBody(contents);
        RestResponse response = client.Execute(request);
        var TemplateContent = response.Content.ToString();
        return new VitalsListModel() { vitals = AQLUtils.GetVitalRecords(TemplateContent), isPost = false};
    }
}
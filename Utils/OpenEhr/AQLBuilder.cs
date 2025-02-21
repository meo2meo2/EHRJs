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
        options = !OpenEhrAppConstants.IsLocal ? new RestClientOptions(OpenEhrAppConstants.BaseUrl) : new RestClientOptions(OpenEhrAppConstants.BaseUrlLocal);
        client = new RestClient(options);
    }
    public VitalsListModel BuildQuery()
    {
        DirectoryInfo info = new DirectoryInfo(Environment.CurrentDirectory);
        String location = info.FullName + QueryLocation;
        StringBuilder template = new StringBuilder( File.ReadAllText(location));
        
        template.Replace("{{TEMPLATE_ID}}", OpenEhrAppConstants.TemplateId);
        template.Replace("{{EHR_ID}}",  OpenEhrAppConstants.EhrId);
        var contents = template.ToString();

        var requestParms = OpenEhrAppConstants.IsLocal
            ? OpenEhrAppConstants.OpenEhrBaseQueryPostLocal.Replace("{{}}", OpenEhrAppConstants.EhrId)
            : OpenEhrAppConstants.OpenEhrBaseQueryPost.Replace("{{}}", OpenEhrAppConstants.EhrId);
        StringBuilder sb = new StringBuilder(requestParms);
        
        var request = new RestRequest(sb.ToString(), Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Prefer", "return=representation");
       
        request.AddBody(contents);
        RestResponse response = client.Execute(request);
        var TemplateContent = response.Content.ToString();
        return new VitalsListModel() { vitals = AQLUtils.GetVitalRecords(TemplateContent), isPost = false, isError = false, isSuccess = false, ErrorMessage = "", SuccessMessage = ""};
    }
}
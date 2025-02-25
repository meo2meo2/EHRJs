using System.Text;
using EHRJs.Models;
using RestSharp;

namespace EHRJs.Utils;

public class AQLBuilder
{
    public readonly string QueryLocation = "/assets/query.txt";
    private readonly RestClient client;
    private readonly RestClientOptions options;

    public AQLBuilder()
    {
        options = !OpenEhrAppConstants.IsLocal
            ? new RestClientOptions(OpenEhrAppConstants.BaseUrl)
            : new RestClientOptions(OpenEhrAppConstants.BaseUrlLocal);
        client = new RestClient(options);
    }

    public VitalsListModel BuildQuery()
    {
        var info = new DirectoryInfo(Environment.CurrentDirectory);
        var location = info.FullName + QueryLocation;
        var template = new StringBuilder(File.ReadAllText(location));

        template.Replace("{{TEMPLATE_ID}}", OpenEhrAppConstants.TemplateId);
        template.Replace("{{EHR_ID}}", OpenEhrAppConstants.EhrId);
        var contents = template.ToString();

        var requestParms = OpenEhrAppConstants.IsLocal
            ? OpenEhrAppConstants.OpenEhrBaseQueryPostLocal.Replace("{{}}", OpenEhrAppConstants.EhrId)
            : OpenEhrAppConstants.OpenEhrBaseQueryPost.Replace("{{}}", OpenEhrAppConstants.EhrId);
        var sb = new StringBuilder(requestParms);

        var request = new RestRequest(sb.ToString(), Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Prefer", "return=representation");

        request.AddBody(contents);
        var response = client.Execute(request);
        var TemplateContent = response.Content;
        return new VitalsListModel
        {
            vitals = AQLUtils.GetVitalRecords(TemplateContent), isPost = false, isError = false, isSuccess = false,
            ErrorMessage = "", SuccessMessage = ""
        };
    }
}
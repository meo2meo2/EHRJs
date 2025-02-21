using System.Resources;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using RestSharp;

namespace EHRJs.Utils;

public class TemplateBuilder
{
    private RestClient client;
    private RestClientOptions options;
    public readonly String templateLocation = "/assets/template.xml";
    public string TemplateContent { get; private set; }

    public TemplateBuilder()
    {
        options = !OpenEhrAppConstants.IsLocal ? new RestClientOptions(OpenEhrAppConstants.BaseUrl) : new RestClientOptions(OpenEhrAppConstants.BaseUrlLocal);
        client = new RestClient(options);
        TemplateContent = "";
    }

    public JsonDocument GetTemplates()
    {
        StringBuilder sb = new StringBuilder(OpenEhrAppConstants.OpenEhrBaseUrl);
        var request = new RestRequest(sb.ToString(), Method.Get);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Prefer", "return=representation");
        RestResponse response = client.Execute(request);

        return JsonDocument.Parse(response.Content.ToString());


    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <param name="templateName"></param>
    /// <returns></returns>
    
    public String BuildTemplate(string content, string templateName = "test-template")
    {
        DirectoryInfo info = new DirectoryInfo(Environment.CurrentDirectory);
        String location = info.FullName + templateLocation;
        String template = File.ReadAllText(location);
        template = template.Replace("{{templateName}}", templateName);
        StringBuilder sb = new StringBuilder(OpenEhrAppConstants.OpenEhrBaseUrl);
        var request = new RestRequest(sb.ToString(), Method.Post);
        request.AddHeader("Content-Type", "application/xml");
        request.AddHeader("Prefer", "return=representation");
        request.AddBody(template);
        RestResponse response = client.Execute(request);
        TemplateContent = response.Content.ToString();
        return content;
    }
}
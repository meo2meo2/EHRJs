using System.Text;
using System.Text.Json;
using RestSharp;

namespace EHRJs.Utils;

public class TemplateBuilder
{
    public readonly string templateLocation = "/assets/template.xml";
    private readonly RestClient client;
    private readonly RestClientOptions options;

    public TemplateBuilder()
    {
        options = !OpenEhrAppConstants.IsLocal
            ? new RestClientOptions(OpenEhrAppConstants.BaseUrl)
            : new RestClientOptions(OpenEhrAppConstants.BaseUrlLocal);
        client = new RestClient(options);
        TemplateContent = "";
    }

    public string TemplateContent { get; private set; }

    public JsonDocument GetTemplates()
    {
        var sb = new StringBuilder(OpenEhrAppConstants.OpenEhrBaseUrl);
        var request = new RestRequest(sb.ToString());
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Prefer", "return=representation");
        var response = client.Execute(request);

        return JsonDocument.Parse(response.Content);
    }

    /// <summary>
    /// </summary>
    /// <param name="content"></param>
    /// <param name="templateName"></param>
    /// <returns></returns>
    public string BuildTemplate(string content, string templateName = "test-template")
    {
        var info = new DirectoryInfo(Environment.CurrentDirectory);
        var location = info.FullName + templateLocation;
        var template = File.ReadAllText(location);
        template = template.Replace("{{templateName}}", templateName);
        var sb = new StringBuilder(OpenEhrAppConstants.OpenEhrBaseUrl);
        var request = new RestRequest(sb.ToString(), Method.Post);
        request.AddHeader("Content-Type", "application/xml");
        request.AddHeader("Prefer", "return=representation");
        request.AddBody(template);
        var response = client.Execute(request);
        TemplateContent = response.Content;
        return content;
    }
}
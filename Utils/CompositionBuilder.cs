using System.Resources;
using System.Text;
using Newtonsoft.Json;
using RestSharp;

namespace EHRJs.Utils;

public class CompositionBuilder
{
    private RestClient client;
    private RestClientOptions options;
    public readonly String compositionLocation = "/assets/composition.json";
    public string TemplateContent { get; private set; }

    public CompositionBuilder()
    {
        options = !AppConstants.IsLocal ? new RestClientOptions(AppConstants.BaseUrl) : new RestClientOptions(AppConstants.BaseUrlLocal);
        client = new RestClient(options);
        TemplateContent = "";
    }

    public  void GetCompositionResponse()
    {
        StringBuilder sb = new StringBuilder(AppConstants.OpenEhrBaseUrl).Append(AppConstants.TemplateId);
        var request = new RestRequest(sb.ToString(), Method.Get);
        request.AddHeader("Accept", "application/openehr.wt+json");
        request.AddHeader("Prefer", "return=representation");
        RestResponse response = client.Execute(request);
        TemplateContent = response.Content.ToString();
    }
    
    public String BuildComposition(string content)
    {
        DirectoryInfo info = new DirectoryInfo(Environment.CurrentDirectory);
        String location = info.FullName + compositionLocation;
        String template = File.ReadAllText(location);
        StringBuilder sb = new StringBuilder(AppConstants.OpenEhrBaseUrl);
        var request = new RestRequest(sb.ToString(), Method.Post);
        request.AddHeader("Content-Type", "application/xml");
        request.AddHeader("Prefer", "return=representation");
        request.AddBody(template);
        RestResponse response = client.Execute(request);
        TemplateContent = response.Content.ToString();
        return content;
    }
}
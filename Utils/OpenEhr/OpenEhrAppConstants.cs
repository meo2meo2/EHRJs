namespace EHRJs.Utils;

public static class OpenEhrAppConstants
{
    public static bool IsLocal = false;
    public static readonly string AppVersion = "1.0.0";
    public static readonly string AppDescription = "Vitals Signs for {%NAME%}";
    public static readonly string AppAuthor = "Mohamed Omar";
    public static readonly string AppAuthorEmail = "momar.95051@gmail.com";
    public static readonly string AppAuthorWebsite = "";
    public static readonly string AppAuthorWebsiteUrl = "";


    public static readonly string TemplateId = "mohamed-omar-2025-02-03.v2";
    public static readonly string SubjectId = "";
    public static readonly string SubjectNamespace = "";
    public static readonly string EhrId = "154ec7af-8dfa-45b9-a29c-dd6f10a862f9";
    public static readonly string EhrUuid = "";

    public static readonly string BaseUrl = @"https://openehr-bootcamp.medblocks.com";
    public static readonly string OpenEhrBaseCompositionPost = @"/ehrbase/rest/openehr/v1/ehr/{{ehrId}}/composition";

    public static readonly string OpenEhrBaseCompositionDelete =
        @"/ehrbase/rest/openehr/v1/ehr/{{ehrId}}/composition/{{composition_id}}";

    public static readonly string OpenEhrBaseCompositionUpdate =
        @"/ehrbase/rest/openehr/v1/ehr/{{ehrId}}/composition/{{composition_id}}";

    public static readonly string OpenEhrBaseUrl = @"/ehrbase/rest/openehr/v1/definition/template/adl1.4/";

    public static readonly string BaseUrlLocal = @"http://localhost:8080";

    public static readonly string OpenEhrBaseCompositionPostLocal =
        @"/ehrbase/rest/openehr/v1/ehr/{{ehrId}}/composition";

    public static readonly string OpenEhrBaseCompositionDeleteLocal =
        @"/ehrbase/rest/openehr/v1/ehr/{{ehrId}}/composition/{{composition_id}}";

    public static readonly string OpenEhrBaseCompositionUpdateLocal =
        @"/ehrbase/rest/openehr/v1/ehr/{{ehrId}}/composition/{{composition_id}}";

    public static readonly string OpenEhrBaseUrlLocal = @"/ehrbase/rest/openehr/v1/definition/template/adl1.4/";

    public static readonly string OpenEhrBaseQueryPostLocal = @"/ehrbase/rest/openehr/v1/query/aql";
    public static readonly string OpenEhrBaseQueryPost = @"/ehrbase/rest/openehr/v1/query/aql";
}
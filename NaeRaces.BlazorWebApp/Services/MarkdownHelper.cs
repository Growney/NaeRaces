using Ganss.Xss;
using Markdig;
using Microsoft.AspNetCore.Components;

namespace NaeRaces.BlazorWebApp.Services;

public static class MarkdownHelper
{
    static MarkdownHelper()
    {
        Sanitizer = new HtmlSanitizer();
        //Add the class attribute back in as we don't use jquery so class jacking isn't an issue
        Sanitizer.AllowedAttributes.Add("class");

    }
    public static MarkdownPipeline DefaultPipeline { get; } = new MarkdownPipelineBuilder().Build();
    public static HtmlSanitizer Sanitizer { get;  }

    public static MarkupString ToHtml(string value) => (MarkupString)Sanitizer.Sanitize(Markdig.Markdown.ToHtml(value, DefaultPipeline));
}

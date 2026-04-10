namespace NaeRaces.WebAPI;

public sealed class OpenIddictClientSettings
{
    public const string SectionName = "OpenIddictClients";

    public string ClientId { get; set; } = string.Empty;
    public string ConsentType { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string ClientType { get; set; } = string.Empty;
    public List<string> PostLogoutRedirectUris { get; set; } = [];
    public List<string> RedirectUris { get; set; } = [];
    public List<string> Permissions { get; set; } = [];
    public List<string> Requirements { get; set; } = [];
}

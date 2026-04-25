namespace backend.Common;

public static class Config
{
    public const int TokenValidMins = 30;
    public const int CookieExpirationHours = 7 * 24;
    public static bool CookieSecure { get; private set; } = true;
    public static SameSiteMode CookieSameSite { get; private set; } = SameSiteMode.None; // Sajnos localhoston ez kell,
                                                                                         // hogy minden bongeszoben
                                                                                         // mukodjon
    public static string SupportEmail { get; private set; } = "comove@comove.app";

    public static void LoadFromConfiguration(IConfiguration config)
    {
         CookieSecure = (config["Auth:Cookie:Secure"]?.ToLower() ?? "true") == "true";
         CookieSameSite = Enum.Parse<SameSiteMode>(config["Auth:Cookie:SameSite"] ?? "None");
    }
}
namespace backend.Common;

public static class Config
{
    public const int TokenValidMins = 30;
    public const int CookieExpirationHours = 7 * 24;
    public const bool CookieSecure = true;
    public const SameSiteMode CookieSameSite = SameSiteMode.None; // Sajnos localhoston ez kell,
                                                                  // hogy minden bongeszoben mukodjon
    public const string SupportEmail = "comove@comove.app";
}
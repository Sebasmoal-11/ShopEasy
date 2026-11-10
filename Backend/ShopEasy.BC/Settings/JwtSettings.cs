namespace ShopEasy.BC.Settings;

public class JwtSettings
{
    public string Key { get; set; } = string.Empty;

    public string Issuer { get; set; } = "ShopEasy.API";

    public string Audience { get; set; } = "ShopEasy.Frontend";

    public int ExpiresMinutes { get; set; } = 60;
}

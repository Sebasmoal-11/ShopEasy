namespace ShopEasy.BW.Auth;

public record LoginResponse(
    string Token,
    string TokenType,
    DateTime ExpiresAtUtc,
    CurrentUserDto User);

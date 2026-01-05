namespace CoursePlatform.Application.DTOs.Auth;

public record RegisterRequest(
    string Email,
    string Password
);

public record LoginRequest(
    string Email,
    string Password
);

public record AuthResponse(
    string Token,
    string RefreshToken,
    string Email
);

public record RefreshTokenRequest(
    string Token,
    string RefreshToken
);

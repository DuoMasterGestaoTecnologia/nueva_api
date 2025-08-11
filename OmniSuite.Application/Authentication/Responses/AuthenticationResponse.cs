public record AuthenticationResponse(
    string Token, 
    string RefreshToken, 
    string Name, 
    string Email, 
    DateTime expireDate,
    string? ProfilePhoto
);
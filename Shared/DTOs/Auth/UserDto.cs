﻿using BlogApi.Shared.DTOs.Auth;

namespace BlogApi.Shared.DTOs.Auth;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public JwtDto Tokens { get; set; }
}
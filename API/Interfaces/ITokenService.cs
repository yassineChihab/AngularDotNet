﻿using API.Entities;

namespace API.Interfaces
{
    public interface ITokenService
    {
        string GetToken(AppUser user);
    }
}

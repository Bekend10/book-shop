﻿using book_shop.Models;

namespace book_shop.Services.Interfaces
{
    public interface IJWTService
    {
        public string GenerateJwtToken(Account account);
        public string GenerateRefreshToken();
    }
}

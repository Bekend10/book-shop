﻿using book_shop.Data;
using book_shop.Dto;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace book_shop.Repositories.Implementations
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Account> GetAccountByEmailAsync(string email)
        {
            return await _context.Accounts.FirstOrDefaultAsync(u => u.email == email);
        }

        public async Task AddUserWithAccountAsync(User user, Account account)
        {
            user.Account = account;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _context.Accounts
                .Select(_ => new Account
                {
                    account_id = _.account_id,
                    email = _.email,
                    password = _.password,
                    refresh_token = _.refresh_token,
                    refresh_token_ext = _.refresh_token_ext,
                    user_id = _.user_id,
                    role_id = _.role_id,
                    is_verify = _.is_verify,
                    is_active = _.is_active,
                    last_active = _.last_active,
                })
                .ToListAsync();
        }

        public async Task<Account> GetByIdAsync(int id)
        {
            return await _context.Accounts.FirstOrDefaultAsync(x => x.account_id == id);

        }

        public async Task AddAsync(Account entity)
        {
            await _context.Accounts.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Account entity)
        {
            _context.Accounts.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Account> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Accounts.FirstOrDefaultAsync(u => u.refresh_token == refreshToken);
        }        
    }
}

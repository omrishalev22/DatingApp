using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data {
    public class AuthRepository : IAuthRepository
    {
        DataContext _context;
        public AuthRepository(DataContext context ){
            this._context = context;
        }
         
        public async Task<User> Login(string username, string password)
        {
            var isUser = UserExists(username);
            var user = await this._context.Users.FirstOrDefaultAsync(_user => _user.Username == username);
            if(isUser == null) {
                return null;
            }

            if(VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)){
                return user;
            }

            return null;

        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)){
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                    for(int i = 0 ; i < computedHash.Length ; i++){
                        if(computedHash[i] != passwordHash[i]){
                            return false;
                        }
                    }
            }
             
            return true;
            
            
        }

        public async Task<User> Register(User user, string password)
        {
         byte[] passwordHash , passwordSalt;
         CreatePasswordHash(password, out passwordHash,out passwordSalt); // out passes ref to the function.
         user.PasswordHash = passwordHash;
         user.PasswordSalt = passwordSalt;
         await this._context.Users.AddAsync(user);
         await this._context.SaveChangesAsync();

         return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512()) {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            
        }

        public async Task<bool> UserExists(string username)
        {
            if(await this._context.Users.AnyAsync(x=>x.Username == username))
                return true;

            return false; 
        }
    }
}
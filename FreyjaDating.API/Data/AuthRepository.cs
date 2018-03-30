using System;
using System.Threading.Tasks;
using FreyjaDating.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FreyjaDating.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        // Compares the username and password to what's stored in the database
        public async Task<User> Login(string username, string password)
        {
            // return the first user in the database that matches the username.
            // else it returs NULL
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            // returns NUll if no user are found.
            if (user == null)
            {
                return null;
            }
            // Verifies if the hashed version of the password the user passed in, matches
            // the hashed password stored in the database
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            // auth successfull
            return user;
        }

        // Register user with a hashed and salted password
        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            // Hashes and salts the password
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            // Assign password hash and salt to the user objects propertie fields
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            // Adds and saves the newely registed user to the database
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Returns the user object
            return user;
        }
        #region Helpers
        // Checks if the user exists
        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(x => x.Username == username))
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                // Compare the computehash with what is stored in the database
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;

        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // Hashes the password with SHA512
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                // Assigns the hmac.Key to the salt
                passwordSalt = hmac.Key;
                // Computes the hash
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        #endregion
    }
}

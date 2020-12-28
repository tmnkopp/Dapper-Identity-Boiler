using DIB.Models;
using Dapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DIB.Identity
{
    public class UserStore : IUserEmailStore<ApplicationUser>
        , IUserStore<ApplicationUser> 
        , IUserPasswordStore<ApplicationUser>
        , IUserRoleStore<ApplicationUser>
        , IDisposable 
    {
        private readonly IDbConnection _dbConnection;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

        public UserStore(IDbConnection dbConnection, IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _dbConnection = dbConnection;
            _passwordHasher = passwordHasher;
        }

        public async Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            using (var conn = _dbConnection.GetConnection)
            {
                var query = $"INSERT INTO  AspNetUserRoles (UserId, RoleId) VALUES " +
                    $"( @UserId, (SELECT TOP 1 ID From AspNetRoles WHERE Name = @roleName))" ;
                var param = new DynamicParameters();
                param.Add("@UserId", user.Id);
                param.Add("@roleName", roleName);
                var result = await conn.ExecuteAsync(query, param: param, commandType: CommandType.Text);
            }
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            using (var conn = _dbConnection.GetConnection)
            {
                var query = $"INSERT INTO [AspNetUsers](" +
                $" [Id],[UserName],[NormalizedUserName],[Email],[NormalizedEmail],[EmailConfirmed]," +
                $" [PasswordHash],[SecurityStamp],[ConcurrencyStamp],[PhoneNumber],[PhoneNumberConfirmed]," +
                $" [TwoFactorEnabled],[LockoutEnd],[LockoutEnabled],[AccessFailedCount]" +
                $") VALUES ( " +
                $" @Id,@UserName,@NormalizedUserName,@Email,@NormalizedEmail,@EmailConfirmed," + 
                $" @PasswordHash,@SecurityStamp,@ConcurrencyStamp,@PhoneNumber,@PhoneNumberConfirmed," +
                $" @TwoFactorEnabled,@LockoutEnd,@LockoutEnabled,@AccessFailedCount)";
                var param = new DynamicParameters();
                param.Add("@Id", user.Id);
                param.Add("@UserName", user.UserName);
                param.Add("@NormalizedUserName", user.NormalizedUserName);
                param.Add("@Email", user.Email);
                param.Add("@NormalizedEmail", string.IsNullOrEmpty(user.Email) ? null : user.Email.ToUpper());
                param.Add("@EmailConfirmed", user.EmailConfirmed);
                param.Add("@PasswordHash", user.PasswordHash);
                param.Add("@SecurityStamp", user.SecurityStamp);
                param.Add("@ConcurrencyStamp", user.ConcurrencyStamp);
                param.Add("@PhoneNumber", user.PhoneNumber);
                param.Add("@PhoneNumberConfirmed", user.PhoneNumberConfirmed);
                param.Add("@TwoFactorEnabled", user.TwoFactorEnabled);
                param.Add("@LockoutEnd", user.LockoutEnd);
                param.Add("@LockoutEnabled", user.LockoutEnabled);
                param.Add("@AccessFailedCount", user.AccessFailedCount);
                var result = await conn.ExecuteAsync(query, param: param, commandType: CommandType.Text );

                if (result > 0)
                    return IdentityResult.Success;
                else
                    return IdentityResult.Failed(new IdentityError() { Code = "120", Description = "Cannot Create User!" });
            }
        }
  
        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            using (var conn = _dbConnection.GetConnection)
            {
                var query = $"DELETE FROM [AspNetUsers] WHERE [Id] = @Id";
                var param = new DynamicParameters();
                param.Add("@Id", user.Id);

                var result = await conn.ExecuteAsync(query, param: param, commandType: CommandType.Text);

                if (result > 0)
                    return IdentityResult.Success;
                else
                    return IdentityResult.Failed(new IdentityError() { Code = "120", Description = "Cannot Update User!" });
            }
        }
        

        public void Dispose()
        {
            //Dispose();
        }

        public async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            using (var conn = _dbConnection.GetConnection)
            {
                var query = $"SELECT * FROM [AspNetUsers] WHERE [normalizedEmail] = @normalizedEmail";
                var param = new DynamicParameters();
                param.Add("@normalizedEmail", normalizedEmail.ToUpper());
                return await conn.QueryFirstOrDefaultAsync<ApplicationUser>(query, param: param, commandType: CommandType.Text);
            }
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            using (var conn = _dbConnection.GetConnection)
            {
                var query = $"SELECT * FROM [AspNetUsers] WHERE [Id] = @Id";
                var param = new DynamicParameters();
                param.Add("@Id", userId);
                return await conn.QueryFirstOrDefaultAsync<ApplicationUser>(query, param: param, commandType: CommandType.Text);
            }
        } 
        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            using (var conn = _dbConnection.GetConnection)
            {
                var query = $"SELECT * FROM [AspNetUsers] WHERE [NormalizedUserName] = @normalizedUserName";
                var param = new DynamicParameters();
                param.Add("@normalizedUserName", normalizedUserName);
                return await conn.QueryFirstOrDefaultAsync<ApplicationUser>(query, param: param, commandType: CommandType.Text);
            } 
        } 
        public async Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await Task.Run(() => user.Email.ToString());
        }

        public async Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await Task.Run(() => user.EmailConfirmed == true);
        }

        public async Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await Task.Run(() => user.Email.ToUpper());
        }

        public async Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await Task.Run(() => user.UserName.ToUpper());
        }

        public async Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await Task.Run(() => user.PasswordHash);
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            using (var conn = _dbConnection.GetConnection)
            {
                var query = @"SELECT AspNetRoles.Name FROM [AspNetUsers] 
                INNER JOIN AspNetUserRoles ON AspNetUsers.Id = AspNetUserRoles.UserId
                INNER JOIN AspNetRoles ON AspNetRoles.Id = AspNetUserRoles.RoleId
                WHERE AspNetUsers.Id = @UserId";
                var param = new DynamicParameters();
                param.Add("@UserId", user.Id);
                return await conn.QueryFirstOrDefaultAsync<IList<string>>(query, param: param, commandType: CommandType.Text);
            }
        } 
        public async Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await Task.Run(() => user.Id.ToString());
        }

        public async Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await Task.Run(() => user.UserName); 
        }

        public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            using (var conn = _dbConnection.GetConnection)
            {
                var query = @"SELECT AspNetUsers.* FROM [AspNetUsers] 
                INNER JOIN AspNetUserRoles ON AspNetUsers.Id = AspNetUserRoles.UserId
                INNER JOIN AspNetRoles ON AspNetRoles.Id = AspNetUserRoles.RoleId
                WHERE AspNetRoles.Name = @roleName";
                var param = new DynamicParameters();
                param.Add("@roleName", roleName);
                return await conn.QueryFirstOrDefaultAsync<IList<ApplicationUser>>(query, param: param, commandType: CommandType.Text); 
            }
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            using (var conn = _dbConnection.GetConnection)
            {
                var query = @"SELECT COUNT(*) FROM [AspNetUserRoles] 
                INNER JOIN AspNetRoles ON AspNetRoles.Id = AspNetUserRoles.RoleId
                WHERE [UserId] = @UserId";
                var param = new DynamicParameters();
                param.Add("@UserId", user.Id); 
                var result = await conn.QueryFirstOrDefaultAsync<int>(query, param: param, commandType: CommandType.Text);
                return result > 0 ;
            }
        }

        public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            using (var conn = _dbConnection.GetConnection)
            {
                var query = $" DELETE FROM AspNetUserRoles " +
                    $" WHERE AspNetUserRoles.UserId = @userID " +
                    $" AND AspNetUserRoles.RoleID IN " +
                    $" (SELECT ID FROM AspNetRoles WHERE Name = @RoleName) ";
                var param = new DynamicParameters();
                param.Add("@userID", user.Id);
                param.Add("@RoleName", roleName);
                var result = await conn.ExecuteAsync(query, param: param, commandType: CommandType.Text); 
            }
        }

        public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask; 
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.NormalizedUserName = normalizedName;
            return Task.FromResult<object>(null);
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.PasswordHash = passwordHash;
            return Task.FromResult<object>(null);
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.UserName = userName;
            return Task.FromResult<object>(null);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            using (var conn = _dbConnection.GetConnection)
            {
                var query = $"UPDATE [AspNetUsers]" +
                    $"SET" +
                    $"[PasswordHash] = @PasswordHash," +
                    $"[SecurityStamp] = @SecurityStamp," +
                    $"[ConcurrencyStamp] = @ConcurrencyStamp," +
                    $"[TwoFactorEnabled] = @TwoFactorEnabled," +
                    $"[LockoutEnd] = @LockoutEnd," +
                    $"[LockoutEnabled] = @LockoutEnabled," +
                    $"[AccessFailedCount] = @AccessFailedCount " +
                    $"WHERE [Id] = @Id";
                var param = new DynamicParameters();
                param.Add("@Id", user.Id);
                param.Add("@PasswordHash", user.PasswordHash);
                param.Add("@SecurityStamp", user.SecurityStamp);
                param.Add("@ConcurrencyStamp", user.ConcurrencyStamp);
                param.Add("@PhoneNumber", user.PhoneNumber);
                param.Add("@PhoneNumberConfirmed", user.PhoneNumberConfirmed);
                param.Add("@TwoFactorEnabled", user.TwoFactorEnabled);
                param.Add("@LockoutEnd", user.LockoutEnd);
                param.Add("@LockoutEnabled", user.LockoutEnabled);
                param.Add("@AccessFailedCount", user.AccessFailedCount);

                var result = await conn.ExecuteAsync(query, param: param, commandType: CommandType.Text);

                if (result > 0)
                    return IdentityResult.Success;
                else
                    return IdentityResult.Failed(new IdentityError() { Code = "120", Description = "Cannot Update User!" });
            }
        }
    }
}

using DIB.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using System.Data;

namespace DIB.Identity
{
    public class RoleStore : IRoleStore<ApplicationUserRole>, IDisposable
    {
        private readonly IDbConnection _dbConnection;
        public RoleStore(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<IdentityResult> CreateAsync(ApplicationUserRole role, CancellationToken cancellationToken)
        {
            using (var conn = _dbConnection.GetConnection)
            {
                if (role.Id == null) 
                    role.Id = new Guid().ToString();
               
                var query = $"INSERT INTO [AspNetRoles]( [Id] , [Name] )" +
                $" VALUES ( @Id , @Name )";
                var param = new DynamicParameters();
                param.Add("@Id", role.Id);
                param.Add("@Name", role.Name); 
                var result = await conn.ExecuteAsync(query, param: param, commandType: CommandType.Text);

                if (result > 0)
                    return IdentityResult.Success;
                else
                    return IdentityResult.Failed(new IdentityError() { Code = "120", Description = "Cannot Create Role" });
            }
        }
 
        public async Task<IdentityResult> DeleteAsync(ApplicationUserRole role, CancellationToken cancellationToken)
        {
            using (var conn = _dbConnection.GetConnection)
            {
                var query = $"DELETE FROM [AspNetRoles] WHERE [Id] = @Id";
                var param = new DynamicParameters();
                param.Add("@Id", role.Id); 
                var result = await conn.ExecuteAsync(query, param: param, commandType: CommandType.Text); 
                if (result > 0)
                    return IdentityResult.Success;
                else
                    return IdentityResult.Failed(new IdentityError() { Code = "120", Description = "Cannot Update Role" });
            }
        }

        public void Dispose()
        {
            //Dispose();
        }

        public async Task<ApplicationUserRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            using (var conn = _dbConnection.GetConnection)
            {
                var query = $"SELECT * FROM [AspNetRoles] WHERE [Id] = @Id";
                var param = new DynamicParameters();
                param.Add("@Id", roleId);
                return await conn.QueryFirstOrDefaultAsync<ApplicationUserRole>(query, param: param, commandType: CommandType.Text);
            }
        }

        public async Task<ApplicationUserRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            using (var conn = _dbConnection.GetConnection)
            {
                var query = $"SELECT * FROM [AspNetRoles] WHERE [Name] = @normalizedRoleName";
                var param = new DynamicParameters();
                param.Add("@normalizedRoleName", normalizedRoleName.ToUpper());
                try
                {
                    return await conn.QueryFirstOrDefaultAsync<ApplicationUserRole>(query, param: param, commandType: CommandType.Text);
                } 
                catch (Exception ex)
                {
                    throw ex;
                }  
            }
        }

        public async Task<string> GetNormalizedRoleNameAsync(ApplicationUserRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));
            return await Task.Run(() => role.Name.ToUpper());
        }

        public async Task<string> GetRoleIdAsync(ApplicationUserRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));
            return await Task.Run(() => role.Id.ToUpper());
        }

        public async Task<string> GetRoleNameAsync(ApplicationUserRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));
            return await Task.Run(() => role.Name);
        }

        public Task SetNormalizedRoleNameAsync(ApplicationUserRole role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(ApplicationUserRole role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUserRole role, CancellationToken cancellationToken)
        {
            using (var conn = _dbConnection.GetConnection)
            {
                var query = $"UPDATE [AspNetRoles] " +
                    $"SET " +
                    $"[Name] = @Name " + 
                    $"WHERE [Id] = @Id ";
                var param = new DynamicParameters();
                param.Add("@Id", role.Id);
                param.Add("@Name", role.Name);  
                var result = await conn.ExecuteAsync(query, param: param, commandType: CommandType.Text); 
                if (result > 0)
                    return IdentityResult.Success;
                else
                    return IdentityResult.Failed(new IdentityError() { Code = "120", Description = "Cannot Update Role" });
            }
        }
    }
}

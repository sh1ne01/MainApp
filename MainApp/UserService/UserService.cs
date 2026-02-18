using Dapper;
using Microsoft.Data.SqlClient;
using BCrypt.Net;
using MainApp;

public class UserService
{
    private readonly string connectionString;

    public UserService(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection");                              
    }

    public async Task AddUser(string login, string password)
    {
        using SqlConnection sqlConnection = new SqlConnection(connectionString);

        string query = "INSERT INTO dbo.Allusers (Login, PasswordHash) VALUES (@Login, @PasswordHash);";

        string hashed = BCrypt.Net.BCrypt.HashPassword(password);

        await sqlConnection.ExecuteAsync(query, new
        {
            Login = login,
            PasswordHash = hashed
        });
    }

    public async Task<bool> CheckPassword(string login, string password)
    {
        using SqlConnection sqlConnection = new SqlConnection(connectionString);

        string query = "SELECT PasswordHash FROM dbo.Allusers WHERE Login = @Login";

        string storedHash = await sqlConnection.QueryFirstOrDefaultAsync<string>(
            query,
            new { Login = login });

        if (storedHash == null)
            return false;

        return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }

    public async Task<string?> GetUserRole(string login)
    {
        using SqlConnection sqlConnection = new SqlConnection(connectionString);

        string query = "SELECT Role FROM dbo.Allusers WHERE Login = @Login;";

        return await sqlConnection.QueryFirstOrDefaultAsync<string>(
            query,
            new { Login = login });
    }
    public async Task<bool> IsUserBanned(string login)
    {
        using SqlConnection sqlConnection = new SqlConnection(connectionString);

        string query = "SELECT IsBanned FROM dbo.Allusers WHERE Login = @Login;";

        return await sqlConnection.QueryFirstOrDefaultAsync<bool>(
            query,
            new { Login = login });
    }
    public async Task<IEnumerable<User>> GetAllUsers()
    {
        using SqlConnection sqlConnection = new SqlConnection(connectionString);

        string query = "SELECT Login, Role, IsBanned FROM dbo.Allusers";

        return await sqlConnection.QueryAsync<User>(query);
    }

    public async Task ToggleBan(string login)
    {
        using SqlConnection sqlConnection = new SqlConnection(connectionString);

        string query = @"
        UPDATE dbo.Allusers
        SET IsBanned = CASE WHEN IsBanned = 1 THEN 0 ELSE 1 END
        WHERE Login = @Login";

        await sqlConnection.ExecuteAsync(query, new { Login = login });
    }
    public async Task TogglePrem(string login)
    {
        using SqlConnection sqlConnection = new SqlConnection(connectionString);

        string query = @"
        UPDATE dbo.Allusers
        SET Role = CASE WHEN Role = 'Premium' THEN 'User' ELSE 'Premium' END
        WHERE Login = @Login"; ;

        await sqlConnection.ExecuteAsync(query, new { Login = login});
    }
    public async Task<User> GetUserByLogin(string login)
    {
        using SqlConnection sqlConnection = new SqlConnection(connectionString);

        string query = @"SELECT Login, Role, IsBanned, AvatarPath
                 FROM dbo.Allusers
                 WHERE Login = @Login";


        return await sqlConnection.QueryFirstOrDefaultAsync<User>(
            query,
            new { Login = login });
    }
    public async Task UpdatePassword(string login, string newPassword)
    {
        using SqlConnection sqlConnection = new SqlConnection(connectionString);

        string newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

        string query = @"UPDATE dbo.Allusers
                     SET PasswordHash = @PasswordHash
                     WHERE Login = @Login";

        await sqlConnection.ExecuteAsync(query,
            new { Login = login, PasswordHash = newHash });
    }
    public async Task UpdateAvatar(string login, string avatarPath)
    {
        using SqlConnection sqlConnection = new SqlConnection(connectionString);

        string query = @"UPDATE dbo.Allusers
                     SET AvatarPath = @AvatarPath
                     WHERE Login = @Login";

        await sqlConnection.ExecuteAsync(query,
            new { Login = login, AvatarPath = avatarPath });
    }
    public async Task AddPremium(string login)
    {
        using SqlConnection sqlConnection = new SqlConnection(connectionString);

        string query = @"UPDATE dbo.Allusers
                 SET Role = @Role
                 WHERE Login = @Login";

        await sqlConnection.ExecuteAsync(query,
            new { Login = login,Role = "Premium"});
    }


}
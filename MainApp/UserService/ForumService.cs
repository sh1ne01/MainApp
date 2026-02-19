using Dapper;
using Microsoft.Data.SqlClient;
using MainApp.Models;
using MainApp;
using Microsoft.AspNetCore.Mvc;

public class ForumService
{
    private readonly string connectionString;

    public ForumService(IConfiguration config)
    {
        connectionString =
            config.GetConnectionString("DefaultConnection");
    }

    public async Task<List<ForumPost>> GetPosts()
    {
        using SqlConnection sql = new(connectionString);

        string query =
            "SELECT * FROM ForumPosts ORDER BY CreatedAt DESC";

        var posts = await sql.QueryAsync<ForumPost>(query);

        return posts.ToList();
    }
    public async Task<string?> GetUserAvatar(string login)
    {
        using SqlConnection sqlConnection = new SqlConnection(connectionString);

        string query = @"SELECT AvatarPath
                 FROM dbo.Allusers
                 WHERE Login = @Login";

        return await sqlConnection.QueryFirstOrDefaultAsync<string>(
            query,
            new { Login = login });

    }

    public async Task AddPost(string title, string content, string login,string role,string avatar)
    {
        using SqlConnection sql = new(connectionString);

        string query = @"
        INSERT INTO ForumPosts
        (Title, Content, CreatedAt, UserLogin, Role, AvatarPath)
        VALUES
        (@Title, @Content, GETDATE(), @UserLogin, @Role ,@Avatar)";

        await sql.ExecuteAsync(query, new
        {
            Title = title,
            Content = content,
            UserLogin = login,
            Role = role,
            Avatar = avatar
        }); ; ;
    }
}

using BlogApi.Models;
using BlogApi.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace BlogApi.Controllers
{
    [Route("blogposts")]
    [ApiController]
    public class BlogPostController : ControllerBase
    {
        public string ConnectionString = "server=localhost;database=blog13B;uid=root;password=";

        [HttpGet("all")]
        public object GetAllBlogPosts()
        {
            List<BlogPost> posts = new List<BlogPost>();

            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"SELECT * FROM `blogpost`";

            var cmd = new MySqlCommand(sql, connector);

            var datareader = cmd.ExecuteReader();

            while (datareader.Read())
            {
                var post = new BlogPost
                {
                    Id = datareader.GetInt32(0),
                    Title = datareader.GetString(1),
                    Content = datareader.GetString(2),
                    BloggerId = datareader.GetInt32(3)
                };

                posts.Add(post);
            }

            connector.Close();

            return new { message = "Sikeres lekérdezés.", result = posts };
        }

        [HttpGet("byId")]
        public object GetBlogPostById([FromQuery] int id)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"SELECT * FROM `blogpost` WHERE `id` = @id;";

            var cmd = new MySqlCommand(sql, connector);
            cmd.Parameters.AddWithValue("@id", id);

            var datareader = cmd.ExecuteReader();

            datareader.Read();

            var post = new BlogPost
            {
                Id = datareader.GetInt32(0),
                Title = datareader.GetString(1),
                Content = datareader.GetString(2),
                BloggerId = datareader.GetInt32(3)
            };

            connector.Close();
            return new { message = "Sikeres találat.", result = post };
        }

        [HttpPost("add")]
        public object AddNewBlogPost(AddNewBlogPostDto addNewBlogPostDto)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"INSERT INTO `blogpost`(`title`, `content`, `bloggerId`) VALUES (@title,@content,@bloggerId)";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@title", addNewBlogPostDto.Title);
            cmd.Parameters.AddWithValue("@content", addNewBlogPostDto.Content);
            cmd.Parameters.AddWithValue("@bloggerId", addNewBlogPostDto.BloggerId);

            cmd.ExecuteNonQuery();

            connector.Close();

            return new { message = "Sikeres hozzáadás.", result = addNewBlogPostDto };
        }

        [HttpPut("update")]
        public object UpdateBlogPost([FromQuery] int id, [FromBody] UpdateBlogPostDto updateBlogPostDto)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"UPDATE `blogpost` SET `title`=@title,`content`=@content,`bloggerId`=@bloggerId 
               WHERE `id`= @id;";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@title", updateBlogPostDto.Title);
            cmd.Parameters.AddWithValue("@content", updateBlogPostDto.Content);
            cmd.Parameters.AddWithValue("@bloggerId", updateBlogPostDto.BloggerId);
            cmd.Parameters.AddWithValue("@id", id);

            object result = cmd.ExecuteNonQuery() > 0 ? new { message = "Sikeres frissítés." } : new { message = "Nincs ilyen bejegyzés." };

            connector.Close();

            return result;
        }

        [HttpDelete("deleteById")]
        public object DeleteBlogPost([FromBody] int id)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"DELETE FROM `blogpost` WHERE `id` = @id";

            var cmd = new MySqlCommand(sql, connector);
            cmd.Parameters.AddWithValue("@id", id);

            object result = cmd.ExecuteNonQuery() > 0 ? new { message = "Sikeres törlés." } : new { message = "Nincs ilyen bejegyzés." };

            connector.Close();

            return result;
        }

        [HttpGet("bloggerNameEmail")]
        public object GetBloggerNameAndEmail([FromQuery] int id)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"SELECT `name`, `email` FROM `blogger` WHERE `id` = @id;";

            var cmd = new MySqlCommand(sql, connector);
            cmd.Parameters.AddWithValue("@id", id);

            var datareader = cmd.ExecuteReader();

            datareader.Read();

            var result = new
            {
                Name = datareader.GetString(0),
                Email = datareader.GetString(1)
            };

            connector.Close();
            return new { message = "Sikeres lekérdezés.", result = result };
        }

        [HttpGet("bloggerWithPosts")]
        public object GetBloggerWithPosts([FromQuery] int id)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"SELECT `name` FROM `blogger` WHERE `id` = @id;";

            var cmd = new MySqlCommand(sql, connector);
            cmd.Parameters.AddWithValue("@id", id);

            var datareader = cmd.ExecuteReader();

            datareader.Read();

            string name = datareader.GetString(0);

            datareader.Close();

            string sql2 = @"SELECT `title`, `content` FROM `blogpost` WHERE `bloggerId` = @id;";

            var cmd2 = new MySqlCommand(sql2, connector);
            cmd2.Parameters.AddWithValue("@id", id);

            var datareader2 = cmd2.ExecuteReader();

            List<object> posts = new List<object>();

            while (datareader2.Read())
            {
                posts.Add(new
                {
                    Title = datareader2.GetString(0),
                    Content = datareader2.GetString(1)
                });
            }

            connector.Close();

            return new { message = "Sikeres lekérdezés.", result = new { Name = name, Posts = posts } };
        }

        [HttpGet("totalCount")]
        public object GetTotalPostCount()
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"SELECT COUNT(*) FROM `blogpost`";

            var cmd = new MySqlCommand(sql, connector);

            var datareader = cmd.ExecuteReader();

            datareader.Read();

            int count = datareader.GetInt32(0);

            connector.Close();

            return new { message = "Sikeres lekérdezés.", result = count };
        }

        [HttpGet("countByBlogger")]
        public object GetPostCountByBlogger([FromQuery] int id)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"SELECT COUNT(*) FROM `blogpost` WHERE `bloggerId` = @id;";

            var cmd = new MySqlCommand(sql, connector);
            cmd.Parameters.AddWithValue("@id", id);

            var datareader = cmd.ExecuteReader();

            datareader.Read();

            int count = datareader.GetInt32(0);

            connector.Close();

            return new { message = "Sikeres lekérdezés.", result = count };
        }
    }
}

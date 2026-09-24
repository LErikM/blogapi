using BlogApi.Models;
using BlogApi.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace BlogApi.Controllers
{
    [Route("bloggers")]
    [ApiController]
    public class BloggerController : ControllerBase
    {
        public string ConnectionString = "server=localhost;database=blog13B;uid=root;password=";

        [HttpGet("all")]
        public object GetAllBlogger() 
        {
            List<Blogger> bloggers = new List<Blogger>();

            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"SELECT * FROM `blogger`";

            var cmd = new MySqlCommand(sql,connector);

            var datareader = cmd.ExecuteReader();

            while (datareader.Read()) 
            {
                var blogger = new Blogger
                {
                    Id = datareader.GetInt32(0),
                    Name = datareader.GetString(1),
                    Email = datareader.GetString(2),
                    Age = datareader.GetInt32(3),
                    Password = datareader.GetString(4),
                    RegistrationTime = datareader.GetDateTime(5)
                };

                bloggers.Add(blogger);
            }

            connector.Close();

            return new { message = "Sikeres lekérdezés.", result = bloggers};
        }

        [HttpGet("byId")]
        public object GetBloggerById([FromQuery]int id) 
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"SELECT * FROM `blogger` WHERE `id` = @id;";

            var cmd = new MySqlCommand(sql, connector);
            cmd.Parameters.AddWithValue("@id", id);

            var datareader = cmd.ExecuteReader();

            datareader.Read();

            var blogger = new Blogger
            {
                Id = datareader.GetInt32(0),
                Name = datareader.GetString(1),
                Email = datareader.GetString(2),
                Age = datareader.GetInt32(3),
                Password = datareader.GetString(4),
                RegistrationTime = datareader.GetDateTime(5)
            };

            connector.Close();
            return new { message = "Sikeres találat.", result = blogger};
        }

        [HttpPost("register")]
        public object AddNewBlogger(AddNewBloggerDto addNewBloggerDto)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"INSERT INTO `blogger`(`name`, `email`, `age`, `password`, `RegistrationTime`) VALUES (@name,@email,@age,@password,@registrationTime)";

            var cmd = new MySqlCommand(sql,connector);

            cmd.Parameters.AddWithValue("@name", addNewBloggerDto.Name);
            cmd.Parameters.AddWithValue("@email", addNewBloggerDto.Email);
            cmd.Parameters.AddWithValue("@age", addNewBloggerDto.Age);
            cmd.Parameters.AddWithValue("@password", addNewBloggerDto.Password);
            cmd.Parameters.AddWithValue("@registrationTime", DateTime.Now);

            cmd.ExecuteNonQuery();

            connector.Close();

            return new { message = "Sikeres hozzáadás.", result = addNewBloggerDto };
        }

        [HttpPost("login")]
        public object LoginBlogger([FromBody]LoginBloggerDto loginBloggerDto)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"SELECT `id` FROM `blogger` 
                            WHERE `email` = @email AND `password`= @password;";

            var cmd = new MySqlCommand(sql, connector);
            cmd.Parameters.AddWithValue("@email", loginBloggerDto.Email);
            cmd.Parameters.AddWithValue("@password", loginBloggerDto.Password);

            var datareader = cmd.ExecuteReader();


            if (datareader.Read() == true)
            {
                return new { message = "Sikeres belépés.", result = datareader.GetInt32("id") };
            }
            else
            {
                return new { message = "Sikertelen belépés.", result = loginBloggerDto };
            }
            
        }

        [HttpDelete("deleteById")]
        public object DeleteBlogger([FromBody]int id)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"DELETE FROM `blogger` WHERE `id` = @id";

            var cmd = new MySqlCommand(sql, connector);
            cmd.Parameters.AddWithValue("@id", id);
            
            object result = cmd.ExecuteNonQuery() > 0 ? new { message = "Sikeres törlés." } : new { message = "Nincs ilyen felhasználó." };

            connector.Close();

            return result;
          
        }

        [HttpPut("update")]
        public object UpateBlogger([FromQuery]int id, [FromBody]UpdateBloggerDto updateBloggerDto)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"UPDATE `blogger` SET                                                 `name`=@name,`email`=@email,`age`=@age,`password`=@password 
               WHERE `id`= @id;";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@name", updateBloggerDto.Name);
            cmd.Parameters.AddWithValue("@email", updateBloggerDto.Email);
            cmd.Parameters.AddWithValue("@age", updateBloggerDto.Age);
            cmd.Parameters.AddWithValue("@password", updateBloggerDto.Password);
            cmd.Parameters.AddWithValue("@id", id);

            object result = cmd.ExecuteNonQuery() > 0 ? new { message = "Sikeres frissítés." } : new { message = "Nincs ilyen felhasználó." };

            connector.Close();

            return result;

           
        }
    }
}

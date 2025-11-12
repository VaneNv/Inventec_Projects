using Microsoft.AspNetCore.Mvc; //Por default en la creacion del controller (Model-View-Controller)
using System.Security.Cryptography; //Encriptacion de informacion
using StorageCore.Models.Entity; //Solicitamos informacion para el Login
using StorageCore.Models.DTO; //Solicitamos informacion de los usuarios
using System.Data.SQLite; //Nos permite la manipulacion de informacion de la base de datos
using StorageCore.Utils;
using System.Text;
using System;

namespace StorageCore.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    public class LogingController1 : ControllerBase
    {
        private readonly JwtHelper _jwtHelper;
        private readonly string _dbName = "Storage.sqlite";

        public LoginController(JwtHelper jwtHelper)
        {
            _jwtHelper = jwtHelper;
        }

        [HttpPost] //Indica que hare una accion especifica despues de obtener informacion
        public IActionResult Login([FromBody] LoginRequest request) //dicha informacion la voy a obtener especificamente de la clase 'LoginReques'
        {
            //Primero validamos que no este vacia la informacion solicitada
            if (string.IsNullOrEmpty(request.Username) || (string.IsNullOrEmpty(request.Password))) {
                return BadRequest("User and Password can not be empty");
            }

            try
            {
                //Creamos conexion a la base de datos 
                using (var conn = new SQLiteConnection($"Data Source = {_dbName}; Version = 3"))
                {
                    conn.Open();

                    //Solicitamos informacion del usuario en la BD con el usuario ingresado
                    var user =  GetUserByUser(conn,request.Username);
                    if (user == null)
                    {
                        return Unauthorized("Username does not exist");
                    }

                    //Solicitamos el Rol del usuario
                    if (user.Status != 1)
                    {
                        return Unauthorized("Account is disabled");
                    }

                    //Verificacion de la Contrasenia
                    if (!VerifyPassword(request.Password, user.PasswordH))
                    {
                        return Unauthorized("INCORRECT password");
                    }

                    //Obtenemos el rol 
                    var role = GetUserRole(conn, user.Id_employee);
                    if (string.IsNullOrEmpty(role))
                    {
                        return Unauthorized("User has no assigned role");
                    }

                    //Actualizamos ultima fecha de ingreso
                    UpdateLastLoginTime(conn, user.Id_employee);

                    //Generacion del Token

                    //Return de la infromacion en caso de todo ser correcto 
                    return Ok(new
                    {
                        Success = true,
                        Token = token,
                        UserName = user.UserName,
                        Name = user.Name,
                        Role = role,
                        ExpiresIn = 3600
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Login failed: {ex.Message}");
            }
            
        }

        //Funcion para obtener la informacion del usuario 
        private UsersInfo GetUserByUser(SQLiteConnection conn, string UserName)
        {
            //Consulta que quiero hacer a la BD,
            //obtendre: informamcion del usuario, pero solo si coniciden los usuarios (consulta y bd)
            string sql = "SELECT Id_employee, UserName, PasswordH, Name, Status FROM Employee Where UserName = @UserName";

            //SQLite va a ejecutar la consulta 'sql' 
            //conn es la conexion entre la BD
            using (var cmd = new SQLiteCommand (sql,conn)) //objeto que utilizara la ejecucion del SQL usando cnn 
            {
                //se sustituye @UserName por el valor del parametro UserName que recibido en la funcion
                cmd.Parameters.AddWithValue("@UserName", UserName); 

                using (var reader = cmd.ExecuteReader()) //Ejecuta consulta y devuelve un lector de datos
                {
                    if (reader.Read()) //Si el parametro regresado es true, significa que enconto informacion
                    {
                        return new UsersInfo //Aqui se van a copiar los datos almacenados en la BD
                        {
                            Id_employee = reader.GetString(0),
                            UserName = reader.GetString(1),
                            PasswordH = reader.GetString (2),
                            Name = reader.GetString(3),
                            Status = reader.GetInt32 (4)
                        };
                    }
                }
            }
            return null; //En caso de que no coincida la informacion con con la BD
        }

        //Obtenemos el Rol del Usuario 
        private string GetUserRole(SQLiteConnection conn, string UserId)
        {
            //Buscamos el rol del usuario mediante una union de tablas con el Id del usuario
            string sql = @"
                    SELECT r.RoleName
                    FROM Roles r
                    INNER JOIN UserRoles ur ON r.Id = ur.RoleId
                    WHERE ur.UserID = @UserId
                    LIMIT 1";

            //conn es la conexion entre la BD
            using (var cmd = new SQLiteCommand (sql, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", UserId); //Asigno el UserId a @UserId
                var roleName = cmd.ExecuteScalar() as string; //Convierte el objeto a un string
                return roleName; //devuelve el rol encontrado del usuario
            }
        }

        //Actualizamos la ultima fecha de Login
        private void UpdateLastLoginTime(SQLiteConnection conn, string Id_employee)
        {
            //Consulta que se le hara para actualizar la fecha
            string sql = "UPDATE Employees SET LastLoginTime = datetime('now','localtime') WHERE Id_employee = @Id_employee";

            using (var cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id_employee", Id_employee);
                cmd.ExecuteNonQuery();
            }
        }

        //Verfica contrasenia
        private bool VerifyPassword(string inputPassword,string storedHash)
        {
            using (var sha256 = SHA256.Create())
            {
                var inputHash = Convert.ToBase64String(
                    sha256.ComputeHash(Encoding.UTF8.GetBytes(storedHash))
                    );
                return inputHash == storedHash;
            }
        }
    }
}

using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CabasusAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuarioController : Controller
    {
        [HttpPut("actualizar")]
        public bool actualizar([FromBody] JObject data)
        {
            var id_usuario = User.FindFirst("id")?.Value;
            Modelos.usuarios datos = data.ToObject<Modelos.usuarios>();
            Conexion c = new Conexion();
            return c.Insertar("update usuarios set nombre='" + datos.nombre + "',contrasena='" + HashSHA1(datos.contrasena) + "', fecha_nacimiento= '" + datos.fecha_nacimiento + "' where id_usuario='" + id_usuario + "'");
        }
        [HttpGet("consultar")]
        public DataTable consultar()
        {
            var id_usuario = User.FindFirst("id")?.Value;
            Conexion c = new Conexion();
            return c.Consultar("select * from usuarios where id_usuario='" + id_usuario + "'");
        }
        [HttpGet("actualizarFoto")]
        public bool actualizarFoto(string URL)
        {
            var id_usuario = User.FindFirst("id")?.Value;
            Conexion c = new Conexion();
            return c.Insertar("update usuarios set foto='" + URL + "' where id_usuario='" + id_usuario + "'");
        }

        [HttpGet("actualizarTokenFB")]
        public bool actualizarTokenFB(string tokenFB, string id_dispositivo)
        {
            var id_usuario = User.FindFirst("id")?.Value;
            Conexion c = new Conexion();
            return c.Insertar("update dispositivos set tonken_fb='" + tokenFB + "', fk_usuario='" + id_usuario + "' where id_dispositivo='" + id_dispositivo + "'");
        }

        public static string HashSHA1(string value)
        {
            var sha1 = SHA1.Create();
            var inputBytes = Encoding.ASCII.GetBytes(value);
            var hash = sha1.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}

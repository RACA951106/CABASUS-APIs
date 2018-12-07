using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using CabasusAPIs.Modelos;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace CabasusAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class AccountController : Controller
    {
        //Hola
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] JObject data)
        {
            var dat = data.ToObject<login>();
            IActionResult request = BuildToken(dat.usuario, dat.contrasena);
            var serializarjson = JsonConvert.SerializeObject(request);
            var obtenerjson = JsonConvert.DeserializeObject<RootObject>(serializarjson);
            var id_usuario = TestJwtSecurityTokenHandler(obtenerjson.Value.token);
            if (request==BadRequest())
            {
                return BadRequest();
            }
            else
            {
                if (!new Conexion().Insertar("insert into dispositivos values('" + dat.id_dispositivo + "','" + dat.SO + "','" + id_usuario + "','" + dat.TokenFB + "')"))
                {
                    new Conexion().Insertar("update dispositivos set token_fb='" + dat.TokenFB + "' where id_dispositivo='" + dat.id_dispositivo + "'");
                }
                return request;
            }
                
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> registrar([FromBody] JObject data)
        {
            usuarios datos = data.ToObject<usuarios>();
            Conexion c = new Conexion();
            bool insertar = false;
            bool idConseguido = true;
            string id_generado = "";

            //consultar para comprobar si el usuario ya existe
            var con = new Conexion();
            var consulta = con.Consultar("SELECT * FROM usuarios WHERE email='" + datos.email + "'");

            if (consulta.Rows.Count < 1)
            {
                while (idConseguido)
                {
                    Guid guid = Guid.NewGuid();
                    id_generado = guid.ToString().Replace("-", "");
                    id_generado = id_generado.Substring(0, 30);
                    insertar = c.Insertar("insert into usuarios values ('" + id_generado + "','" + datos.nombre + "','" + datos.email + "','" + HashSHA1(datos.contrasena) + "','" + datos.foto + "','" + datos.fecha_nacimiento + "');");
                    if (insertar)
                    {
                        idConseguido = false;
                    }
                }
                if (insertar)
                {
                    if (!new Conexion().Insertar("insert into dispositivos values('" + datos.id_dispositivo + "','" + datos.SO + "','" + id_generado + "','" + datos.tokenFB + "')"))
                    {
                        new Conexion().Insertar("update dispositivos set token_fb='" + datos.tokenFB + "' where id_dispositivo='" + datos.id_dispositivo + "'");
                    }

                    return BuildToken(datos.email, datos.contrasena);

                }
                    return BadRequest();
            }
                return BadRequest("el usuario ya existe");

        }

        private IActionResult BuildToken(string usuario, string contrasena)
        {
            var con = new Conexion();

            var contra = HashSHA1(contrasena);
            var consulta = con.Consultar("SELECT * FROM usuarios WHERE email='" + usuario + "'");
            try
            {
                if (consulta.Rows[0]["contrasena"].ToString() == contra)
                {
                    var claims = new[]
                    {
                    new Claim(JwtRegisteredClaimNames.UniqueName,usuario),
                    new Claim("id",consulta.Rows[0][0].ToString())
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ZReyjIu5uOidd1HveXtqZReyjIu5uOidd1HveXtqZReyjIu5uOidd1HveXtq"));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var expiration = DateTime.UtcNow.AddHours(24);

                    JwtSecurityToken token = new JwtSecurityToken
                        (
                            issuer: "dominio.com",
                            audience: "dominio.com",
                            claims: claims,
                            expires: expiration,
                            signingCredentials: creds
                        );

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = expiration
                    });
                }
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


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

        public string TestJwtSecurityTokenHandler(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            var tokenS = handler.ReadToken(token) as JwtSecurityToken;
            var jti = tokenS.Claims.First(claim => claim.Type == "id").Value;
            return jti;
        }
    }
}
 
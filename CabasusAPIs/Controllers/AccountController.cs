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
using System.Net.Mail;
using System.Net;

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
                return BadRequest("usuario mal o contra mal");
            }
            else
            {
                if (!new Conexion().Insertar("insert into dispositivos values('" + dat.id_dispositivo + "','" + dat.SO + "','" + id_usuario + "','" + dat.TokenFB + "')"))
                {
                    new Conexion().Insertar("update dispositivos set tonken_fb='" + dat.TokenFB + "', fk_usuario='" + id_usuario + "' where id_dispositivo='" + dat.id_dispositivo + "'");
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
                    insertar = c.Insertar("insert into usuarios values ('" + id_generado + "','" + datos.nombre + "','" + datos.email + "','" + HashSHA1(datos.contrasena) + "','" + datos.foto + "','" + datos.fecha_nacimiento + "','"+datos.descripcion+"','"+datos.telefono+"');");
                    if (insertar)
                    {
                        idConseguido = false;
                    }
                }
                if (insertar)
                {
                    if (!new Conexion().Insertar("insert into dispositivos values('" + datos.id_dispositivo + "','" + datos.SO + "','" + id_generado + "','" + datos.tokenFB + "')"))
                    {
                        new Conexion().Insertar("update dispositivos set tonken_fb='" + datos.tokenFB + "', fk_usuario='" + id_generado + "' where id_dispositivo='" + datos.id_dispositivo + "'");
                    }

                    return BuildToken(datos.email, datos.contrasena);

                }
                    return BadRequest();
            }
                return BadRequest("el usuario ya existe");

        }

        [HttpGet("recuperarPass")]
        public async Task<bool> recuperarPass(string email, int idioma)
        {
            var smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com", // set your SMTP server name here
                Port = 587, // Port 
                EnableSsl = true,
                Credentials = new NetworkCredential("devteam@cabasus.com", "Admin1234.")
            };

            Conexion c = new Conexion();
            var consulta = c.Consultar("SELECT * FROM usuarios WHERE email='" + email + "'");

            if (consulta.Rows.Count >= 1)
            {
                Guid guid = Guid.NewGuid();
                var pass = guid.ToString().Replace("-", "");
                pass = pass.Substring(0, 10);

                var body = "";
                var subject = "";

                switch (idioma)
                {
                    case 1:
                        subject = "Recuperar Contraseña";
                        body = "hola " + consulta.Rows[0]["nombre"] + ", para ingresar de nuevo a tu cuenta en CABASUS por favor utilizar la siguiente" +
                        " contraseña y despues cambiala en los ajustes de tu cuenta :) \r\n \r\n " + pass + " \r\n \r\n Atte: CABASUS Team México " +
                        "\r\n \r\n \r\n \r\n \r\n \r\n \r\n \r\n \r\n \r\n (Por favor no contestes este mensaje)";
                        break;
                }

                using (var message = new MailMessage("devteam@cabasus.com", email)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    await smtpClient.SendMailAsync(message);
                    return c.Insertar("UPDATE usuarios SET contrasena= '" + HashSHA1(pass) + "' WHERE email='" + email + "'");
                }
            }
            return false;
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
                        expiration = expiration,
                        id_usuario = consulta.Rows[0]["id_usuario"].ToString(),
                        email = consulta.Rows[0]["email"].ToString(),
                        fecha_nacimiento = consulta.Rows[0]["fecha_nacimiento"].ToString(),
                        nombre = consulta.Rows[0]["nombre"].ToString(),
                        foto = consulta.Rows[0]["foto"].ToString(),
                        descripcion= consulta.Rows[0]["descripcion"].ToString(),
                        telefono= consulta.Rows[0]["telefono"].ToString(),
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
 
using System;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CabasusAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : Controller
    {
        [HttpPost("registrar")]
        public string registrar([FromBody] JObject data)
        {
            Modelos.Post datos = data.ToObject<Modelos.Post>();
            Conexion c = new Conexion();

            bool id_concedido = true;
            Guid guid = Guid.NewGuid();
            var id_generado = "";
            while (id_concedido)
            {
                id_generado = guid.ToString().Replace("-", "");
                id_generado = id_generado.Substring(0, 30);
                if (c.Insertar("insert into post values ('"+id_generado+"', '" + datos.contenido + "', " + datos.privacidad + ", '" + datos.fk_usuario + "', '" + datos.fk_caballo + "', '" + datos.fecha + "',0);"))
                {
                    id_concedido = false;
                }
            }
            foreach (var item in datos.lista_enlaces)
            {
                c.Insertar("insert into multimedia values(null,'" + item + "','" + id_generado + "')");
            }
           
            return id_generado;
        }

        [HttpDelete("eliminar/{id_post}")]
        public bool eliminar(string id_post)
        {
            Conexion c = new Conexion();
            return c.Insertar("delete from post where id_post = '" + id_post + "';");
        }

        [HttpPut("actualizar")]
        public bool actualizar([FromBody] JObject data)
        {
            Modelos.Post datos = data.ToObject<Modelos.Post>();
            Conexion c = new Conexion();
            return c.Insertar("update post set contenido = '"+ datos.contenido + "', privacidad = "+ datos.privacidad + "  where id_post = '"+datos.id_post +"';");
        }

        [HttpGet("consultar_id/{id_post}")]
        public DataTable consultar(string id_post)
        {
            Conexion c = new Conexion();
            return c.Consultar("select * from post where id_post = '" + id_post+"';");
        }

        [HttpGet("consultar_id/{id_caballo}")]
        public DataTable consultartodos(string id_caballo)
        {
            Conexion c = new Conexion();
            return c.Consultar("select * from diario where fk_caballo = '" + id_caballo + "';");
        }

        [HttpGet("consultar_id/{id_caballo}")]
        public DataTable consultarpublicos(string id_caballo)
        {
            Conexion c = new Conexion();
            return c.Consultar("select * from diario where fk_caballo = '" + id_caballo + "' and privacidad=1;");
        }

        [HttpPost("quitarlike/{id_post}")]
        public bool quitarlike(string id_post)
        {
            Conexion c =new Conexion();
            var id_usuario = User.FindFirst("id")?.Value;

                var cuenta = c.Consultar("select likes_contable from post where id_post='" + id_post + "'");
                var cuentatotal = int.Parse(cuenta.Rows[0][0].ToString());
            if (cuentatotal <= 1)
            {
                cuentatotal--;
                c.Insertar("update post set likes_contable=" + cuentatotal);
                c.Insertar("delete from likes where fk_usuario='" + id_usuario + "' and fk_post='"+id_post+"'");
                return true;
            }
            else
                return false;

                // c.Insertar("update post set likes_contable=likes_contables+1");
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CabasusAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CaballoController : Controller
    {
        [HttpPost("registrar")]
        public string registrar([FromBody] JObject data)
        {
            var id_usuario = User.FindFirst("id")?.Value;
            Modelos.caballos datos = data.ToObject<Modelos.caballos>();
            Conexion c = new Conexion();
            bool idConseguido = true;
            string id_generado = "";
            while (idConseguido)
            {
                Guid guid = Guid.NewGuid();
                id_generado = guid.ToString().Replace("-", "");
                id_generado = id_generado.Substring(0, 30);
                if (c.Insertar("insert into caballos values('" + id_generado + "', '" + datos.nombre + "'," + datos.peso + "," + datos.altura + ", " + datos.raza + ",'" + datos.fecha_nacimiento + "'," + datos.genero + ",'" + datos.foto + "', '" + id_usuario + "'," + datos.avena + ");"))
                {
                    idConseguido = false;
                }
            }
            return id_generado;
        }
        [HttpPost("actualizarFoto")]
        public bool actualizarFoto([FromBody] JObject data)
        {
            Modelos.caballos datos = data.ToObject<Modelos.caballos>();
            Conexion c = new Conexion();
            return c.Insertar("update caballos set foto='"+datos.foto+"' where id_caballo ='"+datos.id_caballo+"';");
        }
        [HttpPut("actualizar")]
        public bool actualizar([FromBody] JObject data)
        {
            Modelos.caballos datos = data.ToObject<Modelos.caballos>();
            Conexion c = new Conexion();
            return c.Insertar("update caballos set nombre='" + datos.nombre + "', peso=" + datos.peso + ", altura=" + datos.altura + ", raza=" + datos.raza + ",fecha_nacimiento='" + datos.fecha_nacimiento + "', genero=" + datos.genero + ",foto='" + datos.foto + "', avena="+datos.avena+" where id_caballo='" + datos.id_caballo + "' ");
        }
        [HttpGet("consultaridcaballo/{id_caballo}")]
        public DataTable consultaridcaballo(string id_caballo)
        {
            Conexion c = new Conexion();
            return c.Consultar("select * from caballos where id_caballo='" + id_caballo + "'");

        }
        [HttpGet("consultaridusuario")]
        public DataTable consultaridusuario()
        {
            var id_usuario = User.FindFirst("id")?.Value;

            Conexion c = new Conexion();
            return c.Consultar("select * from caballos where fk_usuario='" + id_usuario + "'");

        }
        [HttpDelete("eliminar/{id_caballo}")]
        public bool eliminar(int id_caballo)
        {
            Conexion c = new Conexion();
            return c.Insertar("delete from caballos where id_caballo='"+id_caballo+"'");

        }
    }
}

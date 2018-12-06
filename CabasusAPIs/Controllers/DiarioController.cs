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
    public class DiarioController : Controller
    {
        [HttpPost("registrar")]
        public string registrar([FromBody] JObject data)
        {
            Modelos.diarios datos = data.ToObject<Modelos.diarios>();
            Conexion c = new Conexion();

            bool id_concedido = true;
            Guid guid = Guid.NewGuid();
            var id_generado = "";
            while (id_concedido)
            {
                id_generado = guid.ToString().Replace("-", "");
                id_generado = id_generado.Substring(0, 30);
                if (c.Insertar("insert into diario values ('"+id_generado+"', '" + datos.mensaje + "', " + datos.privacidad + ", '" + datos.fk_usuario + "', '" + datos.fk_caballo + "', '" + datos.fecha + "');"))
                {
                    id_concedido = false;
                }
            }
            return id_generado;
        }

        [HttpDelete("eliminar/{id_diario}")]
        public bool eliminar(string id_diario)
        {
            Conexion c = new Conexion();
            return c.Insertar("delete from diario where id_diario = '" + id_diario + "';");
        }

        [HttpPut("actualizar")]
        public bool actualizar([FromBody] JObject data)
        {
            Modelos.diarios datos = data.ToObject<Modelos.diarios>();
            Conexion c = new Conexion();
            return c.Insertar("update diario set mensaje = '"+ datos.mensaje + "', privacidad = "+ datos.privacidad + ", fecha = '"+ datos.fecha + "' where id_diario = '"+datos.id_diario +"';");
        }

        [HttpGet("consultar_id/{id_diario}")]
        public DataTable consultar(string id_diario)
        {
            Conexion c = new Conexion();
            return c.Consultar("select * from diario where id_diario = '" + id_diario+"';");
        }
        
        [HttpGet("consultar_caballo_usuario")]
        public DataTable consultar_rangos([FromBody] JObject data)
        {
            Modelos.diariosRango datos = data.ToObject<Modelos.diariosRango>();
            Conexion c = new Conexion();
            return c.Consultar("call diarios_rango_fechas('"+datos.fk_usuario+"', '"+datos.fk_caballo+"', '"+datos.fecha_inicio+"', '"+datos.fecha_fin+"');");
        }

        [HttpGet("consultar_privados")]
        public DataTable consultar_privados([FromBody] JObject data)
        {
            Modelos.diariosRango datos = data.ToObject<Modelos.diariosRango>();
            Conexion c = new Conexion();
            return c.Consultar("call diarios_privados('" + datos.fk_usuario + "', '" + datos.fk_caballo + "', '" + datos.fecha_inicio + "', '" + datos.fecha_fin + "');");
        }

        [HttpGet("consultar_prublicos")]
        public DataTable consultar_prublicos([FromBody] JObject data)
        {
            Modelos.diariosRango datos = data.ToObject<Modelos.diariosRango>();
            Conexion c = new Conexion();
            return c.Consultar("call diarios_publicos('" + datos.fk_caballo + "', '" + datos.fecha_inicio + "', '" + datos.fecha_fin + "');");
        }
    }
}

using System.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CabasusAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActividadController : Controller
    {
        [HttpPost("registrar")]
        public bool registrar([FromBody] JObject data)
        {
            Modelos.actividades datos = data.ToObject<Modelos.actividades>();
            Conexion c = new Conexion();
            return c.Insertar("insert into actividades values (null, '"+datos.fk_usuario+"', '"+ datos.fk_caballo+ "', '"+ datos.fecha + "', "+ datos.duracion + ", '"+ datos.intensidad + "', "+ datos.camina +", "+ datos.trota + ", "+ datos.galopa +", '"+ datos.latitudes +"', '"+datos.longitudes+"', "+datos.factor_fitness+","+datos.tipo_actividad+");");
        }

        [HttpGet("consultar")]
        public DataTable consultar(string fk_caballo, string fecha)
        {
            Conexion c = new Conexion();
            return c.Consultar("select * from actividades where fk_caballo = '"+fk_caballo+"' && fecha = '"+fecha+"';");
        }

        [HttpDelete("eliminar/{id_actividad}")]
        public bool eliminar(int id_actividad)
        {
            Conexion c = new Conexion();
            return c.Insertar("delete from actividades where id_actividad = "+id_actividad+";");
        }

        [HttpGet("consultar_rangos")]
        public DataTable consultar_rangos([FromBody] JObject data)
        {
            Modelos.actividadesRango datos = data.ToObject<Modelos.actividadesRango>();
            Conexion c = new Conexion();
            return c.Consultar("call actividades_rango_fechas('"+ datos.fk_usuario + "', '"+ datos.fk_caballo + "', '"+ datos.fecha_inicio + "', '"+ datos.fecha_fin + "');");
        }
    }
}

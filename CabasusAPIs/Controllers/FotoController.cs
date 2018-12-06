using System.Data;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CabasusAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FotoController : Controller
    {
        [HttpPost("registrar")]
        public bool registrar([FromBody] JObject data)
        {
            Modelos.fotos datos = data.ToObject<Modelos.fotos>();
            Conexion c = new Conexion();
            return c.Insertar("insert into foto_diario values('"+datos.foto+"', '"+datos.fk_diario+"');");
        }

        [HttpDelete("eliminar_idfoto")]
        public bool eliminar_idfoto([FromBody] JObject data)
        {
            Modelos.fotos datos = data.ToObject<Modelos.fotos>();
            Conexion c = new Conexion();
            return c.Insertar("delete from foto_diario where foto = '"+datos.foto+"';");
        }

        [HttpDelete("eliminar_fkdiario/{fk_diario}")]
        public bool eliminar_fkdiario(string fk_diario)
        {
            Conexion c = new Conexion();
            return c.Insertar("delete from foto_diario where fk_diario = '" + fk_diario + "';");
        }

        [HttpPut("actualizar")]
        public bool actualizar([FromBody] JObject data)
        {
            Modelos.updateRul datos = data.ToObject<Modelos.updateRul>();
            Conexion c = new Conexion();
            return c.Insertar("update foto_diario set foto = '"+datos.urlNueva+"' where foto = '"+datos.urlAntigua+"';");
        }

        [HttpGet("consultar/{fk_diario}")]
        public DataTable consultar(string fk_diario)
        {
            Conexion c = new Conexion();
            return c.Consultar("select * from foto_diario where fk_diario = '" + fk_diario + "';");
        }

        //Acortar url
        public string acortarurl()
        {
            return CompressURL("https://www.google.com.mx/maps/dir/21.364713,-101.8925403/17+de+Enero,+Pueblo+de+Moya,+47430+Lagos+de+Moreno,+Jal./@21.3650784,-101.9143775,2928m/data=!3m1!1e3!4m10!4m9!1m1!4e1!1m5!1m1!1s0x842bd244166867cf:0x1eb28e93ccaa5013!2m2!1d-101.9187051!2d21.3652124!3e2");
        }

        public static string CompressURL(string strURL)
        {
            var objRequest = System.Net.WebRequest.Create("http://tinyurl.com/api-create.php?url=" + strURL);
            string strResponse = null;

            try
            {
                System.Net.HttpWebResponse objResponse = objRequest.GetResponse() as System.Net.HttpWebResponse;
                StreamReader stmReader = new StreamReader(objResponse.GetResponseStream());

                strResponse = stmReader.ReadToEnd();
            }
            catch { }
            return strResponse;
        }
    }
}

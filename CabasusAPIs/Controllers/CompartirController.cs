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
    public class CompartirController : Controller
    {
        [HttpPost("registrar/{id_caballo}")]
        public bool registrar( string id_caballo)
        {
            var id_usuario = User.FindFirst("id")?.Value;
            Conexion c = new Conexion();
            return c.Insertar("insert into compartidos values ('"+id_usuario+"','"+id_caballo+"');");
        }
        [HttpDelete("eliminarcompartidos/{id_caballo}")]
        public bool eliminarcompartidos(int id_caballo)
        {
            var id_usuario = User.FindFirst("id")?.Value;
            Conexion c = new Conexion();
            return c.Insertar("delete from compartidos where fk_usuario='" + id_usuario + "' and fk_caballo='"+id_caballo+"'");
        }
        [HttpGet ("consultarcompartidos")]
        public DataTable consultarcompartidos()
        {
            var id_usuario = User.FindFirst("id")?.Value;
            Conexion c = new Conexion();
            return c.Consultar("call caballos_usuario('"+id_usuario+"')");
        }
    }
}

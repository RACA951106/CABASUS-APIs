using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CabasusAPIs.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    public class ChatController : Controller
    {
        Conexion c = new Conexion();

        [HttpGet("Miembros")]
        public List<object> ConsultarMiembros(string id_caballo)
        {

            List<ProcedureParameters> para = new List<ProcedureParameters>();
            para.Add(new ProcedureParameters { parametro = "in_horse", valor = id_caballo });

            List<string> campos = new List<string>();
            campos.Add("id_usuario");
            campos.Add("nombre");

            return c.Procedimiento("miembros_chat", para, campos);
        }

        [HttpGet("ElimininarMensaje")]
        public bool EliminarMensaje(string id_mensaje)
        {
            return c.Insertar("delete from mensajes_en_espera where id_mensajes = '" + id_mensaje + "'");
        }

        [HttpGet("EnviarMensaje")]
        public bool EnviarMensaje(string id_caballo, string mensaje)
        {
            var id_usuario = User.FindFirst("id")?.Value;
            var Miembros = ConsultarMiembros(id_caballo);
            var respuesta = false;
            var idConseguido = true;

            mensaje = mensaje.Replace("'", "\'");

            string token = "";

            foreach (Dictionary<object,object> row in Miembros)
            {
                respuesta = false;

                object miembro = "";

                row.TryGetValue("id_usuario", out miembro);

                if (miembro.ToString() != id_usuario)
                {
                    //buscar los dispositivos del usuario 
                    var dispositivos = c.Consultar("select * from dispositivos where fk_usuario = '" + miembro + "'");

                    foreach (DataRow item in dispositivos.Rows)
                    {
                        idConseguido = true;
                        string id_generado = "";
                        while (idConseguido)
                        {
                            //Generar alfanumerico Random
                            Guid guid = Guid.NewGuid();
                            id_generado = guid.ToString().Replace("-", "");
                            id_generado = id_generado.Substring(0, 30);

                            //Insertar tabla en mensajes
                            if (c.Insertar("INSERT INTO mensajes_en_espera VALUES('" + id_generado + "','" + mensaje + "','" + item[3] + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + miembro + "','" + id_caballo + "');")) 
                            {
                                idConseguido = false;
                            }
                        }

                        //enviar notificaciones

                        if (item[1].ToString() == "iOS") 
                        {
                            var data = new
                            {
                                to = item[3],
                                notification = new
                                {
                                    title = "Mensaje para " + "concatenar nombre del caballo aqui XD",
                                    body = mensaje,
                                    data = new
                                    {
                                        id = token,
                                        men = mensaje
                                    }
                                },
                            };
                            SendNotificationAsync(data);
                        }
                        else
                        {

                            var data = new
                            {
                                to = item[3],
                                data = new
                                {
                                    id = token,
                                    men = mensaje
                                }
                            };
                            SendNotificationAsync(data);
                        }
                    }

                }

                respuesta = true;
            }

            return respuesta;
        }

        [HttpGet("enviarchuy")]
        public bool EnviarChuy(string token)
        {

            var data = new
            {
                to = token,
                data = new
                {
                    id = token,
                    men = "hola"
                }
            };

            SendNotificationAsync(data);

            var data2 = new
            {
                to = token,
                notification = new
                {
                    title = "Mensaje para " + "concatenar nombre del caballo aqui XD",
                    body = "hola",
                    data = new
                    {
                        id = token,
                        men = "holaaa"
                    },
                    sound = "default"
                },
            };

            SendNotificationAsync(data2);
            return true;
        }

        private async void SendNotificationAsync(object data)
        {
            var json = JsonConvert.SerializeObject(data);

            Byte[] byteArray = Encoding.UTF8.GetBytes(json);
            SendNotification(byteArray);
        }

        public void SendNotification(Byte[] byteArray)
        {
            try
            {
                string server_api_key = "AAAAHW-HqwA:APA91bGt1eGVmFrR3Y5v_PiNdqclZBsq3y2SUTa2vMOwsT3g99Q1ZH1E60RY_Lz86BXrOVXQ4lZn_dKhgOhwQCzAndRwSgLCam_bpuIzQMBRzNYx8inIZcUxCkRjhAnXaVUa3vF0rf6p";
                string sender_id = "126425213696";

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add($"Authorization: key={server_api_key}");
                tRequest.Headers.Add($"Sender: id={sender_id}");

                tRequest.ContentLength = byteArray.Length;
                Stream dataStrema = tRequest.GetRequestStream();
                dataStrema.Write(byteArray, 0, byteArray.Length);
                dataStrema.Close();

                WebResponse tresponse = tRequest.GetResponse();
                dataStrema = tresponse.GetResponseStream();
                StreamReader tReader = new StreamReader(dataStrema);

                string sResponseFromServer = tReader.ReadToEnd();

                tReader.Close();
                dataStrema.Close();
            }
            catch(Exception ex) 
            {
                var men = ex.Message;
                int x = 9;
            }
        }
    }
}

using System.Collections.Generic;

namespace CabasusAPIs.Modelos
{
    public class Post
    {
        public string id_post { get; set; }
        public string contenido { get; set; }
        public int privacidad { get; set; }
        public string fk_usuario { get; set; }
        public string fk_caballo { get; set; }
        public string fecha { get; set; }
        public int likes_contable { get; set; }
        public List<string> lista_enlaces{get; set;}
    }
    public class Multimedia
    {
        public string id_multimedia { get; set; }
        public string enlace { get; set; }
        public string fk_post { get; set; }
    }
    public class Likes
    {
        public string fk_usuario { get; set; }
        public string fk_post { get; set; }
    }
}

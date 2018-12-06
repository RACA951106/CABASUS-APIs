using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CabasusAPIs.Modelos
{
    public class caballos
    {
        public string id_caballo { get; set; }
        public string nombre { get; set; }
        public double peso { get; set; }
        public double altura { get; set; }
        public int raza { get; set; }
        public string fecha_nacimiento { get; set; }
        public int genero { get; set; }
        public string foto { get; set; }
        public string fk_usuario { get; set; }
        public int avena { get; set; }
    }
}

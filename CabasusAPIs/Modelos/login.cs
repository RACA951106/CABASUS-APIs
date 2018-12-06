using System;
using System.Collections.Generic;

namespace CabasusAPIs.Modelos
{
    public class login
    {
        public string usuario{ get; set; }
        public string contrasena{ get; set; }
        public string TokenFB{ get; set; }
        public string SO{ get; set; }
        public string id_dispositivo{ get; set; }
    }

    public class Value
    {
        public string token { get; set; }
        public DateTime expiration { get; set; }
    }

    public class RootObject
    {
        public Value Value { get; set; }
        public List<object> Formatters { get; set; }
        public List<object> ContentTypes { get; set; }
        public object DeclaredType { get; set; }
        public int StatusCode { get; set; }
    }
}

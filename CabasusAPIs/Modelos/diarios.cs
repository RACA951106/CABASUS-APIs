namespace CabasusAPIs.Modelos
{
    public class diarios
    {
        public string id_diario { get; set; }
        public string mensaje { get; set; }
        public int privacidad { get; set; }
        public string fk_usuario { get; set; }
        public string fk_caballo { get; set; }
        public string fecha { get; set; }
    }
    public class diariosRango
    {
        public string fk_usuario { get; set; }
        public string fk_caballo { get; set; }
        public string fecha_inicio { get; set; }
        public string fecha_fin { get; set; }
    }
}

namespace CabasusAPIs.Modelos
{
    public class actividades
    {
        public int id_actividad { get; set; }
        public string fk_usuario { get; set; }
        public string fk_caballo { get; set; }
        public string fecha { get; set; }
        public int duracion { get; set; }
        public string intensidad { get; set; }
        public double camina { get; set; }
        public double trota { get; set; }
        public double galopa { get; set; }
        public string latitudes { get; set; }
        public string longitudes { get; set; }
        public int factor_fitness { get; set; }
    }
    public class actividadesRango
    {
        public string fk_usuario { get; set; }
        public string fk_caballo { get; set; }
        public string fecha_inicio { get; set; }
        public string fecha_fin { get; set; }
    }
}

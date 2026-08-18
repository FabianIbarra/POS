using System;

namespace POS.Services
{
    /// <summary>
    /// Servicio de tiempo que estandariza la zona horaria para todas las transacciones del sistema.
    /// Define la zona horaria America/Mazatlan (MST) como la única válida, independientemente 
    /// de la configuración del sistema operativo.
    /// </summary>
    public static class TimeService
    {
        private static readonly string TimeZoneId = "America/Mazatlan";

        /// <summary>
        /// Obtiene la fecha y hora actual garantizando que corresponda a la zona horaria definida (MST).
        /// </summary>
        /// <returns>Estructura DateTime que representa el tiempo actual en Mazatlán.</returns>
        public static DateTime ObtenerHoraLocal()
        {
            try
            {
                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
                return TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
            }
            catch (TimeZoneNotFoundException)
            {
                // Fallback (específicamente útil si en Windows / Linux divergen los IDs, 
                // pero asumimos en este contexto que America/Mazatlan o su equivalente de Windows está disponible.
                // En SO Windows el equivalente de MST sin horario de primavera-verano en México suele llamarse "Mountain Standard Time (Mexico)" o similar.
                
                // Si usamos IANA time zones (America/Mazatlan) en Windows, se requiere la librería TimeZoneConverter para que funcione de manera nativa sin problemas.
                // Como mitigación rápida en caso de no encontrarse, retornaremos UTC-7
                var offset = TimeSpan.FromHours(-7);
                return DateTime.UtcNow.Add(offset);
            }
        }

        /// <summary>
        /// Obtiene la fecha y hora actual en formato de texto ISO8601, ideal para almacenar en SQLite.
        /// </summary>
        public static string ObtenerHoraLocalComoString()
        {
            return ObtenerHoraLocal().ToString("yyyy-MM-ddTHH:mm:ss");
        }
    }
}

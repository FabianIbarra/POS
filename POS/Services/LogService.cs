using System;
using System.IO;

namespace POS.Services
{
    /// <summary>
    /// Servicio de registro de errores en archivo.
    /// Escribe la traza de excepciones no controladas en Logs/app.log para su diagnóstico.
    /// </summary>
    public static class LogService
    {
        private static readonly string RutaLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "app.log");

        /// <summary>
        /// Registra una excepción con su mensaje y traza completa.
        /// </summary>
        /// <param name="ex">Excepción a registrar.</param>
        public static void RegistrarError(Exception ex)
        {
            try
            {
                var directorio = Path.GetDirectoryName(RutaLog);
                if (!string.IsNullOrEmpty(directorio))
                {
                    Directory.CreateDirectory(directorio);
                }

                var linea = $"[{TimeService.ObtenerHoraLocal():yyyy-MM-dd HH:mm:ss}] ERROR: {ex}";
                File.AppendAllText(RutaLog, linea + Environment.NewLine);
            }
            catch
            {
                // Nunca propagar errores de logging para no romper la aplicación
            }
        }
    }
}
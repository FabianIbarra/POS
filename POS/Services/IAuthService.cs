using POS.Models;

namespace POS.Services
{
    /// <summary>
    /// Interfaz para el servicio de autenticación.
    /// Facilita la inyección de dependencias y las pruebas unitarias.
    /// </summary>
    public interface IAuthService
    {
        bool IniciarSesion(string username, string passwordPlan);
        void CerrarSesion();
    }
}

using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Services;

namespace POS.ViewModels
{
    /// <summary>
    /// ViewModel para gestionar la lógica de autenticación en la pantalla de Login.
    /// Maneja la validación de credenciales y el control de accesos según el rol.
    /// </summary>
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string mensajeError = string.Empty;

        [ObservableProperty]
        private bool isLoading = false;

        [ObservableProperty]
        private string mensajeEstado = "Autenticando...";

        /// <summary>
        /// Evento que se dispara cuando el login es exitoso.
        /// Las vistas pueden suscribirse para navegar a la pantalla principal.
        /// </summary>
        public event EventHandler? LoginExitoso;

        /// <summary>
        /// Evento que se dispara cuando ocurre un error en el login.
        /// </summary>
        public event EventHandler<string>? LoginFallido;

        public LoginViewModel()
        {
            _authService = new AuthService();
        }

        /// <summary>
        /// Comando para intentar autenticar al usuario.
        /// Valida las credenciales y guarda la sesión en memoria.
        /// </summary>
        [RelayCommand]
        public async Task Autenticar() // Ahora es 'async Task' en lugar de 'void'
        {
            MensajeError = string.Empty;

            if (string.IsNullOrWhiteSpace(Username))
            {
                MensajeError = "Por favor, ingresa el nombre de usuario.";
                LoginFallido?.Invoke(this, MensajeError);
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                MensajeError = "Por favor, ingresa la contraseña.";
                LoginFallido?.Invoke(this, MensajeError);
                return;
            }

            // Restablecemos el mensaje por defecto y mostramos la barra
            MensajeEstado = "Autenticando...";
            IsLoading = true;

            try
            {
                // Usamos Task.Run para no bloquear la interfaz mientras consulta la BD (aunque SQLite es muy rápido)
                bool esValido = await Task.Run(() => _authService.IniciarSesion(Username, Password));

                if (esValido)
                {
                    // ¡Éxito! Cambiamos el texto para darle retroalimentación visual al usuario
                    MensajeEstado = "¡Ingreso exitoso! Abriendo caja...";

                    // Hacemos una pausa de 800 milisegundos para que el usuario alcance a leer el mensaje
                    await Task.Delay(800);

                    Password = string.Empty;
                    LoginExitoso?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MensajeError = "Usuario o contraseña incorrectos.";
                    LoginFallido?.Invoke(this, MensajeError);
                    Password = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al autenticar: {ex.Message}";
                LoginFallido?.Invoke(this, MensajeError);
                Password = string.Empty;
            }
            finally
            {
                // Solo ocultamos la barra si falló. Si tuvo éxito, la ventana ya se estará cerrando.
                if (MensajeError != string.Empty)
                {
                    IsLoading = false;
                }
            }
        }

        /// <summary>
        /// Comando para limpiar los campos del formulario.
        /// </summary>
        [RelayCommand]
        public void Limpiar()
        {
            Username = string.Empty;
            Password = string.Empty;
            MensajeError = string.Empty;
        }
    }
}

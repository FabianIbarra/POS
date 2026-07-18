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
        public void Autenticar()
        {
            // Limpiar mensajes previos
            MensajeError = string.Empty;

            // Validar que los campos no estén vacíos
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

            // Mostrar indicador de carga
            IsLoading = true;

            try
            {
                // Intentar autenticación
                bool esValido = _authService.IniciarSesion(Username, Password);

                if (esValido)
                {
                    // Autenticación exitosa
                    MensajeError = string.Empty;
                    Password = string.Empty; // Limpiar contraseña por seguridad
                    LoginExitoso?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    // Autenticación fallida
                    MensajeError = "Usuario o contraseña incorrectos.";
                    LoginFallido?.Invoke(this, MensajeError);
                    Password = string.Empty; // Limpiar contraseña por seguridad
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
                IsLoading = false;
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

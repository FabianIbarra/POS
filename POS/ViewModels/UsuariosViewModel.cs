using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Data.Repositories;
using POS.Models;

namespace POS.ViewModels
{
    /// <summary>
    /// ViewModel para el modulo de Gestion de Usuarios.
    /// Permite crear, editar y eliminar usuarios del sistema.
    /// </summary>
    public partial class UsuariosViewModel : ObservableObject
    {
        private readonly UsuarioRepository _usuarioRepo;

        public List<string> Roles { get; } = new() { "Administrador", "Cajero" };

        [ObservableProperty]
        private ObservableCollection<Usuario> _usuarios;

        [ObservableProperty]
        private string _idUsuario = string.Empty;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _nombreCompleto = string.Empty;

        [ObservableProperty]
        private string _contrasena = string.Empty;

        [ObservableProperty]
        private string _rolSeleccionado = string.Empty;

        [ObservableProperty]
        private bool _editando;

        [ObservableProperty]
        private Usuario? _usuarioSeleccionado;

        public UsuariosViewModel()
        {
            _usuarioRepo = new UsuarioRepository();
            Usuarios = new ObservableCollection<Usuario>();
            CargarUsuarios();
        }

        partial void OnUsuarioSeleccionadoChanged(Usuario? value)
        {
            if (value == null) return;

            IdUsuario = value.IdUsuario;
            Username = value.Username;
            NombreCompleto = value.NombreCompleto;
            Contrasena = string.Empty;
            RolSeleccionado = value.Rol;
            Editando = true;
        }

        [RelayCommand]
        private void CargarUsuarios()
        {
            var lista = _usuarioRepo.ObtenerUsuarios();
            Usuarios = new ObservableCollection<Usuario>(lista);
        }

        [RelayCommand]
        private void GuardarUsuario()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                MessageBox.Show("El nombre de usuario es obligatorio.", "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(NombreCompleto))
            {
                MessageBox.Show("El nombre completo es obligatorio.", "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(RolSeleccionado))
            {
                MessageBox.Show("Debe seleccionar un rol.", "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validar username duplicado
            var existente = _usuarioRepo.ObtenerUsuarioPorUsername(Username);
            if (existente != null && existente.IdUsuario != IdUsuario)
            {
                MessageBox.Show("Ya existe otro usuario con ese nombre de usuario.", "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var usuario = new Usuario
            {
                IdUsuario = string.IsNullOrEmpty(IdUsuario) ? Guid.NewGuid().ToString() : IdUsuario,
                Username = Username,
                NombreCompleto = NombreCompleto,
                Rol = RolSeleccionado
            };

            if (Editando && string.IsNullOrEmpty(Contrasena))
            {
                // Mantener el hash existente sin actualizar contraseña
                var actual = _usuarioRepo.ObtenerUsuarioPorId(IdUsuario);
                if (actual != null)
                {
                    usuario.PasswordHash = actual.PasswordHash;
                }
            }
            else if (!string.IsNullOrEmpty(Contrasena))
            {
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Contrasena);
            }
            else
            {
                MessageBox.Show("La contraseña es obligatoria al crear un usuario.", "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (Editando)
                {
                    _usuarioRepo.EditarUsuario(usuario);
                }
                else
                {
                    _usuarioRepo.AgregarUsuario(usuario);
                }

                LimpiarFormulario();
                CargarUsuarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar usuario: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void EliminarUsuario(Usuario usuario)
        {
            if (usuario == null) return;

            var result = MessageBox.Show(
                $"Desea eliminar el usuario: {usuario.Username}?",
                "Confirmar Eliminacion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _usuarioRepo.EliminarUsuario(usuario.IdUsuario);
                    CargarUsuarios();
                    LimpiarFormulario();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar usuario: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private void SeleccionarUsuario(Usuario usuario)
        {
            if (usuario == null) return;
            UsuarioSeleccionado = usuario;
        }

        [RelayCommand]
        private void LimpiarFormulario()
        {
            IdUsuario = string.Empty;
            Username = string.Empty;
            NombreCompleto = string.Empty;
            Contrasena = string.Empty;
            RolSeleccionado = string.Empty;
            Editando = false;
            UsuarioSeleccionado = null;
        }
    }
}

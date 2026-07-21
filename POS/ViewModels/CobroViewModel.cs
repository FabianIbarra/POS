using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace POS.ViewModels
{
    /// <summary>
    /// ViewModel para la ventana modal de cobro. Gestiona el cálculo del cambio
    /// y la confirmación de la venta.
    /// </summary>
    public partial class CobroViewModel : ObservableObject
    {
        [ObservableProperty]
        private decimal _total;

        [ObservableProperty]
        private decimal _efectivoRecibido;

        [ObservableProperty]
        private decimal _cambio;

        /// <summary>
        /// Indica si el usuario confirmó el cobro.
        /// </summary>
        public bool Confirmado { get; private set; }

        /// <summary>
        /// Evento que solicita el cierre de la ventana modal.
        /// </summary>
        public event EventHandler? SolicitarCierre;

        public CobroViewModel(decimal total)
        {
            Total = total;
        }

        partial void OnEfectivoRecibidoChanged(decimal value)
        {
            Cambio = value >= Total ? value - Total : 0;
        }

        [RelayCommand]
        private void Confirmar()
        {
            if (EfectivoRecibido < Total)
            {
                MessageBox.Show(
                    "El efectivo recibido debe ser mayor o igual al total de la venta.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            Confirmado = true;
            SolicitarCierre?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void Cancelar()
        {
            Confirmado = false;
            SolicitarCierre?.Invoke(this, EventArgs.Empty);
        }
    }
}

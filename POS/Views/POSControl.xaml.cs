using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace POS.Views
{
    /// <summary>
    /// Control de usuario que encapsula la interfaz del Punto de Venta.
    /// Se utiliza dentro del MainView para la navegacion por modulos.
    /// </summary>
    public partial class POSControl : UserControl
    {
        public POSControl()
        {
            InitializeComponent();
            Loaded += (_, _) => EnfocarBuscador();
            IsVisibleChanged += (_, _) =>
            {
                if (IsVisible) EnfocarBuscador();
            };
        }

        private void EnfocarBuscador()
        {
            if (TxtBuscador == null) return;
            TxtBuscador.Focus();
            Keyboard.Focus(TxtBuscador);
        }
    }
}

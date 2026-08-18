using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Data.Repositories;
using POS.Models;
using POS.Services;

namespace POS.ViewModels
{
    /// <summary>
    /// ViewModel para el módulo de Reportes. Permite consultar ventas
    /// por rango de fechas o por numero de folio, y visualizar el detalle
    /// de cada venta seleccionada.
    /// </summary>
    public partial class ReportesViewModel : ObservableObject
    {
        private readonly VentaRepository _ventaRepo;

        [ObservableProperty]
        private DateTime _fechaInicio;

        [ObservableProperty]
        private DateTime _fechaFin;

        [ObservableProperty]
        private string _folioBusqueda = string.Empty;

        [ObservableProperty]
        private ObservableCollection<Venta> _ventas;

        [ObservableProperty]
        private Venta? _ventaSeleccionada;

        [ObservableProperty]
        private ObservableCollection<DetalleVenta> _detallesVenta;

        [ObservableProperty]
        private decimal _totalIngresos;

        [ObservableProperty]
        private int _totalVentas;

        [ObservableProperty]
        private string _resumenTexto = string.Empty;

        [ObservableProperty]
        private bool _tieneResultados;

        public ReportesViewModel()
        {
            _ventaRepo = new VentaRepository();
            Ventas = new ObservableCollection<Venta>();
            DetallesVenta = new ObservableCollection<DetalleVenta>();
            FechaInicio = DateTime.Today;
            FechaFin = DateTime.Today;
            BuscarPorFechas();
        }

        partial void OnVentaSeleccionadaChanged(Venta? value)
        {
            DetallesVenta.Clear();

            if (value == null || string.IsNullOrEmpty(value.IdVenta)) return;

            var detalles = _ventaRepo.ObtenerDetallesPorVentaId(value.IdVenta);
            foreach (var detalle in detalles)
            {
                DetallesVenta.Add(detalle);
            }
        }

        [RelayCommand]
        private void BuscarPorFechas()
        {
            var fechaInicioStr = FechaInicio.ToString("yyyy-MM-ddTHH:mm:ss");
            var fechaFinStr = FechaFin.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss");

            try
            {
                var resultados = _ventaRepo.ObtenerVentasPorRangoFechas(fechaInicioStr, fechaFinStr);
                Ventas = new ObservableCollection<Venta>(resultados);
                VentasCollectionChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar ventas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void BuscarPorFolio()
        {
            if (!int.TryParse(FolioBusqueda, out int folio))
            {
                MessageBox.Show("Ingrese un número de folio válido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var venta = _ventaRepo.ObtenerVentaPorFolio(folio);
                Ventas = venta != null
                    ? new ObservableCollection<Venta> { venta }
                    : new ObservableCollection<Venta>();
                VentasCollectionChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar venta: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void CargarVentasDelDia()
        {
            FechaInicio = DateTime.Today;
            FechaFin = DateTime.Today;
            BuscarPorFechas();
        }

        private void VentasCollectionChanged()
        {
            TotalIngresos = Ventas.Sum(v => v.Total);
            TotalVentas = Ventas.Count;
            TieneResultados = Ventas.Any();
            VentaSeleccionada = null;
            DetallesVenta.Clear();

            ResumenTexto = TieneResultados
                ? $"Se encontraron {TotalVentas} ventas con un total de {TotalIngresos:C}"
                : "No se encontraron ventas en el período seleccionado.";
        }
    }
}

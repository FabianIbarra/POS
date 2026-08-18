using System.Text.RegularExpressions;

namespace POS.Helpers
{
    /// <summary>
    /// Utilidad para restringir los caracteres permitidos en los campos de nombre de usuario.
    /// Permite únicamente letras, números, puntos y guiones.
    /// </summary>
    public static class UsuarioInputHelper
    {
        private static readonly Regex PatronUsuarioValido = new(@"^[\p{L}\p{N}.\-]+$");

        /// <summary>
        /// Indica si el texto cumple con el formato de usuario permitido.
        /// </summary>
        /// <param name="texto">Texto a validar.</param>
        /// <returns>Verdadero si el texto es válido o está vacío.</returns>
        public static bool EsEntradaValida(string texto)
        {
            return string.IsNullOrEmpty(texto) || PatronUsuarioValido.IsMatch(texto);
        }

        /// <summary>
        /// Elimina cualquier carácter no permitido del texto.
        /// </summary>
        /// <param name="texto">Texto a sanear.</param>
        /// <returns>Texto con únicamente caracteres permitidos.</returns>
        public static string Sanitizar(string texto)
        {
            return string.IsNullOrEmpty(texto) ? texto : Regex.Replace(texto, @"[^\p{L}\p{N}.\-]", string.Empty);
        }
    }
}
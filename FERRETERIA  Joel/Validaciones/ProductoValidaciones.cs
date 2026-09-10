namespace FERRETERIA__Joel.Validaciones
{
    internal class ProductoValidaciones
    {
        public bool EsCodigoValido(string codigo)
        {
            return !string.IsNullOrWhiteSpace(codigo) && codigo.Trim().Length <= 30;
        }

        public bool EsNombreValido(string nombre)
        {
            return !string.IsNullOrWhiteSpace(nombre) && nombre.Trim().Length <= 150;
        }

        public bool EsDescripcionValida(string? descripcion)
        {
            return string.IsNullOrWhiteSpace(descripcion)
                || descripcion.Trim().Length <= 500;
        }

        public bool EsMarcaValida(string? marca)
        {
            return string.IsNullOrWhiteSpace(marca)
                || marca.Trim().Length <= 100;
        }

        public bool EsUnidadMedidaValida(string? unidadMedida)
        {
            return !string.IsNullOrWhiteSpace(unidadMedida)
                && unidadMedida.Trim().Length <= 30;
        }

        public bool EsPrecioValido(decimal precioVenta)
        {
            return (precioVenta > 0);
        }

        public bool EsCategoriaValida(short idCategoria)
        {
            return (idCategoria > 0);
        }

        public bool EsEmpleadoValido(short idEmpleadoResponsable)
        {
            return (idEmpleadoResponsable > 0);
        }
    }
}
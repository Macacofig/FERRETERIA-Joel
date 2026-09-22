using FERRETERIA__Joel.ConfiguracionValidacion;

namespace FERRETERIA__Joel.Validaciones
{
    internal class ProductoValidaciones
    {
        public bool EsCodigoValido(string codigo)
        {
            return !string.IsNullOrWhiteSpace(codigo)
                && codigo.Trim().Length <= ConfiguracionProducto.CodigoMaxLength;
        }

        public bool EsNombreValido(string nombre)
        {
            return !string.IsNullOrWhiteSpace(nombre)
                && nombre.Trim().Length <= ConfiguracionProducto.NombreMaxLength;
        }

        public bool EsDescripcionValida(string? descripcion)
        {
            return string.IsNullOrWhiteSpace(descripcion)
                || descripcion.Trim().Length <= ConfiguracionProducto.DescripcionMaxLength;
        }

        public bool EsMarcaValida(string? marca)
        {
            return !string.IsNullOrWhiteSpace(marca)
                && marca.Trim().Length <= ConfiguracionProducto.MarcaMaxLength;
        }

        public bool EsUnidadMedidaValida(string? unidadMedida)
        {
            return !string.IsNullOrWhiteSpace(unidadMedida)
                && unidadMedida.Trim().Length <= ConfiguracionProducto.UnidadMedidaMaxLength;
        }

        public bool EsPrecioValido(decimal precioVenta)
        {
            return precioVenta > ConfiguracionProducto.PrecioVentaMinimo;
        }

        public bool EsCategoriaValida(short idCategoria)
        {
            return idCategoria >= ConfiguracionProducto.CategoriaMinima;
        }

        public bool EsEmpleadoValido(short idEmpleadoResponsable)
        {
            return idEmpleadoResponsable >= ConfiguracionProducto.EmpleadoMinimo;
        }
    }
}
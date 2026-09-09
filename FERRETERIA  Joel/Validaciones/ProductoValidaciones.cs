namespace FERRETERIA__Joel.Validaciones
{
    internal class ValidacionProducto
    {
        public bool EsCodigoValido(string codigo)
        {
            return (codigo != "") && (codigo.Length <= 30);
        }

        public bool EsNombreValido(string nombre)
        {
            return (nombre != "") && (nombre.Length <= 150);
        }

        public bool EsUnidadMedidaValida(string unidadMedida)
        {
            return (unidadMedida != "");
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
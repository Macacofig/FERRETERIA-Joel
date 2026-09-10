using FERRETERIA__Joel.Models;

namespace FERRETERIA__Joel.Validaciones
{
    public class CategoriaValidaciones
    {
        public bool EsCodigoValido(string? codigo)
        {
            return !string.IsNullOrWhiteSpace(codigo)
                && codigo.Trim().Length <= 20;
        }

        public bool EsNombreValido(string? nombre)
        {
            return !string.IsNullOrWhiteSpace(nombre)
                && nombre.Trim().Length <= 100;
        }

        public bool EsDescripcionValida(string? descripcion)
        {
            return string.IsNullOrWhiteSpace(descripcion)
                || descripcion.Trim().Length <= 255;
        }

		public bool EsPorcentajeGananciaValido(decimal porcentajeGanancia)
		{
			return porcentajeGanancia >= 0 && porcentajeGanancia <= 100;
		}

		public bool EsEmpleadoValido(short idEmpleadoResponsable)
		{
			return idEmpleadoResponsable > 0;
		}

		public bool EsValida(Categoria categoria)
		{
			return EsCodigoValido(categoria.Codigo) &&
				   EsNombreValido(categoria.Nombre) &&
				   EsDescripcionValida(categoria.Descripcion) &&
				   EsPorcentajeGananciaValido(categoria.PorcentajeGanancia) &&
				   EsEmpleadoValido(categoria.IdEmpleadoResponsable);
		}
	}
}
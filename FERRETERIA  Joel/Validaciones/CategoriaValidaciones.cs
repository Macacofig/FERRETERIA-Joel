using FERRETERIA__Joel.Models;

namespace FERRETERIA__Joel.Validaciones
{
	public class CategoriaValidaciones
	{
		public bool EsCodigoValido(string codigo)
		{
			return !string.IsNullOrWhiteSpace(codigo) && codigo.Trim().Length <= 20;
		}

		public bool EsNombreValido(string nombre)
		{
			return !string.IsNullOrWhiteSpace(nombre) && nombre.Trim().Length <= 100;
		}

		public bool EsDescripcionValida(string? descripcion)
		{
			if (string.IsNullOrEmpty(descripcion))
			{
				return true;
			}
			return descripcion.Length <= 255;
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
				   EsEmpleadoValido(categoria.IdEmpleadoResponsable);
		}
	}
}
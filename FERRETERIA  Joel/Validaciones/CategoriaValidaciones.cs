using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.ConfiguracionValidacion;

namespace FERRETERIA__Joel.Validaciones
{
    public class CategoriaValidaciones
    {
        public bool EsCodigoValido(string? codigo)
        {
            return !string.IsNullOrWhiteSpace(codigo)
                && codigo.Trim().Length <= ConfiguracionCategoria.CodigoMaxLength;
        }

        public bool EsNombreValido(string? nombre)
        {
            return !string.IsNullOrWhiteSpace(nombre)
                && nombre.Trim().Length <= ConfiguracionCategoria.NombreMaxLength;
        }

        public bool EsDescripcionValida(string? descripcion)
        {
            return string.IsNullOrWhiteSpace(descripcion)
                || descripcion.Trim().Length <= ConfiguracionCategoria.DescripcionMaxLength;
        }

        public bool EsPorcentajeGananciaValido(decimal porcentajeGanancia)
        {
            return porcentajeGanancia >= ConfiguracionCategoria.PorcentajeGananciaMinimo
                && porcentajeGanancia <= ConfiguracionCategoria.PorcentajeGananciaMaximo;
        }

        public bool EsEmpleadoValido(short idEmpleadoResponsable)
        {
            return idEmpleadoResponsable >= ConfiguracionCategoria.EmpleadoMinimo;
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
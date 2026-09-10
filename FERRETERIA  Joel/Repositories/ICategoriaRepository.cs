using FERRETERIA__Joel.Models;

namespace FERRETERIA__Joel.Repositories
{
	public interface ICategoriaRepository
	{
		List<Categoria> ObtenerTodas();
        List<Categoria> ObtenerActivas();
        Categoria? ObtenerPorId(short id);
		void Insertar(Categoria categoria);
		void Actualizar(Categoria categoria);
		void Desactivar(short id);
	}
}
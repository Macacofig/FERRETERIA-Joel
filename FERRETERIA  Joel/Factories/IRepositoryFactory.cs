using FERRETERIA__Joel.Repositories;

namespace FERRETERIA__Joel.Factories
{
    public interface IRepositoryFactory
    {
        IProductoRepository CreateProductoRepository();
        ICategoriaRepository CreateCategoriaRepository();
        IProveedorRepository CreateProveedorRepository();
        IEmpleadoRepository CreateEmpleadoRepository();
        IHistoricoPrecioRepository CreateHistoricoPrecioRepository();
    }
}

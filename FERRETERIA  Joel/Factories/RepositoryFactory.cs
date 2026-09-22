using FERRETERIA__Joel.Repositories;

namespace FERRETERIA__Joel.Factories
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RepositoryFactory(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IProductoRepository CreateProductoRepository()
        {
            return new MySqlProductoRepository(_connectionFactory);
        }

        public ICategoriaRepository CreateCategoriaRepository()
        {
            return new MySqlCategoriaRepository(_connectionFactory);
        }

        public IProveedorRepository CreateProveedorRepository()
        {
            return new MySqlProveedorRepository(_connectionFactory);
        }

        public IEmpleadoRepository CreateEmpleadoRepository()
        {
            return new MySqlEmpleadoRepository(_connectionFactory);
        }

        public IHistoricoPrecioRepository CreateHistoricoPrecioRepository()
        {
            return new MySqlHistoricoPrecioRepository(_connectionFactory);
        }
    }
}

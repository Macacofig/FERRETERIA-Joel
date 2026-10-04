using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;

namespace FERRETERIA__Joel.Factories
{
    public class ProductoRepositoryCreator : RepositoryCreator<IRepository<Producto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductoRepositoryCreator(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public override IRepository<Producto> CreateRepository()
        {
            return new MySqlProductoRepository(_connectionFactory);
        }
    }
}
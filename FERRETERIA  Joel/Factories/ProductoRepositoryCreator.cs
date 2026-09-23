using FERRETERIA__Joel.Repositories;

namespace FERRETERIA__Joel.Factories
{
    public class ProductoRepositoryCreator : RepositoryCreator<IProductoRepository>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductoRepositoryCreator(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public override IProductoRepository CreateRepository()
        {
            return new MySqlProductoRepository(_connectionFactory);
        }
    }
}

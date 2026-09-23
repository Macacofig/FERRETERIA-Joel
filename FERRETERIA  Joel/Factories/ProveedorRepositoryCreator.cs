using FERRETERIA__Joel.Repositories;

namespace FERRETERIA__Joel.Factories
{
    public class ProveedorRepositoryCreator : RepositoryCreator<IProveedorRepository>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProveedorRepositoryCreator(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public override IProveedorRepository CreateRepository()
        {
            return new MySqlProveedorRepository(_connectionFactory);
        }
    }
}

using FERRETERIA__Joel.Repositories;

namespace FERRETERIA__Joel.Factories
{
    public class CategoriaRepositoryCreator : RepositoryCreator<ICategoriaRepository>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CategoriaRepositoryCreator(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public override ICategoriaRepository CreateRepository()
        {
            return new MySqlCategoriaRepository(_connectionFactory);
        }
    }
}

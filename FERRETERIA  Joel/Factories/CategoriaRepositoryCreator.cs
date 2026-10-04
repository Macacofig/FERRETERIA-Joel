using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;

namespace FERRETERIA__Joel.Factories
{
    public class CategoriaRepositoryCreator : RepositoryCreator<IRepository<Categoria>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CategoriaRepositoryCreator(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public override IRepository<Categoria> CreateRepository()
        {
            return new MySqlCategoriaRepository(_connectionFactory);
        }
    }
}
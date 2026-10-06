using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;

namespace FERRETERIA__Joel.Factories
{
    public class MarcaRepositoryCreator : RepositoryCreator<Marca>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MarcaRepositoryCreator(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public override ICRUD<Marca> CreateRepository()
        {
            return new MySqlMarcaRepository(_connectionFactory);
        }
    }
}
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;

namespace FERRETERIA__Joel.Factories
{
    public class HistoricoPrecioRepositoryCreator : RepositoryCreator<HistoricoPrecio>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public HistoricoPrecioRepositoryCreator(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public override MySqlHistoricoPrecioRepository CreateRepository()
        {
            return new MySqlHistoricoPrecioRepository(_connectionFactory);
        }
    }
}
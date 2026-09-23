using FERRETERIA__Joel.Repositories;

namespace FERRETERIA__Joel.Factories
{
    public class HistoricoPrecioRepositoryCreator : RepositoryCreator<IHistoricoPrecioRepository>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public HistoricoPrecioRepositoryCreator(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public override IHistoricoPrecioRepository CreateRepository()
        {
            return new MySqlHistoricoPrecioRepository(_connectionFactory);
        }
    }
}

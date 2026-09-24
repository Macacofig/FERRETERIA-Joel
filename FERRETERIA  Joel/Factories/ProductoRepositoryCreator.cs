using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;

namespace FERRETERIA__Joel.Factories
{
    public class ProductoRepositoryCreator : RepositoryCreator<Producto>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly MySqlHistoricoPrecioRepository _historicoPrecioRepository;

        public ProductoRepositoryCreator(
            IDbConnectionFactory connectionFactory,
            MySqlHistoricoPrecioRepository historicoPrecioRepository)
        {
            _connectionFactory = connectionFactory;
            _historicoPrecioRepository = historicoPrecioRepository;
        }

        public override ICRUD<Producto> CreateRepository()
        {
            return new MySqlProductoRepository(
                _connectionFactory,
                _historicoPrecioRepository);
        }
    }
}
using FERRETERIA__Joel.Repositories;

namespace FERRETERIA__Joel.Factories
{
    public abstract class RepositoryCreator<T>
    {
        public abstract ICRUD<T> CreateRepository();
    }
}
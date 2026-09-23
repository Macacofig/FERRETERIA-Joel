namespace FERRETERIA__Joel.Factories
{
    // Creator del Factory Method. TRepository es el Producto: la interfaz
    // combinada (ICRUD<T> + funciones especiales) que devuelve cada
    // ConcreteCreator.
    public abstract class RepositoryCreator<TRepository>
    {
        public abstract TRepository CreateRepository();
    }
}

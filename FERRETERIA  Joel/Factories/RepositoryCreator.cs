namespace FERRETERIA__Joel.Factories
{
    public abstract class RepositoryCreator<TContrato>
    {
        public abstract TContrato CreateRepository();
    }
}
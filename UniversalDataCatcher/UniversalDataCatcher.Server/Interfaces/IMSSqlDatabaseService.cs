namespace UniversalDataCatcher.Server.Interfaces
{
    public interface IMSSqlDatabaseService<T> where T : class
    {
        void EnsureDatabaseAndTableCreated();
        void InsertRecord(T record);
        T? FindById(string id);
    }
}

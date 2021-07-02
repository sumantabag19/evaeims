using Microsoft.EntityFrameworkCore;

namespace EVA.EIMS.Contract
{
    /// <summary>
    /// Data Context interface.
    /// </summary>
    public interface IDataContext
    {
        DbSet<T> Set<T>() where T : class;
        int SaveChanges();
        void Dispose();
    }
}

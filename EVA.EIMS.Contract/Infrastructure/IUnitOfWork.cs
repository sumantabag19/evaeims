using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Infrastructure
{
    // <summary>
    /// Interface for the UnitOfWork class.
    /// </summary>
    public interface IUnitOfWork: IDisposable
    {
        DbContext DbContext { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}

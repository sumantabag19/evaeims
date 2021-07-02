using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;

namespace EVA.EIMS.Contract.Repository
{
    public interface IPurchaseorderSalesRepository: ISelectService<PurchaseorderSaleDetails>
    {
        IUnitOfWork UnitOfWork { get; }
    }
}

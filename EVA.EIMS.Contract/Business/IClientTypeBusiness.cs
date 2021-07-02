using EVA.EIMS.Common;
using EVA.EIMS.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
  public interface IClientTypeBusiness : IDisposable
  {
    Task<ReturnResult> Get();
    Task<ReturnResult> GetById(int clientTypeId);
    Task<ReturnResult> Save(string userName, ClientType clientType);
    Task<ReturnResult> Update(string userName, int clientTypeId, ClientType clientType);
    Task<ReturnResult> Delete(string userName, int clientTypeId);
    Task<int> GetByClientTypeName(string clientTypeName);
  }
}

﻿using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;

namespace EVA.EIMS.Contract.Repository
{
    public interface IClientTypeAccessExceptionRepository : IBaseRepository<ClientTypeAccessException>, IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }
}

using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity.ViewModel;
using EVA.EIMS.Entity;
using EVA.EIMS.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;

namespace EVA.EIMS.Business
{
    public class OrganizationTenantBusiness : IOrganizationTenantMapping
    {
        private readonly IOrganizationTenantMappingRepository _organizationTenantMappingRepository;
        private readonly IOrganizationTenantMappingDomainRepository _organizationTenantMappingDomainRepository;
        private readonly IUserRoleMappingRepository _userRoleMappingRepository;
        private readonly IRoleBusiness _roleBusiness;
        IServiceProvider _serviceProvider;
        private bool _disposed;
        private readonly ILogger _logger;
        public OrganizationTenantBusiness(IServiceProvider serviceProvider, IOrganizationTenantMappingRepository organizationTenantMappingRepository, ILogger logger,IUserRoleMappingRepository userRoleMappingRepository,IRoleBusiness roleBusiness, IOrganizationTenantMappingDomainRepository organizationTenantMappingDomainRepository)
        {
            _organizationTenantMappingRepository = organizationTenantMappingRepository;
            _roleBusiness = roleBusiness;
            _userRoleMappingRepository = userRoleMappingRepository;
            _organizationTenantMappingDomainRepository = organizationTenantMappingDomainRepository;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _disposed = false;
        }
        public async Task<IEnumerable<OrganizationTenantMappingDomainModel>> GetAllOrgTenantMapping()
        {
            List<OrganizationTenantMappingDomainModel> organizationTenantMappingModel = new List<OrganizationTenantMappingDomainModel>();
            try
            {
                var orgInfo = await _organizationTenantMappingDomainRepository.SelectAllAsync();
                return orgInfo;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return organizationTenantMappingModel;
            }
            
        }

        public async Task<List<OrganizationTenantMappingModel>> GetOrgIDbyTenantId(string tenantId)
        {
            try
            {
                IExecuterStoreProc<OrganizationTenantMappingModel> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<OrganizationTenantMappingModel>>();

                List<Parameters> param = new List<Parameters>() { new Parameters("TenantId", tenantId)};

                List<OrganizationTenantMappingModel> orgInfo = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetOrganizationByTenant.ToString(), param);
              
                return orgInfo;
            }
            catch (Exception ex)
            {
                _logger.Error("OrganizationTenantBusiness", "GetOrgIDbyTenantId", ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        public async Task<ReturnResult> CreateOrganizationTenantMapping(OrganizationTenantMappingDomainModel organizationTenantMappingModel, Guid guid)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {

              var userrole= await _userRoleMappingRepository.SelectFirstOrDefaultAsync(a => a.UserId == guid);
               var roleresult= await _roleBusiness.GetRole(userrole.RoleId);
                if (roleresult.Success == false)
                {
                    throw new Exception(roleresult.Data.ToString());
                }
                var role = (Role)roleresult.Data;
                if (role.RoleName != "SuperAdmin")
                {
                    throw new Exception("User doesnt have access");
                }
                
                
                _organizationTenantMappingDomainRepository.Add(organizationTenantMappingModel);
                await _organizationTenantMappingDomainRepository.UnitOfWork.SaveChangesAsync();
                returnResult.Success = true;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedSuccess")}";
                return returnResult;

            }
            catch (Exception ex)
            {
                returnResult.Success = false;
               returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        public async Task<ReturnResult> UpdateTenantOrgMapping(OrganizationTenantMappingDomainModel organizationTenantMappingModel,Guid guid)
        {
            ReturnResult returnResult = new ReturnResult();

            try
            {
                var userrole = await _userRoleMappingRepository.SelectFirstOrDefaultAsync(a => a.UserId == guid);
                var roleresult = await _roleBusiness.GetRole(userrole.RoleId);
                if (roleresult.Success == false)
                {
                    throw new Exception(roleresult.Data.ToString());
                }
                var role = (Role)roleresult.Data;
                if (role.RoleName != "SuperAdmin")
                {
                    throw new Exception("User doesnt have access");
                }
                await _organizationTenantMappingDomainRepository.UpdateAsync(organizationTenantMappingModel);
                await _organizationTenantMappingDomainRepository.UnitOfWork.SaveChangesAsync();
                returnResult.Success = true;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataUpdateSuccess")}";
                return returnResult;
            }
            catch (Exception ex)
            {
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataUpdateFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        public async Task<ReturnResult> DeleteOrgTenantMapping(OrganizationTenantMappingDomainModel organizationTenantMappingModel,Guid guid)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var userrole = await _userRoleMappingRepository.SelectFirstOrDefaultAsync(a => a.UserId == guid);
                var roleresult = await _roleBusiness.GetRole(userrole.RoleId);
                if (roleresult.Success == false)
                {
                    throw new Exception(roleresult.Data.ToString());
                }
                var role = (Role)roleresult.Data;
                if (role.RoleName != "SuperAdmin")
                {
                    throw new Exception("User doesnt have access");
                }
                await _organizationTenantMappingDomainRepository.DeleteAsync(organizationTenantMappingModel);
                await _organizationTenantMappingDomainRepository.UnitOfWork.SaveChangesAsync();
                returnResult.Success = true;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataDeleteSuccess")}";
                return returnResult;
            }
            catch (Exception ex)
            {
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataDeleteFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }

        }

        public async Task<List<OrganizationApplicationDetailModel>> GetOrgAppDetails(string tenantId, string azureAppId)
        {
            try
            {
                IExecuterStoreProc<OrganizationApplicationDetailModel> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<OrganizationApplicationDetailModel>>();

                List<Parameters> param = new List<Parameters>() { new Parameters("TenantId", tenantId), new Parameters("AzureAppId",azureAppId) };

                List<OrganizationApplicationDetailModel> orgInfo = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetOrgAppDetails.ToString(), param);

                return orgInfo;
            }
            catch (Exception ex)
            {
                _logger.Error("OrganizationTenantBusiness", "GetOrgandAppDetails", ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~OrganizationTenantBusiness() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        
        #endregion
    }
}

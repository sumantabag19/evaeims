using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ComplexEntities;
using EVA.EIMS.Entity.ViewModel;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVA.EIMS.Business
{
	public class OrganizationApplicationmappingBusiness : IOrganizationApplicationmappingBusiness
	{

		private readonly IOrganizationRepository _organizationRepository;
		private readonly IApplicationRepository _applicationRepository;
		private readonly IOrganizationApplicationmappingRepository _organizationApplicationmappingRepository;
		private readonly IUserRepository _userRepository;
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger _logger;
		private bool _disposed;
		public OrganizationApplicationmappingBusiness(IOrganizationRepository organizationRepository, IApplicationRepository applicationRepository, IOrganizationApplicationmappingRepository organizationApplicationmappingRepository, IUserRepository userRepository, IServiceProvider serviceProvider, ILogger logger)
		{
			_organizationRepository = organizationRepository;
			_applicationRepository = applicationRepository;
			_organizationApplicationmappingRepository = organizationApplicationmappingRepository;
			_userRepository = userRepository;
			_serviceProvider = serviceProvider;
			_logger = logger;
			_disposed = false;
		}
		/// <summary>
		/// to delete appid from orgid   
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="mappingId"></param>
		/// <returns></returns>
		public async Task<ReturnResult> Delete(string userName, int mappingId)
		{
			ReturnResult returnResult = new ReturnResult();
			try
			{
				if (mappingId == 0)
				{
					returnResult.Success = false;
					returnResult.Result = ResourceInformation.GetResValue("ProvideOrgAppMappingId");
					return returnResult;
				}
				var organizationApplication = await _organizationApplicationmappingRepository.SelectFirstOrDefaultAsync(o => o.OrganizationApplicationId == mappingId && o.IsActive.Value);
				if (organizationApplication == null)
				{
					returnResult.Success = false;
					returnResult.Result = ResourceInformation.GetResValue("ProvideOrgAppMappingId");
					return returnResult;
				}
				organizationApplication.IsActive = false;
				var userDetails = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value);
				if (userDetails == null)
				{
					returnResult.Success = false;
					returnResult.Result = ResourceInformation.GetResValue("UserDetailsNotFound");
					return returnResult;
				}
				Guid userId = userDetails.UserId;
				organizationApplication.ModiefiedBy = userId;
				var result = await _organizationApplicationmappingRepository.UpdateAsync(organizationApplication);
				if (result.State.Equals(EntityState.Modified))
				{
					await _organizationApplicationmappingRepository.UnitOfWork.SaveChangesAsync();

					returnResult.Success = true;

					returnResult.Result = ResourceInformation.GetResValue("DataDeleteSuccess");
					return returnResult;
				}
				else
				{
					returnResult.Success = false;
					returnResult.Result = ResourceInformation.GetResValue("DataDeleteFailure");
					return returnResult;
				}

			}
			catch (Exception ex)
			{
				_logger.Error("OrganizationApplicationmappingBusiness", "Delete", ex.Message, ex.StackTrace);
				returnResult.Success = false;
				returnResult.Result = ExceptionLogger.LogException(ex);
				return returnResult;
			}
		}


		/// <summary>
		/// to get all applicationids corrsponding to organizationid
		/// </summary>
		/// <param name="tokenData"></param>
		/// <returns></returns>
		public async Task<IEnumerable<OrganizationApplicationmapping>> Get(TokenData tokenData)
		{
			ReturnResult returnResult = new ReturnResult();
			try
			{
				return await _organizationApplicationmappingRepository.SelectAsync(o => o.IsActive.Value);
			}
			catch (Exception ex)
			{
				_logger.Error("OrganizationApplicationmappingBusiness", "Get", ex.Message, ex.StackTrace);
				returnResult.Success = false;
				returnResult.Result = ExceptionLogger.LogException(ex);
				return null;
			}
		}

        /// <summary>
        /// Method to get all organisation name and application name mapping
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="orgAppMappingId"></param>
        /// <returns></returns>
        public async Task<ReturnResult>GetOrgNameAppName(TokenData tokenData, int orgAppMappingId)
        {
            ReturnResult returnResult = new ReturnResult();
            List<Parameters> param;
            try
            {
                IExecuterStoreProc<OrganizationApplicationModel> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<OrganizationApplicationModel>>();
                if (orgAppMappingId == 0)
                {
                    param = new List<Parameters>() {
                new Parameters("p_OrgAppMappingId", DBNull.Value)
                };
                }
                else
                {
                    param = new List<Parameters>() {
                new Parameters("p_OrgAppMappingId", orgAppMappingId)
                };
                }

                var orgAppMapping = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetAllOrgNameAppNameMapping,param);

                if(orgAppMapping != null && orgAppMapping.Count > 0)
                {
                    returnResult.Success = false;
                    returnResult.Data = orgAppMapping;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("NotExists");
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("OrganizationApplicationmappingBusiness", "GetOrgNameAppName", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ExceptionLogger.LogException(ex);
                return returnResult;
            }
        }

        /// <summary>
        /// method to get all application names which are not assigned to particular organization
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public async Task<ReturnResult> GetAllExceptionAppNameByOrgId(TokenData tokenData,string orgName)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (string.IsNullOrEmpty(orgName))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("OrganizationNotExists");
                    return returnResult;
                }

                IExecuterStoreProc<ApplicationModel> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<ApplicationModel>>();

                List<Parameters> param = new List<Parameters>() {
                            new Parameters("p_OrgName", orgName) };
                var result = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetAllExceptionApplicationsByOrgName, param);

                if (result != null && result.Count > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = result;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("NoRecordsFound");
                    return returnResult;
                }
            }
            catch(Exception ex)
            {
                _logger.Error("OrganizationApplicationmappingBusiness", "GetAllExceptionAppNameByOrgId", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ExceptionLogger.LogException(ex);
                return returnResult;
            }
        }

        /// <summary>
        /// To saver organizatiob application mapping
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="orgApp"></param>
        /// <returns></returns>
        public async Task<ReturnResult> SaveOrganizatonApplicationMapping(string userName, OrganizationApplicationViewModel orgApp)
        {
            ReturnResult returnResult = new ReturnResult();
            List<OrganizationApplicationmapping> orgAppList = new List<OrganizationApplicationmapping>();
            try
            {
                if (orgApp == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidDetails");
                    return returnResult;
                }

                List<MultiSelect> appDetails = orgApp.AppName.ToList();

                var appId = appDetails.Where(x => x.selected == true).Select(x => x.id);

                var orgDetails = await _organizationRepository.SelectFirstOrDefaultAsync(x => x.OrgName.Equals(orgApp.OrgName,StringComparison.OrdinalIgnoreCase) && x.IsActive.Value);
                if (orgDetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("OrganizationNotExists");
                    return returnResult;
                }
                if (orgApp.IsActive == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("IsActiveRequired");
                    return returnResult;
                }

                var userDetails = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value);
                if (userDetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("UserDetailsNotFound");
                    return returnResult;
                }

                foreach (var item in appId)
                {
                    orgAppList.Add(new OrganizationApplicationmapping()
                    {
                        AppId = item,
                        OrgId = orgDetails.OrgId,
                        IsActive = orgApp.IsActive,
                        ModiefiedBy = userDetails.UserId,
                        ModiefiedOn = DateTime.Now,
                        CreatedBy = userDetails.UserId,
                        CreatedOn = DateTime.Now

                    });
                }

                if (orgAppList != null)
                {
                    await _organizationApplicationmappingRepository.AddRange(orgAppList);
                    await _organizationApplicationmappingRepository.UnitOfWork.SaveChangesAsync();
                    returnResult.Success = true;
                    returnResult.Result = ResourceInformation.GetResValue("DataUpdateSuccess");
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("DataUpdateFailure");
                }
                return returnResult;
            }
            catch(Exception ex)
            {
                _logger.Error("OrganizationApplicationmappingBusiness", "SaveOrganizatonApplicationMapping", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ExceptionLogger.LogException(ex);
                return returnResult;
            }
        }

        /// <summary>
        /// to get all applicationid by organizationid
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public async Task<OrganizationApplicationMappingDetails> GetById(int orgId)
		{
			ReturnResult returnResult = new ReturnResult();

			try
			{
				if (orgId == 0)
				{
					returnResult.Success = false;
					returnResult.Result = ResourceInformation.GetResValue("ProvideOrgId");
					return null;
				}
				IExecuterStoreProc<OrganizationApplicationMappingDetails> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<OrganizationApplicationMappingDetails>>();

                List<Parameters> param = new List<Parameters>() {
                new Parameters("p_OrgId", orgId)
                };
                var orgAppMapping = (await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetAllAppIdByOrganizationId.ToString(), param)).FirstOrDefault();
                return orgAppMapping;
            }
            catch (Exception ex)
            {
                _logger.Error("OrganizationApplicationmappingBusiness", "GetById", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ExceptionLogger.LogException(ex);
                return null;
            }
        }


		/// <summary>
		/// To save in the data
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="organizationApplicationmapping"></param>
		/// <returns></returns>
		public async Task<ReturnResult> SaveOrgAppMapping(string userName, OrganizationApplicationmapping organizationApplicationmapping)
		{
			ReturnResult returnResult = new ReturnResult();

			try
			{
				if (organizationApplicationmapping == null)
				{
					returnResult.Success = false;
					returnResult.Result = ResourceInformation.GetResValue("ProvideValidDetails");
					return returnResult;
				}


				var application = await _applicationRepository.SelectFirstOrDefaultAsync(a => a.AppId == organizationApplicationmapping.AppId && a.IsActive.Value);
				var organization = await _organizationRepository.SelectFirstOrDefaultAsync(o => o.OrgId == organizationApplicationmapping.OrgId && o.IsActive.Value);
				if (application == null)
				{
					returnResult.Success = false;
					returnResult.Result = ResourceInformation.GetResValue("ProvideApplicationName");
					return returnResult;
				}
				if (organization == null)
				{
					returnResult.Success = false;
					returnResult.Result = ResourceInformation.GetResValue("ProvideOrganizationName");
					return returnResult;
				}

				var existMapping = await _organizationApplicationmappingRepository.SelectFirstOrDefaultAsync(o => o.OrgId == organizationApplicationmapping.OrgId && o.AppId == organizationApplicationmapping.AppId && !o.IsActive.Value);
				if (existMapping != null)
				{
					returnResult.Success = false;
					returnResult.Result = ResourceInformation.GetResValue("ExistsAndInActive");
					return returnResult;

				}

                var existingMapping = await _organizationApplicationmappingRepository.SelectFirstOrDefaultAsync(o => o.OrgId == organizationApplicationmapping.OrgId && o.AppId == organizationApplicationmapping.AppId && o.IsActive.Value);
                if (existingMapping != null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("AlreadyExist");
                    return returnResult;

				}

				if (organizationApplicationmapping.IsActive == null)
				{
					organizationApplicationmapping.IsActive = true;
				}
				var userDetails = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value);
				if (userDetails == null)
				{
					returnResult.Success = false;
					returnResult.Result = ResourceInformation.GetResValue("UserDetailsNotFound");
					return returnResult;
				}
				var userId = userDetails.UserId;
				organizationApplicationmapping.CreatedBy = userId;
				organizationApplicationmapping.ModiefiedBy = userId;
				var result = await _organizationApplicationmappingRepository.AddAsync(organizationApplicationmapping);
				if (result.State.Equals(EntityState.Added))
				{
					await _organizationApplicationmappingRepository.UnitOfWork.SaveChangesAsync();

					returnResult.Success = true;

					returnResult.Result = ResourceInformation.GetResValue("DataSavedSuccess");
				}
				else
				{
					returnResult.Success = false;
					returnResult.Result = ResourceInformation.GetResValue("DataSavedFailure");

				}

				return returnResult;
			}
			catch (Exception ex)
			{
				_logger.Error("OrganizationApplicationmappingBusiness", "SaveOrgAppMapping", ex.Message, ex.StackTrace);
				returnResult.Success = false;
				returnResult.Result = ExceptionLogger.LogException(ex);
				return returnResult;
			}
		}

		/// <summary>
		/// To UpDate Orgid , appid and active status
		/// </summary>
		/// <param name="tokenData"></param>
		/// <param name="mappingId"></param>
		/// <param name="organizationApplicationmapping"></param>
		/// <returns></returns>
		public async Task<ReturnResult> UpdateOrgAppMapping(TokenData tokenData, int mappingId, OrganizationApplicationmapping organizationApplicationmapping)
		{
			ReturnResult returnResult = new ReturnResult();
			try
			{
				if (mappingId == 0)
				{
					returnResult.Success = false;
					returnResult.Result = ResourceInformation.GetResValue("ProvideOrgAppMappingId");
				}

				if (organizationApplicationmapping == null)
				{
					returnResult.Success = false;
					returnResult.Result = ResourceInformation.GetResValue("ProvideValidDetails");
					return returnResult;
				}

				var updateOrgMapping = await _organizationApplicationmappingRepository.SelectFirstOrDefaultAsync(o => o.OrganizationApplicationId == mappingId);
				if (updateOrgMapping == null)
				{
					returnResult.Success = false;
					returnResult.Result = ResourceInformation.GetResValue("NotExists");
				}

				if (organizationApplicationmapping.OrgId != updateOrgMapping.OrgId && organizationApplicationmapping.OrgId != 0)
				{
					var organization = await _organizationRepository.SelectFirstOrDefaultAsync(o => o.OrgId == organizationApplicationmapping.OrgId && o.IsActive.Value);
					if (organization != null)
					{
						updateOrgMapping.OrgId = organizationApplicationmapping.OrgId;
					}
					else
					{
						returnResult.Success = false;
						returnResult.Result = ResourceInformation.GetResValue("ProvideProperOrgId");
					}
				}
				if (organizationApplicationmapping.AppId != updateOrgMapping.AppId && organizationApplicationmapping.AppId != 0)
				{
					var application = await _applicationRepository.SelectFirstOrDefaultAsync(a => a.AppId == organizationApplicationmapping.AppId && a.IsActive.Value);
					if (application != null)
					{
						updateOrgMapping.AppId = organizationApplicationmapping.AppId;
					}
					else
					{
						returnResult.Success = false;
						returnResult.Result = ResourceInformation.GetResValue("ProvideProperAppId");
					}
				}
				if (organizationApplicationmapping.IsActive == null)
				{
					returnResult.Success = false;
					returnResult.Result = ResourceInformation.GetResValue("IsActiveRequired");
					return returnResult;
				}

                //var existingMapping = await _organizationApplicationmappingRepository.SelectFirstOrDefaultAsync(o => o.OrgId == organizationApplicationmapping.OrgId && o.AppId == organizationApplicationmapping.AppId);

                //if (existingMapping != null && existingMapping.IsActive.Value)
                //{
                //    returnResult.Success = false;
                //    returnResult.Result = ResourceInformation.GetResValue("AlreadyExist");
                //    return returnResult;
                //}

                //if (existingMapping != null && !existingMapping.IsActive.Value)
                //{
                //    returnResult.Success = false;
                //    returnResult.Result = ResourceInformation.GetResValue("ExistsAndInActive");
                //    return returnResult;
                //}

                updateOrgMapping.IsActive = organizationApplicationmapping.IsActive.Value;

				var userDetails = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(tokenData.UserName.ToString()) && u.IsActive.Value);
				if (userDetails == null)
				{
					returnResult.Success = false;
					returnResult.Result = ResourceInformation.GetResValue("UserDetailsNotFound");
					return returnResult;
				}
				var userId = userDetails.UserId;
				updateOrgMapping.ModiefiedBy = userId;
				var result = await _organizationApplicationmappingRepository.UpdateAsync(updateOrgMapping);
				if (result.State.Equals(EntityState.Modified))
				{
					await _organizationApplicationmappingRepository.UnitOfWork.SaveChangesAsync();

					returnResult.Success = true;

					returnResult.Result = ResourceInformation.GetResValue("DataUpdateSuccess");
				}
				else
				{
					returnResult.Success = false;
					returnResult.Result = ResourceInformation.GetResValue("DataUpdateFailure");

				}

				return returnResult;
			}
			catch (Exception ex)
			{
				_logger.Error("OrganizationApplicationmappingBusiness", "UpdateOrgAppMapping", ex.Message, ex.StackTrace);
				returnResult.Success = false;
				returnResult.Result = ResourceInformation.GetResValue("DataUpdateFailure");
				return returnResult;
			}
		}
		#region Dispose
		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;

			if (disposing)
			{
				_organizationRepository.Dispose();
				_userRepository.Dispose();
				_organizationApplicationmappingRepository.Dispose();
				_applicationRepository.Dispose();
			}

			_disposed = true;
		}

		/// <summary>
		/// Method to dispose.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}

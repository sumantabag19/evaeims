using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVA.EIMS.Business
{
	public class AuthProviderBusiness : IAuthProviderBusiness
	{
		#region Private Variable
		private readonly IAuthProviderRepository _authProviderRepository;
		IServiceProvider _serviceProvider;
		private bool _disposed;
		private readonly ILogger _logger;
		#endregion

		public AuthProviderBusiness(IServiceProvider serviceProvider, IAuthProviderRepository authProviderRepository, ILogger logger)
		{
			_authProviderRepository = authProviderRepository;
			_serviceProvider = serviceProvider;
			_logger = logger;
			_disposed = false;
		}

		#region Public Methods
		/// <summary>
		/// This Method returns all authprovider details
		/// </summary>
		/// <returns>list of authprovider</returns>
		public async Task<ReturnResult> Get()
		{
			ReturnResult returnResult = new ReturnResult();
			try
			{
				var listOfAuthProvider = await _authProviderRepository.SelectAllAsync();
				if (listOfAuthProvider != null)
				{
					returnResult.Success = true;
					returnResult.Data = listOfAuthProvider;
					return returnResult;
				}
				else
				{
					returnResult.Success = false;
					returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
										  $"{ResourceInformation.GetResValue("AuthProviderMaster")} " +
										  $"{ ResourceInformation.GetResValue("NotExists")}";
					return returnResult;
				}
			}
			catch (Exception ex)
			{
				_logger.Error("AuthProviderBusiness", "Get", ex.Message, ex.StackTrace);
				returnResult.Success = false;
				returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
				return returnResult;
			}
		}

		/// <summary>
		/// This method gives the provider details based on providerid
		/// </summary>
		/// <param name="providerId"></param>
		/// <returns>return single providermaster entity</returns>
		public async Task<AuthProviderMaster> GetById(int providerId)
		{
			try
			{
				var requiredAuthProviderInfo = await _authProviderRepository.SelectFirstOrDefaultAsync(a => a.ProviderID.Equals(providerId));
				return requiredAuthProviderInfo;
			}
			catch (Exception ex)
			{
				_logger.Error("AuthProviderBusiness", "GetById", ex.Message, ex.StackTrace);
				throw ex;
			}
		}
		/// <summary>
		/// This method returns authprovider details of given provider name
		/// </summary>
		/// <param name="providerName"></param>
		/// <returns>return single authprovider details</returns>
		public async Task<ReturnResult> GetByName(string providerName)
		{
			ReturnResult returnResult = new ReturnResult();
			try
			{
				var requiredAuthProviderInfo =await _authProviderRepository.SelectFirstOrDefaultAsync(a => a.ProviderName.Equals(providerName));
				if (requiredAuthProviderInfo != null)
				{
					returnResult.Success = true;
					returnResult.Data = requiredAuthProviderInfo;
					return returnResult;
				}
				else
				{
					returnResult.Success = false;
					returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")} " +
										  $"{ResourceInformation.GetResValue("AuthProviderMaster")} " +
										  $"{ ResourceInformation.GetResValue("NotExists")}";
					return returnResult;
				}
			}
			catch (Exception ex)
			{
				_logger.Error("AuthProviderBusiness", "GetByName", ex.Message, ex.StackTrace);
				returnResult.Success = false;
				returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
				return returnResult;
			}
		}
		/// <summary>
		/// This Method add new auth provider details
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="authProvider"></param>
		/// <returns>success or failure</returns>
		public async Task<ReturnResult> Save(string userName, AuthProviderMaster authProvider)
		{
			ReturnResult returnResult = new ReturnResult();
			try
			{
				if (authProvider == null)
				{
					returnResult.Success = false;
					returnResult.Result = ResourceInformation.GetResValue("ProvideValidDetails");
					return returnResult;
				}

				if (string.IsNullOrEmpty(authProvider.ProviderName))
				{
					returnResult.Result = ResourceInformation.GetResValue("ProvideAuthProviderName");
					returnResult.Success = false;
					return returnResult;
				}

				if ((await _authProviderRepository.SelectFirstOrDefaultAsync(a => a.ProviderName.Equals(authProvider.ProviderName))) != null)
				{
					returnResult.Result = ResourceInformation.GetResValue("AlreadyExist");
					returnResult.Success = false;
					return returnResult;
				}
				IUserRepository _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
				Guid userId = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive == true)).UserId;

				authProvider.UpdatedBy = userId;
				authProvider.UpdatedOn = DateTime.Now;

				var result = await _authProviderRepository.AddAsync(authProvider);

				if (result.State.Equals(EntityState.Added))
				{
					await _authProviderRepository.UnitOfWork.SaveChangesAsync();

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
				_logger.Error("AuthProviderBusiness", "Save", ex.Message, ex.StackTrace);
				returnResult.Success = false;
				returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
				return returnResult;
			}
		}
		/// <summary>
		/// This Method update the given authprovider details
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="authProviderMaster"></param>
		/// <returns>success or failure</returns>
		public async Task<ReturnResult> Update(string userName, AuthProviderMaster authProviderMaster)
		{
			ReturnResult returnResult = new ReturnResult();

			try
			{
				var updateAuthProvider = await _authProviderRepository.SelectFirstOrDefaultAsync(a => a.ProviderID.Equals(authProviderMaster.ProviderID));

				if (updateAuthProvider == null)
				{
					returnResult.Result = $"{ResourceInformation.GetResValue("AuthProvider")} { ResourceInformation.GetResValue("ProvideValidDetails")}";
					return returnResult;
				}

				if (authProviderMaster.IsActive == null)
				{
					returnResult.Success = false;
					returnResult.Result = ResourceInformation.GetResValue("IsActiveRequired");
					return returnResult;
				}

				updateAuthProvider.ProviderName = authProviderMaster.ProviderName;
				updateAuthProvider.IsActive = authProviderMaster.IsActive;
				updateAuthProvider.ProviderDescription = authProviderMaster.ProviderDescription;
				updateAuthProvider.Configuration = authProviderMaster.Configuration;

				IUserRepository _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
				updateAuthProvider.UpdatedBy = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive == true)).UserId;
				updateAuthProvider.UpdatedOn = DateTime.Now;


				var result = await _authProviderRepository.UpdateAsync(updateAuthProvider);

				if (result.State.Equals(EntityState.Modified))
				{
					await _authProviderRepository.UnitOfWork.SaveChangesAsync();
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
				_logger.Error("AuthProviderBusiness", "Update", ex.Message, ex.StackTrace);
				returnResult.Success = false;
				returnResult.Result = $"{ResourceInformation.GetResValue("DataUpdateFailure")} : {ExceptionLogger.LogException(ex)}";
				return returnResult;
			}
		}
		/// <summary>
		/// This Method delete the given authprovider details
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="providerId"></param>
		/// <returns>success or failure</returns>
		public async Task<ReturnResult> Delete(string userName, int providerId)
		{
			ReturnResult returnResult = new ReturnResult();

			try
			{
				var deleteAuthProvider = await _authProviderRepository.SelectFirstOrDefaultAsync(a => a.ProviderID == providerId && a.IsActive.Value);

				if (deleteAuthProvider == null)
				{
					returnResult.Result = $"{ResourceInformation.GetResValue("AuthProvider")} {ResourceInformation.GetResValue("NotExists")}";
					return returnResult;
				}

				deleteAuthProvider.IsActive = false;
				IUserRepository _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
				deleteAuthProvider.UpdatedBy = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive == true)).UserId;
				deleteAuthProvider.UpdatedOn = DateTime.Now;
				var result = await _authProviderRepository.UpdateAsync(deleteAuthProvider);

				if (result.State.Equals(EntityState.Modified))
				{
					await _authProviderRepository.UnitOfWork.SaveChangesAsync();
					returnResult.Success = true;
					returnResult.Result = ResourceInformation.GetResValue("DataDeleteSuccess");
				}
				else
				{
					returnResult.Success = false;
					returnResult.Result = ResourceInformation.GetResValue("DataDeleteFailure");
				}

				return returnResult;
			}
			catch (Exception ex)
			{
				_logger.Error("AuthProviderBusiness", "Delete", ex.Message, ex.StackTrace);
				returnResult.Success = false;
				returnResult.Result = $"{ResourceInformation.GetResValue("DataDeleteFailure")} : {ExceptionLogger.LogException(ex)}";
				return returnResult;
			}
		}
		#endregion

		#region Dispose
		/// <summary>
		/// Method to dispose by parameter.
		/// </summary>
		/// <param name="disposing"></param>
		/// 
		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;

			if (disposing)
			{
				_authProviderRepository.Dispose();
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

using EVA.EIMS.Common;
using EVA.EIMS.Common.Constants;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVA.EIMS.Business
{
  public class RoleBusiness : IRoleBusiness
  {
    #region Private Variable
    private readonly IRoleRepository _roleRepository;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;
    private bool _disposed;
    #endregion

    #region Constructor
    public RoleBusiness(IServiceProvider serviceProvider, IRoleRepository roleRepository, ILogger logger)
    {
      _roleRepository = roleRepository;
      _serviceProvider = serviceProvider;
      _disposed = false;
      _logger = logger;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// To get all Role details
    /// </summary>
    /// <returns>Returns a list of role details</returns>
    public async Task<ReturnResult> GetRole()
    {
      ReturnResult returnResult = new ReturnResult();
      try
      {
        var roleList = await _roleRepository.SelectAsync(r => r.IsActive.Value);
        if (roleList != null && roleList.Count() > 0)
        {
          returnResult.Success = true;
          returnResult.Data = roleList;
          return returnResult;
        }
        else
        {
          returnResult.Success = false;
          returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                                $"{ResourceInformation.GetResValue("Role")} " +
                                $"{ ResourceInformation.GetResValue("NotExists")}";
          return returnResult;
        }
      }
      catch (Exception ex)
      {
        _logger.Error("RoleBusiness", "GetRole", ex.Message, ex.StackTrace);
        returnResult.Success = false;
        returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
        return returnResult;
      }
    }
    /// <summary>
    /// To get Role details by roleId
    /// </summary>
    /// <param name="roleId">roleId</param>
    /// <returns>Returns single role detail</returns>
    public async Task<ReturnResult> GetRole(int roleId)
    {
      ReturnResult returnResult = new ReturnResult();
      try
      {
        var requiredRole = await _roleRepository.SelectFirstOrDefaultAsync(r => r.RoleId.Equals(roleId) && r.IsActive.Value);
        if (requiredRole != null)
        {
          returnResult.Success = true;
          returnResult.Data = requiredRole;
          return returnResult;
        }
        else
        {
          returnResult.Success = false;
          returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")} " +
                                $"{ResourceInformation.GetResValue("Role")} " +
                                $"{ ResourceInformation.GetResValue("NotExists")}";
          return returnResult;
        }
      }
      catch (Exception ex)
      {
        _logger.Error("RoleBusiness", "GetRole", ex.Message, ex.StackTrace);
        returnResult.Success = false;
        returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
        return returnResult;
      }
    }

    /// <summary>
    /// To get Role details by roleId
    /// </summary>
    /// <param name="roleId">roleId</param>
    /// <returns>Returns single role detail</returns>
    public async Task<ReturnResult> GetRoleByRoleName(string roleName)
    {
      ReturnResult returnResult = new ReturnResult();
      try
      {
        var requiredRole = await _roleRepository.SelectFirstOrDefaultAsync(r => r.RoleName.Equals(roleName,StringComparison.OrdinalIgnoreCase));
        if (requiredRole != null)
        {
          returnResult.Success = true;
          returnResult.Data = requiredRole;
          return returnResult;
        }
        else
        {
          returnResult.Success = false;
          returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")} " +
                                $"{ResourceInformation.GetResValue("Role")} " +
                                $"{ ResourceInformation.GetResValue("NotExists")}";
          return returnResult;
        }
      }
      catch (Exception ex)
      {
        _logger.Error("RoleBusiness", "GetRoleByRoleName", ex.Message, ex.StackTrace);
        returnResult.Success = false;
        returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
        return returnResult;
      }
    }

    /// <summary>
    /// To save new role
    /// </summary>
    /// <param name="role">role</param>
    /// <param name="tokenData">tokenData</param>
    /// <returns>Returns responce message</returns>
    public async Task<ReturnResult> SaveRole(string userName, Role role)
    {
      ReturnResult returnResult = new ReturnResult();
      try
      {
        if (role == null)
        {
          returnResult.Result = ResourceInformation.GetResValue("ProvideRoleDetails");
          return returnResult;
        }

        if ((await _roleRepository.SelectFirstOrDefaultAsync(r => r.RoleName.Equals(role.RoleName) && !r.IsActive.Value)) != null)
        {
          returnResult.Success = false;
          returnResult.Result = ResourceInformation.GetResValue("ExistsAndInActive");
          return returnResult;
        }

        if ((await _roleRepository.SelectFirstOrDefaultAsync(r => r.RoleName.Equals(role.RoleName) && r.IsActive.Value)) != null)
        {
          returnResult.Result = ResourceInformation.GetResValue("AlreadyExist");
          returnResult.Success = false;
          return returnResult;
        }
        // Check Multiple Organization Access should not be null 
        if (role.MultipleOrgAccess == null)
        {
          returnResult.Success = false;
          returnResult.Result = ResourceInformation.GetResValue("MultipleOrgAccessRequired");
          return returnResult;
        }

        if (role.IsActive == null)
        {
          role.IsActive = true;
        }

        IUserRepository _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        Guid userId = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive == true)).UserId;

        role.CreatedBy = userId;
        role.CreatedOn = DateTime.Now;
        role.ModifiedBy = userId;
        role.ModifiedOn = DateTime.Now;

        var result = await _roleRepository.AddAsync(role);

        if (result.State.Equals(EntityState.Added))
        {
          await _roleRepository.UnitOfWork.SaveChangesAsync();

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
        _logger.Error("RoleBusiness", "SaveRole", ex.Message, ex.StackTrace);
        returnResult.Success = false;
        returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
        return returnResult;
      }
    }

    /// <summary>
    /// To Update existing Role
    /// </summary>
    /// <param name="tokenData">tokenData</param>
    /// <param name="roleId">roleId</param>
    /// <param name="role">role</param>
    /// <returns>Returns responce message</returns>
    public async Task<ReturnResult> UpdateRole(string userName, int roleId, Role roleObj)
    {
      ReturnResult returnResult = new ReturnResult();
      try
      {
        if (roleObj == null)
        {
          returnResult.Result = ResourceInformation.GetResValue("ProvideRoleDetails");
          return returnResult;
        }

        if (roleId == 0)
        {
          returnResult.Success = false;
          returnResult.Result = ResourceInformation.GetResValue("ProvideRoleId");
          return returnResult;
        }
        //TrimString.TrimObjectStringValue(role);

        var updateRole = await _roleRepository.SelectFirstOrDefaultAsync(r => r.RoleId == roleId);

        if (updateRole == null)
        {
          returnResult.Result = ResourceInformation.GetResValue("NotExists");
          return returnResult;
        }

        if (string.IsNullOrEmpty(roleObj.RoleName))
        {
          returnResult.Success = false;
          returnResult.Result = ResourceInformation.GetResValue("ProvideRoleName");
        }

        if (roleObj.MultipleOrgAccess == null)
        {
          returnResult.Success = false;
          returnResult.Result = ResourceInformation.GetResValue("MultipleOrgAccessRequired");
          return returnResult;
        }

        if (roleObj.IsActive == null)
        {
          returnResult.Success = false;
          returnResult.Result = ResourceInformation.GetResValue("IsActiveRequired");
          return returnResult;
        }

        if (updateRole.RoleId != roleObj.RoleId)
        {
          returnResult.Result = $"{ResourceInformation.GetResValue("MismatchRole")} ";
          return returnResult;
        }
        IUserRepository _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();

        updateRole.RoleName = roleObj.RoleName;
        updateRole.MultipleOrgAccess = roleObj.MultipleOrgAccess;
        updateRole.IsActive = roleObj.IsActive;
        updateRole.Description = roleObj.Description;
        updateRole.ModifiedBy = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive == true)).UserId;
        updateRole.ModifiedOn = DateTime.Now;

        var result = await _roleRepository.UpdateAsync(updateRole);

        if (result.State.Equals(EntityState.Modified))
        {
          await _roleRepository.UnitOfWork.SaveChangesAsync();
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
        _logger.Error("RoleBusiness", "UpdateRole", ex.Message, ex.StackTrace);
        returnResult.Success = false;
        returnResult.Result = $"{ResourceInformation.GetResValue("DataUpdateFailure")} : {ExceptionLogger.LogException(ex)}";
        return returnResult;
      }

    }

    /// <summary>
    /// To Delete Role by roleId
    /// </summary>
    /// <param name="roleId">roleId</param>
    /// <returns>Returns responce message</returns>
    public async Task<ReturnResult> DeleteRole(string userName, int roleId)
    {
      ReturnResult returnResult = new ReturnResult();
      try
      {
        var deleteRole = await _roleRepository.SelectFirstOrDefaultAsync(r => r.RoleId.Equals(roleId) && r.IsActive.Value);

        if (deleteRole == null)
        {
          returnResult.Result =ResourceInformation.GetResValue("NotExists");
          return returnResult;
        }
        if (deleteRole.RoleName == UserRoles.SiteUser.ToString() || deleteRole.RoleName == UserRoles.SiteAdmin.ToString() || deleteRole.RoleName == UserRoles.SuperAdmin.ToString())
        {
          returnResult.Result = ResourceInformation.GetResValue("CanNotDeleteRole");
          return returnResult;
        }
        IUserRepository _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        deleteRole.ModifiedBy = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive == true)).UserId;
        deleteRole.ModifiedOn = DateTime.Now;
        deleteRole.IsActive = false;
        var result = await _roleRepository.UpdateAsync(deleteRole);

        if (result.State.Equals(EntityState.Modified))
        {
          await _roleRepository.UnitOfWork.SaveChangesAsync();
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
        _logger.Error("RoleBusiness", "DeleteRole", ex.Message, ex.StackTrace);
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
        _roleRepository.Dispose();
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

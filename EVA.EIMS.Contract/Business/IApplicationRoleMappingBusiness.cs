using EVA.EIMS.Common;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
    public interface IApplicationRoleMappingBusiness : IDisposable
    {
        Task<ReturnResult> GetApplicationRoles(int appId);
        Task<ReturnResult> SaveApplicationRoles(string userName, ApplicationRoleMapping applicationRoleMapping);
        Task<ReturnResult> DeleteApplicationRoles(string userName, int applicationRoleId);
        Task<ReturnResult> GetAllApplicationRoles(int appId);
        Task<ReturnResult> GetApplicationRolesById(int applicationRoleId);
        Task<ReturnResult> GetAllRolesByApplicationName(string appName);
        Task<ReturnResult> SaveApplicationRolesMapping(string userName, ApplicationRoleViewModel applicationRole);
        Task<ReturnResult> UpdateApplicationRoles(string userName, ApplicationRoleModel applicationRoleModel);
    }
}

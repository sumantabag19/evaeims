using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ComplexEntities;
using EVA.EIMS.Entity.ViewModel;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EVA.EIMS.Data
{ /// <summary>
  /// DataContext class for Entity Framework.
  /// </summary>
    public class DataContext : DbContext
    {
        #region Constructor
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DataContext() { }
        #endregion

        #region Register Entity
        public virtual DbSet<DeviceModel> DeviceModel { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<OauthClient> OauthClient { get; set; }
        public virtual DbSet<ClientApplicationDetails> ClientApplicationDetails { get; set; }
        public virtual DbSet<ClientType> ClientType { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<UserRoleMapping> UserRoleMapping { get; set; }
        public virtual DbSet<UserClientTypeMapping> UserClientTypeMapping { get; set; }
        public virtual DbSet<UserOTP> UserOTP { get; set; }
        public virtual DbSet<UserAnswer> UserAnswer { get; set; }
        public virtual DbSet<RefreshToken> RefreshToken { get; set; }
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<Language> Language { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplate { get; set; }
        public virtual DbSet<Device> Device { get; set; }
        public virtual DbSet<Application> Application { get; set; }
        public virtual DbSet<ApplicationRoleMapping> ApplicationRoleMapping { get; set; }
        public virtual DbSet<PurchaseorderSaleDetails> PurchaseorderSaleDetails { get; set; }
        public virtual DbSet<SecurityQuestion> SecurityQuestion { get; set; }
        public virtual DbSet<ApplicationUserDetails> ApplicationUserDetails { get; set; }
        public virtual DbSet<OrganizationTenantMappingDomainModel> OrganizationTenantMapping { get; set; }
        public virtual DbSet<OrganizationTenantMappingModel> OrganizationTenantMappingModel { get; set; }
        public virtual DbSet<OrganizationApplicationDetailModel> OrganizationApplicationDetailModel { get; set; }
        public virtual DbSet<OrganizationApplicationmapping> OrganizationApplicationmapping { get; set; }
        public virtual DbSet<ApplicationUserMapping> ApplicationUserMapping { get; set; }
        public virtual DbSet<OrganizationApplicationMappingDetails> OrganizationApplicationMappingDetails { get; set; }
        public virtual DbSet<GetUserQuestions> GetUserQuestions { get; set; }
        public virtual DbSet<UserAnswer> UserAnswers { get; set; }
        public virtual DbSet<PasswordHistory> PasswordHistory { get; set; }
        public virtual DbSet<LockAccount> LockAccount { get; set; }
        public virtual DbSet<ForgotPasswordFlowManagement> ForgotPasswordFlowManagement { get; set; }
        public virtual DbSet<IPTable> IPTable { get; set; }
        public virtual DbSet<SendEmailPwdExpNotify> MaximumNotifcationDays { get; set; }
        public virtual DbSet<IPAddressRange> IPAddress { get; set; }
        public virtual DbSet<AuthProviderMaster> AuthProviderMaster { get; set; }
        public virtual DbSet<OrganizationApplicationDetails> OrganizationApplicationDetails { get; set; }
        public virtual DbSet<UserApplicationDetails> UserApplicationDetails { get; set; }
        public virtual DbSet<IMSLogOutToken> IMSLogOutToken { get; set; }
        public virtual DbSet<OTPType> OTPType { get; set; }


        #region Role Base Access Entities
        public virtual DbSet<AccessType> AccessType { get; set; }
        public virtual DbSet<Module> Module { get; set; }
        public virtual DbSet<Actions> Actions { get; set; }
        public virtual DbSet<RoleModuleAccess> RoleModuleAccess { get; set; }
        public virtual DbSet<ClientTypeModuleAccess> ClientTypeModuleAccess { get; set; }
        public virtual DbSet<RoleAccessException> RoleAccessException { get; set; }
        public virtual DbSet<ClientTypeAccessException> ClientTypeAccessException { get; set; }
        public virtual DbSet<UserOrganizationMapping> UserOrganizationMapping { get; set; }


        #endregion

        #endregion

        #region Register View Model
        public virtual DbSet<UserDetails> UserDetails { get; set; }
        public virtual DbSet<RoleModel> RoleModel { get; set; }
        public virtual DbSet<UserModel> UserModel { get; set; }
        public virtual DbSet<AccessPermission> AccessPermission { get; set; }
        public virtual DbSet<OauthClientModel> OauthClientModel { get; set; }
        public virtual DbSet<ApplicationModel> ApplicationModel { get; set; }
        public virtual DbSet<ApplicationRoleModel> ApplicationRoleModel { get; set; }
        public virtual DbSet<OrganizationApplicationModel> OrganizationApplicationModel { get; set; }
        public virtual DbSet<UserModelForUI> UserModelForUI { get; set; }
        #endregion

        #region Register ComplexTypes
        public virtual DbSet<RefreshTokenCountModel> RefreshTokenCountModel { get; set; }
        public virtual DbSet<proc_GetAllProductsSales> proc_GetAllProductsSales { get; set; }
        public virtual DbSet<SecurityAnswerFromUserModel> SecurityAnswerFromUserModels { get; set; }
        public virtual DbSet<LockedUsers> LockedUsers { get; set; }
        public virtual DbSet<PasswordManagementDetails> PasswordManagementDetails { get; set; }
        public virtual DbSet<ModuleAccessDetails> RoleModuleAccessDetails { get; set; }
        public virtual DbSet<UserOrgAppDetails> OrganizationApplicationmappings { get; set; }

        #endregion

        #region AuditDBSet        
        public DbSet<Audit> z_AuditsJan { get; set; }
        public DbSet<Audit> z_AuditsFeb { get; set; }
        public DbSet<Audit> z_AuditsMar { get; set; }
        public DbSet<Audit> z_AuditsApr { get; set; }
        public DbSet<Audit> z_AuditsMay { get; set; }
        public DbSet<Audit> z_AuditsJun { get; set; }
        public DbSet<Audit> z_AuditsJul { get; set; }
        public DbSet<Audit> z_AuditsAug { get; set; }
        public DbSet<Audit> z_AuditsSept { get; set; }
        public DbSet<Audit> z_AuditsOct { get; set; }
        public DbSet<Audit> z_AuditsNov { get; set; }
        public DbSet<Audit> z_AuditsDec { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<LockAccount>()
              .HasKey(c => new { c.UserId, c.LockTypeId });
            string tblName = Enum.GetName(typeof(MonthTables), DateTime.Today.Month - 1);
            modelBuilder.Entity<Audit>().ToTable(tblName);
        }

        #region Security Audit Log           
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            var auditEntries = OnBeforeSaveChanges();
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            await OnAfterSaveChanges(auditEntries);
            return result;
        }
        public override int SaveChanges()
        {
            var auditEntries = OnBeforeSaveChanges();
            var result = base.SaveChanges();
            OnAfterSaveChanges(auditEntries);
            return result;
        }
        /// <summary>
        /// This method will run before save changes and will check for any data modification happend or not. if yes
        /// then it will create audit log record to save it is respective audit log table
        /// </summary>
        /// <returns>retruns modified data entries</returns>
        private List<AuditEntry> OnBeforeSaveChanges()
        {
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Audit || entry.Entity is RefreshToken || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry(entry);
                foreach (var property in entry.Properties)
                {
                    if (property.IsTemporary)
                    {
                        // value will be generated by the database, get the value after saving
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                if (Convert.ToString(property.OriginalValue) == Convert.ToString(property.CurrentValue))
                                    break;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
                auditEntry.TableName = entry.Metadata.Relational().TableName;
                auditEntries.Add(auditEntry);
            }

            // Save audit entities that have all the modifications
            //foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
            //{
            AuditAsPerMonthOfTheYear(auditEntries.Where(_ => !_.HasTemporaryProperties).ToList());
            //}

            // keep a list of entries where the value of some properties are unknown at this step
            return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
        }
        /// <summary>
        /// This method checks if there is any modification happen dynamically on db side if yes then it will create log entry 
        /// and add it is respective log table
        /// </summary>
        /// <param name="auditEntries"></param>
        /// <returns>retrun task details</returns>
        private Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return Task.CompletedTask;

            foreach (var auditEntry in auditEntries)
            {
                // Get the final value of the temporary properties
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }

                // Save the Audit entry
                //AuditAsPerMonthOfTheYear(auditEntry);
            }
            AuditAsPerMonthOfTheYear(auditEntries);
            return SaveChangesAsync();
        }
        /// <summary>
        /// This method select the appropriate log table as per the running month and save the audit log in that table
        /// </summary>
        /// <param name="auditEntry"></param>
        private void AuditAsPerMonthOfTheYear(IEnumerable<AuditEntry> auditEntryList)
        {
            List<Audit> lstAudit = new List<Audit>();
            foreach (var auditentry in auditEntryList)
            {
                lstAudit.Add(auditentry.ToAudit());
            }

            switch (DateTime.Today.Month)
            {
                case 1:
                    z_AuditsJan.AddRange(lstAudit);
                    break;
                case 2:
                    z_AuditsFeb.AddRange(lstAudit);
                    break;
                case 3:
                    z_AuditsMar.AddRange(lstAudit);
                    break;
                case 4:
                    z_AuditsApr.AddRange(lstAudit);
                    break;
                case 5:
                    z_AuditsMay.AddRange(lstAudit);
                    break;
                case 6:
                    z_AuditsJun.AddRange(lstAudit);
                    break;
                case 7:
                    z_AuditsJul.AddRange(lstAudit);
                    break;
                case 8:
                    z_AuditsAug.AddRange(lstAudit);
                    break;
                case 9:
                    z_AuditsSept.AddRange(lstAudit);
                    break;
                case 10:
                    z_AuditsOct.AddRange(lstAudit);
                    break;
                case 11:
                    z_AuditsNov.AddRange(lstAudit);
                    break;
                case 12:
                    z_AuditsDec.AddRange(lstAudit);
                    break;
            }
        }
        #endregion
    }
}


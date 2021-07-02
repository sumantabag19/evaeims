using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVA.EIMS.Business
{
    public class LanguageBusiness : ILanguageBusiness
    {
        #region Private variables
        private readonly ILanguageRepository _languageRepository;
        private readonly ILogger _logger;
        private bool _disposed;
        #endregion

        #region Constructor
        public LanguageBusiness(ILanguageRepository languageRepository, ILogger logger)
        {
            _languageRepository = languageRepository;
            _logger = logger;
            _disposed = false;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method is used get the languages.
        /// </summary>
        /// <returns>returns multiple languages details</returns>
        public async Task<ReturnResult> Get()
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var listOfLanguages = await _languageRepository.SelectAllAsync();
                if (listOfLanguages != null && listOfLanguages.Count() > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = listOfLanguages;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                                          $"{ResourceInformation.GetResValue("Language")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("LanguageBusiness", "Get", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used get the language details by id
        /// </summary>
        /// <param name="languageId">languageId</param>
        /// <returns>returns single language details</returns>
        public async Task<ReturnResult> GetById(Guid languageId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var requiredLanguageInfo = await _languageRepository.SelectFirstOrDefaultAsync(l => l.LanguageId == languageId);
                if (requiredLanguageInfo != null)
                {
                    returnResult.Success = true;
                    returnResult.Data = requiredLanguageInfo;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                        $" {ResourceInformation.GetResValue("Language")} " +
                        $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

            }
            catch (Exception ex)
            {
                _logger.Error("LanguageBusiness", "GetById", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used get the language details by code
        /// </summary>
        /// <param name="languageCode">languageCode</param>
        /// <returns>returns single language details</returns>
        public async Task<ReturnResult> GetByCode(string languageCode)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var requiredLanguageInfo = await _languageRepository.SelectFirstOrDefaultAsync(l => l.LanguageCode == languageCode);
                if (requiredLanguageInfo != null)
                {
                    returnResult.Success = true;
                    returnResult.Data = requiredLanguageInfo;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                        $" {ResourceInformation.GetResValue("Language")} {languageCode} " +
                        $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("LanguageBusiness", "GetByCode", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to save the language details
        /// </summary>
        /// <param name="language">language object</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> Save(Language language)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var result = await _languageRepository.AddAsync(language);

                if (result.State.Equals(EntityState.Added))
                {
                    await _languageRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("LangaugeBusiness", "Save", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to delete the language details
        /// </summary>
        /// <param name="languageId">languageId</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> Delete(Guid languageId)
        {
            ReturnResult returnResult = new ReturnResult();

            try
            {
                var deleteLangauge = await _languageRepository.SelectFirstOrDefaultAsync(l => l.LanguageId == languageId);

                if (deleteLangauge == null)
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("Language")} {ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                var result = await _languageRepository.DeleteAsync(deleteLangauge);

                if (result.State.Equals(EntityState.Deleted))
                {
                    await _languageRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("LangaugeBusiness", "Delete", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataDeleteFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to update the lanaguage details
        /// </summary>
        /// <param name="languageId">languageId</param>
        /// <param name="language">language object</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> Update(Guid languageId, Language language)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var updateLanguage = await _languageRepository.SelectFirstOrDefaultAsync(l => l.LanguageId.Equals(languageId));

                if (updateLanguage == null)
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("Language")} { ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                updateLanguage.LanguageCode = language.LanguageCode;
                updateLanguage.LanguageName = language.LanguageName;

                var result = await _languageRepository.UpdateAsync(updateLanguage);
                if (result.State.Equals(EntityState.Modified))
                {
                    await _languageRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("LanguageBusiness", "Update", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataUpdateFailure")} : {ExceptionLogger.LogException(ex)}";
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
                _languageRepository.Dispose();
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

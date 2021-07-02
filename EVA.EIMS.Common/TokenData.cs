using EVA.EIMS.Common.Constants;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EVA.EIMS.Common
{
	/// <summary>
	/// TokenData can be used to process token provided by client
	/// </summary>
	public class TokenData
    {
        #region Constuctor
        public TokenData()
        {
            ClientTypeId = 0;
            UserClientTypes = new string[] { };
            Role = new string[] { };
            UserName = string.Empty;
        }
        #endregion

        #region Public Properties
        public string OrgId { get; set; }
        public int ClientTypeId { get; set; }
        public string[] Role { get; set; }
        public string UserName { get; set; }
        public string[] UserClientTypes { get; set; }
        public string ClientId { get; set; }

        public DateTime TokenValidationPeriod { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// This method is used to token data from RouteData from HttpContext
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns>TokenData object</returns>
        public static TokenData GetRequestContextRouteData(HttpContext context)
        {
            var clientTypeId = 0;
            
            if (context.Items.ContainsKey(KeyConstant.Client_Type))
            {
                if (Enum.TryParse(context.Items[KeyConstant.Client_Type].ToString(), out ClientTypes choice))
                    clientTypeId = (int)choice;
            }

            var tokenData = new TokenData
            {
                OrgId = context.Items.Any(v => v.Key.Equals(KeyConstant.OrgId)) ? context.Items[KeyConstant.OrgId].ToString() : string.Empty,
                ClientTypeId = clientTypeId,
                UserName = context.Items.Any(v => v.Key.Equals(KeyConstant.UserName)) ? context.Items[KeyConstant.UserName].ToString() : string.Empty,
                ClientId = context.Items.Any(v => v.Key.Equals(KeyConstant.ClientId)) ? context.Items[KeyConstant.ClientId].ToString() : string.Empty,
            };

            if (context.Items.Any(v => v.Key.Equals(KeyConstant.Role)))
            {
                List<string> role = new List<string>();
                foreach (var item in (List<string>)context.Items[KeyConstant.Role])
                {
                    role.Add(item);
                }
                tokenData.Role = role.ToArray();
            }

        
            if(context.Items.Any(v => v.Key.Equals(KeyConstant.Client_Type)))
            {
                List<string> client_type = new List<string>();
                foreach (var item in (List<string>)context.Items[KeyConstant.Client_Type])
                {
                    client_type.Add(item);
                }
                tokenData.UserClientTypes = client_type.ToArray();

            }
            return tokenData;



            //var routeData = context.GetRouteData();

            //if (routeData.Values.ContainsKey(KeyConstant.Client_Type))
            //{
            //    if (Enum.TryParse(routeData.Values[KeyConstant.Client_Type].ToString(), out ClientTypeEnum choice))
            //        clientTypeId = (int)choice;
            //}

            //var tokenData = new TokenData
            //{
            //    OrgId = routeData.Values.Any(v => v.Key.Equals(KeyConstant.OrgId)) ? routeData.Values[KeyConstant.OrgId].ToString() : string.Empty,
            //    ClientTypeId = clientTypeId,
            //    UserName = routeData.Values.Any(v => v.Key.Equals(KeyConstant.UserName)) ? routeData.Values[KeyConstant.UserName].ToString() : string.Empty,
            //};

            //if (routeData.Values.Any(v => v.Key.Equals(KeyConstant.Role)))
            //{
            //    var roles = routeData.Values[KeyConstant.Role];
            //    if (roles.ToString().Equals(UserRoles.SiteUser.ToString()) || roles.ToString().Equals(UserRoles.SiteAdmin.ToString()) ||
            //        roles.ToString().Equals(UserRoles.SuperAdmin.ToString()))
            //        tokenData.Role = new[] { roles.ToString() };
            //    else
            //        tokenData.Role = ((IEnumerable)routeData.Values[KeyConstant.Role]).Cast<object>()
            //            .Select(x => x.ToString())
            //            .ToArray();
            //    return tokenData;
            //}
            
        }
        #endregion

    }
}

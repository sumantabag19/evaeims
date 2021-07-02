//using EVA.EIMS.Business;
//using EVA.EIMS.Contract.Repository;
//using EVA.EIMS.Entity;
//using EVA.EIMS.Repository;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace AuthorizationServer.Api.Providers
//{
//    public class RefreshTokenProvider : IAuthenticationTokenProvider
//    {
//        //private static readonly Logger Log = LogManager.GetCurrentClassLogger();
//        private readonly IRefreshTokenRepository _refreshTokenRepository;
//        public IConfiguration Configuration { get; }
//        public void Create(AuthenticationTokenCreateContext context)
//        {
//            throw new NotImplementedException();
//        }
//        /// <summary>
//        /// This method is used to generate new refresh token
//        /// </summary>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public async Task CreateAsync(AuthenticationTokenCreateContext context)
//        {
//            try
//            {

//                var expiresUtc =
//                    DateTime.UtcNow.AddMinutes(
//                        Convert.ToInt32(Configuration["ApplicationSettings:RefreshTokenExpireTimeSpanInMinutes"]));
//                // copy all properties and set the desired lifetime of refresh token  
//                var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
//                {
//                    IssuedUtc = context.Ticket.Properties.IssuedUtc,
//                    ExpiresUtc = expiresUtc
//                };

//                var ticket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);

//                var ticketSerializer = new TicketSerializer();
//                var byteArray = ticketSerializer.Serialize(ticket);

//                var refreshToken = new RefreshToken
//                {
//                    RefreshTokenId = Guid.NewGuid(),
//                    RefreshAuthenticationTicket = byteArray,
//                    TokenExpirationDateTime = expiresUtc
//                };

//                var refreshTokenRepository = new RefreshTokenBusiness(_refreshTokenRepository);
//                refreshTokenRepository.Save(refreshToken);

//                //_refreshTokens.TryAdd(guid, new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties));
//                //Log.Info("Created new refresh token " + refreshToken.RefreshTokenGuid);
//                // consider storing only the hash of the handle  
//                context.SetToken(refreshToken.RefreshTokenId.ToString());
//            }
//            catch (Exception e)
//            {
//                // Log.Error("Error while Create new refresh token " + e.Message);
//            }
//        }

//        public void Receive(AuthenticationTokenReceiveContext context)
//        {
//            throw new NotImplementedException();
//        }

//        /// <summary>
//        /// This method is used to validate refresh token,remove the old token and generate new access token for every request
//        /// </summary>
//        /// <param name="context">context</param>
//        /// <returns></returns>
//        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
//        {
//            try
//            {
//                var refreshTokenRepository = new RefreshTokenBusiness(_refreshTokenRepository);
//                var refreshToken = refreshTokenRepository.Get(Guid.Parse(context.Token.Trim()));//.GetAwaiter().GetResult();
//                if (refreshToken != null)
//                {
//                    var ticketSerializer = new TicketSerializer();
//                    var ticket = ticketSerializer.Deserialize(refreshToken.RefreshAuthenticationTicket);
//                    //  Log.Info("Fetched refresh token " + context.Token.Trim());
//                    //refreshTokenRepository.Delete(refreshToken.RefreshTokenGuid);//.GetAwaiter().GetResult();
//                    context.SetTicket(ticket);
//                }
//            }
//            catch (Exception e)
//            {
//                // Log.Error("Fetch refresh token " + context.Token.Trim() + " : Exception: " + e.Message);
//            }
//        }
//    }
//}

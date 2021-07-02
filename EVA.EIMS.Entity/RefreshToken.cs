using System;
using System.ComponentModel.DataAnnotations;

namespace EVA.EIMS.Entity
{
    public class RefreshToken
    {
        [Key]
        public Guid RefreshTokenId { get; set; }
        public byte[] RefreshAuthenticationTicket { get; set; }
        public DateTime TokenExpirationDateTime { get; set; }
        public int AppId { get; set; }
        public Guid? UserId { get; set; }
        public int OrgId { get; set; }
        public string ClientId { get; set; }
    }
}

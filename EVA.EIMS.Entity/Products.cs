using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EVA.EIMS.Entity
{
    public class Products
    {
        [Key]
        public int productId { get; set; }
        public string productName { get; set; }
        public string productCode { get; set; }
        public string releaseDate { get; set; }
        public string description { get; set; }
        public decimal? price { get; set; }
        public decimal? starRating { get; set; }
        public string imageUrl { get; set; }
        [IgnoreDataMember]
        public string CreatedBy { get; set; }

        private DateTime? _createdDate;
        [IgnoreDataMember]
        public DateTime? CreatedDate
        {
            get
            {
                if (_createdDate == null)
                    return DateTime.Now;
                else
                    return _createdDate;
            }
            set { _createdDate = value; }
        }
        [IgnoreDataMember]
        public string ModifiedBy { get; set; }

        private DateTime? _modifiedBy;
        [IgnoreDataMember]
        public DateTime? ModifiedDate
        {
            get
            {
                if (_modifiedBy == null)
                    return DateTime.Now;
                else
                    return _modifiedBy;
            }
            set { _createdDate = value; }
        }

        public List<PurchaseorderSaleDetails> PurchaseorderSaleDetails { get; set; }
    }
}

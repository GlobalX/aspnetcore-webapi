using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Models
{
    public class Book : BaseEntity
    {
        public String Title { get; set; }
        public DateTime Year { get; set; }

        [ForeignKey("tenants")]
        [Column("TenantId")]
        public Guid TenantId { get; set; }
        
        [ForeignKey("authors")]
        [Column("AuthorId")]
        public Guid? AuthorId { get; set; }
    }
}
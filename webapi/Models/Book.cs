using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Models
{
    public class Book : BaseEntity
    {
        public long BookNumber { get; set; }
        public String Title { get; set; }
        public DateTime Year { get; set; }

        [ForeignKey("tenants")]
        [Column("TenantId")]
        public Guid TenantId { get; set; }
        
        [ForeignKey("authors")]
        [Column("AuthorId")]
        public int AuthorId { get; set; }
    }
}
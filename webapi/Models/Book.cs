using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Models
{
    public class Book : BaseEntity
    {
        public String Title { get; set; }
        public DateTime Year { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}
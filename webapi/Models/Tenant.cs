using System;

namespace webapi.Models
{
    public class Tenant : BaseEntity
    {
        public string Name { get; set; }
        
        /// <summary>
        /// Are they active or not?
        /// </summary>
        public string Status { get; set; }
    }
}
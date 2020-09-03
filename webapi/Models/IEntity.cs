using System;

namespace webapi.Models
{
    public interface IEntity
    {
        public Guid Id { get; set; }
    }
}
using System;

namespace webapi.Models
{
    public class BaseSequenceEntity : ISequenceEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
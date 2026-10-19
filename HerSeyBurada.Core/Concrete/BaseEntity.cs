using System;

namespace HerSeyBurada.Core.Concrete
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
        public DateTime? GuncellenmeTarihi { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}
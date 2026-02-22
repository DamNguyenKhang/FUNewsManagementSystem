using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessObject.Entities
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}

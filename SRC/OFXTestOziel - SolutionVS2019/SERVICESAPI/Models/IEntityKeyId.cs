using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SERVICESAPI.Models
{
    public interface IEntityKeyId
    {
        long Id { get; set; }
    }
}

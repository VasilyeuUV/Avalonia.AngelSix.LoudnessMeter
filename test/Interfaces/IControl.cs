using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Interfaces
{
    public interface IControl
    {
        int? Key { get; }
        string? Description { get; }
        string? Type { get; }
        double? Height { get; }
    }
}

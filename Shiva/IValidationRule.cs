using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiva
{
    public interface IValidationRule
    {
        bool Validate(object value);
        string Message { get; }
    }
}

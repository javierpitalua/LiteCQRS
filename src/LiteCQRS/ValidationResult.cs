using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCQRS
{
    public class ValidationFailure
    {
        public string PropertyName { get; set; }
        public string ErrorDescription { get; set; }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Summary { get; set; }
        public IEnumerable<ValidationFailure> ValidationDetails { get; set; }
    }
}

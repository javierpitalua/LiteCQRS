using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCQRS
{
    public class CommandResult
    {
        public string OperationId { get; set; }
        public bool OperationSuccesful { get; set; }
        public ValidationResult ValidationResult { get; set; }
        public Dictionary<string, string> ExtendedProperties { get; set; }

        public CommandResult()
        {
            ValidationResult = new ValidationResult();
            ExtendedProperties = new Dictionary<string, string>();
        }
    }
}

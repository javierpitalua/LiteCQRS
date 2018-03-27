using System.Collections.Generic;

namespace IgniteCQRS
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

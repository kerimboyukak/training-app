using System;
using System.Runtime.Serialization;

namespace Domain
{
    /// <summary>
    /// Thrown when a certain contract or pre-condition is not met.
    /// Usually thrown by <see cref="Contracts"/> class.
    /// </summary>
    [Serializable]
    public class ContractException : Exception
    {
        public ContractException()
        {
        }

        public ContractException(string message)
            : base(message)
        {
        }

        public ContractException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
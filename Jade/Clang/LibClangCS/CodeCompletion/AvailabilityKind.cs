using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClang.CodeCompletion
{
    public enum AvailabilityKind
    {
        /// <summary>
        /// The entity is available.
        /// </summary>
        Available,

        /// <summary>
        /// The entity is available, but has been deprecated (and its use is not recommended).
        /// </summary>
        Deprecated,

        /// <summary>
        /// The entity is not available; any use of it will be an error.
        /// </summary>
        NotAvailable,
        
        /// <summary>
        /// The entity is available, but not accessible; any use of it will be an error.
        /// </summary>
        NotAccessible
    }
}

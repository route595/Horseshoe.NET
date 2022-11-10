using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Application
{
    /// <summary>
    /// The execution environment in which this app is running e.g. Development, Test, Production, etc. 
    /// May be assigned manually or in the configuration file.
    /// </summary>
    public enum AppMode
    {
        /// <summary>
        /// Applies mainly to active development efforts
        /// </summary>
        Development,

        /// <summary>
        /// Internal (possibly informal) testing environment
        /// </summary>
        Test,

        /// <summary>
        /// Information Assurance - security checks, vulnerability scanning, etc.
        /// </summary>
        IA,

        /// <summary>
        /// Quality Assurance - internal formal testing environment
        /// </summary>
        QA,

        /// <summary>
        /// User Acceptance Testing - formal (possibly external) testing environment
        /// </summary>
        UAT,

        /// <summary>
        /// Training - separate from (but supposedly identical to) production with separate data
        /// </summary>
        Training,

        /// <summary>
        /// Production environment
        /// </summary>
        Production
    }
}

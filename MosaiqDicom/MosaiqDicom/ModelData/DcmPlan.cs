// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DcmPlan.cs" company="Elekta">
//   Copy right by Elekta Shanghai Office.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MosaiqDicom.ModelData
{
    using System.Collections.Generic;

    /// <summary>The dcm plan.</summary>
    public class DcmPlan
    {
        /// <summary>The nb beams.</summary>
        public int NbBeams;

        /// <summary>Gets or sets the beam info.</summary>
        public List<BeamTreeInfo> BeamInfo { get; set; }

        /// <summary>Gets or sets the psz rt plan label.</summary>
        public string pszRTPlanLabel { get; set; }

        /// <summary>Gets or sets the rt plan date.</summary>
        public string RTPlanDate { get; set; }

        /// <summary>Gets or sets the rt plan name.</summary>
        public string RTPlanName { get; set; }

        /// <summary>Gets or sets the rt plan time.</summary>
        public string RTPlanTime { get; set; }
    }
}
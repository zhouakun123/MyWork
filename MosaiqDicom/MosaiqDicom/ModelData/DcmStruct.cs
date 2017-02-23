// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DcmStruct.cs" company="Elekta">
//   Copy right by Elekta Shanghai Office.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MosaiqDicom.ModelData
{
    #region Using

    using System.Collections.Generic;

    #endregion

    /// <summary>The dcm struct.</summary>
    public class DcmStruct
    {
        /// <summary>Gets or sets the nb structs.</summary>
        public int NbStructs { get; set; }

        /// <summary>Gets or sets the struct info.</summary>
        public List<StructTreeInfo> StructInfo { get; set; }
    }
}
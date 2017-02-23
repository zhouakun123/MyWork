// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BeamTreeInfo.cs" company="Elekta">
//   Copy right by Elekta Shanghai Office.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MosaiqDicom.ModelData
{
    /// <summary>The beam tree info.</summary>
    public class BeamTreeInfo
    {
        /// <summary>Gets or sets the name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the number.</summary>
        public int Number { get; set; }

        /// <summary>Gets or sets the referenced struct.</summary>
        public string ReferencedStruct { get; set; }

        /// <summary>Gets or sets the sop instance uid.</summary>
        public string SOPInstanceUID { get; set; }
    }
}
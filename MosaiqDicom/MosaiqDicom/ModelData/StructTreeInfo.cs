// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructTreeInfo.cs" company="Elekta">
//   Copy right by Elekta Shanghai Office.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MosaiqDicom.ModelData
{
    /// <summary>The struct tree info.</summary>
    public class StructTreeInfo
    {
        /// <summary>Gets or sets the color.</summary>
        public int Color { get; set; }

        /// <summary>Gets or sets the date.</summary>
        public string Date { get; set; }

        /// <summary>Gets or sets a value indicating whether is marker.</summary>
        public bool IsMarker { get; set; }

        /// <summary>Gets or sets the name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the number.</summary>
        public int Number { get; set; }

        /// <summary>Gets or sets the referenced frame of reference.</summary>
        public string ReferencedFrameOfReference { get; set; }

        /// <summary>Gets or sets the referenced series.</summary>
        public string ReferencedSeries { get; set; }

        /// <summary>Gets or sets the sop instance uid.</summary>
        public string SOPInstanceUID { get; set; }

        /// <summary>Gets or sets the time.</summary>
        public string Time { get; set; }
    }
}
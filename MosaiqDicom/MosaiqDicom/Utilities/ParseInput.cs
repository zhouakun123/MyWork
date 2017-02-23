// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParseInput.cs" company="Elekta">
//   Copy right by Elekta Shanghai Office.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MosaiqDicom.Utilities
{
    #region Using

    using MosaiqDicom.ModelData;

    #endregion

    /// <summary>The parse input.</summary>
    public static class ParseInput
    {
        /// <summary>The cache get referenced scan url for beam.</summary>
        /// <param name="selInfo">The sel info.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static SelInfo CacheGetReferencedScanUrlForBeam(SelInfo selInfo)
        {
            return selInfo;
        }

        /// <summary>The parse dicom info.</summary>
        /// <param name="dcmInfo">The dcm info.</param>
        /// <param name="directoryName">The directory name.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public static int ParseDicomInfo(DcmInfo dcmInfo, string directoryName)
        {
            return 0;
        }

        /// <summary>The parse sel info.</summary>
        /// <param name="selInfo">The sel Info.</param>
        /// <param name="selection">The selection.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public static int ParseSelInfo(SelInfo selInfo, string selection)
        {
            return 0;
        }
    }
}
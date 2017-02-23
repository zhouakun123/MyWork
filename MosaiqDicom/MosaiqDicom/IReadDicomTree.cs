// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReadDicomTree.cs" company="Elekta">
//   Copy right by Elekta Shanghai Office.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MosaiqDicom
{
    #region Using

    using MosaiqDicom.ModelData;
    using MosaiqDicom.Utilities;

    #endregion

    /// <summary>The ReadDicomTree interface.</summary>
    public interface IReadDicomTree
    {
        /// <summary>Gets or sets the dcm info.</summary>
        DcmInfo DcmInfo { get; set; }

        /// <summary>Gets or sets the param name.</summary>
        string ParamName { get; set; }

        /// <summary>Gets or sets the sel info.</summary>
        SelInfo SelInfo { get; set; }

        /// <summary>The read dicom tree.</summary>
        /// <param name="avs">The avs.</param>
        /// <param name="result">The result.</param>
        /// <returns>The <see cref="ErrorType"/>.</returns>
        ErrorType ReadDicomTree(out AVSfield avs, out string result);
    }
}
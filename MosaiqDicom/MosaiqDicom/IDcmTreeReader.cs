// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDcmTreeReader.cs" company="Elekta">
//   Copy right by Elekta Shanghai Office.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MosaiqDicom
{
    #region Using

    using MosaiqDicom.ModelData;
    using MosaiqDicom.Utilities;

    #endregion

    /// <summary>The DcmTreeReader interface.</summary>
    public interface IDcmTreeReader
    {
        /// <summary>The beam to world.</summary>
        /// <param name="avs">The avs.</param>
        /// <param name="selInfo">The sel info.</param>
        /// <returns>The <see cref="ErrorType"/>.</returns>
        ErrorType BeamToWorld(out AVSfield avs, SelInfo selInfo);

        /// <summary>The cache get xfm 1.</summary>
        /// <param name="avs">The avs.</param>
        /// <param name="selInfo">The sel info.</param>
        /// <returns>The <see cref="ErrorType"/>.</returns>
        ErrorType CacheGetXfm1(out AVSfield avs, SelInfo selInfo);

        /// <summary>The cache get xfm 2.</summary>
        /// <param name="avs">The avs.</param>
        /// <param name="selInfo">The sel info.</param>
        /// <returns>The <see cref="ErrorType"/>.</returns>
        ErrorType CacheGetXfm2(out AVSfield avs, SelInfo selInfo);

        /// <summary>The creat tree.</summary>
        /// <param name="avs">The avs.</param>
        /// <param name="dcmInfo">The dcm info.</param>
        /// <returns>The <see cref="ErrorType"/>.</returns>
        ErrorType CreateTree(out AVSfield avs, DcmInfo dcmInfo);

        /// <summary>The dcm get dicom to siddon xfm.</summary>
        /// <param name="avs">The avs.</param>
        /// <returns>The <see cref="int"/>.</returns>
        ErrorType DcmGetDicomToSiddonXfm(out AVSfield avs);

        /// <summary>The get key.</summary>
        /// <param name="result">The result.</param>
        /// <param name="dcmInfo">The dcm info.</param>
        /// <param name="selInfo">The sel info.</param>
        /// <param name="paramName">The param Name.</param>
        /// <returns>The <see cref="ErrorType"/>.</returns>
        ErrorType GetKey(out string result, DcmInfo dcmInfo, SelInfo selInfo, string paramName);

        /// <summary>The read full properties.</summary>
        /// <param name="avs">The avs.</param>
        /// <param name="dcmInfo">The dcm info.</param>
        /// <param name="selInfo">The sel info.</param>
        /// <returns>The <see cref="ErrorType"/>.</returns>
        ErrorType ReadFullProperties(out AVSfield avs, DcmInfo dcmInfo, SelInfo selInfo);

        /// <summary>The read keys.</summary>
        /// <param name="avs">The avs.</param>
        /// <param name="dcmInfo">The dcm info.</param>
        /// <param name="selInfo">The sel info.</param>
        /// <returns>The <see cref="ErrorType"/>.</returns>
        ErrorType ReadKeys(out AVSfield avs, DcmInfo dcmInfo, SelInfo selInfo);

        /// <summary>The read modality.</summary>
        /// <param name="avs">The avs.</param>
        /// <param name="dcmInfo">The dcm info.</param>
        /// <param name="selInfo">The sel info.</param>
        /// <param name="paramName">The param Name.</param>
        /// <returns>The <see cref="ErrorType"/>.</returns>
        ErrorType ReadModality(out AVSfield avs, DcmInfo dcmInfo, SelInfo selInfo, string paramName);

        /// <summary>The read ref beam.</summary>
        /// <param name="avs">The avs.</param>
        /// <param name="dcmInfo">The dcm info.</param>
        /// <param name="selInfo">The sel info.</param>
        /// <returns>The <see cref="ErrorType"/>.</returns>
        ErrorType ReadRefBeam(out AVSfield avs, DcmInfo dcmInfo, SelInfo selInfo);

        /// <summary>The structure to world.</summary>
        /// <param name="avs">The avs.</param>
        /// <param name="selInfo">The sel info.</param>
        /// <returns>The <see cref="ErrorType"/>.</returns>
        ErrorType StructureToWorld(out AVSfield avs, SelInfo selInfo);
    }
}
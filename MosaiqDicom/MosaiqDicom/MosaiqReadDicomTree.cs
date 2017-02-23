// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MosaiqReadDicomTree.cs" company="Elekta">
//   Copy right by Elekta Shanghai Office.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MosaiqDicom
{
    #region Using

    using System;

    using MosaiqDicom.ModelData;
    using MosaiqDicom.Utilities;

    #endregion

    /// <summary>The mosaiq read dicom tree.</summary>
    public class MosaiqReadDicomTree : IReadDicomTree
    {
        /// <summary>The _dcm tree reader.</summary>
        private readonly IDcmTreeReader _dcmTreeReader;

        /// <summary>The _error code.</summary>
        private ErrorType _errorCode;

        /// <summary>Initializes a new instance of the <see cref="MosaiqReadDicomTree"/> class.</summary>
        /// <param name="dcmTreeReader">The dcm Tree Reader.</param>
        /// <param name="directoryName">The directory name.</param>
        /// <param name="selection">The selection.</param>
        /// <param name="paramName">The param name.</param>
        public MosaiqReadDicomTree(
            IDcmTreeReader dcmTreeReader, 
            string directoryName, 
            string selection, 
            string paramName)
        {
            this._dcmTreeReader = dcmTreeReader;
            this.DcmInfo = new DcmInfo(out this._errorCode, directoryName);
            if (this._errorCode != 0)
            {
                return;
            }

            if (string.IsNullOrEmpty(selection))
            {
                this.SelInfo = new SelInfo(out this._errorCode, selection);
                if (this._errorCode != 0)
                {
                    return;
                }
            }

            this.ParamName = paramName.ToLower();
        }

        /// <summary>Gets or sets the dcm info.</summary>
        public DcmInfo DcmInfo { get; set; }

        /// <summary>Gets or sets the param name.</summary>
        public string ParamName { get; set; }

        /// <summary>Gets or sets the sel info.</summary>
        public SelInfo SelInfo { get; set; }

        /// <summary>The read dicom tree.</summary>
        /// <param name="avs">The avs.</param>
        /// <param name="result">The result.</param>
        /// <returns>The <see cref="int"/>.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public ErrorType ReadDicomTree(out AVSfield avs, out string result)
        {
            result = null;

            // throw new NotImplementedException();
            string szParam;
            string pSelCopy;
            string pTmp;
            string pszParamName;

            avs = new AVSfield();

            if (this._errorCode == 0)
            {
                return this._errorCode;
            }

            if (this.SelInfo == null)
            {
                /* Create a TREE */
                this._errorCode = this._dcmTreeReader.CreateTree(out avs, this.DcmInfo);
            }
            else if (string.IsNullOrEmpty(this.ParamName))
            {
                /* Read a modality */
                this._errorCode = this._dcmTreeReader.ReadModality(out avs, this.DcmInfo, this.SelInfo, null);
            }
            else
            {
                /* Return a property (AVSfield or a string */
                if (this.ParamName == "all")
                {
                    /* All properties in an AVSfield */
                    this._errorCode = this._dcmTreeReader.ReadKeys(out avs, this.DcmInfo, this.SelInfo);
                }
                else if (this.ParamName == "fullproperties")
                {
                    /* All properties in an AVSfield, including sequences */
                    this._errorCode = this._dcmTreeReader.ReadFullProperties(out avs, this.DcmInfo, this.SelInfo);
                }
                else if (this.ParamName == "worldxfm" || this.ParamName == "bostonscantosiddonpatient")
                {
                    /* XFM in an AVSfield */
                    if ((this.SelInfo.Modality == ModalityType.DCM_MODALITY_SCAN /*&& SelInfo.bMfScan == 0 */)
                        || (this.SelInfo.Modality == ModalityType.DCM_MODALITY_DOSE /*&& SelInfo.bMfScan == 0 */))
                    {
                        this._errorCode = this._dcmTreeReader.CacheGetXfm1(out avs, this.SelInfo);
                        if (this._errorCode != 0)
                        {
                            this._errorCode = this._dcmTreeReader.ReadModality(
                                out avs, 
                                this.DcmInfo, 
                                this.SelInfo, 
                                null);
                            if (this._errorCode == 0 /*DCM_E_OK*/)
                            {
                                this._errorCode = this._dcmTreeReader.CacheGetXfm1(out avs, this.SelInfo);

                                /*if (*ppOut)
                                {
                                    rc = DCM_E_OK;
                                }
                                else
                                {
                                    rc = DCM_E_NO_XFM;
                                }*/
                            }
                        }
                    }
                    else if (this.SelInfo.Modality == ModalityType.DCM_MODALITY_BEAM)
                    {
                        this._errorCode = this._dcmTreeReader.BeamToWorld(out avs, this.SelInfo);
                        if (this._errorCode == ErrorType.DCM_E_INTERNAL)
                        {
                            // Meaning no cached referenced xfm, here!!
                            AVSfield patientTree;
                            AVSfield Scan_Field;

                            this._errorCode = this._dcmTreeReader.CreateTree(out patientTree, this.DcmInfo);
                            if (this._errorCode == ErrorType.DCM_E_OK)
                            {
                                var scanUrl = ParseInput.CacheGetReferencedScanUrlForBeam(this.SelInfo);
                                this._errorCode = this._dcmTreeReader.ReadModality(
                                    out Scan_Field, 
                                    this.DcmInfo, 
                                    scanUrl, 
                                    null);
                                this._errorCode = this._dcmTreeReader.BeamToWorld(out avs, this.SelInfo);
                            }
                        }

                        if (this._errorCode == ErrorType.DCM_E_NO_XFM)
                        {
                            // Meaning no cached xfm of beam itself, here!!
                            AVSfield Beam_Field;
                            this._errorCode = this._dcmTreeReader.ReadModality(
                                out Beam_Field, 
                                this.DcmInfo, 
                                this.SelInfo, 
                                null);
                            this._errorCode = this._dcmTreeReader.BeamToWorld(out avs, this.SelInfo);
                        }

                        /*if (!*ppOut)
                        {
                            rc = DCM_E_NO_XFM;
                        }*/
                    }
                    else if (this.SelInfo.Modality == ModalityType.DCM_MODALITY_STRUCTURE)
                    {
                        this._errorCode = this._dcmTreeReader.StructureToWorld(out avs, this.SelInfo);
                    }
                    else
                    {
                        this._errorCode = this._dcmTreeReader.DcmGetDicomToSiddonXfm(out avs);
                    }
                }
                else if (this.ParamName == "gantryxfm" || this.ParamName == "bostonscantosiddonworld")
                {
                    /* XFM in an AVSfield */
                    if ((this.SelInfo.Modality == ModalityType.DCM_MODALITY_SCAN)
                        || (this.SelInfo.Modality == ModalityType.DCM_MODALITY_DOSE)
                        || (this.SelInfo.Modality == ModalityType.DCM_MODALITY_BEAM))
                    {
                        this._errorCode = this._dcmTreeReader.CacheGetXfm2(out avs, this.SelInfo);
                        if (this._errorCode != 0)
                        {
                            this._errorCode = this._dcmTreeReader.ReadModality(
                                out avs, 
                                this.DcmInfo, 
                                this.SelInfo, 
                                null);
                            if (this._errorCode == ErrorType.DCM_E_OK)
                            {
                                this._errorCode = this._dcmTreeReader.CacheGetXfm2(out avs, this.SelInfo);

                                /* if (*ppOut)
                                {
                                    this._errorCode = DCM_E_OK;
                                }
                                else
                                {
                                    rc = DCM_E_NO_XFM;
                                }*/
                            }
                        }

                        // rc = DCM_E_INTERNAL;
                    }
                    else if (this.SelInfo.Modality == ModalityType.DCM_MODALITY_STRUCTURE)
                    {
                        if (this.SelInfo.Selection.Contains(".scan"))
                        {
                            var index = this.SelInfo.Selection.IndexOf(".scan") + 5;
                            using (
                                var tmpSel = new SelInfo(
                                    out this._errorCode, 
                                    this.SelInfo.Selection.Substring(0, index)))
                            {
                                /* Get the XFM of underlying scan */
                                this._errorCode = this._dcmTreeReader.CacheGetXfm2(out avs, tmpSel);
                            }
                        }
                        else
                        {
                            this._errorCode = ErrorType.DCM_E_INTERNAL;
                        }

                        /*if (*ppOut)
                            {
                                rc = DCM_E_OK;
                            }
                            else
                            {
                                rc = DCM_E_INTERNAL;
                            }*/
                    }
                    else
                    {
                        this._errorCode = this._dcmTreeReader.DcmGetDicomToSiddonXfm(out avs);
                    }
                }
                else if (this.ParamName == "refbeam")
                {
                    /* The referenced beam of an RTIMAGE */
                    this._errorCode = this._dcmTreeReader.ReadRefBeam(out avs, this.DcmInfo, this.SelInfo);
                }
                else if (this.ParamName == "refbeamworldxfm" || this.ParamName == "refbeambostonscantosiddonpatient")
                {
                    this._errorCode = this._dcmTreeReader.CacheGetXfm1(out avs, this.SelInfo);

                    /* if (*ppOut)
                    {
                        rc = DCM_E_OK;
                    }
                    else
                    {
                        rc = DCM_E_NO_DATASET;
                    }*/
                }
                else if (this.ParamName == "refbeamgantryxfm" || this.ParamName == "refbeambostonscantosiddonworld")
                {
                    this._errorCode = this._dcmTreeReader.CacheGetXfm2(out avs, this.SelInfo);

                    /*if (*ppOut)
                    {
                        rc = DCM_E_OK;
                    }
                    else
                    {
                        rc = DCM_E_NO_DATASET;
                    }*/
                }
                else if (this.ParamName == "matchxfm")
                {
                    this._errorCode = ErrorType.DCM_E_OK;
                }
                else if (this.ParamName == "bitmap")
                {
                    this._errorCode = ErrorType.DCM_E_OK;
                }
                else if (this.ParamName == "directory")
                {
                    result = string.Empty;
                    this._errorCode = ErrorType.DCM_E_OK;
                }
                else if ((this.SelInfo.Modality == ModalityType.DCM_MODALITY_BEAM) && this.ParamName.Contains("[")
                         && this.ParamName.Contains("]"))
                {
                    /* Something of an indexed segment within a beam is wanted. It can be:
	                 - outline[]
	                 - worldxfm[]
	                 - bostonscantosiddonpatient[]
	                 - gantryxfm[]
	                 - bostonscantosiddonworld[]
	                 - some_property[]
	                 Only the last of these returns a string. All the others return a field.
                      */
                    /* For now the XFMs of a segment behave the same as the XFMs of a beam */
                    if (string.Compare(this.ParamName, 0, "worldxfm", 0, 8) == 0
                        || string.Compare(this.ParamName, 0, "bostonscantosiddonpatient", 0, 25) == 0)
                    {
                        this._errorCode = this._dcmTreeReader.BeamToWorld(out avs, this.SelInfo);
                    }
                    else if (string.Compare(this.ParamName, 0, "gantryxfm", 0, 9) == 0
                             || string.Compare(this.ParamName, 0, "bostonscantosiddonworld", 0, 23) == 0)
                    {
                        this._errorCode = this._dcmTreeReader.CacheGetXfm2(out avs, this.SelInfo);

                        /*if (*ppOut)
                        {
                            rc = DCM_E_OK;
                        }
                        else
                        {
                            rc = DCM_E_INTERNAL;
                        }*/
                    }

                    /* The parameters outline[] behaves the same as ReadModality, but now for
	                 a specific segment. The parameter edge[] does some additional postprocessing
	                 in order to return the field edge only.
	                 All this functionality is solved in CqRtObjects; just pass the parameter to it.
                    */
                    else if (string.Compare(this.ParamName, 0, "outline", 0, 7) == 0)
                    {
                        this._errorCode = this._dcmTreeReader.ReadModality(
                            out avs, 
                            this.DcmInfo, 
                            this.SelInfo, 
                            this.ParamName);
                    }
                    else
                    {
                        this._errorCode = this._dcmTreeReader.GetKey(
                            out result, 
                            this.DcmInfo, 
                            this.SelInfo, 
                            this.ParamName);
                    }
                }
                else
                {
                    this._errorCode = this._dcmTreeReader.GetKey(
                        out result, 
                        this.DcmInfo, 
                        this.SelInfo, 
                        this.ParamName);
                }
            }

            return this._errorCode;
        }
    }
}
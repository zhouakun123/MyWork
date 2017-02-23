// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelInfo.cs" company="Elekta">
//   Copy right by Elekta Shanghai Office.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MosaiqDicom.ModelData
{
    #region Using

    using System;

    using MosaiqDicom.Utilities;

    #endregion

    /// <summary>The sel info.</summary>
    public class SelInfo : IDisposable
    {
        /// <summary>Initializes a new instance of the <see cref="SelInfo"/> class.</summary>
        /// <param name="errorCode">The error Code.</param>
        /// <param name="selection">The selection.</param>
        public SelInfo(out ErrorType errorCode, string selection)
        {
            // ParseInput.ParseSelInfo(this, selection);
            errorCode = ErrorType.DCM_E_OK;

            int i, iLength;
            var start = 0;
            string szTmp;
            int tmp;

            iLength = selection.Length;
            this.Selection = selection;

            // strcpy(pSelInfo->szSelection, pszSel);
            // start = this.Selection;

            /* Allow the selectionstring to start with a slash, but assume it is not part
                 of the PatientID.
              */
            if ((this.Selection[0] == '/') || (this.Selection[0] == '\\'))
            {
                ++start;
            }

            for (i = 0; i < iLength - 4; i++)
            {
                switch (this.Selection[i])
                {
                    case '/':
                    case '\\':

                        /* The start of a new modality, except when a slash is part of the PatientID. */
                        if (!string.IsNullOrEmpty(this.PatientID))
                        {
                            start = i + 1;
                        }

                        break;
                    case '.':

                        /* Before the decimal-point is the selected modality; after the
	                       decimal-point is the modality-type
	                    */
                        if (string.Compare(this.Selection, i + 1, "patient", 0, 7, StringComparison.OrdinalIgnoreCase)
                            == 0)
                        {
                            this.Modality = ModalityType.DCM_MODALITY_PATIENT;
                            this.PatientID = this.Selection.Substring(start, i - start);
                        }
                        else if (string.Compare(this.Selection, i + 1, "study", 0, 5, StringComparison.OrdinalIgnoreCase)
                                 == 0)
                        {
                            this.Modality = ModalityType.DCM_MODALITY_STUDY;
                            this.StudyUID = this.Selection.Substring(start, i - start);
                        }
                        else if (string.Compare(
                            this.Selection, 
                            i + 1, 
                            "series", 
                            0, 
                            5, 
                            StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            this.Modality = ModalityType.DCM_MODALITY_SERIES;
                            this.SeriesUID = this.Selection.Substring(start, i - start);
                        }
                        else if (string.Compare(
                            this.Selection, 
                            i + 1, 
                            "scan", 
                            0, 
                            4, 
                            StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            this.Modality = ModalityType.DCM_MODALITY_SCAN;
                            tmp = this.Selection.IndexOf('&', start);
                            var mfScan = this.Selection.IndexOf("&MfScan", start, StringComparison.Ordinal);

                            if (mfScan >= 0)
                            {
                                /* SOP is needed for MultiFrameScans when several occur in one series. */
                                this.MfScan = true;
                                this.SOPInstanceUID = this.Selection.Substring(start, mfScan - start);
                            }
                            else if (tmp > 0)
                            {
                                var exrKey = this.Selection.IndexOf('=', tmp + 1);
                                if (exrKey >= 0)
                                {
                                    this.ExtraKey = this.Selection.Substring(tmp + 1, exrKey - tmp + 1);
                                    this.ExtraValue = this.Selection.Substring(exrKey + 1, i - exrKey - 1);
                                }
                            }
                        }
                        else if (string.Compare(
                            this.Selection, 
                            i + 1, 
                            "image", 
                            0, 
                            5, 
                            StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            this.Modality = ModalityType.DCM_MODALITY_IMAGE;
                            tmp = this.Selection.IndexOf('&', start);

                            if (tmp >= 0)
                            {
                                this.SOPInstanceUID = this.Selection.Substring(start, tmp - start);
                                var tmpExKey = this.Selection.IndexOf('=', tmp + 1);
                                if (tmpExKey >= 0)
                                {
                                    this.ExtraKey = this.Selection.Substring(
                                        tmp + 1, 
                                        tmpExKey - tmp - 1);
                                    this.ExtraValue = this.Selection.Substring(
                                        tmpExKey + 1, 
                                        i - tmpExKey - 1);
                                }
                            }
                            else
                            {
                                this.SOPInstanceUID = this.Selection.Substring(start, i - start);
                            }
                        }
                        else if (string.Compare(
                            this.Selection, 
                            i + 1, 
                            "structset", 
                            0, 
                            9, 
                            StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            this.Modality = ModalityType.DCM_MODALITY_STRUCTSET;
                            this.SOPInstanceUID = this.Selection.Substring(start, i - start);
                        }
                        else if ((string.Compare(
                            this.Selection, 
                            i + 1, 
                            "structure", 
                            0, 
                            9, 
                            StringComparison.OrdinalIgnoreCase) == 0)
                                 || (string.Compare(
                                     this.Selection, 
                                     i + 1, 
                                     "marker", 
                                     0, 
                                     6, 
                                     StringComparison.OrdinalIgnoreCase) == 0))
                        {
                            this.Modality = ModalityType.DCM_MODALITY_STRUCTURE;
                            this.ExtraValue = this.Selection.Substring(start, i - start);
                        }
                        else if (string.Compare(
                            this.Selection, 
                            i + 1, 
                            "plan", 
                            0, 
                            4, 
                            StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            this.Modality = ModalityType.DCM_MODALITY_PLAN;
                            this.SOPInstanceUID = this.Selection.Substring(start, i - start);
                        }
                        else if (string.Compare(
                            this.Selection, 
                            i + 1, 
                            "scan", 
                            0, 
                            4, 
                            StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            this.Modality = ModalityType.DCM_MODALITY_BEAM;
                            this.ExtraValue = this.Selection.Substring(start, i - start);
                        }
                        else if (string.Compare(
                            this.Selection, 
                            i + 1, 
                            "dose", 
                            0, 
                            4, 
                            StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            this.Modality = ModalityType.DCM_MODALITY_DOSE;
                            tmp = this.Selection.IndexOf(
                                "&MfScan", 
                                start, 
                                StringComparison.Ordinal);
                            if (tmp >= 0)
                            {
                                /* SOP is needed for MultiFrameScans when several occur in one series. */
                                this.MfScan = true;
                                this.SOPInstanceUID = this.Selection.Substring(
                                    start, 
                                    tmp - start);
                            }
                        }

                        break;
                }
            }

            if (string.IsNullOrEmpty(this.ExtraKey))
            {
                this.ExtraValue = this.ExtraValue.Replace('#', '\\');

                /* Be backwards compatible with stored URLs that contain 'EchoNumber'.
                    The dicom-guru's changed the name to 'EchoNumbers'.
                */
                if (this.ExtraKey == "EchoNumber")
                {
                    this.ExtraKey = "EchoNumbers";
                }
            }
        }

        /// <summary>EchoNumbers in case of MRI '.scan' and '.image'.</summary>
        public string ExtraKey { get; set; }

        /// <summary>Beamnumber in case of beam modality, StructNumber in case of structure modality</summary>
        public string ExtraValue { get; set; }

        /// <summary>Flag indicating a MultiFrameScan.</summary>
        public bool MfScan { get; set; }

        /// <summary>The modality.</summary>
        public ModalityType Modality { get; set; }

        /// <summary>The patient id.</summary>
        public string PatientID { get; set; }

        /// <summary>The sz selection.</summary>
        public string Selection { get; set; }

        /// <summary>The series uid.</summary>
        public string SeriesUID { get; set; }

        /// <summary>Used for: structure, plan and dose.</summary>
        public string SOPInstanceUID { get; set; }

        /// <summary>The study uid.</summary>
        public string StudyUID { get; set; }

        /// <summary>The dispose.</summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
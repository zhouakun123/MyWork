// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MosaiqDcmTreeReader.cs" company="Elekta">
//   Copy right by Elekta Shanghai Office.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MosaiqDicom
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Eventing;

    using MosaiqDicom.ModelData;
    using MosaiqDicom.Utilities;

    #endregion

    /// <summary>The mosaiq dcm tree reader.</summary>
    public class MosaiqDcmTreeReader : IDcmTreeReader
    {
        /// <summary>The beam to world.</summary>
        /// <param name="avs">The avs.</param>
        /// <param name="selInfo">The sel info.</param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns>The <see cref="ErrorType"/>.</returns>
        /// Will move to utilities.
        public ErrorType BeamToWorld(out AVSfield avs, SelInfo selInfo)
        {
            var fScanInverse = new double[16];
            avs = null;
            AVSfield pBostonScanToSiddonPatient;
            AVSfield pBostonScanToSiddonWorld;

            /* Get the XFMs of the underlying scan */
            // CacheGetReferencedXfm1(pszSelection, &pBostonScanToSiddonPatient);
            // CacheGetReferencedXfm2(pszSelection, &pBostonScanToSiddonWorld);
            if (!pBostonScanToSiddonPatient || !pBostonScanToSiddonWorld)
            {
                return ErrorType.DCM_E_INTERNAL;
            }

            /* Calculate SiddonPatientToSiddonWorld */
            AVSxfm_inverse(fScanInverse, (float*)pBostonScanToSiddonPatient->data);
            AVSxfm_mult((float*)pBostonScanToSiddonWorld->data, fScanInverse);

            /* Multiply this with the beam-xfm */
            CacheGetXfm1(pszSelection, ppOut);
            if (!*ppOut)
            {
                AVSfield_free(pBostonScanToSiddonPatient);
                AVSfield_free(pBostonScanToSiddonWorld);
                return DCM_E_NO_XFM;
            }

            AVSxfm_mult((float*)*ppOut->data, fScanInverse);

            memcpy(*ppOut->data, fScanInverse, 16 * sizeof(float));
            AVSfield_free(pBostonScanToSiddonPatient);
            AVSfield_free(pBostonScanToSiddonWorld);

            return ErrorType.DCM_E_OK;
        }

        /// <summary>The cache get xfm 1.</summary>
        /// <param name="avs">The avs.</param>
        /// <param name="selInfo">The sel info.</param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns>The <see cref="ErrorType"/>.</returns>
        public ErrorType CacheGetXfm1(out AVSfield avs, SelInfo selInfo)
        {
            // SEL_CACHE* pSelCache;
            avs = null;

            /* Find the selection-string in the cache */
            // pSelCache = FindCache(selInfo);
            /*if (pSelCache)
            {
                if (pSelCache->pXfm1)
                {
                    AVSfield_copy(pSelCache->pXfm1, ppOut);
                    return ErrorType.DCM_E_OK;
                }
            }*/
        }

        /// <summary>The cache get xfm 2.</summary>
        /// <param name="avs">The avs.</param>
        /// <param name="selInfo">The sel info.</param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns>The <see cref="ErrorType"/>.</returns>
        public ErrorType CacheGetXfm2(out AVSfield avs, SelInfo selInfo)
        {
            SEL_CACHE* pSelCache;

            if (*ppOut)
            {
                AVSfield_free(*ppOut);
                *ppOut = NULL;
            }

            /* Find the selection-string in the cache */
            pSelCache = FindCache(pszSelection);
            if (pSelCache)
            {
                if (pSelCache->pXfm2)
                {
                    AVSfield_copy(pSelCache->pXfm2, ppOut);
                }
            }
        }

        /// <summary>Retrieves for a Patient all Studies, and for each Study all Series.</summary>
        /// <param name="avs">The avs.</param>
        /// <param name="dcmInfo">The dcm info.</param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns>The <see cref="ErrorType"/>.</returns>
        public ErrorType CreateTree(out AVSfield avs, DcmInfo dcmInfo)
        {
            avs = null;
            ErrorType errorCode;
            int i, j, k;
            AVSfield pQueryResult = new AVSfield();
            int iNbStudies, iNbSeries, iNbSubSeries;
            int iNbImages;
            int iRecordSize;
            var iNbEntries = 17;
            string[] ppszKeys =
                {
                    "QueryRetrieveLevel", "PatientID", "StudyInstanceUID", "SeriesInstanceUID", 
                    "FrameOfReferenceUID", "SOPInstanceUID", "InstanceNumber", "StudyDate", "Modality", 
                    "SeriesNumber", "EchoNumbers", "SeriesTime", "SeriesDescription", "NumberOfFrames", 
                    "ModalitiesInStudy", "Manufacturer", "ImageType"
                };
            string[] ppszValues =
                {
                    "IMAGE", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 
                    string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 
                    string.Empty, string.Empty, string.Empty, string.Empty, string.Empty
                };
            string pFirstRecord;
            string pszStudyUID;
            string pszSeriesUID;
            string pszFrameOfReferenceUID;
            string pszStudyDate;
            string pszStudyModality;
            string pszModality;
            string pszSeriesNumber;
            string pszEchoNumbers;
            string pszSeriesTime;
            string pszSeriesDescription;
            string pszNbFrames;
            string pszManufacturer;
            string pszImageType;
            string pszInstanceNumber;
            string pszSOPInstanceUID;
            string pszLastSeriesUID;
            bool bEchoNumbersNeeded;
            bool bImageTypeNeeded;
            string pData;
            var iDims = new long[3];
            int iSize;
            int iNbFrames;
            int nTmp;
            var iTotNbPlans = 0;
            var iCurPlanNo = 0;

            var iTotNbStructs = 0;

            List<DcmPlan> DcmPlans;
            List<DcmStruct> DcmStructs;

            DICOMDataObject pDDO;
            SelInfo selInfo;
            var bAreStructsPresent = false;
            bool bLeftOverStructures;
            var bImagesOnly = false;
            var bExcludeRTObjects = false;
            var b4DPET = false;
            var szConfineToStudyUID = string.Empty;
            string szTmp;
            int[] piNbRecordsOfVolume;
            int iLastGoodRecord;

            /* Check whether user does not want grouping of images to scans. */
            if (!string.IsNullOrEmpty("gl_pszExtraParams"))
            {
                nTmp = "gl_pszExtraParams".IndexOf("IMAGESONLY", StringComparison.Ordinal);
                if (nTmp >= 0)
                {
                    nTmp = "gl_pszExtraParams".IndexOf('=', nTmp);
                }

                if (nTmp >= 0)
                {
                    if (Convert.ToInt32("gl_pszExtraParams".Substring(nTmp + 1)) == 1)
                    {
                        bImagesOnly = true;
                    }
                }
            }

            /* Check whether user wants to exclude RT Objects. */
            if (!string.IsNullOrEmpty("gl_pszExtraParams"))
            {
                nTmp = "gl_pszExtraParams".IndexOf("EXCLUDERTOBJECTS", StringComparison.Ordinal);
                if (nTmp >= 0)
                {
                    nTmp = "gl_pszExtraParams".IndexOf('=', nTmp);
                }

                if (nTmp >= 0)
                {
                    if (Convert.ToInt32("gl_pszExtraParams".Substring(nTmp + 1)) == 1)
                    {
                        bExcludeRTObjects = true;
                    }
                }
            }

            /* Check whether user wants confine the query to a specific STUDY. */
            if (!string.IsNullOrEmpty("gl_pszExtraParams"))
            {
                nTmp = "gl_pszExtraParams".IndexOf("STUDYINSTANCEUID", StringComparison.Ordinal);
                if (nTmp >= 0)
                {
                    nTmp = "gl_pszExtraParams".IndexOf('=', nTmp);
                }

                if (nTmp >= 0)
                {
                    var nTmp2 = "gl_pszExtraParams".IndexOf(';', nTmp);

                    if (nTmp2 >= 0)
                    {
                        szConfineToStudyUID = "gl_pszExtraParams".Substring(nTmp + 1, nTmp2 - nTmp - 1);
                    }
                    else
                    {
                        szConfineToStudyUID = "gl_pszExtraParams".Substring(nTmp + 1, 64);
                    }
                }
            }

            if (string.IsNullOrEmpty(dcmInfo.PatientID))
            {
                return ErrorType.DCM_E_MISSING_PARAMETER;
            }

            ppszValues[1] = dcmInfo.PatientID;
            ppszValues[2] = szConfineToStudyUID;

            /*if (DllProgressMsg(0, "Issuing query") != DCM_PROGRESS_CONTINUE)
            {
                return DCM_E_INTERRUPTED;
            }*/

            // TODO: Infrustructure function...
            errorCode = ErrorType.DCM_E_OK; //= DoQuery(&pQueryResult, pDcmInfo, ppszKeys, ppszValues, iNbEntries);

            /*if (DllProgressMsg(100, "Obtained query result") != DCM_PROGRESS_CONTINUE)
            {
                return DCM_E_INTERRUPTED;
            }*/

            if (errorCode != ErrorType.DCM_E_OK)
            {
                //AVSfield_free(pQueryResult);
                return errorCode;
            }

            if (pQueryResult.dimensions[1] <= 1)
            {
                // Special Query for ELEKTA MDD server => First Study level, then Image level Query
                AVSfield pQueryStudyResult;
                AVSfield pQuerySingleStudyResult;

                ppszValues[0] = "STUDY";
                // TODO
                errorCode = ErrorType.DCM_E_OK; //DoQuery(&pQueryStudyResult, pDcmInfo, ppszKeys, ppszValues, iNbEntries);
                if (errorCode != ErrorType.DCM_E_OK)
                {
                    //AVSfield_free(pQueryResult);
                    //AVSfield_free(pQueryStudyResult);
                    return errorCode;
                }

                var iNbTotalStudy = pQueryStudyResult.dimensions[1] - 1;
                int iStudyRecordSize = pQueryStudyResult.dimensions[0] * pQueryStudyResult.veclen;

                //Todo:
                //pszStudyUID = GetValuePointer(pQueryStudyResult, "StudyInstanceUID");
                ppszValues[0] = "IMAGE";

                for (var iStudy = 0; iStudy < iNbTotalStudy; ++iStudy)
                {
                    // StudyInstanceUID set
                    ppszValues[2] = pszStudyUID + iStudy * iStudyRecordSize + 1;
                    int iSizeOfUIDArray = ppszValues[2].Length;
                    ppszValues[2][iSizeOfUIDArray - 1] = 0;
                    //todo
                    //rc = DoQuery(&pQuerySingleStudyResult, pDcmInfo, ppszKeys, ppszValues, iNbEntries);
                    if (errorCode!=ErrorType.DCM_E_OK)
                    {
                        return errorCode;
                    }

                    if (pQuerySingleStudyResult.dimensions[1] <= 1)
                    {
                        // no images found in study, do a series query then, prob a PukkaJ server
                        ppszValues[0] = "SERIES";

                        AVSfield pQuerySeriesResult;
                        AVSfield pQuerySingleSeriesResult;

                        //Todo:rc = DoQuery(&pQuerySeriesResult, pDcmInfo, ppszKeys, ppszValues, iNbEntries);
                        if (errorCode!=ErrorType.DCM_E_OK)
                        {
                            return errorCode;
                        }

                        // any series?
                        var iNbTotalSeries = pQuerySeriesResult.dimensions[1] - 1;
                        int iSeriesRecordSize = pQuerySeriesResult.dimensions[0] * pQuerySeriesResult.veclen;

                        //Todo:pszSeriesUID = GetValuePointer(pQuerySeriesResult, "SeriesInstanceUID");
                        ppszValues[0] = "IMAGE";

                        //Todo:pszModality = GetValuePointer(pQuerySeriesResult, "Modality");

                        for (var iSeries = 0; iSeries < iNbTotalSeries; ++iSeries)
                        {
                            // yes, do an image query for each serie, the modality value MUST be set

                            // SeriesInstanceUID set
                            ppszValues[3] = pszSeriesUID.Substring(iSeries * iSeriesRecordSize + 1,);
                            iSizeOfUIDArray = ppszValues[3].Remove();
                            ppszValues[3][iSizeOfUIDArray - 1] = 0;

                            // Modality set
                            ppszValues[8] = pszModality + iSeries * iSeriesRecordSize + 1;
                            iSizeOfUIDArray = ppszValues[8].Length;
                            ppszValues[8][iSizeOfUIDArray - 1] = 0;

                            //Todo:rc = DoQuery(&pQuerySingleSeriesResult, pDcmInfo, ppszKeys, ppszValues, iNbEntries);
                            if (errorCode!=ErrorType.DCM_E_OK)
                            {
                                return errorCode;
                            }

                            var pOverallQueryData = (string)pQueryResult.data;
                            var pSingleSeriesQueryData = (string)pQuerySingleSeriesResult.data;

                            // build series tree
                            if (iSeries == 0)
                            {
                                DllFree(pOverallQueryData);
                                var iSingleQuerySize = sizeof(char) * pQuerySingleSeriesResult->dimensions[0]
                                                       * pQuerySingleSeriesResult->dimensions[1]
                                                       * pQuerySingleSeriesResult->veclen;
                                pOverallQueryData = (string)DllMalloc(iSingleQuerySize);
                                memcpy(pOverallQueryData, pSingleSeriesQueryData, iSingleQuerySize);
                                pQueryResult->data = pOverallQueryData;
                                pQueryResult->dimensions[1] = pQuerySingleSeriesResult->dimensions[1];
                            }
                            else
                            {
                                var iOverallQuerySize = sizeof(char)
                                                        * (pQueryResult->dimensions[0] * pQueryResult->dimensions[1]
                                                           * pQueryResult->veclen);
                                var iSingleQuerySize = sizeof(char) * pQuerySingleSeriesResult->dimensions[0]
                                                       * (pQuerySingleSeriesResult->dimensions[1] - 1)
                                                       * pQuerySingleSeriesResult->veclen;

                                var pOverallQueryData_new = (string)DllMalloc(iOverallQuerySize + iSingleQuerySize);
                                memcpy(pOverallQueryData_new, pOverallQueryData, iOverallQuerySize);
                                memcpy(
                                    pOverallQueryData_new + iOverallQuerySize / sizeof(char), 
                                    pSingleSeriesQueryData
                                    + pQuerySingleSeriesResult->dimensions[0] * pQuerySingleSeriesResult->veclen, 
                                    iSingleQuerySize);

                                DllFree(pOverallQueryData);
                                pQueryResult->data = pOverallQueryData_new;
                                pQueryResult->dimensions[1] += pQuerySingleSeriesResult->dimensions[1] - 1;
                            }

                            AVSfield_free(pQuerySingleSeriesResult);
                        }

                        // end of series loop
                        AVSfield_free(pQuerySeriesResult);
                    }
                    else
                    {
                        var pOverallQueryData = (string)pQueryResult->data;
                        var pSingleQueryData = (string)pQuerySingleStudyResult->data;

                        if (iStudy == 0)
                        {
                            DllFree(pOverallQueryData);
                            var iSingleQuerySize = sizeof(char) * pQuerySingleStudyResult->dimensions[0]
                                                   * pQuerySingleStudyResult->dimensions[1]
                                                   * pQuerySingleStudyResult->veclen;
                            pOverallQueryData = (string)DllMalloc(iSingleQuerySize);
                            memcpy(pOverallQueryData, pSingleQueryData, iSingleQuerySize);
                            pQueryResult->data = pOverallQueryData;
                            pQueryResult->dimensions[1] = pQuerySingleStudyResult->dimensions[1];
                        }
                        else
                        {
                            var iOverallQuerySize = sizeof(char)
                                                    * (pQueryResult->dimensions[0] * pQueryResult->dimensions[1]
                                                       * pQueryResult->veclen);
                            var iSingleQuerySize = sizeof(char) * pQuerySingleStudyResult->dimensions[0]
                                                   * (pQuerySingleStudyResult->dimensions[1] - 1)
                                                   * pQuerySingleStudyResult->veclen;

                            var pOverallQueryData_new = (string)DllMalloc(iOverallQuerySize + iSingleQuerySize);
                            memcpy(pOverallQueryData_new, pOverallQueryData, iOverallQuerySize);
                            memcpy(
                                pOverallQueryData_new + iOverallQuerySize / sizeof(char), 
                                pSingleQueryData
                                + pQuerySingleStudyResult->dimensions[0] * pQuerySingleStudyResult->veclen, 
                                iSingleQuerySize);

                            DllFree(pOverallQueryData);
                            pQueryResult->data = pOverallQueryData_new;
                            pQueryResult->dimensions[1] += pQuerySingleStudyResult->dimensions[1] - 1;
                        }

                        AVSfield_free(pQuerySingleStudyResult);
                    }
                }

                AVSfield_free(pQueryStudyResult);
            }

            /* Find whether the response is on the correct PatientID. */
            rc = CheckQueryResult(pQueryResult, "PatientID", pDcmInfo->szPatientID);
            if (rc)
            {
                AVSfield_free(pQueryResult);
                return rc;
            }

            CacheCleanupRtObjectFiles(pDcmInfo->szPatientID);

            /* The Query-response was on the correct PatientID. And now the bad news:
                - Not all requested information might be given
                - The order of the information in each record is arbitrary
                - In case of General Electric, even the order of the matching records
                is arbitrary.
                The records in the QueryResult field have to be sorted on StudyInstance,
                SeriesInstance, Modality and (possibly) InstanceNumber and EchoNumbers.
            */
            pszStudyUID = GetValuePointer(pQueryResult, "StudyInstanceUID");
            pszSeriesUID = GetValuePointer(pQueryResult, "SeriesInstanceUID");
            pszFrameOfReferenceUID = GetValuePointer(pQueryResult, "FrameOfReferenceUID");
            pszModality = GetValuePointer(pQueryResult, "Modality");
            pszInstanceNumber = GetValuePointer(pQueryResult, "InstanceNumber");
            pszEchoNumbers = GetValuePointer(pQueryResult, "EchoNumbers");
            pszSOPInstanceUID = GetValuePointer(pQueryResult, "SOPInstanceUID");
            pszManufacturer = GetValuePointer(pQueryResult, "Manufacturer");
            pszNbFrames = GetValuePointer(pQueryResult, "NumberOfFrames");
            pszImageType = GetValuePointer(pQueryResult, "ImageType");
            pszSeriesDescription = GetValuePointer(pQueryResult, "SeriesDescription");

            /* The StudyUID, SeriesUID and Modality should be provided */
            if (!pszStudyUID || !pszSeriesUID || !pszModality)
            {
                AVSfield_free(pQueryResult);
                return ErrorType.DCM_E_NO_KEYWORD;
            }

            /* Quicksort will sort the QueryResult field; it needs to know the offsets
                of the VRs in each record. I'm afraid I need some globals to accomplish this
            */
            pFirstRecord = (string)pQueryResult->data + pQueryResult->dimensions[0] * pQueryResult->veclen;
            gl_iOffsetStudy = pszStudyUID - pFirstRecord;
            gl_iOffsetSeries = pszSeriesUID - pFirstRecord;
            gl_iOffsetModality = pszModality - pFirstRecord;
            if (pszEchoNumbers)
            {
                gl_iOffsetEcho = pszEchoNumbers - pFirstRecord;
            }
            else
            {
                gl_iOffsetEcho = -1;
            }

            if (pszInstanceNumber)
            {
                gl_iOffsetImage = pszInstanceNumber - pFirstRecord;
            }
            else
            {
                gl_iOffsetImage = -1;
            }

            if (pszSOPInstanceUID)
            {
                gl_iOffsetSOPInstanceUID = pszSOPInstanceUID - pFirstRecord;
            }
            else
            {
                gl_iOffsetSOPInstanceUID = -1;
            }

            if (pszImageType)
            {
                gl_iOffsetImageType = pszImageType - pFirstRecord;
            }
            else
            {
                gl_iOffsetImageType = -1;
            }

            qsort(
                pFirstRecord, 
                pQueryResult->dimensions[1] - 1, 
                pQueryResult->dimensions[0] * pQueryResult->veclen, 
                fnSortDicomQueryResult);

            /* Try to group images -that are part of a volume- together. */
            iNbImages = pQueryResult->dimensions[1] - 1;
            iRecordSize = pQueryResult->dimensions[0] * pQueryResult->veclen;

            piNbRecordsOfVolume = (int*)malloc(iNbImages * sizeof(int));
            if (!piNbRecordsOfVolume)
            {
                return ErrorType.DCM_E_MEMORY;
            }

            for (i = 0; i < iNbImages; i++)
            {
                piNbRecordsOfVolume[i] = 1;
            }

            var iFirstRecordOfVolume = 0;
            for (i = 0; i < iNbImages; i++)
            {
                /* Maybe RTSTRUCT and RTPLAN must not be loaded */
                if (bExcludeRTObjects
                    && ((strstr(pszModality + i * iRecordSize, "[RTSTR") != NULL)
                        || (strstr(pszModality + i * iRecordSize, "[RTPLA") != NULL)))
                {
                    piNbRecordsOfVolume[i] = 0;
                    iFirstRecordOfVolume = i + 1;
                    continue;
                }

                if (bImagesOnly)
                {
                    continue;
                }

                /* If the current record is a multi-frame image, do not try
	 to group it further.
      */
                if (pszNbFrames[i * iRecordSize + 1] != '\0')
                {
                    iNbFrames = atoi(pszNbFrames + i * iRecordSize + 1);
                    if (iNbFrames > 1)
                    {
                        iFirstRecordOfVolume = i + 1;
                        continue;
                    }
                }
                else
                {
                    if (strstr(pszModality + i * iRecordSize, "[RTDOSE"))
                    {
                        iNbFrames = 99;
                        iFirstRecordOfVolume = i + 1;
                        continue;
                    }
                }

                /* Grouping is only done for some modalities */
                if ((strstr(pszModality + i * iRecordSize, "[CT") == NULL)
                    && (strstr(pszModality + i * iRecordSize, "[PT") == NULL)
                    && (strstr(pszModality + i * iRecordSize, "[DR") == NULL)
                    && (strstr(pszModality + i * iRecordSize, "[DS") == NULL)
                    && (strstr(pszModality + i * iRecordSize, "[RTDOSE") == NULL)
                    && (strstr(pszModality + i * iRecordSize, "[US") == NULL)
                    && (strstr(pszModality + i * iRecordSize, "[MR") == NULL))
                {
                    /* Not a modality of the above? No grouping is possible */
                    iFirstRecordOfVolume = i + 1;
                    continue;
                }

                /* For ultrasound, try grouping when the manufacturer is 'V' (Variseed??) */
                if (strstr(pszModality + i * iRecordSize, "[US"))
                {
                    if (!pszManufacturer || (strcmp(pszManufacturer + i * iRecordSize, "[V ]") != 0))
                    {
                        /* Ultrasound but not Variseed? Lets not try grouping... */
                        iFirstRecordOfVolume = i + 1;
                        continue;
                    }
                }

                /* If the current record is the first of a possible volume, no
	 grouping is possible yet.
      */
                if (i == iFirstRecordOfVolume)
                {
                    continue;
                }

                /* Do not group when Study, Series or Modality is different from the previous */
                if ((strcmp(pszStudyUID + i * iRecordSize, pszStudyUID + (i - 1) * iRecordSize) != 0)
                    || (strcmp(pszSeriesUID + i * iRecordSize, pszSeriesUID + (i - 1) * iRecordSize) != 0)
                    || (strcmp(pszModality + i * iRecordSize, pszModality + (i - 1) * iRecordSize) != 0))
                {
                    iFirstRecordOfVolume = i;
                    continue;
                }

                /* In case of MR, do not group if ImageType is different */
                if ((strstr(pszModality + i * iRecordSize, "[MR") != NULL) && pszImageType
                    && (strcmp(pszImageType + i * iRecordSize, pszImageType + (i - 1) * iRecordSize) != 0))
                {
                    iFirstRecordOfVolume = i;
                    continue;
                }

                /* In case of MR, do not group if Echonumbers is different */
                if ((strstr(pszModality + i * iRecordSize, "[MR") != NULL) && pszEchoNumbers
                    && (strcmp(pszEchoNumbers + i * iRecordSize, pszEchoNumbers + (i - 1) * iRecordSize) != 0))
                {
                    /* Well, in case of BOLD MR images, group them to create 4D volumes */
                    if (
                        !(pszSeriesDescription
                          && (strstr(pszSeriesDescription + i * iRecordSize, "BOLD")
                              || strstr(pszSeriesDescription + i * iRecordSize, "R2*"))))
                    {
                        iFirstRecordOfVolume = i;
                        continue;
                    }
                }

                /* OK, we can group them together */
                piNbRecordsOfVolume[iFirstRecordOfVolume] ++;
                piNbRecordsOfVolume[i] = 0;
            }

            RemoveBracesFromQueryResult(pQueryResult, piNbRecordsOfVolume);

            /* Count the number of Studies and Series and SubSeries */
            iNbStudies = 0;
            iNbSeries = 0;
            iNbSubSeries = 0;
            iLastGoodRecord = -1;
            for (i = 0; i < iNbImages; i++)
            {
                /* Skip if record was deleted */
                if (piNbRecordsOfVolume[i] == 0)
                {
                    continue;
                }

                /* The first good record : add a study, series and subseries */
                if (iLastGoodRecord == -1)
                {
                    iNbStudies ++;
                    iNbSeries ++;
                    iNbSubSeries ++;
                }
                
                /* ...or a new StudyUID means one extra study, serie and subserie */
                else if (strcmp(pszStudyUID + i * iRecordSize, pszStudyUID + iLastGoodRecord * iRecordSize) != 0)
                {
                    iNbStudies ++;
                    iNbSeries ++;
                    iNbSubSeries ++;
                }
                
                /* ...or a new SeriesUID means one serie and subserie */
                else if (strcmp(pszSeriesUID + i * iRecordSize, pszSeriesUID + iLastGoodRecord * iRecordSize) != 0)
                {
                    iNbSeries ++;
                    iNbSubSeries ++;
                }
                else
                {
                    iNbSubSeries ++;
                }

                iLastGoodRecord = i;

                if (strcmp(pszModality + i * iRecordSize, "RTPLAN") == 0)
                {
                    /* An RTPLAN contains beams. In order to find out how many, the plan has to
         be retrieved... */
                    SelInfo.pszPatientID = pDcmInfo->szPatientID;
                    SelInfo.pszStudyUID = pszStudyUID + i * iRecordSize;
                    SelInfo.pszSeriesUID = pszSeriesUID + i * iRecordSize;
                    SelInfo.pszSOPInstanceUID = pszSOPInstanceUID + i * iRecordSize;
                    rc = GetRtDDO(&pDDO, pDcmInfo, &SelInfo);
                    DcmPlans.resize(DcmPlans.size() + 1);
                    if (!rc)
                    {
                        GetPlanInfo(pDDO, &DcmPlans[iTotNbPlans]);
                        GetBeamInfo(pDDO, &DcmPlans[iTotNbPlans].iNbBeams, &DcmPlans[iTotNbPlans].pBeamInfo);
                        delete pDDO;
                        iNbSubSeries += DcmPlans[iTotNbPlans].iNbBeams;
                    }
                    else
                    {
                        /* Apparently unable to load the plan...
	   Just don't show the beams. */
                        DcmPlans[iTotNbPlans].iNbBeams = 0;
                        DcmPlans[iTotNbPlans].pBeamInfo = NULL;
                        DcmPlans[iTotNbPlans].pszRTPlanLabel = NULL;
                        DcmPlans[iTotNbPlans].pszRTPlanName = NULL;
                        DcmPlans[iTotNbPlans].pszRTPlanDate = NULL;
                        DcmPlans[iTotNbPlans].pszRTPlanTime = NULL;
                    }

                    iTotNbPlans++;
                }
                else if (strcmp(pszModality + i * iRecordSize, "RTSTRUCT") == 0)
                {
                    /* An RTSTRUCT may contain contours of several organs, and may also contain
	 several markers. To find out how many, the rtstruct has to
         be retrieved... */
                    SelInfo.pszPatientID = pDcmInfo->szPatientID;
                    SelInfo.pszStudyUID = pszStudyUID + i * iRecordSize;
                    SelInfo.pszSeriesUID = pszSeriesUID + i * iRecordSize;
                    SelInfo.pszSOPInstanceUID = pszSOPInstanceUID + i * iRecordSize;
                    rc = GetRtDDO(&pDDO, pDcmInfo, &SelInfo);
                    DcmStructs.resize(DcmStructs.size() + 1);
                    if ((rc == DCM_E_OK) && pDDO)
                    {
                        rc = GetStructInfo(
                            pDDO, 
                            &DcmStructs[iTotNbStructs].iNbStructs, 
                            &DcmStructs[iTotNbStructs].pStructInfo);
                        delete pDDO;
                        if (rc == DCM_E_OK)
                        {
                            iNbSubSeries += DcmStructs[iTotNbStructs++].iNbStructs;
                            bAreStructsPresent = TRUE;
                        }
                    }
                }

                // else if (strcmp(pszModality + i*iRecordSize, "RTDOSE") == 0)
                // { /* RTDOSE */

                // SelInfo.pszPatientID      = pDcmInfo->szPatientID;
                // SelInfo.pszStudyUID       = pszStudyUID + i*iRecordSize;
                // SelInfo.pszSeriesUID      = pszSeriesUID + i*iRecordSize;
                // SelInfo.pszSOPInstanceUID = pszSOPInstanceUID + i*iRecordSize;
                // rc = GetRtDDO(&pDDO, pDcmInfo, &SelInfo);

                // if ((rc == DCM_E_OK) && pDDO)
                // { GetDoseFrameNumber(pDDO, &iNbOfDoseFrames[iCurDoseNo]);
                // delete pDDO;
                // ++iCurDoseNo;
                // }
                // }
                else if (strcmp(pszModality + i * iRecordSize, "US") == 0)
                {
                    if ((pszManufacturer != NULL)
                        && ((strncmp(pszManufacturer + i * iRecordSize, "Kretz", 5) == 0)
                            || (strncmp(pszManufacturer + i * iRecordSize, "Philips", 7) == 0)))
                    {
                        /* We do not know, but assume a 'private' carthesian volume is incorporated
	   in this image...
	*/
                        iNbSubSeries ++;
                    }
                }
            }

            /* Allocate a TREE-field now we know the number of records it will contain */
            iDims[0] = 3;
            iDims[1] = 1 + iNbStudies + iNbSeries + iNbSubSeries;
            iDims[2] = 0;
            *ppOut = (AVSfield*)AVSdata_alloc(AAPM_TREE_FIELD, iDims);
            if (!*ppOut)
            {
                AVSfield_free(pQueryResult);
                free(piNbRecordsOfVolume); // memory leak until 2013-04-23
                return DCM_E_MEMORY;
            }

            /* Fill the TREE */
            iSize = *ppOut->veclen;
            pData = (string)*ppOut->data;
            memset(pData, 0, iDims[0] * iDims[1] * iSize);

            /* The patient is where its all about... */
            strcpy(pData, "DEPTH=1");
            strcpy(pData + iSize, pDcmInfo->szPatientID);
            strcpy(pData + 2 * iSize, "patient");
            pData += 3 * iSize;

            pszStudyDate = GetValuePointer(pQueryResult, "StudyDate");
            pszStudyModality = GetValuePointer(pQueryResult, "ModalitiesInStudy");
            pszSeriesNumber = GetValuePointer(pQueryResult, "SeriesNumber");
            pszSeriesTime = GetValuePointer(pQueryResult, "SeriesTime");
            pszSeriesDescription = GetValuePointer(pQueryResult, "SeriesDescription");

            bEchoNumbersNeeded = false;
            bImageTypeNeeded = false;
            iLastGoodRecord = -1;

            // iCurDoseNo = 0;
            for (i = 0; i < iNbImages; i++)
            {
                /* Skip if record was deleted */
                if (piNbRecordsOfVolume[i] == 0)
                {
                    continue;
                }

                /* Do RTSTRUCTs later, after all scans have been added */
                if (strcmp(pszModality + i * iRecordSize, "RTSTRUCT") == 0)
                {
                    continue;
                }

                /* The first good record or a new study : add a study */
                if ((iLastGoodRecord == -1)
                    || (strcmp(pszStudyUID + i * iRecordSize, pszStudyUID + iLastGoodRecord * iRecordSize) != 0))
                {
                    strcpy(pData, "DEPTH=2~ALIAS=");
                    strncat(pData, pszStudyDate + i * iRecordSize, AAPM_TREE_VECLEN - 14 - 1);

                    // 14 chars for "DEPTH=2~ALIAS="
                    pData[AAPM_TREE_VECLEN - 1] = '\0';

                    if (pszStudyModality)
                    {
                        if (pszStudyModality[i * iRecordSize] && strlen(pData) < AAPM_TREE_VECLEN - 1)
                        {
                            strcat(pData, " ");
                            strncat(pData, pszStudyModality + i * iRecordSize, AAPM_TREE_VECLEN - strlen(pData) - 1);
                            pData[AAPM_TREE_VECLEN - 1] = '\0';
                        }
                    }

                    strcpy(pData + iSize, pszStudyUID + i * iRecordSize);
                    strcpy(pData + 2 * iSize, "study");
                    pData += 3 * iSize;
                }

                /* The first good record or a new series : add a series */
                if ((iLastGoodRecord == -1)
                    || (strcmp(pszSeriesUID + i * iRecordSize, pszSeriesUID + iLastGoodRecord * iRecordSize) != 0))
                {
                    /* Add a new Series */
                    strcpy(pData, "DEPTH=3~ALIAS=");
                    if (pszSeriesTime && pszSeriesTime[i * iRecordSize])
                    {
                        sprintf(
                            pData + strlen(pData), 
                            "%c%c:%c%c:%c%c", 
                            pszSeriesTime[i * iRecordSize + 0], 
                            pszSeriesTime[i * iRecordSize + 1], 
                            pszSeriesTime[i * iRecordSize + 2], 
                            pszSeriesTime[i * iRecordSize + 3], 
                            pszSeriesTime[i * iRecordSize + 4], 
                            pszSeriesTime[i * iRecordSize + 5]);
                    }

                    if (pszSeriesNumber && pszSeriesNumber[i * iRecordSize])
                    {
                        /* SerieNumber of our PT is rather arbitrary and not useful */
                        if (strcmp(pszModality + i * iRecordSize, "PT") != 0)
                        {
                            if (pData[strlen(pData) - 1] != ' ')
                            {
                                strcat(pData, " ");
                            }

                            strcat(pData, pszSeriesNumber + i * iRecordSize);
                        }
                    }

                    if (pszSeriesDescription && pszSeriesDescription[i * iRecordSize])
                    {
                        /* Remove Siemens advertizing */
                        if (strstr(pszSeriesDescription + i * iRecordSize, " [Dicom Toolbox"))
                        {
                            *strstr(pszSeriesDescription + i * iRecordSize, " [Dicom Toolbox") = 0;
                        }

                        if (pData[strlen(pData) - 1] != ' ')
                        {
                            strcat(pData, " ");
                        }

                        strcat(pData, pszSeriesDescription + i * iRecordSize);

                        /* Determine whether 4D PT is possibly present */
                        if (!b4DPET)
                        {
                            if (strchr(pszSeriesDescription + i * iRecordSize, '%')
                                && (strcmp(pszModality + i * iRecordSize, "PT") == 0))
                            {
                                b4DPET = true;
                            }
                        }
                    }

                    if (pData[14] == 0)
                    {
                        strcat(pData, "(no alias)");
                    }

                    strcpy(pData + iSize, pszSeriesUID + i * iRecordSize);
                    strcpy(pData + 2 * iSize, "series");
                    pData += 3 * iSize;

                    /* Also determine whether EchoNumbers and/or ImageType are needed to discern subvolumes
                	 in an MR Series
                      */
                    bEchoNumbersNeeded = false;
                    bImageTypeNeeded = false;
                    if (strcmp(pszModality + i * iRecordSize, "MR") == 0)
                    {
                        for (j = i + 1; j < iNbImages; j++)
                        {
                            if (strcmp(pszSeriesUID + i * iRecordSize, pszSeriesUID + j * iRecordSize) != 0)
                            {
                                break;
                            }

                            if (strcmp(pszImageType + i * iRecordSize, pszImageType + j * iRecordSize) != 0)
                            {
                                bImageTypeNeeded = true;
                                continue;
                            }

                            if (strcmp(pszEchoNumbers + i * iRecordSize, pszEchoNumbers + j * iRecordSize) != 0)
                            {
                                if (
                                    !(pszSeriesDescription
                                      && (strstr(pszSeriesDescription + i * iRecordSize, "BOLD")
                                          || strstr(pszSeriesDescription + i * iRecordSize, "R2*"))))
                                {
                                    bEchoNumbersNeeded = true;
                                }
                            }
                        }
                    }
                }

                iLastGoodRecord = i;

                strcpy(pData, "DEPTH=4~ALIAS=");
                strcat(pData, pszModality + i * iRecordSize);
                strcpy(pData + iSize, pszSOPInstanceUID + i * iRecordSize);

                if (piNbRecordsOfVolume[i] == 1)
                {
                    /* Object that have not been grouped together */
                    if (strcmp(pszModality + i * iRecordSize, "RTSTRUCT") == 0)
                    {
                        // RTSTRUCT
                        strcpy(pData + 2 * iSize, "structure");
                    }
                    else if (strcmp(pszModality + i * iRecordSize, "RTDOSE") == 0)
                    {
                        // RTDOSE (Single image or Multiframe)
                        strcpy(pData + 2 * iSize, "dose");
                        sprintf(pData + strlen(pData), "~SLICES=%d", 99); // atoi(pszNbFrames + i*iRecordSize));
                        strcat(pData + iSize, "&MfScan");
                    }
                    else if (strcmp(pszModality + i * iRecordSize, "RTPLAN") == 0)
                    {
                        // RTPLAN
                        /* Fill in the alias of the plan */
                        if (DcmPlans[iCurPlanNo].pBeamInfo != NULL)
                        {
                            sprintf(
                                szTmp, 
                                "DEPTH=4~ALIAS=_%s_%s_%s_%s", 
                                DcmPlans[iCurPlanNo].pszRTPlanLabel, 
                                DcmPlans[iCurPlanNo].pszRTPlanName, 
                                DcmPlans[iCurPlanNo].pszRTPlanDate, 
                                DcmPlans[iCurPlanNo].pszRTPlanTime);
                            strncpy(pData, szTmp, iSize);
                        }
                        else
                        {
                            strcat(pData, " (LoadError)");
                        }

                        strcpy(pData + 2 * iSize, "plan");

                        /* Add all beams of this plan */
                        for (j = 0; j < DcmPlans[iCurPlanNo].iNbBeams; j++)
                        {
                            pData += 3 * iSize;
                            sprintf(
                                szTmp, 
                                "DEPTH=5~ALIAS=%d %s", 
                                (DcmPlans[iCurPlanNo].pBeamInfo + j)->iNumber, 
                                (DcmPlans[iCurPlanNo].pBeamInfo + j)->szName);

                            // 	  itoa((DcmPlans[iCurPlanNo].pBeamInfo + j)->iNumber, pData + iSize, 10);
                            sprintf(pData + iSize, "%d", (DcmPlans[iCurPlanNo].pBeamInfo + j)->iNumber);
                            strncpy(pData, szTmp, iSize);
                            strcpy(pData + 2 * iSize, "beam");
                        }

                        iCurPlanNo++;
                    }
                    else
                    {
                        sprintf(pData + strlen(pData), " %s", pszInstanceNumber + i * iRecordSize);
                        if (pszNbFrames && (atoi(pszNbFrames + i * iRecordSize) > 1))
                        {
                            // A multiframe scan
                            strcpy(pData + 2 * iSize, "scan");
                            sprintf(pData + strlen(pData), "~SLICES=%d", atoi(pszNbFrames + i * iRecordSize));
                            strcat(pData + iSize, "&MfScan");
                        }
                        else
                        {
                            strcpy(pData + 2 * iSize, "image"); // A single image
                        }

                        /* Maybe add Kretz private carthesian volume */
                        if ((strcmp(pszModality + i * iRecordSize, "US") == 0)
                            && ((strncmp(pszManufacturer + i * iRecordSize, "Kretz", 5) == 0)
                                || (strncmp(pszManufacturer + i * iRecordSize, "Philips", 7) == 0)))
                        {
                            /* Make a copy of the image entry */
                            memcpy(pData + 3 * iSize, pData, 3 * iSize);
                            pData += 3 * iSize;
                            pData[6]++; // increase depth
                            strcat(pData, "~SLICES=?");
                            if (strncmp(pszManufacturer + i * iRecordSize, "Kretz", 5) == 0)
                            {
                                strcat(pData + iSize, "&Kretz");
                            }
                            else
                            {
                                strcat(pData + iSize, "&Philips");
                            }

                            strcpy(pData + 2 * iSize, "scan");
                        }
                    }

                    if (bEchoNumbersNeeded)
                    {
                        sprintf(
                            pData + iSize + strlen(pData + iSize), 
                            "&EchoNumbers=%s", 
                            pszEchoNumbers + i * iRecordSize);
                    }

                    if (bImageTypeNeeded)
                    {
                        int iLength = strlen(pszImageType + i * iRecordSize);
                        k = 0;

                        // Replace double backslash by a single pound
                        for (j = 0; j < iLength; j++)
                        {
                            if (*(pszImageType + i * iRecordSize + j) != '\\')
                            {
                                *(pszImageType + i * iRecordSize + k) = *(pszImageType + i * iRecordSize + j);
                                k++;
                            }
                            else if (*(pszImageType + i * iRecordSize + j + 1) != '\\')
                            {
                                *(pszImageType + i * iRecordSize + k) = '#';
                                k++;
                            }
                        }

                        *(pszImageType + i * iRecordSize + k) = 0;
                        sprintf(pData + iSize + strlen(pData + iSize), "&ImageType=%s", pszImageType + i * iRecordSize);
                    }
                }
                else
                {
                    /* Objects that have been grouped together */
                    strcat(pData, " ");
                    strcat(pData, pszInstanceNumber + i * iRecordSize);
                    if (strcmp(pszModality + i * iRecordSize, "RTDOSE") == 0)
                    {
                        strcpy(pData + 2 * iSize, "dose");
                    }
                    else
                    {
                        strcpy(pData + 2 * iSize, "scan");

                        // overwrite SOPInstanceUID with "any", since it is not used anywhere
                        strcpy(pData + iSize, "any");
                    }

                    sprintf(pData + strlen(pData), "~SLICES=%d", piNbRecordsOfVolume[i]);
                    if (bEchoNumbersNeeded)
                    {
                        sprintf(
                            pData + iSize + strlen(pData + iSize), 
                            "&EchoNumbers=%s", 
                            pszEchoNumbers + i * iRecordSize);
                    }

                    if (bImageTypeNeeded)
                    {
                        int iLength = strlen(pszImageType + i * iRecordSize);
                        k = 0;

                        // Replace double backslash by a single pound
                        for (j = 0; j < iLength; j++)
                        {
                            if (*(pszImageType + i * iRecordSize + j) != '\\')
                            {
                                *(pszImageType + i * iRecordSize + k) = *(pszImageType + i * iRecordSize + j);
                                k++;
                            }
                            else if (*(pszImageType + i * iRecordSize + j + 1) != '\\')
                            {
                                *(pszImageType + i * iRecordSize + k) = '#';
                                k++;
                            }
                        }

                        *(pszImageType + i * iRecordSize + k) = 0;
                        sprintf(pData + iSize + strlen(pData + iSize), "&ImageType=%s", pszImageType + i * iRecordSize);
                    }
                }

                pData += 3 * iSize;
            }

            free(piNbRecordsOfVolume);

            if (bAreStructsPresent)
            {
                /* There should be space left in order to insert the structures. */
                int iNbRecords;
                List<DcmStruct> Structs;
                bool bIsFirstStructInRtStruct;
                bool bChanged;
                DcmStruct StructSave;

                // pData += 3 * iSize;
                iNbRecords = (pData - (string)*ppOut->data) / (3 * iSize);

                /* RTSTRUCTs are updated repeatedly, so many of them may occur...
       Bubblesort them chronologically.
    */
                bChanged = true;
                while (bChanged)
                {
                    bChanged = false;
                    for (j = 0; j < iTotNbStructs - 1; j++)
                    {
                        if (strcmp(DcmStructs[j].pStructInfo->szDate, DcmStructs[j + 1].pStructInfo->szDate) > 0)
                        {
                            StructSave = DcmStructs[j];
                            DcmStructs[j] = DcmStructs[j + 1];
                            DcmStructs[j + 1] = StructSave;
                            bChanged = TRUE;
                        }
                        else if ((strcmp(DcmStructs[j].pStructInfo->szDate, DcmStructs[j + 1].pStructInfo->szDate) == 0)
                                 && (strcmp(DcmStructs[j].pStructInfo->szTime, DcmStructs[j + 1].pStructInfo->szTime) > 0))
                        {
                            StructSave = DcmStructs[j];
                            DcmStructs[j] = DcmStructs[j + 1];
                            DcmStructs[j + 1] = StructSave;
                            bChanged = true;
                        }
                    }
                }

                /* Find the '.scan' in the tree */
                pData = (string)*ppOut->data;
                for (i = 0; i < iNbRecords; i++)
                {
                    if (strcmp(pData + 2 * iSize, "series") == 0)
                    {
                        pszLastSeriesUID = pData + iSize;
                    }
                    else if (strcmp(pData + 2 * iSize, "scan") == 0)
                    {
                        /* Search the RTSTRUCTs whether they belong to this scan */
                        for (j = 0; j < iTotNbStructs; j++)
                        {
                            pStruct = &DcmStructs[j];
                            bIsFirstStructInRtStruct = TRUE;
                            for (k = 0; k < pStruct->iNbStructs; k++)
                            {
                                if (strcmp(Struct.StructInfo[k]->szReferencedSeries, pszLastSeriesUID) == 0)
                                {
                                    /* Found! Insert it after the scan */
                                    strcpy((pStruct->pStructInfo + k)->szReferencedSeries, "Done!");
                                    if (bIsFirstStructInRtStruct)
                                    {
                                        bIsFirstStructInRtStruct = FALSE;
                                        pData += 3 * iSize;
                                        if (iNbRecords - i - 1 != 0)
                                        {
                                            memmove(pData + 3 * iSize, pData, (iNbRecords - i - 1) * 3 * iSize);
                                        }

                                        memset(pData, 0, 3 * iSize);
                                        sprintf(
                                            pData, 
                                            "DEPTH=5~ALIAS=%s %s", 
                                            (pStruct->pStructInfo + k)->szDate, 
                                            (pStruct->pStructInfo + k)->szTime);
                                        strcpy(pData + iSize, (pStruct->pStructInfo + k)->szSOPInstanceUID);
                                        strcpy(pData + 2 * iSize, "structset");
                                        i++;
                                        iNbRecords++;
                                    }

                                    pData += 3 * iSize;
                                    if (iNbRecords - i - 1 != 0)
                                    {
                                        memmove(pData + 3 * iSize, pData, (iNbRecords - i - 1) * 3 * iSize);
                                    }

                                    memset(pData, 0, 3 * iSize);
                                    sprintf(
                                        pData, 
                                        "DEPTH=6~COLOR=%d~ALIAS=%s", 
                                        (pStruct->pStructInfo + k)->iColor, 
                                        (pStruct->pStructInfo + k)->szName);
                                    sprintf(pData + iSize, "%d", (pStruct->pStructInfo + k)->iNumber);
                                    if ((pStruct->pStructInfo + k)->bIsMarker)
                                    {
                                        strcpy(pData + 2 * iSize, "marker");
                                    }
                                    else
                                    {
                                        strcpy(pData + 2 * iSize, "structure");
                                    }

                                    i++;
                                    iNbRecords++;
                                }
                            }
                        }
                    }

                    pData += 3 * iSize;
                }

                /* Maybe there are 'left-overs': structures that could not be related to
       a scan in this tree...
    */
                bLeftOverStructures = false;
                for (j = 0; j < iTotNbStructs; j++)
                {
                    pStruct = &DcmStructs[j];
                    for (k = 0; k < pStruct->iNbStructs; k++)
                    {
                        if (strcmp((pStruct->pStructInfo + k)->szReferencedSeries, "Done!") != 0)
                        {
                            bLeftOverStructures = true;
                        }
                    }
                }

                *ppOut->dimensions[1] = iNbRecords;

                /* Do some cleanup */
                for (j = 0; j < iTotNbStructs; j++)
                {
                    pStruct = &DcmStructs[j];
                    free(pStruct->pStructInfo);
                }
            }

            AVSfield_free(pQueryResult);

            /* If ImageType was needed to discern subvolumes in MR series, adjust the ALIAS */
            TreeAdjustImageTypeMR(*ppOut);

            // if (b4DPET)
            // TreeAdd4DPET(*ppOut);
            CacheInit(*ppOut);

            for (i = 0; i < iTotNbPlans; i++)
            {
                if (DcmPlans[i].pBeamInfo)
                {
                    CacheSetReferencedStruct(
                        DcmPlans[i].pBeamInfo->szSOPInstanceUID, 
                        DcmPlans[i].pBeamInfo->szReferencedStruct);
                }

                free(DcmPlans[i].pBeamInfo);
                free(DcmPlans[i].pszRTPlanLabel);
                free(DcmPlans[i].pszRTPlanName);
                free(DcmPlans[i].pszRTPlanDate);
                free(DcmPlans[i].pszRTPlanTime);
            }

            return DCM_E_OK;
        }

        /// <summary>The dcm get dicom to siddon xfm.</summary>
        /// <param name="avs">The avs.</param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns>The <see cref="ErrorType"/>.</returns>
        public ErrorType DcmGetDicomToSiddonXfm(out AVSfield avs)
        {
            throw new NotImplementedException();
        }

        /// <summary>The get key.</summary>
        /// <param name="result">The result.</param>
        /// <param name="dcmInfo">The dcm info.</param>
        /// <param name="selInfo">The sel info.</param>
        /// <param name="paramName">The param Name.</param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns>The <see cref="ErrorType"/>.</returns>
        public ErrorType GetKey(out string result, DcmInfo dcmInfo, SelInfo selInfo, string paramName)
        {
            throw new NotImplementedException();
        }

        /// <summary>The read full properties.</summary>
        /// <param name="avs">The avs.</param>
        /// <param name="dcmInfo">The dcm info.</param>
        /// <param name="selInfo">The sel info.</param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns>The <see cref="ErrorType"/>.</returns>
        public ErrorType ReadFullProperties(out AVSfield avs, DcmInfo dcmInfo, SelInfo selInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>The read keys.</summary>
        /// <param name="avs">The avs.</param>
        /// <param name="dcmInfo">The dcm info.</param>
        /// <param name="selInfo">The sel info.</param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns>The <see cref="ErrorType"/>.</returns>
        public ErrorType ReadKeys(out AVSfield avs, DcmInfo dcmInfo, SelInfo selInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>The read modality.</summary>
        /// <param name="avs">The avs.</param>
        /// <param name="dcmInfo">The dcm info.</param>
        /// <param name="selInfo">The sel info.</param>
        /// <param name="paramName">The param Name.</param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns>The <see cref="ErrorType"/>.</returns>
        public ErrorType ReadModality(out AVSfield avs, DcmInfo dcmInfo, SelInfo selInfo, string paramName)
        {
            throw new NotImplementedException();
        }

        /// <summary>The read ref beam.</summary>
        /// <param name="avs">The avs.</param>
        /// <param name="dcmInfo">The dcm info.</param>
        /// <param name="selInfo">The sel info.</param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns>The <see cref="ErrorType"/>.</returns>
        public ErrorType ReadRefBeam(out AVSfield avs, DcmInfo dcmInfo, SelInfo selInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>The structure to world.</summary>
        /// <param name="avs">The avs.</param>
        /// <param name="selInfo">The sel info.</param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns>The <see cref="ErrorType"/>.</returns>
        public ErrorType StructureToWorld(out AVSfield avs, SelInfo selInfo)
        {
            avs = null;
            int rc;
            SEL_INFO SelInfo;
            DICOMDataObject* pDDO;
            VR* pVR;
            char s [
            100]
            ;

            // int			iReferencedBeamNo;

            /* Parse the selection-string */
            ParseSelInfo(&SelInfo, pszSel);
            if (!SelInfo.pszPatientID || !SelInfo.pszStudyUID || !SelInfo.pszSeriesUID)
            {
                return DCM_E_MISSING_PARAMETER;
            }

            /* A reference beam only occurs in an RTIMAGE */
            if (SelInfo.iModality != DCM_MODALITY_IMAGE)
            {
                return DCM_E_MISSING_REFBEAM;
            }

            /* Well, it is an image allright, but is it an RTIMAGE ?
                We only need to query the server to find out. On the other hand, we need the
                header to get the referenced beam-data.
                Skip the query...
            */
            rc = GetRtDDO(&pDDO, pDcmInfo, &SelInfo);
            if (rc || !pDDO)
            {
                return DCM_E_NO_DATASET;
            }

            /* Search for the modality */
            pVR = pDDO->GetVR(0x0008, 0x0060);
            strncpy(s, (string)pVR->Data, pVR->Length);
            s[pVR->Length] = 0;
            if (strncmp(s, "RTIMAGE", 7) != 0)
            {
                delete pDDO;
                return DCM_E_MISSING_REFBEAM;
            }

            /* OK, it is an RTIMAGE
                 The beam-data may be:
                 - In the header of the RTIMAGE itself
                 - Referenced in the header of the RTIMAGE
                 - Absent
              */
            /* Search for the ExposureSequence */
            pVR = pDDO->GetVR(0x3002, 0x0030);
            if (pVR)
            {
                /* Yes, we should be able to construct the beam-data from the RTIMAGE.
                   Were it not that ISOC-info is missing, at least in GeneralElectric
                   RTIMAGEs...
                   OK, no shift!
                */
                rc = GetRtBeamFromRtImage(ppOut, pDDO);
                delete pDDO;

                /* The XFMs are stored in cache under the RTIMAGE */
                CalcBeamXfms(pszSel);
                return rc;
            }

            delete pDDO;
            return ErrorType.DCM_E_MISSING_REFBEAM;
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DcmInfo.cs" company="Elekta">
//   Copy right by Elekta Shanghai Office.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MosaiqDicom.ModelData
{
    #region Using

    using MosaiqDicom.Utilities;

    #endregion

    /// <summary>The dcm info.</summary>
    public class DcmInfo
    {
        /// <summary>Initializes a new instance of the <see cref="DcmInfo"/> class.</summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="directoryName">The directory name.</param>
        public DcmInfo(out ErrorType errorCode, string directoryName)
        {
            errorCode = ErrorType.DCM_E_MISSING_PARAMETER;

            var infoOrder = 0;

            if (!string.IsNullOrEmpty(directoryName))
            {
                return;
            }

            if ((directoryName[0] == ';') || (directoryName[0] == ':'))
            {
                directoryName = directoryName.Substring(1);
            }

            var arrSegments = directoryName.Split(';', ':');
            foreach (var str in arrSegments)
            {
                switch (infoOrder)
                {
                    case 0:
                        this.RemoteAE = str;
                        break;
                    case 1:
                        this.RemoteIP = str;
                        break;
                    case 2:
                        this.RemotePort = str;
                        break;
                    case 3:
                        this.LocalAE = str;
                        break;
                    case 4:
                        this.LocalPort = str;
                        break;
                    case 5:
                        this.SOP = str;
                        break;

                    /* The old format: LocalAE and LocalPort are included in DirectoryName */
                    case 6:
                        this.PatientID = str;
                        errorCode = ErrorType.DCM_E_OK;
                        break;
                    default:
                        errorCode = ErrorType.DCM_E_MISSING_PARAMETER;
                        break;
                }

                ++infoOrder;
            }

            if (infoOrder == 5)
            {
                /* New format: LocalAE and LocalPort must be set seperately */
                this.PatientID = this.LocalPort;
                this.SOP = this.LocalAE;

                // TODO: Build the global parameters.
                this.LocalPort = string.Copy("gl_szLocalPort");
                this.LocalAE = string.Copy("gl_szLocalAE");
                errorCode = ErrorType.DCM_E_OK;
            }
        }

        /// <summary>Gets or sets the local ae.</summary>
        public string LocalAE { get; set; }

        /// <summary>Gets or sets the local port.</summary>
        public string LocalPort { get; set; }

        /// <summary>Gets or sets the patient id.</summary>
        public string PatientID { get; set; }

        /// <summary>Gets or sets the remote ae.</summary>
        public string RemoteAE { get; set; }

        /// <summary>Gets or sets the remote ip.</summary>
        public string RemoteIP { get; set; }

        /// <summary>Gets or sets the remote port.</summary>
        public string RemotePort { get; set; }

        /// <summary>Gets or sets the sop.</summary>
        public string SOP { get; set; }
    }
}
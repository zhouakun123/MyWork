// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModalityType.cs" company="Elekta">
//   Copy right by Elekta Shanghai Office.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MosaiqDicom.Utilities
{
    /// <summary>The modality type.</summary>
    public enum ModalityType
    {
        /* Modalities supported by the TREE-reader: */

        /// <summary>The dc m_ modalit y_ unknown.</summary>
        DCM_MODALITY_UNKNOWN, 

        /// <summary>The dc m_ modalit y_ scan.</summary>
        DCM_MODALITY_SCAN, 

        /// <summary>The dc m_ modalit y_ image.</summary>
        DCM_MODALITY_IMAGE, 

        /// <summary>The dc m_ modalit y_ structure.</summary>
        DCM_MODALITY_STRUCTURE, 

        /// <summary>The dc m_ modalit y_ beam.</summary>
        DCM_MODALITY_BEAM, 

        /// <summary>The dc m_ modalit y_ dose.</summary>
        DCM_MODALITY_DOSE, 

        /* Other 'modalities': (only properties can be asked; no imaging-data present) */

        /// <summary>The dc m_ modalit y_ patient.</summary>
        DCM_MODALITY_PATIENT, 

        /// <summary>The dc m_ modalit y_ study.</summary>
        DCM_MODALITY_STUDY, 

        /// <summary>The dc m_ modalit y_ series.</summary>
        DCM_MODALITY_SERIES, 

        /// <summary>The dc m_ modalit y_ plan.</summary>
        DCM_MODALITY_PLAN, 

        /// <summary>The dc m_ modalit y_ structset.</summary>
        DCM_MODALITY_STRUCTSET
    }
}
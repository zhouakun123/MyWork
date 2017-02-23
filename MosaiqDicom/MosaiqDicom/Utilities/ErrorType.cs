// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorType.cs" company="Elekta">
//   Copy right by Elekta Shanghai Office.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MosaiqDicom.Utilities
{
    /// <summary>The error type.</summary>
    public enum ErrorType
    {
        /* CqDicom ErrorCodes */

        /// <summary>The dc m_ e_ ok.</summary>
        DCM_E_OK, 

        /// <summary>The dc m_ e_ inpu t_ lib.</summary>
        DCM_E_INPUT_LIB, // unused

        /// <summary>The dc m_ e_ outpu t_ lib.</summary>
        DCM_E_OUTPUT_LIB, // unused

        /// <summary>The dc m_ e_ inpu t_ dicom.</summary>
        DCM_E_INPUT_DICOM, 

        /// <summary>The dc m_ e_ outpu t_ header.</summary>
        DCM_E_OUTPUT_HEADER, 

        /// <summary>The dc m_ e_ memory.</summary>
        DCM_E_MEMORY, 

        /// <summary>The dc m_ e_ n o_ inputfield.</summary>
        DCM_E_NO_INPUTFIELD, 

        /// <summary>The dc m_ e_ connect.</summary>
        DCM_E_CONNECT, 

        /// <summary>The dc m_ e_ incompatibl e_ veclen.</summary>
        DCM_E_INCOMPATIBLE_VECLEN, 

        /// <summary>The dc m_ e_ format.</summary>
        DCM_E_FORMAT, 

        /// <summary>The dc m_ e_ invali d_ vr.</summary>
        DCM_E_INVALID_VR, 

        /// <summary>The dc m_ e_ create.</summary>
        DCM_E_CREATE, 

        /// <summary>The dc m_ e_ assoc.</summary>
        DCM_E_ASSOC, 

        /// <summary>The dc m_ e_ fin d_ rsp.</summary>
        DCM_E_FIND_RSP, 

        /// <summary>The dc m_ e_ mov e_ rsp.</summary>
        DCM_E_MOVE_RSP, 

        /// <summary>The dc m_ e_ n o_ dataset.</summary>
        DCM_E_NO_DATASET, 

        /// <summary>The dc m_ e_ rc v_ data.</summary>
        DCM_E_RCV_DATA, 

        /// <summary>The dc m_ e_ rc v_ grou p 0.</summary>
        DCM_E_RCV_GROUP0, // discontinued

        /// <summary>The dc m_ e_ incomplet e_ rsp.</summary>
        DCM_E_INCOMPLETE_RSP, // discontinued

        /// <summary>The dc m_ e_ internal.</summary>
        DCM_E_INTERNAL, 

        /// <summary>The dc m_ e_ remote.</summary>
        DCM_E_REMOTE, 

        /// <summary>The dc m_ e_ serverthread.</summary>
        DCM_E_SERVERTHREAD, 

        /// <summary>The dc m_ e_ sliceorder.</summary>
        DCM_E_SLICEORDER, // discontinued

        /// <summary>The dc m_ e_ inputfile.</summary>
        DCM_E_INPUTFILE, 

        /// <summary>The dc m_ e_ inputfil e_ format.</summary>
        DCM_E_INPUTFILE_FORMAT, 

        /// <summary>The dc m_ e_ outputfile.</summary>
        DCM_E_OUTPUTFILE, 

        /// <summary>The dc m_ e_ unsupporte d_ compression.</summary>
        DCM_E_UNSUPPORTED_COMPRESSION, 

        /// <summary>The dc m_ e_ missin g_ vr.</summary>
        DCM_E_MISSING_VR, 

        /// <summary>The dc m_ e_ unsupporte d_ format.</summary>
        DCM_E_UNSUPPORTED_FORMAT, 

        /// <summary>The dc m_ e_ n o_ xfm.</summary>
        DCM_E_NO_XFM, 

        /// <summary>The dc m_ e_ duplicat e_ slices.</summary>
        DCM_E_DUPLICATE_SLICES, 

        /// <summary>The dc m_ e_ inconsistan t_ slices.</summary>
        DCM_E_INCONSISTANT_SLICES, 

        /// <summary>The dc m_ e_ missin g_ coords.</summary>
        DCM_E_MISSING_COORDS, 

        /// <summary>The dc m_ e_ missin g_ parameter.</summary>
        DCM_E_MISSING_PARAMETER, 

        /// <summary>The dc m_ e_ n o_ response.</summary>
        DCM_E_NO_RESPONSE, 

        /// <summary>The dc m_ e_ serverinfo.</summary>
        DCM_E_SERVERINFO, 

        /// <summary>The dc m_ e_ servercorrupt.</summary>
        DCM_E_SERVERCORRUPT, 

        /// <summary>The dc m_ e_ n o_ keyword.</summary>
        DCM_E_NO_KEYWORD, 

        /// <summary>The dc m_ e_ stor e_ rsp.</summary>
        DCM_E_STORE_RSP, 

        /// <summary>The dc m_ e_ missin g_ refbeam.</summary>
        DCM_E_MISSING_REFBEAM, 

        /// <summary>The dc m_ e_ prin t_ film.</summary>
        DCM_E_PRINT_FILM, 

        /// <summary>The dc m_ e_ prin t_ session.</summary>
        DCM_E_PRINT_SESSION, 

        /// <summary>The dc m_ e_ kret z_ no t_ present.</summary>
        DCM_E_KRETZ_NOT_PRESENT, 

        /// <summary>The dc m_ e_ kret z_ no t_ supported.</summary>
        DCM_E_KRETZ_NOT_SUPPORTED, 

        /// <summary>The dc m_ e_ invali d_ rtplan.</summary>
        DCM_E_INVALID_RTPLAN, 

        /// <summary>The dc m_ e_ multipl e_ rtplan.</summary>
        DCM_E_MULTIPLE_RTPLAN, 

        /// <summary>The dc m_ e_ respirator y_ cycle.</summary>
        DCM_E_RESPIRATORY_CYCLE, 

        /// <summary>The dc m_ e_ ctre f_ redefined.</summary>
        DCM_E_CTREF_REDEFINED, 

        /// <summary>The dc m_ e_ iso c_ redefined.</summary>
        DCM_E_ISOC_REDEFINED, 

        /// <summary>The dc m_ e_ thr d_ fail.</summary>
        DCM_E_THRD_FAIL, 

        /// <summary>The dc m_ e_ thr d_ par m_ fail.</summary>
        DCM_E_THRD_PARM_FAIL, 

        /// <summary>The dc m_ e_ thr d_ re t_ re s_ fail.</summary>
        DCM_E_THRD_RET_RES_FAIL, 

        /// <summary>The dc m_ e_ thr d_ timeout.</summary>
        DCM_E_THRD_TIMEOUT, 

        /// <summary>The dc m_ e_ interrupted.</summary>
        DCM_E_INTERRUPTED
    }
}
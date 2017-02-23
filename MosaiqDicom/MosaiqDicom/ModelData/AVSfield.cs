// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AVSfield.cs" company="Elekta">
//   Copy right by Elekta Shanghai Office.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MosaiqDicom.ModelData
{
    #region Using

    using System.IO;

    #endregion

    /// <summary>The av sfield.</summary>
    public class AVSfield
    {
        /// <summary>The av s_ maxdim.</summary>
        private static readonly int AVS_MAXDIM = 5;

        /// <summary>The nspace*2 or sum(dims) or prod(dims) coords points.</summary>
        public double[] points;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AVSfield" /> class. Prevents a default instance of the
        ///     <see cref="AVSfield" /> class from being created.
        /// </summary>
        public AVSfield()
        {
            this.dimensions = new long[AVS_MAXDIM];
            this.max_extent = new double[AVS_MAXDIM];
            this.min_extent = new double[AVS_MAXDIM];
        }

        /// <summary>points to data array[dimensions x veclen].</summary>
        public Stream data { get; set; }

        /// <summary>The dimensions.</summary>
        public long[] dimensions { get; set; }

        /// <summary>The nspace, maximum coordinate.</summary>
        public double[] max_extent { get; set; }

        /// <summary>The nspace, minimum coordinate.</summary>
        public double[] min_extent { get; set; }

        /// <summary>The dimensionality of data.</summary>
        public long ndim { get; set; }

        /// <summary>The number of coordinate dimensions.</summary>
        public long nspace { get; set; }

        /// <summary>Gets or sets the veclen * sizeof(type).</summary>
        public long size { get; set; }

        /// <summary>Gets or sets the type of data (byte, short, int, float).</summary>
        public long type { get; set; }

        /// <summary>The type of coordinate system.</summary>
        public long uniform { get; set; }

        /// <summary>Gets or sets the number of elements per data point.</summary>
        public long veclen { get; set; }
    }
}
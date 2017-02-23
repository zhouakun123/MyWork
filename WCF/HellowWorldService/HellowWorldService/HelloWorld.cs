// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HelloWorld.cs" company="Hx">
//   CopyRight by Hx
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace HellowWorldService
{
    #region Using

    using System.Runtime.Serialization;
    using System.ServiceModel;

    #endregion

    /// <summary>The name.</summary>
    [DataContract]
    public class Name
    {
        #region Fields

        /// <summary>The first.</summary>
        [DataMember]
        public string First;

        /// <summary>The last.</summary>
        [DataMember]
        public string Last;

        #endregion
    }

    /// <summary>The HelloWorld interface.</summary>
    [ServiceContract]
    public interface IHelloWorld
    {
        #region Public methods and operators

        /// <summary>The say hello.</summary>
        /// <param name="persoName">The perso name.</param>
        /// <returns>The <see cref="string"/>.</returns>
        [OperationContract]
        string SayHello(Name persoName);

        #endregion
    }

    /// <summary>The hello world service.</summary>
    public class HelloWorldService : IHelloWorld
    {
        #region Interface methods

        /// <summary>The say hello.</summary>
        /// <param name="persoName">The perso name.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string SayHello(Name persoName)
        {
            return string.Format("Hello {0} {1}", persoName.First, persoName.Last);
        }

        #endregion
    }
}
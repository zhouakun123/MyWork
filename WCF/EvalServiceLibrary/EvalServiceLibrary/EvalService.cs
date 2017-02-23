using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvalServiceLibrary
{
    using System.Runtime.Serialization;
    using System.ServiceModel;

    [DataContract]
    public class Eval
    {
        [DataMember]
        public string Submitter;
        [DataMember]
        public DateTime Timesent;
        [DataMember]
        public string Comments;
    }

    [ServiceContract]
    public interface IEvalService
    {
        [OperationContract]
        void SubmitEval(Eval eval);
        [OperationContract]
        List<Eval> GetEvals();

    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class EvalService : IEvalService
    {
        List<Eval> evals = new List<Eval>();
 
        public void SubmitEval(Eval eval)
        {
            if (eval.Submitter.Equals("Throw"))
            {
                throw new FaultException("Error within SubmitEval");
            }
            this.evals.Add(eval);
            //throw new FaultException("Something bad happened.");
        }

        public List<Eval> GetEvals()
        {
            System.Threading.Thread.Sleep(5000);
            return this.evals;
        }
    }
}

namespace Client
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Description;

    using Client.EvalServiceReference;

    using EvalServiceLibrary;

    using IEvalService = EvalServiceLibrary.IEvalService;

    internal class Program
    {
        #region Static methods

        public static void Main(string[] args)
        {
            // var cf = new ChannelFactory<IEvalServiceChannel>("NetNamedPipeBinding_IEvalService");
            // var channel = cf.CreateChannel();
            var endpoints = MetadataResolver.Resolve(
                typeof(IEvalService), 
                new EndpointAddress("http://localhost:8080/evals/mex"));
            foreach (var se in endpoints)
            {
                var channel = new EvalServiceClient(se.Binding, se.Address);

                // var channel = new EvalServiceClient("WSHttpBinding_IEvalService");
                try
                {
                    var eval = new Eval();
                    eval.Submitter = "Howard";
                    eval.Timesent = DateTime.Now;
                    eval.Comments = "I'm thinking this...";

                    channel.SubmitEval(eval);
                    channel.SubmitEval(eval);
                    var evals = channel.GetEvals();
                    Console.WriteLine("Number of evals: {0}", evals.Count);
                    channel.Close();
                }
                catch (FaultException fe)
                {
                    Console.WriteLine("FaultException handler: {0}", fe.GetType());
                    channel.Abort();
                }
                catch (CommunicationException ce)
                {
                    Console.WriteLine("CommunicationException handler: {0}", ce.GetType());
                    channel.Abort();
                }
                catch (TimeoutException te)
                {
                    Console.WriteLine("TimeoutException handler: {0}", te.GetType());
                    channel.Abort();
                }
            }

            Console.ReadLine();
        }

        #endregion
    }
}
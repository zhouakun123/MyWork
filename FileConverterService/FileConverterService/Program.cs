using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileConverterService
{
    using System.Threading;

    using Topshelf;

    class Program
    {
        static void Main(string[] args)
        {
           // Console.WriteLine("I'm here.");
            //Thread.Sleep(5000);

            HostFactory.Run(
                serviceConfig =>
                    {
                        //serviceConfig.DependsOn("serviceName");
                        //serviceConfig.DependsOnEventLog();
                        //serviceConfig.DependsOnIis();
                        //serviceConfig.DependsOnMsSql();
                        //serviceConfig.DependsOnMsmq();

                        //serviceConfig.RunAs("username", "password");
                        //serviceConfig.RunAsLocalService();
                        //serviceConfig.RunAsLocalSystem();
                        //serviceConfig.RunAsNetworkService();
                        //serviceConfig.RunAsPrompt();

                        serviceConfig.StartManually();
                        serviceConfig.StartAutomatically();
                        serviceConfig.StartAutomaticallyDelayed();
                        serviceConfig.Disabled();

                        serviceConfig.UseNLog();
                        serviceConfig.Service<ConverterService>(
                            serviceInstance =>
                                {
                                    serviceInstance.ConstructUsing(() => new ConverterService());
                                    serviceInstance.WhenStarted(excute => excute.Start());
                                    serviceInstance.WhenStopped(excute => excute.Stop());
                                    serviceInstance.WhenPaused(excute => excute.Pause());
                                    serviceInstance.WhenContinued(excute => excute.Continue());
                                    serviceInstance.WhenCustomCommandReceived((excute, hostControl, commandNumber) => excute.CustomCommand(commandNumber));
                                });

                        serviceConfig.EnableServiceRecovery(
                            recoveryOption =>
                                {
                                    recoveryOption.RestartService(1);
                                    recoveryOption.RestartComputer(60, "Elekta demo");
                                    recoveryOption.RunProgram(5, @"c:\someprogram.exe");
                                });

                        serviceConfig.EnablePauseAndContinue();

                        serviceConfig.SetServiceName("AwesomeFileConverter");
                        serviceConfig.SetDisplayName("Awesome File Converter");
                        serviceConfig.SetDescription("A Elekta demo service");

                        serviceConfig.StartAutomatically();
                    });
        }
    }
}

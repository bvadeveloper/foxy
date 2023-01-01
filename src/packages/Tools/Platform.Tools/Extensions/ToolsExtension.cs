using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Tools.Abstractions;
using Platform.Tools.Models;

namespace Platform.Tools.Extensions
{
    public static class ToolsExtension
    {
        public static IServiceCollection AddTools(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<List<ToolModel>>(options =>
                configuration.GetSection("Tools").Bind(options));

            services.AddSingleton<IToolsHolder, ToolsHolder>();
            services.AddSingleton<TargetMarksRepository>();

            return services;
        }

        public static IEnumerable<Task<OutputModel>> RunToolProcesses(this IEnumerable<ToolModel> tools, string target) =>
            tools.Select(model => model.RunCommandline(model.MakeCommandLine(target)));

        public static IEnumerable<Task<OutputModel>> RunToolsVersionProcesses(this IEnumerable<ToolModel> tools) =>
            tools.Select(model => model.RunCommandline(model.VersionCommandLine));

        private static Task<OutputModel> RunCommandline(this ToolModel tool, string commandLine) =>
            Task.Run(() =>
            {
                var outputModel = new OutputModel { ToolName = tool.Name };

                try
                {
                    using var process = new Process();
                    process.StartInfo = MakeProcessInfo(commandLine);
                    process.Start();

                    outputModel.Output += process.StandardOutput.ReadToEnd();
                    outputModel.ErrorOutput += process.StandardError.ReadToEnd();

                    process.WaitForExit();
                    outputModel.Successful = !string.IsNullOrWhiteSpace(outputModel.Output) 
                                             || string.IsNullOrWhiteSpace(outputModel.ErrorOutput);
                }
                catch (Exception e)
                {
                    outputModel.ErrorOutput += Environment.NewLine + commandLine + Environment.NewLine + e.GetBaseException().Message;
                    outputModel.ExecutionException = e;
                    outputModel.Successful = false;
                }

                return outputModel;
            }, MakeTimeoutCancellationToken(tool.Timeout));

        private static ProcessStartInfo MakeProcessInfo(string command) =>
            new()
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{command.Replace("\"", "\\\"")}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

        private static CancellationToken MakeTimeoutCancellationToken(int timeout) =>
            new CancellationTokenSource(timeout).Token;
    }
}
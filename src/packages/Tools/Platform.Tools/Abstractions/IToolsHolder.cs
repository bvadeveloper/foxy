using System.Collections.Generic;
using System.Threading.Tasks;
using Platform.Contract;
using Platform.Contract.Enums;
using Platform.Tools.Models;

namespace Platform.Tools.Abstractions
{
    public interface IToolsHolder
    {
        /// <summary>
        /// Run tools for target
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        Task<OutputModel[]> RunTools(string target);

        /// <summary>
        /// Run tool version command
        /// </summary>
        /// <returns></returns>
        Task<OutputModel[]> RunVersion();

        /// <summary>
        /// Get new toolset of filtered tools
        /// </summary>
        /// <param name="tools"></param>
        /// <returns>IToolsHolder</returns>
        IToolsHolder FilterByTools(IEnumerable<string> tools);

        /// <summary>
        /// Get new toolset of filtered tags
        /// </summary>
        /// <param name="targetMarks"></param>
        /// <returns>IToolsHolder</returns>
        IToolsHolder FilterByTargetMarks(Dictionary<TargetType, List<string>> targetMarks);
    }
}
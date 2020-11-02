using System;
using System.Collections.Generic;
using System.Text;

namespace Shatter.Discord.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ExecutionModuleAttribute : Attribute
    {
        public ExecutionModuleAttribute(string Group)
        {
            GroupName = Group;
        }

        public string GroupName { get; set; }
    }
}

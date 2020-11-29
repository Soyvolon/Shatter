using System;

namespace Shatter.Discord.Commands.Attributes
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class ExecutionModuleAttribute : Attribute
    {
        public ExecutionModuleAttribute(string Group)
        {
			this.GroupName = Group;
        }

        public string GroupName { get; set; }
    }
}

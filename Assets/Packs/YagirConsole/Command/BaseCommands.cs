using ConsoleShell;
using System.Collections.Generic;
using UnityEngine;

namespace MyProj
{
    public class BaseCommands : ConsoleManagementBase
    {
        public BaseCommands() 
        {
            TestCommand();
        }


        private void TestCommand()
        {
            AddCommand("/test", 
                new List<Argument>(), 
                delegate(ArgumentsShell shell) 
                {
                    Debug.Log("Hello i ready work");
                });
        }
    }
}

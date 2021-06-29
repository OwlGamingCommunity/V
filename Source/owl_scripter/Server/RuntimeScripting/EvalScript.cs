using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ScripterCommands.RuntimeScripting
{
	public static class EvalScript
	{
		public static async Task<Script> CreateScriptAsync(string strInput, Globals globals, ScriptOptions options)
		{
			 return await Task.Run(() => CreateScript(strInput, globals, options)).ConfigureAwait(true);
		}

		private static Script CreateScript(string strInput, Globals globals, ScriptOptions options)
		{
			Microsoft.CodeAnalysis.Scripting.Script script = null;
			try
			{
				script = CSharpScript.Create(strInput, options, typeof(Globals));
				script.Compile();
			}
			catch(Exception)
			{

			}

			return script;
		}
	}
}
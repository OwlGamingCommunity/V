using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Archiver
{
	internal class Program
	{
		struct EventDescriptorFile
		{
#pragma warning disable 0649
			public EventType Type;
			public Dictionary<string, string> Parameters;
#pragma warning restore 0649
		}

		static void Main()
		{
			const string strClientEventManagerOutputPath = "EventManager.Definitions.Client.cs";
			const string strServerEventManagerOutputPath = "EventManager.Definitions.cs";
			const string strEventIDsOutputPath = "NetworkEventIDs.cs";
			const string strEventIDsUIOutputPath = "UIEventIDs.Client.cs";

			List<string> strLines_ClientEventManager = new List<string>()
			{
				"using System;",
				"using System.Collections.Generic;",
				"using System.Linq;",
				"using System.Reflection;",
				"using System.Collections;",
				"using RAGE;",
				"using RAGE.Elements;",
				"using PlayerType = RAGE.Elements.Player;",
				"using ObjectType = RAGE.Elements.MapObject;",
				"using VehicleType = RAGE.Elements.Vehicle;",
				"public static class NetworkEvents",
				"{"
			};

			List<string> strLines_ClientEventManager_Sender = new List<string>()
			{
				"public static class NetworkEventSender",
				"{"
			};

			List<string> strLines_ClientEventManager_UI = new List<string>()
			{
				"public static class UIEvents",
				"{"
			};

			List<string> strLines_ServerEventManager = new List<string>()
			{
				"using System;",
				"using System.Collections.Generic;",
				"using System.Linq;",
				"using System.Reflection;",
				"using System.Collections;",
				"using GTANetworkAPI;",
				"using PlayerType = GTANetworkAPI.Player;",
				"using ObjectType = GTANetworkAPI.Object;",
				"using VehicleType = GTANetworkAPI.Vehicle;",
				"public static class NetworkEvents",
				"{"
			};

			List<string> strLines_ServerEventManager_Sender = new List<string>()
			{
				"public static class NetworkEventSender",
				"{"
			};

			List<string> strLines_EventIDs = new List<string>()
			{
				"using System;",
				"public enum NetworkEventID",
				"{",
			};

			List<string> strLines_EventIDs_UI = new List<string>()
			{
				"using System;",
				"public enum UIEventID",
				"{",
			};

			Directory.SetCurrentDirectory("..");

			// UI DESCRIPTORS
			foreach (string descriptorfile in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "..", "EventDescriptors", "UI"), "*.json"))
			{
				string strEventName = Path.GetFileNameWithoutExtension(descriptorfile);

				string data = File.ReadAllText(descriptorfile);
				EventDescriptorFile descriptor = JsonConvert.DeserializeObject<EventDescriptorFile>(data);

				if (descriptor.Type == EventType.UI)
				{
					//public delegate void UIGotoFullScreenBrowserDelegate(string strURL);
					//public static UIGotoFullScreenBrowserDelegate UI_GotoFullScreenBrowser;
					string strClientDelegate = String.Format("\tpublic delegate void {0}Delegate(", strEventName);

					// Parse parameters
					strClientDelegate += GenerateFunctionParametersDefinition(descriptor.Parameters, false);

					// terminator
					strClientDelegate += ");";

					// Add to list
					strLines_ClientEventManager_UI.Add(strClientDelegate);
					strLines_ClientEventManager_UI.Add(String.Format("\tpublic static {0}Delegate {0};", strEventName));

					///////////////////// ENUM UI EVENT ID /////////////////////
					strLines_EventIDs_UI.Add(String.Format("\t{0},", strEventName));
					///////////////////// ENUM UI EVENT ID END /////////////////////
				}
			}

			// NETWORK DESCRIPTORS
			foreach (string descriptorfile in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "..", "EventDescriptors", "Network"), "*.json"))
			{
				bool bAddedToEnum = false;
				string strEventName = Path.GetFileNameWithoutExtension(descriptorfile);

				string data = File.ReadAllText(descriptorfile);
				EventDescriptorFile descriptor = JsonConvert.DeserializeObject<EventDescriptorFile>(data);

				if (descriptor.Type == EventType.Network_ClientToServer_RemoteOnly || descriptor.Type == EventType.Network_ClientToServer_RemoteAndLocal || descriptor.Type == EventType.Network_Bidirectional_RemoteOnly || descriptor.Type == EventType.Network_Bidirectional_RemoteAndLocal)
				{
					///////////////////// SERVER DELEGATE /////////////////////
					string strServerDelegate = String.Format("\tpublic delegate void {0}Delegate(", strEventName);

					// Parse parameters
					strServerDelegate += GenerateFunctionParametersDefinition(descriptor.Parameters, true);

					// terminator
					strServerDelegate += ");";

					// Add to list
					strLines_ServerEventManager.Add(strServerDelegate);
					strLines_ServerEventManager.Add(String.Format("\tpublic static {0}Delegate {0};", strEventName));
					///////////////////// SERVER DELEGATE END /////////////////////

					///////////////////// CLIENT TRIGGER CODE /////////////////////
					string strClientTriggerFunc = String.Format("\tpublic static void SendNetworkEvent_{0}(", strEventName);

					// Parse parameters
					// Add player for server events
					strClientTriggerFunc += GenerateFunctionParametersDefinition(descriptor.Parameters, false);
					string strClientFuncArgs = GenerateFunctionParameters(descriptor.Parameters, false);

					// terminator
					strClientTriggerFunc += ") { ";
					strClientTriggerFunc += String.Format("EventManager.TriggerRemoteEvent(NetworkEventID.{0}{1});", strEventName, strClientFuncArgs.Length > 0 ? String.Format(", {0}", strClientFuncArgs) : "");
					strClientTriggerFunc += " }";

					// Add to list
					strLines_ClientEventManager_Sender.Add(strClientTriggerFunc);
					///////////////////// CLIENT TRIGGER CODE END /////////////////////

					///////////////////// ENUM NETWORK EVENT ID /////////////////////
					if (!bAddedToEnum)
					{
						bAddedToEnum = true;
						strLines_EventIDs.Add(String.Format("\t{0},", strEventName));
					}
					///////////////////// ENUM NETWORK EVENT ID END /////////////////////

					// add local trigger code if required (this is added to the target)
					if (descriptor.Type == EventType.Network_ClientToServer_RemoteAndLocal)
					{
						string strServerTriggerFunc = String.Format("\tpublic static void SendLocalEvent_{0}(", strEventName);

						// Parse parameters
						// Add player for server events
						string strServerFuncArgs = GenerateFunctionParameters(descriptor.Parameters, true);
						strServerTriggerFunc += GenerateFunctionParametersDefinition(descriptor.Parameters, true);

						// terminator
						strServerTriggerFunc += ") { ";
						strServerTriggerFunc += String.Format("NetworkEvents.{0}?.Invoke({1});", strEventName, strServerFuncArgs);
						strServerTriggerFunc += " }";

						// Add to list
						strLines_ServerEventManager.Add(strServerTriggerFunc);
					}
				}

				if (descriptor.Type == EventType.Network_ServerToClient_RemoteOnly || descriptor.Type == EventType.Network_ServerToClient_RemoteAndLocal || descriptor.Type == EventType.Network_Bidirectional_RemoteOnly || descriptor.Type == EventType.Network_Bidirectional_RemoteAndLocal)
				{
					///////////////////// CLIENT DELEGATE /////////////////////
					string strClientDelegate = String.Format("\tpublic delegate void {0}Delegate(", strEventName);

					// Parse parameters
					strClientDelegate += GenerateFunctionParametersDefinition(descriptor.Parameters, false);

					// terminator
					strClientDelegate += ");";

					// Add to list
					strLines_ClientEventManager.Add(strClientDelegate);

					strLines_ClientEventManager.Add(String.Format("\tpublic static {0}Delegate {0};", strEventName));
					///////////////////// CLIENT DELEGATE END /////////////////////

					///////////////////// SERVER TRIGGER CODE /////////////////////
					string strServerTriggerFunc = String.Format("\tpublic static void SendNetworkEvent_{0}(", strEventName);

					// Parse parameters
					// Add player for server events
					strServerTriggerFunc += GenerateFunctionParametersDefinition(descriptor.Parameters, true);
					string strServerFuncArgs = GenerateFunctionParameters(descriptor.Parameters, false);

					// terminator
					strServerTriggerFunc += ") { ";
					strServerTriggerFunc += String.Format("EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.{0}{1});", strEventName, strServerFuncArgs.Length > 0 ? String.Format(", {0}", strServerFuncArgs) : "");
					strServerTriggerFunc += " }";

					// Add to list
					strLines_ServerEventManager_Sender.Add(strServerTriggerFunc);

					// Create ForAll edition
					//
					//SendNetworkEvent_{0}
					string strFuncCall = "foreach (var a_Player in PlayerPool.GetAllPlayers()) { ";
					strFuncCall += String.Format("EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.{0}{1});", strEventName, strServerFuncArgs.Length > 0 ? String.Format(", {0}", strServerFuncArgs) : "");
					strFuncCall += "}";

					string strForAllFunc = String.Format("\tpublic static void SendNetworkEvent_{0}_ForAll_SpawnedOnly({1})", strEventName, GenerateFunctionParametersDefinition(descriptor.Parameters, false));
					strForAllFunc += "{ ";
					strForAllFunc += strFuncCall;
					strForAllFunc += " }";

					strLines_ServerEventManager_Sender.Add(strForAllFunc);

					// ForAll which includes not logged/spawned in players
					string strFuncCallIncludeNotLoggedIn = "foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { ";
					strFuncCallIncludeNotLoggedIn += String.Format("EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.{0}{1});", strEventName, strServerFuncArgs.Length > 0 ? String.Format(", {0}", strServerFuncArgs) : "");
					strFuncCallIncludeNotLoggedIn += "}";

					string strForAllFunc_IncludeNotLoggedIn = String.Format("\tpublic static void SendNetworkEvent_{0}_ForAll_IncludeEveryone({1})", strEventName, GenerateFunctionParametersDefinition(descriptor.Parameters, false));
					strForAllFunc_IncludeNotLoggedIn += "{ ";
					strForAllFunc_IncludeNotLoggedIn += strFuncCallIncludeNotLoggedIn;
					strForAllFunc_IncludeNotLoggedIn += " }";

					strLines_ServerEventManager_Sender.Add(strForAllFunc_IncludeNotLoggedIn);
					///////////////////// SERVER TRIGGER CODE END /////////////////////

					///////////////////// ENUM NETWORK EVENT ID /////////////////////
					if (!bAddedToEnum)
					{
						bAddedToEnum = true;
						strLines_EventIDs.Add(String.Format("\t{0},", strEventName));
					}

					///////////////////// ENUM NETWORK EVENT ID END /////////////////////

					// add local trigger code if required (this is added to the target)
					if (descriptor.Type == EventType.Network_ServerToClient_RemoteAndLocal || descriptor.Type == EventType.Network_Bidirectional_RemoteAndLocal)
					{
						string strClientTriggerFunc = String.Format("\tpublic static void SendLocalEvent_{0}(", strEventName);

						// Parse parameters
						// Add player for server events
						string strClientFuncArgs = GenerateFunctionParameters(descriptor.Parameters, false);
						strClientTriggerFunc += GenerateFunctionParametersDefinition(descriptor.Parameters, false);

						// terminator
						strClientTriggerFunc += ") { ";
						strClientTriggerFunc += String.Format("NetworkEvents.{0}?.Invoke({1});", strEventName, strClientFuncArgs);
						strClientTriggerFunc += " }";

						// Add to list
						strLines_ClientEventManager.Add(strClientTriggerFunc);
					}
				}

				if (descriptor.Type == EventType.ServerLocal)
				{
					///////////////////// SERVER DELEGATE /////////////////////
					string strServerDelegate = String.Format("\tpublic delegate void {0}Delegate(", strEventName);

					// Parse parameters
					strServerDelegate += GenerateFunctionParametersDefinition(descriptor.Parameters, false);

					// terminator
					strServerDelegate += ");";

					// Add to list
					strLines_ServerEventManager.Add(strServerDelegate);
					strLines_ServerEventManager.Add(String.Format("\tpublic static {0}Delegate {0};", strEventName));
					///////////////////// SERVER DELEGATE END /////////////////////

					///////////////////// SERVER TRIGGER CODE /////////////////////
					string strServerTriggerFunc = String.Format("\tpublic static void SendLocalEvent_{0}(", strEventName);

					// Parse parameters
					// Add player for server events
					strServerTriggerFunc += GenerateFunctionParametersDefinition(descriptor.Parameters, false);
					string strServerFuncArgs = GenerateFunctionParameters(descriptor.Parameters, false);

					// terminator
					strServerTriggerFunc += ") { ";
					strServerTriggerFunc += String.Format("NetworkEvents.{0}?.Invoke({1});", strEventName, strServerFuncArgs.Length > 0 ? String.Format("{0}", strServerFuncArgs) : "");
					strServerTriggerFunc += " }";

					// Add to list
					strLines_ServerEventManager.Add(strServerTriggerFunc);
					///////////////////// SERVER TRIGGER CODE END /////////////////////

					///////////////////// ENUM NETWORK EVENT ID /////////////////////
					if (!bAddedToEnum)
					{
						bAddedToEnum = true;
						strLines_EventIDs.Add(String.Format("\t{0},", strEventName));
					}
					///////////////////// ENUM NETWORK EVENT ID END /////////////////////
				}

				if (descriptor.Type == EventType.ClientLocal)
				{
					///////////////////// CLIENT DELEGATE /////////////////////
					string strClientDelegate = String.Format("\tpublic delegate void {0}Delegate(", strEventName);

					// Parse parameters
					strClientDelegate += GenerateFunctionParametersDefinition(descriptor.Parameters, false);

					// terminator
					strClientDelegate += ");";

					// Add to list
					strLines_ClientEventManager.Add(strClientDelegate);
					strLines_ClientEventManager.Add(String.Format("\tpublic static {0}Delegate {0};", strEventName));
					///////////////////// CLIENT DELEGATE END /////////////////////

					///////////////////// CLIENT TRIGGER CODE /////////////////////
					string strClientTriggerFunc = String.Format("\tpublic static void SendLocalEvent_{0}(", strEventName);

					// Parse parameters
					// Add player for server events
					strClientTriggerFunc += GenerateFunctionParametersDefinition(descriptor.Parameters, false);
					string strClientFuncArgs = GenerateFunctionParameters(descriptor.Parameters, false);

					// terminator
					strClientTriggerFunc += ") { ";
					strClientTriggerFunc += String.Format("NetworkEvents.{0}?.Invoke({1});", strEventName, strClientFuncArgs.Length > 0 ? String.Format("{0}", strClientFuncArgs) : "");
					strClientTriggerFunc += " }";

					// Add to list
					strLines_ClientEventManager.Add(strClientTriggerFunc);
					///////////////////// CLIENT TRIGGER CODE END /////////////////////
				}
			}

			// FINALIZE FILES (only the eventIDs file needs a terminator currently)
			strLines_EventIDs_UI.Add("}");
			strLines_EventIDs.Add("}");
			strLines_ClientEventManager.Add("}");
			strLines_ClientEventManager_Sender.Add("}");
			strLines_ClientEventManager_UI.Add("}");
			strLines_ServerEventManager.Add("}");
			strLines_ServerEventManager_Sender.Add("}");

			// client & server
			try { File.SetAttributes(Path.Combine("owl_core", strEventIDsOutputPath), FileAttributes.Normal); } catch { }
			File.WriteAllLines(Path.Combine("owl_core", strEventIDsOutputPath), strLines_EventIDs.ToArray());
			File.SetAttributes(Path.Combine("owl_core", strEventIDsOutputPath), FileAttributes.Normal);

			try { File.SetAttributes(Path.Combine("owl_core.client", strEventIDsOutputPath), FileAttributes.Normal); } catch { }
			File.WriteAllLines(Path.Combine("owl_core.client", strEventIDsOutputPath), strLines_EventIDs.ToArray());
			File.WriteAllLines(Path.Combine("owl_core.client", strEventIDsUIOutputPath), strLines_EventIDs_UI.ToArray());
			File.SetAttributes(Path.Combine("owl_core.client", strEventIDsOutputPath), FileAttributes.Normal);

			try { File.SetAttributes(Path.Combine("owl_core.client", strClientEventManagerOutputPath), FileAttributes.Normal); } catch { }
			File.WriteAllLines(Path.Combine("owl_core.client", strClientEventManagerOutputPath), strLines_ClientEventManager.ToArray());
			File.AppendAllLines(Path.Combine("owl_core.client", strClientEventManagerOutputPath), strLines_ClientEventManager_Sender.ToArray());
			File.AppendAllLines(Path.Combine("owl_core.client", strClientEventManagerOutputPath), strLines_ClientEventManager_UI.ToArray());
			File.SetAttributes(Path.Combine("owl_core.client", strClientEventManagerOutputPath), FileAttributes.Normal);

			File.SetAttributes(Path.Combine("owl_core", strServerEventManagerOutputPath), FileAttributes.Normal);
			File.WriteAllLines(Path.Combine("owl_core", strServerEventManagerOutputPath), strLines_ServerEventManager.ToArray());
			File.AppendAllLines(Path.Combine("owl_core", strServerEventManagerOutputPath), strLines_ServerEventManager_Sender.ToArray());
			File.SetAttributes(Path.Combine("owl_core", strServerEventManagerOutputPath), FileAttributes.Normal);
		}

		private static string GenerateFunctionParametersDefinition(Dictionary<string, string> parameters, bool bPrependPlayer)
		{
			string strParams = String.Empty;
			int index = 0;

			if (bPrependPlayer)
			{
				strParams = "CPlayer a_Player";
				++index;
			}

			if (parameters == null || parameters.Count == 0)
			{
				return strParams;
			}
			else
			{
				foreach (var kvPair in parameters)
				{
					if (index > 0)
					{
						strParams += ", ";
					}

					string strVariableName = kvPair.Key;
					string strVariableType = kvPair.Value;
					strParams += String.Format("{0} {1}", strVariableType, strVariableName);

					++index;
				}

				return strParams;
			}
		}

		private static string GenerateFunctionParameters(Dictionary<string, string> parameters, bool bPrependPlayer)
		{
			string strParams = String.Empty;
			int index = 0;

			if (bPrependPlayer)
			{
				strParams = "a_Player";
				++index;
			}

			if (parameters.Count == 0)
			{
				return strParams;
			}
			else
			{
				foreach (var kvPair in parameters)
				{
					if (index > 0)
					{
						strParams += ", ";
					}

					string strVariableName = kvPair.Key;
					strParams += String.Format("{0}", strVariableName);

					++index;
				}

				return strParams;
			}
		}
	}
}

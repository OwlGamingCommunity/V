#define EVENT_PARSER_SPLIT_STRINGS

using GTANetworkAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// TODO_CSHARP: Should be able to register event flags like on commands to only happen when logged in for example

// RAGE EVENTS
public static class RageEvents
{
	// NOTE: Disabled on purpose, use the Owl Script loader
	//public delegate void RAGE_OnScriptStartDelegate();
	//public delegate void RAGE_OnScriptStopDelegate();
	//public delegate void RAGE_OnResourceStartDelegate(string resourceName);
	//public delegate void RAGE_OnResourceStopDelegate(string resourceName);
	public delegate void RAGE_OnChatMessageDelegate(Player client, System.String message);
	public delegate void RAGE_OnUpdateDelegate();
	public delegate void RAGE_OnEntityCreatedDelegate(Entity entity);
	public delegate void RAGE_OnEntityDeletedDelegate(Entity entity);
	public delegate void RAGE_OnEntityModelChangeDelegate(Entity entity, System.UInt32 oldModel);
	public delegate void RAGE_OnPlayerConnectedDelegate(Player client);
	public delegate void RAGE_OnPlayerDisconnectedDelegate(Player client, DisconnectionType type, string reason);
	public delegate void RAGE_OnPlayerSpawnDelegate(Player client);
	public delegate void RAGE_OnPlayerDeathDelegate(Player client, Player killer, System.UInt32 reason);
	public delegate void RAGE_OnPlayerDamageDelegate(Player client, float healthLoss, float armorLoss);
	public delegate void RAGE_OnPlayerPickupDelegate(Player client, Pickup pickup);
	public delegate void RAGE_OnPlayerWeaponSwitchDelegate(Player client, WeaponHash oldWeaponHash, GTANetworkAPI.WeaponHash newWeaponHash);
	public delegate void RAGE_OnPlayerDetonateStickiesDelegate(Player client);
	public delegate void RAGE_OnPlayerEnterCheckpointDelegate(Checkpoint checkpoint, Player client);
	public delegate void RAGE_OnPlayerExitCheckpointDelegate(Checkpoint checkpoint, Player client);
	public delegate void RAGE_OnPlayerEnterColshapeDelegate(ColShape colShape, Player client);
	public delegate void RAGE_OnPlayerExitColshapeDelegate(ColShape colShape, Player client);
	public delegate void RAGE_OnPlayerEnterVehicleAttemptDelegate(Player client, Vehicle vehicle, sbyte seatId);
	public delegate void RAGE_OnPlayerExitVehicleAttemptDelegate(Player client, Vehicle vehicle);
	public delegate void RAGE_OnPlayerEnterVehicleDelegate(Player client, Vehicle vehicle, sbyte seatId);
	public delegate void RAGE_OnPlayerExitVehicleDelegate(Player client, Vehicle vehicle);
	public delegate void RAGE_OnVehicleDamageDelegate(Vehicle vehicle, float bodyHealthLoss, float engineHealthLoss);
	public delegate void RAGE_OnVehicleDeathDelegate(Vehicle vehicle);
	public delegate void RAGE_OnVehicleHornToggleDelegate(Vehicle vehicle, bool bToggle);
	public delegate void RAGE_OnVehicleSirenToggleDelegate(Vehicle vehicle);
	public delegate void RAGE_OnVehicleDoorBreakDelegate(Vehicle vehicle, int index);
	public delegate void RAGE_OnVehicleWindowSmashDelegate(Vehicle vehicle, int index);
	public delegate void RAGE_OnVehicleTyreBurstDelegate(Vehicle vehicle, int index);
	public delegate void RAGE_OnVehicleTrailerChangeDelegate(Vehicle vehicle, Vehicle trailer);
	public delegate void RAGE_OnFirstChanceExceptionDelegate(Exception exception);
	public delegate void RAGE_OnUnhandledExceptionDelegate(Exception exception);
	// NOTE: Disabled on purpose, use the Owl Script loader
	//public static RAGE_OnScriptStartDelegate RAGE_OnScriptStart;
	//public static RAGE_OnScriptStopDelegate RAGE_OnScriptStop;
	//public static RAGE_OnResourceStartDelegate RAGE_OnResourceStart;
	//public static RAGE_OnResourceStopDelegate RAGE_OnResourceStop;
	public static RAGE_OnChatMessageDelegate RAGE_OnChatMessage;
	public static RAGE_OnUpdateDelegate RAGE_OnUpdate;
	public static RAGE_OnEntityCreatedDelegate RAGE_OnEntityCreated;
	public static RAGE_OnEntityDeletedDelegate RAGE_OnEntityDeleted;
	public static RAGE_OnEntityModelChangeDelegate RAGE_OnEntityModelChange;
	public static RAGE_OnPlayerConnectedDelegate RAGE_OnPlayerConnected;
	public static RAGE_OnPlayerDisconnectedDelegate RAGE_OnPlayerDisconnected;
	public static RAGE_OnPlayerSpawnDelegate RAGE_OnPlayerSpawn;
	public static RAGE_OnPlayerDeathDelegate RAGE_OnPlayerDeath;
	public static RAGE_OnPlayerDamageDelegate RAGE_OnPlayerDamage;
	public static RAGE_OnPlayerPickupDelegate RAGE_OnPlayerPickup;
	public static RAGE_OnPlayerWeaponSwitchDelegate RAGE_OnPlayerWeaponSwitch;
	public static RAGE_OnPlayerDetonateStickiesDelegate RAGE_OnPlayerDetonateStickies;
	public static RAGE_OnPlayerEnterCheckpointDelegate RAGE_OnPlayerEnterCheckpoint;
	public static RAGE_OnPlayerExitCheckpointDelegate RAGE_OnPlayerExitCheckpoint;
	public static RAGE_OnPlayerEnterColshapeDelegate RAGE_OnPlayerEnterColshape;
	public static RAGE_OnPlayerExitColshapeDelegate RAGE_OnPlayerExitColshape;
	public static RAGE_OnPlayerEnterVehicleAttemptDelegate RAGE_OnPlayerEnterVehicleAttempt;
	public static RAGE_OnPlayerExitVehicleAttemptDelegate RAGE_OnPlayerExitVehicleAttempt;
	public static RAGE_OnPlayerEnterVehicleDelegate RAGE_OnPlayerEnterVehicle;
	public static RAGE_OnPlayerExitVehicleDelegate RAGE_OnPlayerExitVehicle;
	public static RAGE_OnVehicleDamageDelegate RAGE_OnVehicleDamage;
	public static RAGE_OnVehicleDeathDelegate RAGE_OnVehicleDeath;
	public static RAGE_OnVehicleHornToggleDelegate RAGE_OnVehicleHornToggle;
	public static RAGE_OnVehicleSirenToggleDelegate RAGE_OnVehicleSirenToggle;
	public static RAGE_OnVehicleDoorBreakDelegate RAGE_OnVehicleDoorBreak;
	public static RAGE_OnVehicleWindowSmashDelegate RAGE_OnVehicleWindowSmash;
	public static RAGE_OnVehicleTyreBurstDelegate RAGE_OnVehicleTyreBurst;
	public static RAGE_OnVehicleTrailerChangeDelegate RAGE_OnVehicleTrailerChange;
	public static RAGE_OnFirstChanceExceptionDelegate RAGE_OnFirstChanceException;
	public static RAGE_OnUnhandledExceptionDelegate RAGE_OnUnhandledException;
}

public static class EventManager
{
	public static void TriggerEventForAll(NetworkEventID ev, params object[] parameters)
	{
		NAPI.Task.Run(() =>
		{
			if (parameters != null && parameters.Length > 0)
			{
				NAPI.ClientEvent.TriggerClientEventForAll("CE", ev.ToString(), parameters);
			}
			else
			{
				NAPI.ClientEvent.TriggerClientEventForAll("CE", ev.ToString());

			}
		});
	}

	private static bool IsUserDefinedClass(Type t)
	{
		return t.IsClass && !t.FullName.StartsWith("System.") && !t.FullName.StartsWith("GTANetworkAPI.");
	}

	private static object[] PrepareEvent(NetworkEventID ev, params object[] parameters)
	{
		List<object> lstParams = new List<object>();
		lstParams.Add(ev.ToString());

		if (parameters == null)
		{
			return lstParams.ToArray();
		}

		UInt32 lenUncompressed = 0;
		UInt32 lenCompressed = 0;

		// Serialize any non-primitives + sanitize strings
		for (int i = 0; i < parameters.Length; ++i)
		{
			if (parameters[i] != null)
			{
				if (parameters[i] is GTANetworkAPI.Entity) // TODO_LAUNCH: Do other entities need this? seems to only be objects?
				{
					GTANetworkAPI.Entity entity = (GTANetworkAPI.Entity)parameters[i];

					byte[] packedData = MessagePack.MessagePackSerializer.Serialize(entity.Handle.Value, MessagePack.Resolvers.ContractlessStandardResolver.Options.WithCompression(MessagePack.MessagePackCompression.Lz4Block));
					parameters[i] = packedData;
					lenUncompressed += sizeof(ushort);
					lenCompressed += (UInt32)packedData.Length;
				}
				else if (parameters[i].GetType().IsEnum) // Write as int, optimization
				{
					byte[] packedData = MessagePack.MessagePackSerializer.Serialize((int)parameters[i], MessagePack.Resolvers.ContractlessStandardResolver.Options.WithCompression(MessagePack.MessagePackCompression.Lz4Block));
					parameters[i] = packedData;
					lenUncompressed += sizeof(ushort);
					lenCompressed += (UInt32)packedData.Length;
				}
				else if (parameters[i] is ICollection || parameters[i] is Array || IsUserDefinedClass(parameters[i].GetType()))
				{
					string strJsonEncode = Newtonsoft.Json.JsonConvert.SerializeObject(parameters[i], Newtonsoft.Json.Formatting.None, new Newtonsoft.Json.JsonSerializerSettings()
					{
						StringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.EscapeNonAscii
					});
					lenUncompressed += (UInt32)strJsonEncode.Length * sizeof(char);

					byte[] packedData = MessagePack.MessagePackSerializer.ConvertFromJson(strJsonEncode, MessagePack.Resolvers.ContractlessStandardResolver.Options.WithCompression(MessagePack.MessagePackCompression.Lz4Block));
					parameters[i] = packedData;

					lenCompressed += (UInt32)packedData.Length;
				}
				else if (parameters[i].GetType() == typeof(string))
				{
					string strVal = ((string)parameters[i]).Replace("\"", "\'");

					byte[] packedData = MessagePack.MessagePackSerializer.Serialize(strVal, MessagePack.Resolvers.ContractlessStandardResolver.Options.WithCompression(MessagePack.MessagePackCompression.Lz4Block));
					parameters[i] = packedData;

					lenUncompressed += (UInt32)strVal.Length * sizeof(char);
					lenCompressed += (UInt32)packedData.Length;
				}
				else
				{
					if (!(parameters[i] is Vector3)) // vectors arent serializable...
					{
						UInt32 sizeUncompressed = (UInt32)System.Runtime.InteropServices.Marshal.SizeOf(parameters[i]);

						lenUncompressed += sizeUncompressed;

						byte[] packedData = MessagePack.MessagePackSerializer.Serialize(parameters[i], MessagePack.Resolvers.ContractlessStandardResolver.Options.WithCompression(MessagePack.MessagePackCompression.Lz4Block));
						parameters[i] = packedData;
						lenUncompressed += sizeUncompressed;
						lenCompressed += (UInt32)packedData.Length;
					}
				}
			}
			else
			{
				parameters[i] = "NETNULL";
			}
		}

		ServerPerfManager.UpdateTotalBytesSentData(lenUncompressed, lenCompressed);

#if EVENT_PARSER_SPLIT_STRINGS
		for (int i = 0; i < parameters.Length; ++i)
		{
			if (parameters[i] != null && parameters[i].GetType() == typeof(string))
			{
				// split large strings and re-join on other side, RAGE limits string size
				string temp = parameters[i].ToString();
				const int maxStringLen = 1024;
				int numStringSplits = Math.Max(1, (int)Math.Ceiling(((float)temp.Length / (float)maxStringLen)));

				lstParams.Add(numStringSplits);

				string[] splits = new string[numStringSplits];
				for (int j = 0; j < numStringSplits; ++j)
				{
					int start = j * maxStringLen;
					splits[j] = temp.Substring(start, Math.Min(maxStringLen, temp.Length - start));

					lstParams.Add(splits[j]);
				}
			}
			else
			{
				lstParams.Add(parameters[i]);
			}
		}
#else
		lstParams.AddRange(parameters);
#endif

		CheckEventSize(true, lstParams);
		return lstParams.ToArray();
	}

#if CHECK_EVENT_SIZE
	private static Dictionary<string, long> m_dictLargeEvents_Inbound = new Dictionary<string, long>();
	private static Dictionary<string, long> m_dictLargeEvents_Outbound = new Dictionary<string, long>();
#endif
	private static void CheckEventSize(bool IsOutbound, List<object> lstParams)
	{
#if CHECK_EVENT_SIZE
		const bool bConsole = false;
		const bool bLogFile = true;

		const int EventSizeThreshold = 100; // 100 bytes

		string strEventName = lstParams[0].ToString();
		long sizeBytes = 0;
		using (Stream s = new MemoryStream())
		{
			BinaryFormatter formatter = new BinaryFormatter();
			foreach (object param in lstParams)
			{
				try
				{
					if (!(param is Vector3)) // vectors arent serializable...
					{
						formatter.Serialize(s, param);
					}
				}
				catch
				{

				}
			}

			sizeBytes = s.Length;
		}

		if (sizeBytes >= EventSizeThreshold)
		{
			if (bConsole)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("\tLARGE EVENT FOUND: {0} Event: {1} is {2} bytes", IsOutbound ? "Outbound" : "Inbound", strEventName, sizeBytes);
				Console.ForegroundColor = ConsoleColor.Green;
			}
			
			if (bLogFile)
			{
				if (IsOutbound)
				{
					if (!m_dictLargeEvents_Outbound.ContainsKey(strEventName))
					{
						m_dictLargeEvents_Outbound[strEventName] = sizeBytes;
						DumpEventsFile();
					}
				}
				else
				{
					if (!m_dictLargeEvents_Inbound.ContainsKey(strEventName))
					{
						m_dictLargeEvents_Inbound[strEventName] = sizeBytes;
						DumpEventsFile();
					}
				}
			}
		}
#endif
	}

#if CHECK_EVENT_SIZE
	private static void DumpEventsFile()
	{
		const string strInboundFilename = "logs/large_events_inbound.txt";
		const string strOutboundFilename = "logs/large_events_outbound.txt";

		if (File.Exists(strInboundFilename))
		{
			File.Delete(strInboundFilename);
		}

		if (File.Exists(strOutboundFilename))
		{
			File.Delete(strOutboundFilename);
		}

		List<string> strLines = new List<string>();

		foreach (var kvPair in m_dictLargeEvents_Inbound)
		{
			strLines.Add(Helpers.FormatString("{0} = {1} bytes", kvPair.Key, kvPair.Value));
		}

		File.WriteAllLines(strInboundFilename, strLines.ToArray());
		strLines.Clear();

		foreach (var kvPair in m_dictLargeEvents_Outbound)
		{
			strLines.Add(Helpers.FormatString("{0} = {1} bytes", kvPair.Key, kvPair.Value));
		}
		File.WriteAllLines(strOutboundFilename, strLines.ToArray());
	}
#endif

	// TODO_CSHARP: When we're fully C#, we should get rid of the tostring and just send as int instead to save BW. Will have to update DataHandlers!
	public static void TriggerRemoteEventForPlayer(CPlayer a_TargetPlayer, NetworkEventID ev, params object[] parameters)
	{
		NAPI.Task.Run(() =>
		{
			if (parameters != null && parameters.Length > 0)
			{
				object[] preparedParams = PrepareEvent(ev, parameters);
				NAPI.ClientEvent.TriggerClientEvent(a_TargetPlayer.Client, "CE", preparedParams);
			}
			else
			{
				NAPI.ClientEvent.TriggerClientEvent(a_TargetPlayer.Client, "CE", ev.ToString());
			}
		});
	}

	public static void OnCustomEvent(Player sender, params object[] argumentsIn_DoNotUse)
	{
		WeakReference<CPlayer> PlayerRef = PlayerPool.GetPlayerFromClient_IncludeOutOfGame(sender);
		CPlayer player = PlayerRef.Instance();

		List<object> lstSafeParams = new List<object>();
		List<object> lstRealParams = new List<object>();

		UInt32 lenUncompressed = 0;
		UInt32 lenCompressed = 0;

		// TODO_RAGE: Hack. Sometimes a random amount of integers appears appended to the front!?
		int garbageCount = 0;
		for (garbageCount = 0; garbageCount < argumentsIn_DoNotUse.Length; ++garbageCount)
		{
			int temp;
			if (!int.TryParse(argumentsIn_DoNotUse[garbageCount].ToString(), out temp))
			{
				break;
			}
		}

		lstSafeParams.AddRange(argumentsIn_DoNotUse.Skip(garbageCount));

		if (player != null)
		{
#if EVENT_PARSER_SPLIT_STRINGS

			int lastIndexProcessed = 0;
			for (int i = 0; i < lstSafeParams.Count; ++i)
			{
				object o = lstSafeParams[i];

				if (o.GetType() == typeof(string) && i > 0)
				{
					// one before should be length
					object prevObj = lstSafeParams[i - 1];

					if (prevObj.GetType() == typeof(int))
					{
						int length = (int)prevObj;

						// Add everything up to the length, if there is anything
						if (lastIndexProcessed != (i - 1))
						{
							lstRealParams.AddRange(lstSafeParams.Skip(lastIndexProcessed).Take(i - lastIndexProcessed - 1));
						}

						lastIndexProcessed = i + length;

						string strJoined = "";
						for (int j = 0; j < length; ++j)
						{
							strJoined += (string)lstSafeParams[i + j];
						}

						lstRealParams.Add(strJoined);
						i += (length - 1);
					}
				}
			}

			// append remainder
			if (lastIndexProcessed != lstSafeParams.Count)
			{
				lstRealParams.AddRange(lstSafeParams.Skip(lastIndexProcessed).Take(lstSafeParams.Count - lastIndexProcessed));
			}

#else
			lstRealParams.AddRange(lstSafeParams);
#endif

			if (lstRealParams.Count >= 1)
			{
				string strEventName = lstRealParams[0].ToString();
				try
				{
					System.Reflection.MemberInfo[] members = typeof(NetworkEvents).GetMember(strEventName);
					if (members.Length == 1)
					{
						System.Reflection.MemberInfo targetMember = members[0];
						FieldInfo field = (FieldInfo)targetMember;
						Type t = field.FieldType;
						System.Reflection.MethodInfo method = t.GetMethod("Invoke");

						if ((lstRealParams.Count - 1) == method.GetParameters().Length - 1) // -1 to subtract event name from arguments, player from method
						{
							object fieldObj = field.GetValue(null);
							if (fieldObj != null)
							{
								// Deserialize any non-primitives
								int index = 1; // 1 to skip the event name
								foreach (var methodParam in method.GetParameters().Skip(1)) // skip CPlayer
								{
									// TODO: Better way of checking
									if (lstRealParams[index].ToString() == "NETNULL")
									{
										lstRealParams[index] = null;
									}
									else if (methodParam.ParameterType.IsArray || methodParam.ParameterType.ToString().Contains("System.Collection") || IsUserDefinedClass(methodParam.ParameterType)) // TODO_CSHARP: Does this work with arrays?
									{
										lstRealParams[index] = Newtonsoft.Json.JsonConvert.DeserializeObject(lstRealParams[index].ToString(), methodParam.ParameterType);

										//byte[] byteData = (byte[])lstRealParams[index];
										//lenCompressed += (UInt32)byteData.Length;

										//string strUnpack = MessagePack.MessagePackSerializer.ConvertToJson(byteData, MessagePack.Resolvers.TypelessContractlessStandardResolver.Options.WithCompression(MessagePack.MessagePackCompression.Lz4Block));
										//lstRealParams[index] = Newtonsoft.Json.JsonConvert.DeserializeObject(strUnpack, methodParam.ParameterType);

										//lenUncompressed += (UInt32)strUnpack.Length;
									}
									else
									{
										if (methodParam.ParameterType.IsEnum)
										{
											lstRealParams[index] = Enum.ToObject(methodParam.ParameterType, Convert.ToInt32(lstRealParams[index]));
										}
										else
										{
											// NOTE: We have to hackily convert when the target is uint because RAGE sends as signed always. ChangeType would throw exception due to size.
											if (methodParam.ParameterType == typeof(UInt16)) { lstRealParams[index] = (UInt16)(Int16)(lstRealParams[index] is String ? Int16.Parse(lstRealParams[index].ToString()) : lstRealParams[index]); }
											else if (methodParam.ParameterType == typeof(UInt32)) { lstRealParams[index] = (UInt32)(Int32)(lstRealParams[index] is String ? Int32.Parse(lstRealParams[index].ToString()) : lstRealParams[index]); }
											else if (methodParam.ParameterType == typeof(UInt64)) { lstRealParams[index] = (UInt64)(Int64)(lstRealParams[index] is String ? Int64.Parse(lstRealParams[index].ToString()) : lstRealParams[index]); }
											else if (methodParam.ParameterType == typeof(Int16)) { lstRealParams[index] = (Int16)(lstRealParams[index] is String ? Int16.Parse(lstRealParams[index].ToString()) : lstRealParams[index]); }
											else if (methodParam.ParameterType == typeof(Int32)) { lstRealParams[index] = (Int32)(lstRealParams[index] is String ? Int32.Parse(lstRealParams[index].ToString()) : lstRealParams[index]); }
											else if (methodParam.ParameterType == typeof(Int64)) { lstRealParams[index] = (Int64)(lstRealParams[index] is String ? Int64.Parse(lstRealParams[index].ToString()) : lstRealParams[index]); }
											else if (methodParam.ParameterType.BaseType == typeof(GTANetworkAPI.Entity))
											{
												ushort netHandle = (UInt16)(Int16)Convert.ToInt16(lstRealParams[index]);

												if (methodParam.ParameterType == typeof(GTANetworkAPI.Vehicle))
												{
													GTANetworkAPI.Vehicle obj = NAPI.Entity.GetEntityFromHandle<GTANetworkAPI.Vehicle>(new NetHandle(netHandle, EntityType.Vehicle));
													lstRealParams[index] = Convert.ChangeType(obj, methodParam.ParameterType);
												}
												else if (methodParam.ParameterType == typeof(GTANetworkAPI.Object))
												{
													GTANetworkAPI.Object obj = NAPI.Entity.GetEntityFromHandle<GTANetworkAPI.Object>(new NetHandle(netHandle, EntityType.Object));
													lstRealParams[index] = Convert.ChangeType(obj, methodParam.ParameterType);
												}
												else if (methodParam.ParameterType == typeof(GTANetworkAPI.Player))
												{
													GTANetworkAPI.Player obj = NAPI.Entity.GetEntityFromHandle<GTANetworkAPI.Player>(new NetHandle(netHandle, EntityType.Player));
													lstRealParams[index] = Convert.ChangeType(obj, methodParam.ParameterType);
												}

												// TODO_CSHARP: Exception for unhandled type
											}
											else
											{
												lstRealParams[index] = Convert.ChangeType(lstRealParams[index], methodParam.ParameterType);
											}
										}
									}

									++index;
								}

								CheckEventSize(false, lstRealParams);
								long millisecondsStart = DateTime.Now.Ticks;
								method.Invoke(fieldObj, lstRealParams.Skip(1).Prepend(player).ToArray());

								string strExtraDetails = Helpers.FormatString("Target: {0}::{1}", method.DeclaringType.FullName, method.Name);
								if (strEventName == NetworkEventID.PlayerRawCommand.ToString() && lstRealParams.Count > 1)
								{
									strExtraDetails = Helpers.FormatString("/{0}", lstRealParams[1].ToString());
								}

								ServerPerfManager.RegisterStatistic(method, millisecondsStart, strExtraDetails);
							}
						}
						else
						{
							NAPI.Util.ConsoleOutput("[EVENTSYSTEM] Not triggering {0} due to argument count mismatch ({1} vs {2}).", strEventName, (lstRealParams.Count - 1), method.GetParameters().Length - 1);
						}
					}
					else
					{
						NAPI.Util.ConsoleOutput("[EVENTSYSTEM] Not triggering {0} due to method not being found.", strEventName);
					}
				}
				catch (Exception ex)
				{
					NAPI.Util.ConsoleOutput("[EVENTSYSTEM] Not triggering {0} due to exception: {1}.", strEventName, ex.Message);
				}
			}
		}

		ServerPerfManager.UpdateTotalBytesReceivedData(lenUncompressed, lenCompressed);
	}

	public static void InvokeRageDelegate(Delegate parentDelegate, bool bRecordTime, params object[] args)
	{
		if (parentDelegate != null)
		{
			Delegate[] list = parentDelegate.GetInvocationList();
			foreach (Delegate subscriber in list)
			{
				long millisecondsStart = DateTime.Now.Ticks;
				subscriber.DynamicInvoke(args);

				if (bRecordTime)
				{
					ServerPerfManager.RegisterStatistic(subscriber.GetMethodInfo(), millisecondsStart, Helpers.FormatString("Target: {0}::{1}", subscriber.GetMethodInfo().DeclaringType.FullName, subscriber.GetMethodInfo().Name));
				}
			}
		}
	}
}

class CustomEventManagerScript
{
	internal Dictionary<ConsoleKey, bool> KeyList = new Dictionary<ConsoleKey, bool>();

	public CustomEventManagerScript()
	{

	}
}
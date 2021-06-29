#define EVENT_PARSER_SPLIT_STRINGS

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class RageEvents
{
	public static void AddDataHandler(EDataNames dataNames, RAGE.Events.OnEntityDataChangeDelegate handler)
	{
		RAGE.Events.AddDataHandler(((int)dataNames).ToString(), handler);
	}

	public delegate void RAGE_OnPlayerEnterCheckpointDelegate(RAGE.Elements.Checkpoint checkpoint, RAGE.Events.CancelEventArgs cancel);
	public delegate void RAGE_OnEventTriggeredByKeyDelegate(ulong key, object[] args, RAGE.Events.CancelEventArgs cancel);
	public delegate void RAGE_OnEntityDataChangeByKeyDelegate(ulong key, RAGE.Elements.Entity entity, object arg);
	public delegate void RAGE_TickDelegate();
	public delegate void RAGE_OnPlayerStopTalkingDelegate(RAGE.Elements.Player player);
	public delegate void RAGE_OnPlayerStartTalkingDelegate(RAGE.Elements.Player player);
	public delegate void RAGE_OnPlayerLeaveVehicleDelegate();
	public delegate void RAGE_OnPlayerEnterVehicleDelegate(RAGE.Elements.Vehicle vehicle, int seatId);
	public delegate void RAGE_OnPlayerStartEnterVehicleDelegate(RAGE.Elements.Vehicle vehicle, int seatId, RAGE.Events.CancelEventArgs cancel);
	public delegate void RAGE_OnBrowserLoadingFailedDelegate(RAGE.Ui.HtmlWindow window);
	public delegate void RAGE_OnBrowserDomReadyDelegate(RAGE.Ui.HtmlWindow window);
	public delegate void RAGE_OnBrowserCreatedDelegate(RAGE.Ui.HtmlWindow window);
	public delegate void RAGE_OnGuiReadyDelegate();
	public delegate void RAGE_OnPlayerWeaponShotDelegate(RAGE.Vector3 targetPos, RAGE.Elements.Player target, RAGE.Events.CancelEventArgs cancel);
	public delegate void RAGE_OnClickWithRaycastDelegate(int x, int y, bool up, bool right, float relativeX, float relativeY, RAGE.Vector3 worldPos, int entityHandle);
	public delegate void RAGE_OnClickDelegate(int x, int y, bool up, bool right);
	public delegate void RAGE_OnPlayerResurrectDelegate();
	public delegate void RAGE_OnPlayerDeathDelegate(RAGE.Elements.Player player, uint reason, RAGE.Elements.Player killer, RAGE.Events.CancelEventArgs cancel);
	public delegate void RAGE_OnPlayerExitCheckpointDelegate(RAGE.Elements.Checkpoint checkpoint, RAGE.Events.CancelEventArgs cancel);
	public delegate void RAGE_OnPlayerEnterColshapeDelegate(RAGE.Elements.Colshape colshape, RAGE.Events.CancelEventArgs cancel);
	public delegate void RAGE_OnPlayerExitColshapeDelegate(RAGE.Elements.Colshape colshape, RAGE.Events.CancelEventArgs cancel);
	public delegate void RAGE_OnPlayerChatDelegate(string text, RAGE.Events.CancelEventArgs cancel);
	public delegate void RAGE_OnPlayerCommandDelegate(string cmd, RAGE.Events.CancelEventArgs cancel);
	public delegate void RAGE_OnEntityCreatedDelegate(RAGE.Elements.Entity entity);
	public delegate void RAGE_OnEntityDestroyedDelegate(RAGE.Elements.Entity entity);
	public delegate void RAGE_OnScriptWindowDestroyedDelegate(RAGE.Ui.HtmlWindow window);
	public delegate void RAGE_OnEntityStreamInDelegate(RAGE.Elements.Entity entity);
	public delegate void RAGE_OnEntityStreamOutDelegate(RAGE.Elements.Entity entity);
	public delegate void RAGE_OnPlayerJoinDelegate(RAGE.Elements.Player player);
	public delegate void RAGE_OnPlayerQuitDelegate(RAGE.Elements.Player player);
	public delegate void RAGE_OnPlayerSpawnDelegate(RAGE.Events.CancelEventArgs cancel);
	public delegate void RAGE_OnScriptWindowCreatedDelegate(RAGE.Ui.HtmlWindow window);

	// new in RAGE 1.1
	public delegate void RAGE_OnPlayerCreateWaypointDelegate(RAGE.Vector3 position);
	public delegate void RAGE_OnPlayerRemoveWaypointDelegate();
	public delegate void RAGE_OnEntityControllerChangeDelegate(RAGE.Elements.Entity entity, RAGE.Elements.Player newController);
	public delegate void RAGE_OnIncomingDamageDelegate(RAGE.Elements.Player sourcePlayer, RAGE.Elements.Entity sourceEntity, RAGE.Elements.Entity targetEntity, ulong weaponHash, ulong boneIdx, int damage, RAGE.Events.CancelEventArgs cancel);
	public delegate void RAGE_OnOutgoingDamageDelegate(RAGE.Elements.Entity sourceEntity, RAGE.Elements.Entity targetEntity, RAGE.Elements.Player sourcePlayer, ulong weaponHash, ulong boneIdx, int damage, RAGE.Events.CancelEventArgs cancel);
	public delegate void RAGE_OnExplosionDelegate(RAGE.Elements.Player sourcePlayer, uint explosionType, RAGE.Vector3 position, RAGE.Events.CancelEventArgs cancel);
	public delegate void RAGE_OnProjectileDelegate(RAGE.Elements.Player sourcePlayer, uint weaponHash, uint ammoType, RAGE.Vector3 position, RAGE.Vector3 velocity, RAGE.Events.CancelEventArgs cancel);
	public delegate void RAGE_OnDummyEntityCreatedDelegate(int type, RAGE.Elements.DummyEntity entity);
	public delegate void RAGE_OnDummyEntityDestroyedDelegate(int type, RAGE.Elements.DummyEntity entity);
	public delegate void RAGE_OnConsoleCommandDelegate(string cmd);
	public delegate void RAGE_OnPlayerReadyDelegate();

	public static RAGE_OnPlayerEnterCheckpointDelegate RAGE_OnPlayerEnterCheckpoint;
	public static RAGE_OnEventTriggeredByKeyDelegate RAGE_OnEventTriggeredByKey;
	public static RAGE_OnEntityDataChangeByKeyDelegate RAGE_OnEntityDataChangeByKey;
	public static RAGE_TickDelegate RAGE_OnTick_PerFrame;
	public static RAGE_TickDelegate RAGE_OnTick_HighFrequency;
	public static RAGE_TickDelegate RAGE_OnTick_MediumFrequency;
	public static RAGE_TickDelegate RAGE_OnTick_LowFrequency;
	public static RAGE_TickDelegate RAGE_OnTick_OncePerSecond;
	public static RAGE_TickDelegate RAGE_OnRender;
	public static RAGE_TickDelegate RAGE_TimerTickInternal_DO_NOT_USE;
	public static RAGE_OnPlayerStopTalkingDelegate RAGE_OnPlayerStopTalking;
	public static RAGE_OnPlayerStartTalkingDelegate RAGE_OnPlayerStartTalking;
	// NOTE: Do not use. Use EnterVehicleReal instead. This one triggers way too early.
	//public static RAGE_OnPlayerEnterVehicleDelegate RAGE_OnPlayerEnterVehicle;
	// NOTE: Do not use. Use ExitVehicleReal instead. This one doesn't trigger in certain cases (e.g. warp out of vehicle)
	//public static RAGE_OnPlayerLeaveVehicleDelegate RAGE_OnPlayerLeaveVehicle;
	public static RAGE_OnPlayerStartEnterVehicleDelegate RAGE_OnPlayerStartEnterVehicle;
	public static RAGE_OnBrowserLoadingFailedDelegate RAGE_OnBrowserLoadingFailed;
	public static RAGE_OnBrowserDomReadyDelegate RAGE_OnBrowserDomReady;
	public static RAGE_OnBrowserCreatedDelegate RAGE_OnBrowserCreated;
	public static RAGE_OnGuiReadyDelegate RAGE_OnGuiReady;
	public static RAGE_OnPlayerWeaponShotDelegate RAGE_OnPlayerWeaponShot;
	public static RAGE_OnClickWithRaycastDelegate RAGE_OnClickWithRaycast;
	public static RAGE_OnClickDelegate RAGE_OnClick;
	public static RAGE_OnPlayerResurrectDelegate RAGE_OnPlayerResurrect;
	public static RAGE_OnPlayerDeathDelegate RAGE_OnPlayerDeath;
	public static RAGE_OnPlayerExitCheckpointDelegate RAGE_OnPlayerExitCheckpoint;
	public static RAGE_OnPlayerEnterColshapeDelegate RAGE_OnPlayerEnterColshape;
	public static RAGE_OnPlayerExitColshapeDelegate RAGE_OnPlayerExitColshape;
	public static RAGE_OnPlayerChatDelegate RAGE_OnPlayerChat;
	public static RAGE_OnPlayerCommandDelegate RAGE_OnPlayerCommand;
	public static RAGE_OnEntityCreatedDelegate RAGE_OnEntityCreated;
	public static RAGE_OnEntityDestroyedDelegate RAGE_OnEntityDestroyed;
	public static RAGE_OnScriptWindowDestroyedDelegate RAGE_OnScriptWindowDestroyed;
	public static RAGE_OnEntityStreamInDelegate RAGE_OnEntityStreamIn;
	public static RAGE_OnEntityStreamOutDelegate RAGE_OnEntityStreamOut;
	public static RAGE_OnPlayerJoinDelegate RAGE_OnPlayerJoin;
	public static RAGE_OnPlayerQuitDelegate RAGE_OnPlayerQuit;
	public static RAGE_OnPlayerSpawnDelegate RAGE_OnPlayerSpawn;
	public static RAGE_OnScriptWindowCreatedDelegate RAGE_OnScriptWindowCreated;
	public static RAGE_OnPlayerCreateWaypointDelegate RAGE_OnPlayerCreateWaypoint;
	public static RAGE_OnPlayerRemoveWaypointDelegate RAGE_OnPlayerRemoveWaypoint;
	public static RAGE_OnEntityControllerChangeDelegate RAGE_OnEntityControllerChange;
	public static RAGE_OnIncomingDamageDelegate RAGE_OnIncomingDamage;
	public static RAGE_OnOutgoingDamageDelegate RAGE_OnOutgoingDamage;
	public static RAGE_OnExplosionDelegate RAGE_OnExplosion;
	public static RAGE_OnProjectileDelegate RAGE_OnProjectile;
	public static RAGE_OnDummyEntityCreatedDelegate RAGE_OnDummyEntityCreated;
	public static RAGE_OnDummyEntityDestroyedDelegate RAGE_OnDummyEntityDestroyed;
	public static RAGE_OnConsoleCommandDelegate RAGE_OnConsoleCommand;
	public static RAGE_OnPlayerReadyDelegate RAGE_OnPlayerReady;
}

public static class EventManager
{
	public static void TriggerLocalEvent(NetworkEventID ev, params object[] parameters)
	{
		object[] preparedParams = PrepareEvent(ev, parameters);
		OnCustomEvent(false, true, preparedParams);
	}

	private static bool IsUserDefinedClass(Type t)
	{
		return t.IsClass && !t.FullName.StartsWith("System.") && !t.FullName.StartsWith("RAGE.");
	}

	private static object[] PrepareEvent(NetworkEventID ev, params object[] parameters)
	{
		DateTime dtStart = DateTime.Now;
		List<object> lstParams = new List<object>();
		lstParams.Add(ev.ToString());

		// Serialize any non-primitives + sanitize strings
		for (int i = 0; i < parameters.Length; ++i)
		{
			if (parameters[i] != null)
			{
				if (parameters[i] is RAGE.Elements.Entity) // TODO_LAUNCH: Do other entities need this? seems to only be objects?
				{
					RAGE.Elements.GameEntityBase entity = (RAGE.Elements.GameEntityBase)parameters[i];
					parameters[i] = entity.RemoteId;
				}
				else if (parameters[i].GetType().IsEnum) // Write as int, optimization
				{
					parameters[i] = (int)parameters[i];
				}
				else if (parameters[i] is ICollection || parameters[i] is Array || IsUserDefinedClass(parameters[i].GetType()))
				{
					string strJsonEncode = OwlJSON.SerializeObject(parameters[i], Newtonsoft.Json.Formatting.None, new Newtonsoft.Json.JsonSerializerSettings()
					{
						StringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.EscapeNonAscii
					}, EJsonTrackableIdentifier.PrepareEvent, ev.ToString());

					//byte[] packedData = RAGE.Util.MsgPack.ConvertFromJson(strJsonEncode, MessagePack.Resolvers.ContractlessStandardResolver.Options.WithCompression(MessagePack.MessagePackCompression.Lz4Block));
					//parameters[i] = packedData;
					parameters[i] = strJsonEncode;
				}
				else if (parameters[i].GetType() == typeof(string))
				{
					parameters[i] = ((string)parameters[i]).Replace("\"", "\'");
				}
			}
			else
			{
				parameters[i] = "NETNULL";
			}
		}

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

		var p = lstParams.ToArray();
		CustomEventManagerScript.TimeSpend_PrepareEvent += (DateTime.Now - dtStart).TotalMilliseconds;
		return p;
	}

	// TODO_CSHARP: When we're fully C#, we should get rid of the tostring and just send as int instead to save BW.
	public static void TriggerRemoteEvent(NetworkEventID ev, params object[] parameters)
	{
		if (parameters != null && parameters.Length > 0)
		{
			object[] preparedParams = PrepareEvent(ev, parameters);
			RAGE.Events.CallRemote("CE", preparedParams);
		}
		else
		{
			RAGE.Events.CallRemote("CE", ev.ToString());
		}
	}

	public static void OnCustomEvent(bool bIsUI, bool bSkipNetworkDecompress, object[] arguments_DoNotUse)
	{
		DateTime dtStart = DateTime.Now;
		List<object> lstSafeParams = new List<object>();
		List<object> lstRealParams = new List<object>();

		if (!bIsUI)
		{
			// TODO_RAGE: Hack. Sometimes a random amount of integers appears appended to the front!?
			int garbageCount;
			for (garbageCount = 0; garbageCount < arguments_DoNotUse.Length; ++garbageCount)
			{
				int temp;
				if (!int.TryParse(arguments_DoNotUse[garbageCount].ToString(), out temp))
				{
					break;
				}
			}

			lstSafeParams.AddRange(arguments_DoNotUse.Skip(garbageCount));

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
		}
		else
		{
			// No network parsing for UI events
			lstRealParams.AddRange(arguments_DoNotUse);
		}

		if (lstRealParams.Count >= 1)
		{
			string strEventName = lstRealParams[0].ToString();
			try
			{
				System.Reflection.MemberInfo[] members = bIsUI ? typeof(UIEvents).GetMember(strEventName) : typeof(NetworkEvents).GetMember(strEventName);
				if (members.Length == 1)
				{
					System.Reflection.MemberInfo targetMember = members[0];
					FieldInfo field = (FieldInfo)targetMember;
					Type t = field.FieldType;
					System.Reflection.MethodInfo method = t.GetMethod("Invoke");

					if ((lstRealParams.Count - 1) == method.GetParameters().Length)
					{
						object fieldObj = field.GetValue(null);
						if (fieldObj != null)
						{
							// Deserialize any non-primitives
							int index = 1; // 1 to skip the event name
							foreach (var methodParam in method.GetParameters()) // nothing to skip clientside
							{
								// TODO: Better way of checking
								if (lstRealParams[index].ToString() == "NETNULL")
								{
									lstRealParams[index] = null;
								}
								else if (methodParam.ParameterType.IsArray || methodParam.ParameterType.ToString().Contains("System.Collection") || IsUserDefinedClass(methodParam.ParameterType)) // TODO_CSHARP: Does this work with arrays?
								{
									if (bIsUI || bSkipNetworkDecompress) // no msgpack on UI or local events from JS :(
									{
										try
										{
											lstRealParams[index] = OwlJSON.DeserializeObject(lstRealParams[index].ToString(), methodParam.ParameterType, EJsonTrackableIdentifier.UIEvent, strEventName);
										}
										catch (Exception e)
										{
											ExceptionHelper.SendException(e);
										}
									}
									else
									{
										try
										{
											string strUnpack = RAGE.Util.MsgPack.ConvertToJson((byte[])lstRealParams[index], MessagePack.Resolvers.TypelessContractlessStandardResolver.Options.WithCompression(MessagePack.MessagePackCompression.Lz4Block));
											lstRealParams[index] = OwlJSON.DeserializeObject(strUnpack, methodParam.ParameterType, EJsonTrackableIdentifier.NetworkEvent, strEventName);
										}
										catch (Exception e)
										{
											ExceptionHelper.SendException(e);
										}
									}
								}
								else
								{
									if (methodParam.ParameterType.IsEnum)
									{
										if (bIsUI || bSkipNetworkDecompress)
										{
											lstRealParams[index] = Enum.ToObject(methodParam.ParameterType, Convert.ToInt32(lstRealParams[index]));
										}
										else
										{
											int enumVal = RAGE.Util.MsgPack.Deserialize<int>((byte[])lstRealParams[index], MessagePack.Resolvers.TypelessContractlessStandardResolver.Options.WithCompression(MessagePack.MessagePackCompression.Lz4Block));
											lstRealParams[index] = Enum.ToObject(methodParam.ParameterType, enumVal);
										}
									}
									else if (methodParam.ParameterType == typeof(string))
									{
										if (bIsUI || bSkipNetworkDecompress)
										{
											lstRealParams[index] = lstRealParams[index].ToString().Replace("`", "");
										}
										else
										{
											string strUnpack = RAGE.Util.MsgPack.Deserialize<string>((byte[])lstRealParams[index], MessagePack.Resolvers.TypelessContractlessStandardResolver.Options.WithCompression(MessagePack.MessagePackCompression.Lz4Block));
											lstRealParams[index] = strUnpack.Replace("`", "");
										}
									}
									else
									{
										if (!bIsUI && !bSkipNetworkDecompress)
										{
											if (methodParam.ParameterType == typeof(UInt16)) { lstRealParams[index] = RAGE.Util.MsgPack.Deserialize<UInt16>((byte[])lstRealParams[index], MessagePack.Resolvers.TypelessContractlessStandardResolver.Options.WithCompression(MessagePack.MessagePackCompression.Lz4Block)); }
											else if (methodParam.ParameterType == typeof(UInt32)) { lstRealParams[index] = RAGE.Util.MsgPack.Deserialize<UInt32>((byte[])lstRealParams[index], MessagePack.Resolvers.TypelessContractlessStandardResolver.Options.WithCompression(MessagePack.MessagePackCompression.Lz4Block)); }
											else if (methodParam.ParameterType == typeof(UInt64)) { lstRealParams[index] = RAGE.Util.MsgPack.Deserialize<UInt64>((byte[])lstRealParams[index], MessagePack.Resolvers.TypelessContractlessStandardResolver.Options.WithCompression(MessagePack.MessagePackCompression.Lz4Block)); }
											else if (methodParam.ParameterType == typeof(Int16)) { lstRealParams[index] = RAGE.Util.MsgPack.Deserialize<Int16>((byte[])lstRealParams[index], MessagePack.Resolvers.TypelessContractlessStandardResolver.Options.WithCompression(MessagePack.MessagePackCompression.Lz4Block)); }
											else if (methodParam.ParameterType == typeof(Int32)) { lstRealParams[index] = RAGE.Util.MsgPack.Deserialize<Int32>((byte[])lstRealParams[index], MessagePack.Resolvers.TypelessContractlessStandardResolver.Options.WithCompression(MessagePack.MessagePackCompression.Lz4Block)); }
											else if (methodParam.ParameterType == typeof(Int64)) { lstRealParams[index] = RAGE.Util.MsgPack.Deserialize<Int64>((byte[])lstRealParams[index], MessagePack.Resolvers.TypelessContractlessStandardResolver.Options.WithCompression(MessagePack.MessagePackCompression.Lz4Block)); }
											else if (methodParam.ParameterType == typeof(RAGE.Elements.Vehicle))
											{
												if (!(lstRealParams[index] is RAGE.Elements.Vehicle))
												{
													ushort netHandle = RAGE.Util.MsgPack.Deserialize<ushort>((byte[])lstRealParams[index], MessagePack.Resolvers.TypelessContractlessStandardResolver.Options.WithCompression(MessagePack.MessagePackCompression.Lz4Block));
													lstRealParams[index] = RAGE.Elements.Entities.Vehicles.All.FirstOrDefault(x => x.RemoteId == netHandle);
												}
											}
											else if (methodParam.ParameterType == typeof(RAGE.Elements.MapObject))
											{
												if (!(lstRealParams[index] is RAGE.Elements.MapObject))
												{
													ushort netHandle = RAGE.Util.MsgPack.Deserialize<ushort>((byte[])lstRealParams[index], MessagePack.Resolvers.TypelessContractlessStandardResolver.Options.WithCompression(MessagePack.MessagePackCompression.Lz4Block));
													lstRealParams[index] = RAGE.Elements.Entities.Objects.All.FirstOrDefault(x => x.RemoteId == netHandle);
												}
											}
											else if (methodParam.ParameterType == typeof(RAGE.Elements.Player))
											{
												if (!(lstRealParams[index] is RAGE.Elements.Player))
												{
													ushort netHandle = RAGE.Util.MsgPack.Deserialize<ushort>((byte[])lstRealParams[index], MessagePack.Resolvers.TypelessContractlessStandardResolver.Options.WithCompression(MessagePack.MessagePackCompression.Lz4Block));
													lstRealParams[index] = RAGE.Elements.Entities.Players.All.FirstOrDefault(x => x.RemoteId == netHandle);
												}
											}
											else
											{
												try
												{
													if (!(methodParam.ParameterType == typeof(RAGE.Vector3))) // vectors arent serializable...
													{
														object obj = RAGE.Util.MsgPack.Deserialize<object>((byte[])lstRealParams[index], MessagePack.Resolvers.TypelessContractlessStandardResolver.Options.WithCompression(MessagePack.MessagePackCompression.Lz4Block));
														lstRealParams[index] = Convert.ChangeType(obj, methodParam.ParameterType);
													}
													else
													{
														lstRealParams[index] = Convert.ChangeType(lstRealParams[index], methodParam.ParameterType);
													}

												}
												catch (Exception ex)
												{
													throw new Exception(Helpers.FormatString("[EVENTSYSTEM] Not triggering {0} due to exception: Deserialize: {1} ParamIndex: {5} ParamVal: {4} FromType: {2} ToType: {3}.", strEventName, ex.Message, lstRealParams[index].GetType().Name, methodParam.ParameterType.Name, lstRealParams[index], index));
												}
											}
										}
										else
										{
											// NOTE: We have to hackily convert when the target is uint because RAGE sends as signed always. ChangeType would throw exception due to size.
											if (methodParam.ParameterType == typeof(UInt16)) { lstRealParams[index] = (UInt16)(Int16)lstRealParams[index]; }
											else if (methodParam.ParameterType == typeof(UInt32)) { lstRealParams[index] = (UInt32)(Int32)lstRealParams[index]; }
											else if (methodParam.ParameterType == typeof(UInt64)) { lstRealParams[index] = (UInt64)(Int64)lstRealParams[index]; }
											else if (methodParam.ParameterType == typeof(Int16)) { lstRealParams[index] = (lstRealParams[index] is String ? Int16.Parse(lstRealParams[index].ToString()) : Convert.ToInt16(lstRealParams[index])); }
											else if (methodParam.ParameterType == typeof(Int32)) { lstRealParams[index] = (lstRealParams[index] is String ? Int32.Parse(lstRealParams[index].ToString()) : Convert.ToInt32(lstRealParams[index])); }
											else if (methodParam.ParameterType == typeof(Int64)) { lstRealParams[index] = (lstRealParams[index] is String ? Int64.Parse(lstRealParams[index].ToString()) : Convert.ToInt64(lstRealParams[index])); }
											else if (methodParam.ParameterType == typeof(RAGE.Elements.Vehicle))
											{
												if (!(lstRealParams[index] is RAGE.Elements.Vehicle))
												{
													ushort netHandle = (UInt16)(Int16)Convert.ToInt16(lstRealParams[index]);
													lstRealParams[index] = RAGE.Elements.Entities.Vehicles.All.FirstOrDefault(x => x.RemoteId == netHandle);
												}
											}
											else if (methodParam.ParameterType == typeof(RAGE.Elements.MapObject))
											{
												if (!(lstRealParams[index] is RAGE.Elements.MapObject))
												{
													ushort netHandle = (UInt16)(Int16)Convert.ToInt16(lstRealParams[index]);
													lstRealParams[index] = RAGE.Elements.Entities.Objects.All.FirstOrDefault(x => x.RemoteId == netHandle);
												}
											}
											else if (methodParam.ParameterType == typeof(RAGE.Elements.Player))
											{
												if (!(lstRealParams[index] is RAGE.Elements.Player))
												{
													ushort netHandle = (UInt16)(Int16)Convert.ToInt16(lstRealParams[index]);
													lstRealParams[index] = RAGE.Elements.Entities.Players.All.FirstOrDefault(x => x.RemoteId == netHandle);
												}
											}
											else
											{
												try
												{
													lstRealParams[index] = Convert.ChangeType(lstRealParams[index], methodParam.ParameterType);
												}
												catch (Exception ex)
												{
													throw new Exception(Helpers.FormatString("[EVENTSYSTEM] Not triggering {0} due to exception: Deserialize: {1} ParamIndex: {5} ParamVal: {4} FromType: {2} ToType: {3}.", strEventName, ex.Message, lstRealParams[index].GetType().Name, methodParam.ParameterType.Name, lstRealParams[index], index));
												}
											}
										}
									}
								}

								++index;
							}

							long millisecondsStart = DateTime.Now.Ticks;
							method.Invoke(fieldObj, lstRealParams.Skip(1).ToArray());
							EventManager.RegisterStatistic(method, millisecondsStart, strEventName);
							PerfManager.RegisterStatistic(method, millisecondsStart, strEventName);

						}
					}
					else
					{
						throw new Exception(Helpers.FormatString("[EVENTSYSTEM] Not triggering {0} due to argument count mismatch ({1} vs {2}).", strEventName, (lstRealParams.Count - 1), method.GetParameters().Length - 1));
					}
				}
				else
				{
					throw new Exception(Helpers.FormatString("[EVENTSYSTEM] Not triggering {0} due to method not being found.", strEventName));
				}
			}
			catch (Exception ex)
			{
				ExceptionHelper.SendException(ex);
				throw new Exception(Helpers.FormatString("[EVENTSYSTEM] Not triggering {0} due to exception: {1} (StackTrace: {2}).", strEventName, ex.Message, ex.StackTrace));
			}
		}

		CustomEventManagerScript.lstUIEvents.Add(lstRealParams[0].ToString());
		CustomEventManagerScript.TimeSpend_OnCustomEvent += (DateTime.Now - dtStart).TotalMilliseconds;
	}

	public static void OnUIEvent(object[] arguments)
	{
		DateTime dtStart = DateTime.Now;
		if (arguments.Length >= 1)
		{
			arguments[0] = Helpers.FormatString("{0}", arguments[0].ToString());
			OnCustomEvent(true, true, arguments);
		}

		CustomEventManagerScript.lstUIEvents.Add(arguments[0].ToString());
		CustomEventManagerScript.TimeSpend_OnUIEvent += (DateTime.Now - dtStart).TotalMilliseconds;
	}

	private class FrameStatistics
	{
		public FrameStatistics()
		{
			m_dictPerfCountersPerModule = new Dictionary<string, double>();
			m_dictPerfCountersExpensiveFunctions = new Dictionary<string, double>();
			m_dictPerfCountersExpensiveTimers = new Dictionary<string, double>();
			m_dictHighExecutionUIs = new Dictionary<string, double>();
		}

		public Dictionary<string, double> m_dictPerfCountersPerModule;
		public Dictionary<string, double> m_dictPerfCountersExpensiveFunctions;
		public Dictionary<string, double> m_dictPerfCountersExpensiveTimers;
		public Dictionary<string, double> m_dictHighExecutionUIs;
	}

	private static Stack<FrameStatistics> g_FrameStatisticsActive = new Stack<FrameStatistics>();
	private static List<FrameStatistics> g_FrameStatisticsRender = new List<FrameStatistics>();

	public static void ClearStatistics(object[] parameters)
	{
		g_FrameStatisticsRender.Clear();
		g_FrameStatisticsRender.AddRange(g_FrameStatisticsActive);

		g_FrameStatisticsActive.Clear();
	}

	public static void RegisterHighExecutionUI(string strUI, int count)
	{
		if (m_StatsMode != EStatsMode.Detailed)
		{
			return;
		}

		if (g_FrameStatisticsActive.Count == 0)
		{
			return;
		}

		string[] strSplit = strUI.Split("/");
		string strFileName = strSplit[strSplit.Length - 1];

		foreach (var frame in g_FrameStatisticsActive)
		{
			if (frame.m_dictHighExecutionUIs.ContainsKey(strFileName))
			{
				return;
			}
		}

		FrameStatistics currentFrame = g_FrameStatisticsActive.Peek();
		if (currentFrame != null)
		{
			currentFrame.m_dictHighExecutionUIs[strFileName] = count;
		}
	}

	private static double m_dTimeSpentInScript_CachedRender = 0.0;
	private static double m_FrameTime_CachedRender = 0.0;
	private static double m_dTimeSpentInScript = 0.0f;
	private static DateTime m_LastUpdateScriptTime = DateTime.Now;
	private static int m_iTimeSpentInScript_Frame = -1;

	public static void RegisterStatistic(MethodInfo methodInfo, long time_started, string strExtraInfo)
	{
		if (m_StatsMode < EStatsMode.FPS)
		{
			return;
		}

		if (g_FrameStatisticsActive.Count == 0)
		{
			return;
		}

		int currentFrameID = RAGE.Game.Misc.GetFrameCount();
		if (currentFrameID != m_iTimeSpentInScript_Frame)
		{
			// last frame time
			long currentTime = DateTime.Now.Ticks;
			m_FrameTime = ((double)currentTime - (double)m_LastFrameStartTime) / (double)TimeSpan.TicksPerMillisecond;
			m_LastFrameStartTime = currentTime;

			// Should we render the change?
			double timeSinceLastProc = (DateTime.Now - m_LastUpdateScriptTime).TotalMilliseconds;
			if (timeSinceLastProc >= 1000)
			{
				m_LastUpdateScriptTime = DateTime.Now;
				m_dTimeSpentInScript_CachedRender = m_dTimeSpentInScript;
				m_FrameTime_CachedRender = m_FrameTime;
			}

			m_dTimeSpentInScript = 0.0f;
			m_iTimeSpentInScript_Frame = currentFrameID;
		}

		FrameStatistics currentFrame = g_FrameStatisticsActive.Peek();
		if (currentFrame != null)
		{
			long end = DateTime.Now.Ticks;
			double time_diff = ((double)end - (double)time_started) / (double)TimeSpan.TicksPerMillisecond;
			string strModuleName = methodInfo.DeclaringType.FullName;

			m_dTimeSpentInScript += time_diff;

			if (m_StatsMode != EStatsMode.Detailed)
			{
				return;
			}

			if (currentFrame.m_dictPerfCountersPerModule.ContainsKey(strModuleName))
			{
				currentFrame.m_dictPerfCountersPerModule[strModuleName] += time_diff;
			}
			else
			{
				currentFrame.m_dictPerfCountersPerModule[strModuleName] = time_diff;
			}

			// increment total time for module
			string strFunctionName = methodInfo.Name;

			if (time_diff >= m_fThresholdCaution)
			{
				string strEntryName = Helpers.FormatString("{0}::{1}", strModuleName, strFunctionName);
				if (strEntryName.Length > 32 && strEntryName.Contains("::"))
				{
					string[] strSplit = strEntryName.Split("::");
					strEntryName = Helpers.FormatString("{0}...::{1}", strSplit[0].Substring(0, Math.Min(strSplit[0].Length, 32)), strSplit[1]);
				}

				if (strExtraInfo != null)
				{
					strEntryName += Helpers.FormatString(" {0}", strExtraInfo);
				}

				if (!strEntryName.Contains("MultiFrameWorkScheduler"))
				{
					currentFrame.m_dictPerfCountersExpensiveFunctions[strEntryName] = time_diff;
				}
			}
		}
	}

	enum EStatToDraw
	{
		PerModule,
		ExpensiveFunctions,
		ExpensiveTimers,
		HighExecutionUIs
	}

	private static void RenderStats(EStatToDraw statToDraw, ref float x, ref float y, string strDisplayName)
	{
		const int renderLimit = 25;
		const float fDeltaY = 0.02f;

		TextHelper.Draw2D(strDisplayName, x, y, 0.3f, new RAGE.RGBA(255, 255, 255, m_Alpha), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Right, true, false);
		y += fDeltaY;

		float fInitialYForReset = y;

		int frameIndex = 0;
		int numRenders = 0;
		foreach (FrameStatistics frame in g_FrameStatisticsRender)
		{
			Dictionary<string, double> dict = null;
			if (statToDraw == EStatToDraw.PerModule)
			{
				dict = frame.m_dictPerfCountersPerModule;
			}
			else if (statToDraw == EStatToDraw.ExpensiveFunctions)
			{
				dict = frame.m_dictPerfCountersExpensiveFunctions;
			}
			else if (statToDraw == EStatToDraw.ExpensiveTimers)
			{
				dict = frame.m_dictPerfCountersExpensiveTimers;
			}
			else if (statToDraw == EStatToDraw.HighExecutionUIs)
			{
				dict = frame.m_dictHighExecutionUIs;
			}

			bool bFrameWasDrawn = false;

			if (dict.Count == 0)
			{
				//TextHelper.Draw2D("Nothing To Show", x, y, 0.3f, new RAGE.RGBA(255, 255, 255, m_Alpha), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Right, true, false);
			}
			else
			{
				foreach (var keyValPair in dict)
				{
					float fThresholdToShow = m_fThresholdCaution;

					if (keyValPair.Value > fThresholdToShow)
					{
						RAGE.RGBA color = null;

						if (keyValPair.Value >= m_fThresholdDanger)
						{
							color = new RAGE.RGBA(255, 0, 0, m_Alpha);
						}
						else if (keyValPair.Value >= m_fThresholdCaution)
						{
							color = new RAGE.RGBA(255, 255, 0, m_Alpha);
						}
						else
						{
							color = new RAGE.RGBA(0, 255, 0, m_Alpha);
						}

						string strFuncName = keyValPair.Key;
						string strUnit = "ms";

						if (statToDraw == EStatToDraw.HighExecutionUIs)
						{
							strUnit = "calls per sec";
						}

						TextHelper.Draw2D(Helpers.FormatString("{0}: {1:0.00} {3} [#{2}]", strFuncName, keyValPair.Value, frameIndex, strUnit), x, y, 0.3f, color, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Right, true, false);

						bFrameWasDrawn = true;
						y += fDeltaY;
					}
				}
			}

			if (bFrameWasDrawn)
			{
				++numRenders;
				//y += (fDeltaY * 2);

				if (y >= 0.9f)
				{
					x -= 0.25f;
					y = fInitialYForReset;
				}
			}

			++frameIndex;

			if (numRenders >= renderLimit)
			{
				break;
			}
		}

		y += fDeltaY;
	}

	// TODO_CSHARP: Move stats logic someplace better
	static EventManager()
	{
		ClientTimerPool.CreateTimer(ClearStatistics, 10000);
		ClientTimerPool.CreateTimer(UpdateFPS, 1000);

		ScriptControls.SubscribeToControl(EScriptControlID.ToggleStatistics, ToggleStatsMode);
		//KeyBinds.Bind(ConsoleKey.F10, NukeCEF, EKeyBindType.Pressed, EKeyBindFlag.Default);
		NetworkEvents.CharacterSelectionApproved += ResetFully;
	}

	private static void ToggleStatsMode(EControlActionType actionType)
	{
		if (m_StatsMode == EStatsMode.Off)
		{
			m_StatsMode = EStatsMode.FPS;
		}
		else if (m_StatsMode == EStatsMode.FPS)
		{
			m_StatsMode = EStatsMode.Detailed;
		}
		else if (m_StatsMode == EStatsMode.Detailed)
		{
			m_StatsMode = EStatsMode.Off;
		}

		NotificationManager.ShowNotification("Statistics", Helpers.FormatString("Stats mode changed to {0}.", m_StatsMode.ToString()), ENotificationIcon.InfoSign);
	}

	private static void ResetFully()
	{
		g_FrameStatisticsActive.Clear();
		g_FrameStatisticsRender.Clear();

#if DEBUG
		EAdminLevel adminLevel = DataHelper.GetLocalPlayerEntityData<EAdminLevel>(EDataNames.ADMIN_LEVEL);
		if (adminLevel == EAdminLevel.HeadAdmin)
		{
			m_StatsMode = EStatsMode.FPS;
		}
#endif
	}

	private enum EStatsMode
	{
		Off,
		FPS,
		Detailed
	}

	private static int m_FrameCounter_StartFrameNumber = -1;
	private static int m_LastFPS = -1;
	private static void UpdateFPS(object[] parameters)
	{
		int endFrameNumber = RAGE.Game.Misc.GetFrameCount();
		m_LastFPS = endFrameNumber - m_FrameCounter_StartFrameNumber;
		m_FrameCounter_StartFrameNumber = endFrameNumber;
	}

	public static void RenderLastFrame()
	{
		const float default_x = 0.925f;
		float x = default_x;

		if (m_StatsMode != EStatsMode.Off)
		{
			TextHelper.Draw2D(Helpers.FormatString("FPS: {0}", m_LastFPS == -1 ? "Calculating..." : m_LastFPS.ToString()), x, 0.09f, 0.3f, new RAGE.RGBA(255, 194, 15, m_Alpha), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Right, true, false);

			bool bPerformanceBound = m_LastFPS < g_TargetFPS;
			RAGE.RGBA color = bPerformanceBound ? new RAGE.RGBA(255, 0, 0, m_Alpha) : new RAGE.RGBA(0, 255, 0, m_Alpha);
			TextHelper.Draw2D(Helpers.FormatString("Performance Bound: {0}", bPerformanceBound ? "Yes" : "No"), x, 0.11f, 0.3f, color, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Right, true, false);

			int latency = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.PING);
			RAGE.RGBA latencyColor;
			if (latency < 30)
			{
				latencyColor = new RAGE.RGBA(0, 255, 0, m_Alpha);
			}
			else if (latency < 75)
			{
				latencyColor = new RAGE.RGBA(255, 255, 0, m_Alpha);
			}
			else
			{
				latencyColor = new RAGE.RGBA(255, 0, 0, m_Alpha);
			}

			TextHelper.Draw2D(Helpers.FormatString("Latency: {0} ms", latency), x, 0.13f, 0.3f, latencyColor, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Right, true, false);

			if (m_StatsMode != EStatsMode.Off)
			{
				// Render totals
				double dTotalTimeSpentInScript = m_dTimeSpentInScript_CachedRender;
				double dTotalTimeSpentInGTA = m_FrameTime_CachedRender - dTotalTimeSpentInScript;

				TextHelper.Draw2D(Helpers.FormatString("Frametime: {0:0.00} ms", m_FrameTime_CachedRender), x, 0.15f, 0.3f, new RAGE.RGBA(255, 194, 15, m_Alpha), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Right, true, false);
				TextHelper.Draw2D(Helpers.FormatString("Script Time: {0:0.00} ms ({1:0.0}%)", dTotalTimeSpentInScript, (dTotalTimeSpentInScript / m_FrameTime_CachedRender) * 100.0), x, 0.17f, 0.3f, new RAGE.RGBA(255, 194, 15, m_Alpha), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Right, true, false);
				TextHelper.Draw2D(Helpers.FormatString("GTA Time: {0:0.00} ms ({1:0.0}%)", dTotalTimeSpentInGTA, (dTotalTimeSpentInGTA / m_FrameTime_CachedRender) * 100.0), x, 0.19f, 0.3f, new RAGE.RGBA(255, 194, 15, m_Alpha), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Right, true, false);

				if (m_StatsMode == EStatsMode.Detailed)
				{
					const float default_y = 0.23f;
					float y = default_y;

					// PER MODULE
					//RenderStats(EStatToDraw.PerModule, ref x, ref y, "Per Module (Last 10 sec)");

					// EXPENSIVE FUNCTIONS
					RenderStats(EStatToDraw.ExpensiveFunctions, ref x, ref y, "Slowdowns (Last 10 sec)");

					// EXPENSIVE TIMERS
					RenderStats(EStatToDraw.ExpensiveTimers, ref x, ref y, "Slow Timers (Last 10 sec)");

					// HIGH EXECUTIONS UIs
					RenderStats(EStatToDraw.HighExecutionUIs, ref x, ref y, "High Execution UIs (Last 10 sec)");
				}
			}
		}

		g_FrameStatisticsActive.Push(new FrameStatistics());
	}

	public static void RegisterTimerPerf(Delegate timerDelegate, long time_started)
	{
		long end = DateTime.Now.Ticks;
		double time_diff = ((double)end - (double)time_started) / (double)TimeSpan.TicksPerMillisecond;

		if (time_diff >= m_fThresholdCaution / 2.0f)
		{
			string strModuleName = timerDelegate.GetMethodInfo().DeclaringType.FullName;
			string strFunctionName = timerDelegate.GetMethodInfo().Name;

			string strEntryName = Helpers.FormatString("{0}::{1}", strModuleName, strFunctionName);

			if (g_FrameStatisticsActive.Count > 0)
			{
				FrameStatistics currentFrame = g_FrameStatisticsActive.Peek();
				currentFrame.m_dictPerfCountersExpensiveTimers[strEntryName] = time_diff;
			}
		}
	}

	public static void InvokeRageDelegate(Delegate parentDelegate, bool bRegisterTime, params object[] args)
	{
		try
		{
			if (parentDelegate != null)
			{
				Delegate[] list = parentDelegate.GetInvocationList();
				foreach (Delegate subscriber in list)
				{
					long millisecondsStart = DateTime.Now.Ticks;
					subscriber.DynamicInvoke(args);

					if (bRegisterTime)
					{
						EventManager.RegisterStatistic(subscriber.GetMethodInfo(), millisecondsStart, parentDelegate.GetMethodInfo().Name);
						PerfManager.RegisterStatistic(subscriber.GetMethodInfo(), millisecondsStart, parentDelegate.GetMethodInfo().Name);
					}
				}
			}
		}
		catch (Exception ex)
		{
			ExceptionHelper.SendException(ex);
		}
	}

	private static double m_FrameTime = 0.0;
	private static long m_LastFrameStartTime = 0;
	private static uint m_Alpha = 255;
	private static EStatsMode m_StatsMode = EStatsMode.Off;

	private static float m_fThresholdCaution = 1.0f;
	private static float m_fThresholdDanger = 2.0f;
	private const int g_TargetFPS = 60;
}

// This must still inheirt from rage script because it needs RAGE events
class CustomEventManagerScript : RAGE.Events.Script
{
	internal Dictionary<ConsoleKey, bool> KeyList = new Dictionary<ConsoleKey, bool>();

	const int g_TickRate_MAX = 60;
	const int g_TickRate_HALF = 30;
	const int g_TickRate_QUARTER = 15;

	const float g_TickPerFrameCount_MAX = 1000 / g_TickRate_MAX;
	const float g_TickPerFrameCount_HALF = 1000 / g_TickRate_HALF;
	const float g_TickPerFrameCount_QUARTER = 1000 / g_TickRate_QUARTER;

	long g_LastTick_MAX = DateTime.Now.Ticks;
	long g_LastTick_HALF = DateTime.Now.Ticks;
	long g_LastTick_QUARTER = DateTime.Now.Ticks;
	long g_LastTick_ONCE_PER_SECOND = DateTime.Now.Ticks;

	public static double TimeSpend_OnCustomEvent = 0.0f;
	public static double TimeSpend_OnUIEvent = 0.0f;
	public static double TimeSpend_PrepareEvent = 0.0f;
	public static List<string> lstUIEvents = new List<string>();
	public static bool m_bDebugSpamEnabled = false;

	CustomEventManagerScript()
	{
		RAGE.Events.Add("CE", (object[] arguments) => { EventManager.OnCustomEvent(false, false, arguments); });
		RAGE.Events.Add("CE_LEGACY", (object[] arguments) => { EventManager.OnCustomEvent(false, true, arguments); });
		RAGE.Events.Add("UI", EventManager.OnUIEvent);

		NetworkEvents.ToggleDebugSpam += () =>
		{
			m_bDebugSpamEnabled = !m_bDebugSpamEnabled;
		};

		RAGE.Events.OnPlayerEnterCheckpoint += (checkpoint, cancel) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerEnterCheckpoint, true, checkpoint, cancel); };
		RAGE.Events.OnEventTriggeredByKey += (ulong key, object[] args, RAGE.Events.CancelEventArgs cancel) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnEventTriggeredByKey, true, key, args, cancel); };
		RAGE.Events.OnEntityDataChangeByKey += (ulong key, RAGE.Elements.Entity entity, object arg) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnEntityDataChangeByKey, true, key, entity, arg); };
		RAGE.Events.Tick += (List<RAGE.Events.TickNametagData> nametags) =>
		{
			EventManager.RenderLastFrame();

			// last frame
			if (m_bDebugSpamEnabled)
			{
				const float fLimit = 2.0f;
				if (TimeSpend_OnCustomEvent > fLimit || TimeSpend_PrepareEvent > fLimit || TimeSpend_OnUIEvent > fLimit)
					ChatHelper.DebugMessage("[FRAME {4}] TimeSpend_OnCustomEvent: {0}, TimeSpend_PrepareEvent: {1}, TimeSpend_OnUIEvent: {2} ({3})", TimeSpend_OnCustomEvent, TimeSpend_PrepareEvent, TimeSpend_OnUIEvent, String.Join("\n", lstUIEvents), RAGE.Game.Misc.GetFrameCount());
			}

			TimeSpend_OnCustomEvent = 0.0f;
			TimeSpend_PrepareEvent = 0.0f;
			TimeSpend_OnUIEvent = 0.0f;
			lstUIEvents.Clear();

			// TICK MAX
			{
				double time_diff = ((double)DateTime.Now.Ticks - (double)g_LastTick_MAX) / (double)TimeSpan.TicksPerMillisecond;
				if (time_diff > g_TickPerFrameCount_MAX)
				{
					EventManager.InvokeRageDelegate(RageEvents.RAGE_OnTick_HighFrequency, true, null);
					g_LastTick_MAX = DateTime.Now.Ticks;
				}
			}

			// TICK HALF
			{
				double time_diff = ((double)DateTime.Now.Ticks - (double)g_LastTick_HALF) / (double)TimeSpan.TicksPerMillisecond;
				if (time_diff > g_TickPerFrameCount_HALF)
				{
					EventManager.InvokeRageDelegate(RageEvents.RAGE_OnTick_MediumFrequency, true, null);
					g_LastTick_HALF = DateTime.Now.Ticks;
				}
			}

			// TICK QUARTER
			{
				double time_diff = ((double)DateTime.Now.Ticks - (double)g_LastTick_QUARTER) / (double)TimeSpan.TicksPerMillisecond;
				if (time_diff > g_TickPerFrameCount_QUARTER)
				{
					EventManager.InvokeRageDelegate(RageEvents.RAGE_OnTick_LowFrequency, true, null);
					g_LastTick_QUARTER = DateTime.Now.Ticks;
				}

				// Timers are ticked at quarter framerate... we don't need that level of accuracy - ~66ms means nothing to us for a timer that is typically >= 1 sec
				// We don't register timer tick because the time spent calling tick is tiny and we don't want to include the timer CPU time in OnTick. We count timers seperately.
				EventManager.InvokeRageDelegate(RageEvents.RAGE_TimerTickInternal_DO_NOT_USE, false);
			}

			// TICK ONCE PER SEC
			{
				double time_diff = ((double)DateTime.Now.Ticks - (double)g_LastTick_ONCE_PER_SECOND) / (double)TimeSpan.TicksPerMillisecond;
				if (time_diff > 1000)
				{
					EventManager.InvokeRageDelegate(RageEvents.RAGE_OnTick_OncePerSecond, true, null);
					g_LastTick_ONCE_PER_SECOND = DateTime.Now.Ticks;
				}
			}

			EventManager.InvokeRageDelegate(RageEvents.RAGE_OnTick_PerFrame, true, null);
			EventManager.InvokeRageDelegate(RageEvents.RAGE_OnRender, true, null);
		};
		RAGE.Events.OnPlayerStopTalking += (RAGE.Elements.Player player) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerStopTalking, true, player); };
		RAGE.Events.OnPlayerStartTalking += (RAGE.Elements.Player player) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerStartTalking, true, player); };
		RAGE.Events.OnPlayerLeaveVehicle += (RAGE.Elements.Vehicle vehicle, int seatId) => { EventManager.TriggerLocalEvent(NetworkEventID.ExitVehicleStart, vehicle, seatId); };
		RAGE.Events.OnPlayerStartEnterVehicle += (RAGE.Elements.Vehicle vehicle, int seatId, RAGE.Events.CancelEventArgs cancel) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerStartEnterVehicle, true, vehicle, seatId, cancel); };
		RAGE.Events.OnBrowserLoadingFailed += (RAGE.Ui.HtmlWindow window) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnBrowserLoadingFailed, true, window); };
		RAGE.Events.OnBrowserDomReady += (RAGE.Ui.HtmlWindow window) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnBrowserDomReady, true, window); };
		RAGE.Events.OnBrowserCreated += (RAGE.Ui.HtmlWindow window) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnBrowserCreated, true, window); };
		RAGE.Events.OnGuiReady += () => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnGuiReady, true); };
		RAGE.Events.OnPlayerWeaponShot += (RAGE.Vector3 targetPos, RAGE.Elements.Player target, RAGE.Events.CancelEventArgs cancel) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerWeaponShot, true, targetPos, target, cancel); };
		RAGE.Events.OnClickWithRaycast += (int x, int y, bool up, bool right, float relativeX, float relativeY, RAGE.Vector3 worldPos, int entityHandle) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnClickWithRaycast, true, x, y, up, right, relativeX, relativeY, worldPos, entityHandle); };
		RAGE.Events.OnClick += (int x, int y, bool up, bool right) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnClick, true, x, y, up, right); };
		RAGE.Events.OnPlayerResurrect += () => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerResurrect, true); };
		RAGE.Events.OnPlayerDeath += (RAGE.Elements.Player player, uint reason, RAGE.Elements.Player killer, RAGE.Events.CancelEventArgs cancel) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerDeath, true, player, reason, killer, cancel); };
		RAGE.Events.OnPlayerExitCheckpoint += (RAGE.Elements.Checkpoint checkpoint, RAGE.Events.CancelEventArgs cancel) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerExitCheckpoint, true, checkpoint, cancel); };
		RAGE.Events.OnPlayerEnterColshape += (RAGE.Elements.Colshape colshape, RAGE.Events.CancelEventArgs cancel) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerEnterColshape, true, colshape, cancel); };
		RAGE.Events.OnPlayerExitColshape += (RAGE.Elements.Colshape colshape, RAGE.Events.CancelEventArgs cancel) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerExitColshape, true, colshape, cancel); };
		RAGE.Events.OnPlayerChat += (string text, RAGE.Events.CancelEventArgs cancel) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerChat, true, text, cancel); };
		RAGE.Events.OnPlayerCommand += (string cmd, RAGE.Events.CancelEventArgs cancel) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerCommand, true, cmd, cancel); };
		RAGE.Events.OnEntityCreated += (RAGE.Elements.Entity entity) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnEntityCreated, true, entity); };
		RAGE.Events.OnEntityDestroyed += (RAGE.Elements.Entity entity) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnEntityDestroyed, true, entity); };
		RAGE.Events.OnScriptWindowDestroyed += (RAGE.Ui.HtmlWindow window) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnScriptWindowDestroyed, true, window); };
		RAGE.Events.OnEntityStreamIn += (RAGE.Elements.Entity entity) => { if (entity != null) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnEntityStreamIn, true, entity); } };
		RAGE.Events.OnEntityStreamOut += (RAGE.Elements.Entity entity) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnEntityStreamOut, true, entity); };
		RAGE.Events.OnPlayerJoin += (RAGE.Elements.Player player) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerJoin, true, player); };
		RAGE.Events.OnPlayerQuit += (RAGE.Elements.Player player) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerQuit, true, player); };
		RAGE.Events.OnPlayerSpawn += (RAGE.Events.CancelEventArgs cancel) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerSpawn, true, cancel); };
		RAGE.Events.OnScriptWindowCreated += (RAGE.Ui.HtmlWindow window) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnScriptWindowCreated, true, window); };

		// new 1.1 events
		RAGE.Events.OnPlayerCreateWaypoint += (RAGE.Vector3 position) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerCreateWaypoint, true, position); };
		RAGE.Events.OnPlayerRemoveWaypoint += () => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerRemoveWaypoint, true); };
		RAGE.Events.OnEntityControllerChange += (RAGE.Elements.Entity entity, RAGE.Elements.Player newController) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnEntityControllerChange, true, entity, newController); };
		RAGE.Events.OnIncomingDamage += (RAGE.Elements.Player sourcePlayer, RAGE.Elements.Entity sourceEntity, RAGE.Elements.Entity targetEntity, ulong weaponHash, ulong boneIdx, int damage, RAGE.Events.CancelEventArgs cancel) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnIncomingDamage, true, sourcePlayer, sourceEntity, targetEntity, weaponHash, boneIdx, damage, cancel); };
		RAGE.Events.OnOutgoingDamage += (RAGE.Elements.Entity sourceEntity, RAGE.Elements.Entity targetEntity, RAGE.Elements.Player sourcePlayer, ulong weaponHash, ulong boneIdx, int damage, RAGE.Events.CancelEventArgs cancel) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnOutgoingDamage, true, sourcePlayer, sourceEntity, targetEntity, weaponHash, boneIdx, damage, cancel); };
		RAGE.Events.OnExplosion += (RAGE.Elements.Player sourcePlayer, uint explosionType, RAGE.Vector3 position, RAGE.Events.CancelEventArgs cancel) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnExplosion, true, sourcePlayer, explosionType, position, cancel); };
		RAGE.Events.OnProjectile += (RAGE.Elements.Player sourcePlayer, uint weaponHash, uint ammoType, RAGE.Vector3 position, RAGE.Vector3 velocity, RAGE.Events.CancelEventArgs cancel) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnProjectile, true, sourcePlayer, weaponHash, ammoType, position, velocity, cancel); };
		RAGE.Events.OnDummyEntityCreated += (int type, RAGE.Elements.DummyEntity entity) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnDummyEntityCreated, true, type, entity); };
		RAGE.Events.OnDummyEntityDestroyed += (int type, RAGE.Elements.DummyEntity entity) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnDummyEntityDestroyed, true, type, entity); };
		RAGE.Events.OnConsoleCommand += (string cmd) => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnConsoleCommand, true, cmd); };
		RAGE.Events.OnPlayerReady += () => { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerReady, true); };
	}
}

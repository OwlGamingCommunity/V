//#define TEST_LARGE_DATA_TRANSFER
//#define DISABLE_LFT_CACHE

using System;
using System.Collections.Generic;

public class LargeDataTransfer
{
	public ELargeDataTransferType m_TransferType { get; set; }
	public int m_Identifier { get; set; }
	private int m_DataOffset = 0;
	private byte[] m_DataBytes; // NOTE: ON receive, this is encrypted, use GetBytes()

	private Action<ELargeDataTransferType, int, int, int, int> m_CallbackOnTransferProgress = null;
	private Action<ELargeDataTransferType, int> m_CallbackOnTransferComplete = null;

	private bool m_bIsIncoming = false;

	private bool m_bAllowClientsideCaching = false;

	private int m_crc32 = -1;
	private byte[] m_key = null;

	public ELargeDataTransferState TransferState { get; set; } = ELargeDataTransferState.PendingAck;

	public byte[] GetBytes()
	{
		if (EncryptionHelper.DecryptBytesToBytes(m_DataBytes, m_key, out byte[] decryptedBytes))
		{
			return decryptedBytes;
		}

		return null;
	}

	public byte[] GetBytesEncrypted()
	{
		return m_DataBytes;
	}

	public int GetDataOffset()
	{
		return m_DataOffset;
	}

	public int GetDataLengthDecrypted()
	{
		return GetBytes().Length;
	}

	public int GetDataLengthEncrypted()
	{
		return m_DataBytes.Length;
	}

	public void TriggerCompletion()
	{
		m_CallbackOnTransferComplete?.Invoke(m_TransferType, m_Identifier);
	}

	public bool AllowCaching()
	{
		return m_bAllowClientsideCaching;
	}

	public byte[] GetKey()
	{
		return m_key;
	}

	public void RestoreFromCache(byte[] encryptedData)
	{
		m_DataOffset = encryptedData.Length;
		m_DataBytes = encryptedData;
	}

	public LargeDataTransfer(ELargeDataTransferType a_TransferType, int a_Identifier, byte[] a_DataBytes, Action<ELargeDataTransferType, int, int, int, int> a_CallbackOnTransferProgress, Action<ELargeDataTransferType, int> a_CallbackOnTransferComplete, byte[] key)
	{
		m_TransferType = a_TransferType;
		m_Identifier = a_Identifier;
		m_DataOffset = 0;
		m_crc32 = CRC32.ComputeHash(a_DataBytes); // CRC is pre-compression & pre-encryption

		// encrypt data first
		a_DataBytes = EncryptionHelper.EncryptBytesToBytes(a_DataBytes, key);
		m_DataBytes = RAGE.Util.MsgPack.Serialize(a_DataBytes, MessagePack.Resolvers.ContractlessStandardResolver.Options.WithCompression(MapTransferConstants.CompressionAlgorithm));

		m_CallbackOnTransferProgress = a_CallbackOnTransferProgress;
		m_CallbackOnTransferComplete = a_CallbackOnTransferComplete;

		// inform event
		m_bIsIncoming = false;

		m_key = key;
		InformServer();

		m_bAllowClientsideCaching = false;
	}

	private void InformServer()
	{
		NetworkEventSender.SendNetworkEvent_LargeDataTransfer_ClientToServer_Begin(m_TransferType, m_Identifier, m_DataBytes.Length, m_crc32, m_key); // send compressed length
		TransferState = ELargeDataTransferState.PendingAck;
	}

	public void ResetForReTransfer()
	{
		m_DataOffset = 0;
		InformServer();
	}

	public LargeDataTransfer(ELargeDataTransferType a_TransferType, int a_Identifier, int totalNumBytes, int crc32, bool bAllowClientsideCaching, byte[] key)
	{
		m_TransferType = a_TransferType;
		m_Identifier = a_Identifier;
		m_DataOffset = 0;
		m_DataBytes = new byte[totalNumBytes];
		m_CallbackOnTransferProgress = null;
		m_CallbackOnTransferComplete = null;

		m_crc32 = crc32;
		m_bIsIncoming = false;
		m_bAllowClientsideCaching = bAllowClientsideCaching;
		m_key = key;
	}

	public bool ReceiveTransferFrame(byte[] data, out bool bSuccess)
	{
		bSuccess = true;

		// safety check to avoid network attacks
		if (m_DataOffset + data.Length <= m_DataBytes.Length)
		{
			// copy bytes
			Array.Copy(data, 0, m_DataBytes, m_DataOffset, data.Length);
			m_DataOffset += data.Length;

			// did we finish with this transfer?
			if (m_DataOffset >= m_DataBytes.Length)
			{
				// decompress first (our CRC is pre-compression)
				try
				{
					if (AllowCaching())
					{
						// cache the data (pre-decompression)
						string strCacheKey = LargeDataTransferManager.GenerateCacheKey(m_TransferType, m_Identifier);
						RageClientStorageManager.Container.LFTCache[strCacheKey] = Convert.ToBase64String(m_DataBytes);
						RageClientStorageManager.Flush();
					}

					m_DataBytes = RAGE.Util.MsgPack.Deserialize<byte[]>(m_DataBytes, MessagePack.Resolvers.TypelessContractlessStandardResolver.Options.WithCompression(MapTransferConstants.CompressionAlgorithm));
				}
				catch
				{
					NetworkEventSender.SendNetworkEvent_LargeDataTransfer_ServerToClient_RequestResend(m_TransferType, m_Identifier);
					// done, but we failed :(
					bSuccess = false;
					return true;
				}

				// do the checksums match?
				bSuccess = CRC32.ComputeHash(GetBytes()) == m_crc32;
				return true;
			}
		}
		else
		{
			// Fake our completion, with a failed flag
			bSuccess = false;
			return true;
		}

		return false;
	}

	public void SendTransferFrame()
	{
		if (!m_bIsIncoming)
		{
			if (TransferState == ELargeDataTransferState.InProgress_Ready)
			{
				// process transfer data
				int bytesRemaining = m_DataBytes.Length - m_DataOffset;
				int bytesToSend = Math.Min(bytesRemaining, LargeTransferConstants.MaxBytesPer100ms);

				byte[] buffer = new byte[bytesToSend];
				Array.Copy(m_DataBytes, m_DataOffset, buffer, 0, bytesToSend);
				NetworkEventSender.SendNetworkEvent_LargeDataTransfer_SendBytes(m_TransferType, m_Identifier, buffer);

				m_DataOffset += bytesToSend;

				m_CallbackOnTransferProgress?.Invoke(m_TransferType, m_Identifier, m_DataOffset, m_DataBytes.Length, m_DataBytes.Length - m_DataOffset);

				if (m_DataOffset == m_DataBytes.Length)
				{
					TransferState = ELargeDataTransferState.DoneWaitingAck;
				}
			}

			TransferState = ELargeDataTransferState.InProgress_PendingAck;
		}
	}
}

public static class LargeDataTransferManager
{
	private static List<LargeDataTransfer> m_lstPendingTransfers_Outgoing = new List<LargeDataTransfer>();

	private static List<LargeDataTransfer> m_lstPendingTransfers_Incoming = new List<LargeDataTransfer>();

	private static Dictionary<ELargeDataTransferType, Action<LargeDataTransfer>> m_dictIncomingTransferStartedCallbacks = new Dictionary<ELargeDataTransferType, Action<LargeDataTransfer>>();
	private static Dictionary<ELargeDataTransferType, Action<LargeDataTransfer>> m_dictIncomingTransferProgressCallbacks = new Dictionary<ELargeDataTransferType, Action<LargeDataTransfer>>();
	private static Dictionary<ELargeDataTransferType, Action<LargeDataTransfer, bool>> m_dictIncomingTransferCompleteCallbacks = new Dictionary<ELargeDataTransferType, Action<LargeDataTransfer, bool>>();

	public static void Initialize()
	{
		// react to the storage data being loaded, we cant access this in constructors because it hasn't loaded yet
		NetworkEvents.RageClientStorageLoaded += () =>
		{
#if DISABLE_LFT_CACHE
			RageClientStorageManager.Container.LFTCache = null;
			RageClientStorageManager.Flush();
#else
			// if null, create
			if (RageClientStorageManager.Container.LFTCache == null)
			{
				RageClientStorageManager.Container.LFTCache = new Dictionary<string, string>();
			}
#endif
		};

		ClientTimerPool.CreateTimer(OnTransferTick, 100, -1);

		NetworkEvents.LargeDataTransfer_ServerToClient_Begin += RegisterIncomingTransfer;
		NetworkEvents.LargeDataTransfer_SendBytes += OnIncomingData;
		NetworkEvents.LargeDataTransfer_ClientToServer_ServerAck += OnServerAckTransfer;

		NetworkEvents.LargeDataTransfer_ClientToServer_RequestResend += OnRequestResend;
		NetworkEvents.LargeDataTransfer_ClientToServer_AckFinalTransfer += OnAckFinalTransfer;
		NetworkEvents.LargeDataTransfer_ClientToServer_AckBlock += OnAckTransferBlock;

#if TEST_LARGE_DATA_TRANSFER
		LargeDataTransferManager.RegisterIncomingTransferCallbacks(ELargeDataTransferType.Test, TransferStarted, TransferProgress, TransferComplete);

		KeyBinds.Bind(ConsoleKey.Insert, () =>
		{
			ChatHelper.DebugMessage("Queueing");

			string strData = "Hello World!";
			byte[] data = System.Text.Encoding.ASCII.GetBytes(strData);
			LargeDataTransferManager.QueueOutgoingTransfer(ELargeDataTransferType.Test, 123, data, OnProgress, OnComplete);

		}, EKeyBindType.Released, EKeyBindFlag.Default);
#endif
	}

#if TEST_LARGE_DATA_TRANSFER
	private static void OnProgress(ELargeDataTransferType type, int identifier, int numBytesReceivedTotal, int totalSize, int bytesRemaining)
	{
		ChatHelper.DebugMessage("PROGRESS [{0} - {1}] Bytes: {2} TotalSize: {3} Remaining: {4}", type, identifier, numBytesReceivedTotal, totalSize, bytesRemaining);
	}

	private static void OnComplete(ELargeDataTransferType type, int identifier)
	{
		ChatHelper.DebugMessage("COMPLETED [{0} - {1}]", type, identifier);
	}

	private static void TransferStarted(LargeDataTransfer transfer)
	{
		ChatHelper.DebugMessage("Incoming request started [{0} - {1}] ExpectedSize: {2}", transfer.m_TransferType, transfer.m_Identifier, transfer.GetDataLength());
	}

	private static void TransferProgress(LargeDataTransfer transfer)
	{
		ChatHelper.DebugMessage("Request progress [{0} - {1}] {2}/{3}", transfer.m_TransferType, transfer.m_Identifier, transfer.GetDataOffset(), transfer.GetDataLength());
	}

	private static void TransferComplete(LargeDataTransfer transfer, bool bSuccess)
	{
		ChatHelper.DebugMessage("Request complete [{0} - {1}] Success: {2}", transfer.m_TransferType, transfer.m_Identifier, bSuccess);
		ChatHelper.DebugMessage("Data was: {0}", System.Text.Encoding.ASCII.GetString(transfer.GetBytes()));
	}
#endif

	private static void OnRequestResend(ELargeDataTransferType a_TransferType, int a_Identifier)
	{
		LargeDataTransfer transfer = null;

		// find the transfer
		foreach (LargeDataTransfer dataTransfer in m_lstPendingTransfers_Outgoing)
		{
			if (dataTransfer.m_TransferType == a_TransferType && dataTransfer.m_Identifier == a_Identifier)
			{
				transfer = dataTransfer;
				break;
			}
		}

		if (transfer != null)
		{
			// reset pointers etc
			transfer.ResetForReTransfer();
		}
	}

	private static void OnAckTransferBlock(ELargeDataTransferType a_TransferType, int a_Identifier)
	{
		LargeDataTransfer transfer = null;

		// find the transfer
		foreach (LargeDataTransfer dataTransfer in m_lstPendingTransfers_Outgoing)
		{
			if (dataTransfer.m_TransferType == a_TransferType && dataTransfer.m_Identifier == a_Identifier)
			{
				transfer = dataTransfer;
				break;
			}
		}

		if (transfer != null)
		{
			// set to block acked / ready for more
			transfer.TransferState = ELargeDataTransferState.InProgress_Ready;
		}
	}

	private static void OnAckFinalTransfer(ELargeDataTransferType a_TransferType, int a_Identifier)
	{
		LargeDataTransfer transfer = null;

		// find the transfer
		foreach (LargeDataTransfer dataTransfer in m_lstPendingTransfers_Outgoing)
		{
			if (dataTransfer.m_TransferType == a_TransferType && dataTransfer.m_Identifier == a_Identifier)
			{
				transfer = dataTransfer;
				break;
			}
		}

		if (transfer != null)
		{
			// set to acked
			transfer.TransferState = ELargeDataTransferState.DoneAndAcked;
		}
	}

	public static bool QueueOutgoingTransfer(ELargeDataTransferType a_Type, int a_Identifier, byte[] a_DataBytes, Action<ELargeDataTransferType, int, int, int, int> a_CallbackOnTransferProgress, Action<ELargeDataTransferType, int> a_CallbackOnTransferComplete, byte[] key)
	{
		// check for dupes
		foreach (LargeDataTransfer existingTransfer in m_lstPendingTransfers_Outgoing)
		{
			if (existingTransfer.m_TransferType == a_Type && existingTransfer.m_Identifier == a_Identifier)
			{
				return false;
			}
		}

		m_lstPendingTransfers_Outgoing.Add(new LargeDataTransfer(a_Type, a_Identifier, a_DataBytes, a_CallbackOnTransferProgress, a_CallbackOnTransferComplete, key));
		return true;

	}

	public static void RegisterIncomingTransferCallbacks(ELargeDataTransferType a_TransferType, Action<LargeDataTransfer> a_StartedCallback, Action<LargeDataTransfer> a_ProgressCallback, Action<LargeDataTransfer, bool> a_CompletionCallback)
	{
		if (a_StartedCallback != null)
		{
			m_dictIncomingTransferStartedCallbacks[a_TransferType] = a_StartedCallback;
		}

		if (a_ProgressCallback != null)
		{
			m_dictIncomingTransferProgressCallbacks[a_TransferType] = a_ProgressCallback;
		}

		if (a_CompletionCallback != null)
		{
			m_dictIncomingTransferCompleteCallbacks[a_TransferType] = a_CompletionCallback;
		}
	}

	private static void RegisterIncomingTransfer(ELargeDataTransferType a_TransferType, int a_Identifier, int totalBytes, int crc32, bool bAllowClientsideCaching, byte[] key)
	{
#if DISABLE_LFT_CACHE
		bAllowClientsideCaching = false;
#endif

		LargeDataTransfer newIncomingTransfer = new LargeDataTransfer(a_TransferType, a_Identifier, totalBytes, crc32, bAllowClientsideCaching, key);
		m_lstPendingTransfers_Incoming.Add(newIncomingTransfer);

		// trigger start, even if cached data, so client follows normal flow
		if (m_dictIncomingTransferStartedCallbacks.ContainsKey(a_TransferType))
		{
			m_dictIncomingTransferStartedCallbacks[a_TransferType]?.Invoke(newIncomingTransfer);
		}

		if (bAllowClientsideCaching)
		{
			bool bCacheOutOfDate = true;

			// check cache
			try
			{
				string strCacheKey = GenerateCacheKey(a_TransferType, a_Identifier);
				if (RageClientStorageManager.Container.LFTCache.ContainsKey(strCacheKey))
				{
					// is it in sync?
					string strCachedData = RageClientStorageManager.Container.LFTCache[strCacheKey];

					byte[] encryptedData = Convert.FromBase64String(strCachedData);

					// does the len match?
					if (totalBytes == encryptedData.Length)
					{
						// decompress data first

						encryptedData = RAGE.Util.MsgPack.Deserialize<byte[]>(encryptedData, MessagePack.Resolvers.TypelessContractlessStandardResolver.Options.WithCompression(MapTransferConstants.CompressionAlgorithm));

						// can we decrypt it?
						if (EncryptionHelper.DecryptBytesToBytes(encryptedData, key, out byte[] decryptedData))
						{
							// do the crcs match?
							if (CRC32.ComputeHash(decryptedData) == crc32)
							{
								// just fake a completion now, we have the data
								bCacheOutOfDate = false;

								m_lstPendingTransfers_Incoming.Remove(newIncomingTransfer);

								newIncomingTransfer.RestoreFromCache(encryptedData);
								// trigger complete callback
								if (m_dictIncomingTransferCompleteCallbacks.ContainsKey(a_TransferType))
								{
									m_dictIncomingTransferCompleteCallbacks[a_TransferType]?.Invoke(newIncomingTransfer, true);
								}
							}
						}
					}
				}
			}
			catch
			{
				bCacheOutOfDate = true;
			}

			NetworkEventSender.SendNetworkEvent_LargeDataTransfer_ServerToClient_ClientAck(a_TransferType, a_Identifier, bCacheOutOfDate);
		}
		else // just ack saying we need the latest
		{
			NetworkEventSender.SendNetworkEvent_LargeDataTransfer_ServerToClient_ClientAck(a_TransferType, a_Identifier, true);
		}
	}

	private static void OnServerAckTransfer(ELargeDataTransferType a_TransferType, int a_Identifier)
	{
		LargeDataTransfer transfer = null;

		// find the transfer
		foreach (LargeDataTransfer dataTransfer in m_lstPendingTransfers_Outgoing)
		{
			if (dataTransfer.m_TransferType == a_TransferType && dataTransfer.m_Identifier == a_Identifier)
			{
				transfer = dataTransfer;
				break;
			}
		}

		if (transfer != null)
		{
			transfer.TransferState = ELargeDataTransferState.InProgress_Ready;
		}
	}

	public static string GenerateCacheKey(ELargeDataTransferType a_TransferType, int a_Identifier)
	{
		// -1 stores just type, e.g. for ones where we don't care about identifiers
		return RAGE.Util.XxHash64.Hash(a_Identifier == -1 ? a_TransferType.ToString() : Helpers.FormatString("{0}_{1}", a_TransferType, a_Identifier)).ToString();
	}

	private static void OnIncomingData(ELargeDataTransferType a_TransferType, int a_Identifier, byte[] data)
	{
		// ack it so the server can send more
		NetworkEventSender.SendNetworkEvent_LargeDataTransfer_ServerToClient_AckBlock(a_TransferType, a_Identifier);

		List<LargeDataTransfer> lstIncomingTransfersDone = new List<LargeDataTransfer>();

		// find the transfer
		foreach (LargeDataTransfer dataTransfer in m_lstPendingTransfers_Incoming)
		{
			if (dataTransfer.m_TransferType == a_TransferType && dataTransfer.m_Identifier == a_Identifier)
			{
				bool bTransferComplete = dataTransfer.ReceiveTransferFrame(data, out bool bSuccess);

				// trigger progress callback
				if (m_dictIncomingTransferProgressCallbacks.ContainsKey(a_TransferType))
				{
					m_dictIncomingTransferProgressCallbacks[a_TransferType]?.Invoke(dataTransfer);
				}

				if (bTransferComplete)
				{
					if (bSuccess)
					{
						// trigger complete callback
						if (m_dictIncomingTransferCompleteCallbacks.ContainsKey(a_TransferType))
						{
							m_dictIncomingTransferCompleteCallbacks[a_TransferType]?.Invoke(dataTransfer, bSuccess);
						}
					}

					// queue for removal
					lstIncomingTransfersDone.Add(dataTransfer);

					// ack it fully
					NetworkEventSender.SendNetworkEvent_LargeDataTransfer_ServerToClient_AckFinalTransfer(dataTransfer.m_TransferType, dataTransfer.m_Identifier);
				}

				break;
			}
		}

		foreach (LargeDataTransfer completedIncomingTransfer in lstIncomingTransfersDone)
		{
			m_lstPendingTransfers_Incoming.Remove(completedIncomingTransfer);
		}
	}

	private static void OnTransferTick(object[] parameters)
	{
		List<LargeDataTransfer> lstTransfersDone = new List<LargeDataTransfer>();
		foreach (LargeDataTransfer transfer in m_lstPendingTransfers_Outgoing)
		{
			transfer.SendTransferFrame();

			if (transfer.TransferState == ELargeDataTransferState.DoneAndAcked)
			{
				lstTransfersDone.Add(transfer);
			}
		}

		foreach (LargeDataTransfer completedTransfer in lstTransfersDone)
		{
			m_lstPendingTransfers_Outgoing.Remove(completedTransfer);

			// trigger callback - we do this here instead of inside so the collection isnt modified if the callback adds more data, also caused QueueOutgoingTransfer to fail dupe checks if same time (since it is not yet removed)
			completedTransfer.TriggerCompletion();
		}
	}
}
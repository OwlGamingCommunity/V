//#define TEST_LARGE_DATA_TRANSFER

using System;
using System.Collections.Generic;

public class LargeDataTransferManager_ServerInit : GTANetworkAPI.Script
{
	public LargeDataTransferManager_ServerInit()
	{
		LargeDataTransferManager.Initialize();
	}
}

public class LargeDataTransfer
{
	public ELargeDataTransferType m_TransferType { get; set; }
	public int m_Identifier { get; set; }
	private int m_DataOffset = 0;
	private byte[] m_DataBytes; // NOTE: ON receive, this is encrypted, use GetBytes()
	public WeakReference<CPlayer> m_PlayerInstance { get; set; } = new WeakReference<CPlayer>(null);

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

	public byte[] GetKey()
	{
		return m_key;
	}

	public void TriggerCompletion()
	{
		m_CallbackOnTransferComplete?.Invoke(m_TransferType, m_Identifier);
	}

	public LargeDataTransfer(CPlayer a_AssociatedPlayer, ELargeDataTransferType a_TransferType, int a_Identifier, byte[] a_DataBytes,
		Action<ELargeDataTransferType, int, int, int, int> a_CallbackOnTransferProgress, Action<ELargeDataTransferType, int> a_CallbackOnTransferComplete, bool a_bAllowClientsideCaching, byte[] key)
	{
		m_PlayerInstance.SetTarget(a_AssociatedPlayer);
		m_TransferType = a_TransferType;
		m_Identifier = a_Identifier;
		m_DataOffset = 0;
		m_crc32 = CRC32.ComputeHash(a_DataBytes); // CRC is pre-compression & pre-encryption
		m_bAllowClientsideCaching = a_bAllowClientsideCaching;

		// encrypt data first
		a_DataBytes = EncryptionHelper.EncryptBytesToBytes(a_DataBytes, key);
		m_DataBytes = MessagePack.MessagePackSerializer.Serialize(a_DataBytes, MessagePack.Resolvers.ContractlessStandardResolver.Options.WithCompression(MapTransferConstants.CompressionAlgorithm));

		m_CallbackOnTransferProgress = a_CallbackOnTransferProgress;
		m_CallbackOnTransferComplete = a_CallbackOnTransferComplete;

		// inform event
		m_bIsIncoming = false;

		m_key = key;
		InformClient();
	}

	private void InformClient()
	{
		// serves two purposes - we have to ensure the client knows about the transfer BEFORE we start sending data, and we need to check cache state, if required
		NetworkEventSender.SendNetworkEvent_LargeDataTransfer_ServerToClient_Begin(m_PlayerInstance.Instance(), m_TransferType, m_Identifier, m_DataBytes.Length, m_crc32, m_bAllowClientsideCaching, m_key); // send compressed length
		TransferState = ELargeDataTransferState.PendingAck;
	}

	public void ResetForReTransfer()
	{
		m_DataOffset = 0;
		InformClient();
	}

	public LargeDataTransfer(CPlayer a_AssociatedPlayer, ELargeDataTransferType a_TransferType, int a_Identifier, int totalNumBytes, int crc32, byte[] key)
	{
		m_PlayerInstance.SetTarget(a_AssociatedPlayer);
		m_TransferType = a_TransferType;
		m_Identifier = a_Identifier;
		m_DataOffset = 0;
		m_DataBytes = new byte[totalNumBytes];
		m_CallbackOnTransferProgress = null;
		m_CallbackOnTransferComplete = null;

		m_crc32 = crc32;
		m_bIsIncoming = false;
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
					m_DataBytes = MessagePack.MessagePackSerializer.Deserialize<byte[]>(m_DataBytes, MessagePack.Resolvers.TypelessContractlessStandardResolver.Options.WithCompression(MapTransferConstants.CompressionAlgorithm));
				}
				catch
				{
					NetworkEventSender.SendNetworkEvent_LargeDataTransfer_ClientToServer_RequestResend(m_PlayerInstance.Instance(), m_TransferType, m_Identifier);
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

	// TODO_TRANSFER: Handle player null for sends (e.g. player quits mid transfer, remove in this case)
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
				NetworkEventSender.SendNetworkEvent_LargeDataTransfer_SendBytes(m_PlayerInstance.Instance(), m_TransferType, m_Identifier, buffer);

				m_DataOffset += bytesToSend;

				m_CallbackOnTransferProgress?.Invoke(m_TransferType, m_Identifier, m_DataOffset, m_DataBytes.Length, m_DataBytes.Length - m_DataOffset);

				if (m_DataOffset == m_DataBytes.Length)
				{
					TransferState = ELargeDataTransferState.DoneWaitingAck;
				}

				TransferState = ELargeDataTransferState.InProgress_PendingAck;
			}
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
		MainThreadTimerPool.CreateGlobalTimer(OnTransferTick, 100, -1);

		NetworkEvents.LargeDataTransfer_ClientToServer_Begin += RegisterIncomingTransfer;
		NetworkEvents.LargeDataTransfer_SendBytes += OnIncomingData;

		NetworkEvents.LargeDataTransfer_ServerToClient_ClientAck += OnClientAckTransfer;

		NetworkEvents.LargeDataTransfer_ServerToClient_RequestResend += OnRequestResend;
		NetworkEvents.LargeDataTransfer_ServerToClient_AckFinalTransfer += OnAckFinalTransfer;
		NetworkEvents.LargeDataTransfer_ServerToClient_AckBlock += OnAckTransferBlock;

#if TEST_LARGE_DATA_TRANSFER
		LargeDataTransferManager.RegisterIncomingTransferCallbacks(ELargeDataTransferType.Test, TransferStarted, TransferProgress, TransferComplete);

		// test cmd
		CommandManager.RegisterCommand("test", "test", new Action<CPlayer, CVehicle>(TestCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
#endif
	}

#if TEST_LARGE_DATA_TRANSFER
	private static void TestCommand(CPlayer player, CVehicle vehicle)
	{
		Console.WriteLine("Queueing");

		string strData = "Hello From Server!";
		byte[] data = System.Text.Encoding.ASCII.GetBytes(strData);
		LargeDataTransferManager.QueueOutgoingTransfer(player, ELargeDataTransferType.Test, 123, data, OnProgress, OnComplete);
	}

	private static void OnProgress(ELargeDataTransferType type, int identifier, int numBytesReceivedTotal, int totalSize, int bytesRemaining)
	{
		Console.WriteLine("PROGRESS [{0} - {1}] Bytes: {2} TotalSize: {3} Remaining: {4}", type, identifier, numBytesReceivedTotal, totalSize, bytesRemaining);
	}

	private static void OnComplete(ELargeDataTransferType type, int identifier)
	{
		Console.WriteLine("COMPLETED [{0} - {1}]", type, identifier);
	}

	private static void TransferStarted(LargeDataTransfer transfer)
	{
		Console.WriteLine("Incoming request started [{0} - {1}] ExpectedSize: {2}", transfer.m_TransferType, transfer.m_Identifier, transfer.GetDataLength());
	}

	private static void TransferProgress(LargeDataTransfer transfer)
	{
		Console.WriteLine("Request progress [{0} - {1}] {2}/{3}", transfer.m_TransferType, transfer.m_Identifier, transfer.GetDataOffset(), transfer.GetDataLength());
	}

	private static void TransferComplete(LargeDataTransfer transfer, bool bSuccess)
	{
		Console.WriteLine("Request complete [{0} - {1}] Success: {2}", transfer.m_TransferType, transfer.m_Identifier, bSuccess);
		Console.WriteLine("Data was: {0}", System.Text.Encoding.ASCII.GetString(transfer.GetBytes()));
	}
#endif

	private static void OnRequestResend(CPlayer a_Player, ELargeDataTransferType a_TransferType, int a_Identifier)
	{
		LargeDataTransfer transfer = null;

		// find the transfer
		foreach (LargeDataTransfer dataTransfer in m_lstPendingTransfers_Outgoing)
		{
			if (dataTransfer.m_TransferType == a_TransferType && dataTransfer.m_Identifier == a_Identifier && dataTransfer.m_PlayerInstance.Instance() == a_Player)
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

	private static void OnAckFinalTransfer(CPlayer a_PlayerAcking, ELargeDataTransferType a_TransferType, int a_Identifier)
	{
		LargeDataTransfer transfer = null;

		// find the transfer
		foreach (LargeDataTransfer dataTransfer in m_lstPendingTransfers_Outgoing)
		{
			if (dataTransfer.m_TransferType == a_TransferType && dataTransfer.m_Identifier == a_Identifier && dataTransfer.m_PlayerInstance.Instance() == a_PlayerAcking)
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

	private static void OnAckTransferBlock(CPlayer a_PlayerAcking, ELargeDataTransferType a_TransferType, int a_Identifier)
	{
		LargeDataTransfer transfer = null;

		// find the transfer
		foreach (LargeDataTransfer dataTransfer in m_lstPendingTransfers_Outgoing)
		{
			if (dataTransfer.m_TransferType == a_TransferType && dataTransfer.m_Identifier == a_Identifier && dataTransfer.m_PlayerInstance.Instance() == a_PlayerAcking)
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

	private static void OnClientAckTransfer(CPlayer a_PlayerAcking, ELargeDataTransferType a_TransferType, int a_Identifier, bool bNeedsTransfer)
	{
		LargeDataTransfer transfer = null;

		// find the transfer
		foreach (LargeDataTransfer dataTransfer in m_lstPendingTransfers_Outgoing)
		{
			if (dataTransfer.m_TransferType == a_TransferType && dataTransfer.m_Identifier == a_Identifier && dataTransfer.m_PlayerInstance.Instance() == a_PlayerAcking)
			{
				transfer = dataTransfer;
				break;
			}
		}

		if (transfer != null)
		{
			// did we need the data? This could mean either it doesnt support caching, or the cache is out of sync
			if (bNeedsTransfer)
			{
				// set it as in progress
				transfer.TransferState = ELargeDataTransferState.InProgress_Ready;
			}
			else // trigger completion, so the script continues as if we gave them the data
			{
				transfer.TriggerCompletion();

				// also remove
				m_lstPendingTransfers_Outgoing.Remove(transfer);
			}
		}
	}

	public static bool QueueOutgoingTransfer(CPlayer a_Target, ELargeDataTransferType a_Type, int a_Identifier, byte[] a_DataBytes, Action<ELargeDataTransferType, int, int, int, int> a_CallbackOnTransferProgress, Action<ELargeDataTransferType, int> a_CallbackOnTransferComplete, bool a_bAllowClientsideCaching, byte[] key)
	{
		// TODO_TRANSFER: Support for cancel / overwrite?
		// check for dupes
		foreach (LargeDataTransfer existingTransfer in m_lstPendingTransfers_Outgoing)
		{
			if (existingTransfer.m_PlayerInstance.Instance() == a_Target && existingTransfer.m_TransferType == a_Type && existingTransfer.m_Identifier == a_Identifier)
			{
				return false;
			}
		}

		m_lstPendingTransfers_Outgoing.Add(new LargeDataTransfer(a_Target, a_Type, a_Identifier, a_DataBytes, a_CallbackOnTransferProgress, a_CallbackOnTransferComplete, a_bAllowClientsideCaching, key));

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

	private static void RegisterIncomingTransfer(CPlayer a_Sender, ELargeDataTransferType a_TransferType, int a_Identifier, int totalBytes, int crc32, byte[] key)
	{
		LargeDataTransfer newIncomingTransfer = new LargeDataTransfer(a_Sender, a_TransferType, a_Identifier, totalBytes, crc32, key);
		m_lstPendingTransfers_Incoming.Add(newIncomingTransfer);

		if (m_dictIncomingTransferStartedCallbacks.ContainsKey(a_TransferType))
		{
			m_dictIncomingTransferStartedCallbacks[a_TransferType]?.Invoke(newIncomingTransfer);
		}

		NetworkEventSender.SendNetworkEvent_LargeDataTransfer_ClientToServer_ServerAck(a_Sender, a_TransferType, a_Identifier);
	}

	private static void OnIncomingData(CPlayer a_Sender, ELargeDataTransferType a_TransferType, int a_Identifier, byte[] data)
	{
		// ack it so the server can send more
		NetworkEventSender.SendNetworkEvent_LargeDataTransfer_ClientToServer_AckBlock(a_Sender, a_TransferType, a_Identifier);

		List<LargeDataTransfer> lstIncomingTransfersDone = new List<LargeDataTransfer>();

		// find the transfer
		foreach (LargeDataTransfer dataTransfer in m_lstPendingTransfers_Incoming)
		{
			if (dataTransfer.m_TransferType == a_TransferType && dataTransfer.m_Identifier == a_Identifier && dataTransfer.m_PlayerInstance.Instance() == a_Sender)
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
					NetworkEventSender.SendNetworkEvent_LargeDataTransfer_ClientToServer_AckFinalTransfer(a_Sender, dataTransfer.m_TransferType, dataTransfer.m_Identifier);
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
using System;
using System.Collections.Generic;

public static class ExceptionHelper
{
	private static List<ExceptionCooldown> m_lstExceptionCooldowns = new List<ExceptionCooldown>();

	static ExceptionHelper()
	{

	}

	private static bool IsClientsideExceptionOnCooldown(string strMessage)
	{
		// Remove any expired
		List<ExceptionCooldown> lstToRemove = new List<ExceptionCooldown>();
		foreach (var cooldownInst in m_lstExceptionCooldowns)
		{
			if (cooldownInst.Expired())
			{
				lstToRemove.Add(cooldownInst);
			}
		}

		foreach (var cooldownToRemove in lstToRemove)
		{
			m_lstExceptionCooldowns.Remove(cooldownToRemove);
		}

		bool bCooldown = false;
		foreach (var cooldownInst in m_lstExceptionCooldowns)
		{
			if (cooldownInst.Message == strMessage)
			{
				bCooldown = true;
			}
		}

		return bCooldown;
	}

	private static void AddClientsideExceptionCooldown(string strMessage)
	{
		m_lstExceptionCooldowns.Add(new ExceptionCooldown(strMessage));
	}

	class ExceptionCooldown
	{
		public ExceptionCooldown(string strMessage)
		{
			timeStarted = DateTime.Now;
			Message = strMessage;
		}

		public string Message { get; }

		public bool Expired()
		{
			double timeSinceStarted = (DateTime.Now - timeStarted).TotalMilliseconds;
			return timeSinceStarted >= ExceptionConstants.CooldownPeriod;
		}

		DateTime timeStarted;
	}

	public static void SendException(Exception ex)
	{
		// Use inner exception if available, outer is irrelevant if inner exists
		if (ex.InnerException != null)
		{
			ex = ex.InnerException;
		}

		if (!IsClientsideExceptionOnCooldown(ex.Message))
		{
			AddClientsideExceptionCooldown(ex.Message);
			ClientsideException exceptionObject = new ClientsideException(ex.Message, ex.StackTrace);
			NetworkEventSender.SendNetworkEvent_ClientsideException(exceptionObject);
		}
	}
}
using Elasticsearch.Net;
using Nest;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logging
{
	public class ElasticSearch : IDisposable
	{
		// TODO_GITHUB: You should set the variables below
		private const string ELASTICSEARCH_INDEX = "TODO_GITHUB";
		private readonly string URI = Environment.GetEnvironmentVariable("ELASTICSEARCH_HOSTS");

#pragma warning disable CA1823
		// TODO_GITHUB: You should set the environment variables below
		private readonly string ELASTICSEARCH_USERNAME = Environment.GetEnvironmentVariable("ELASTICSEARCH_USERNAME");
		private readonly string ELASTICSEARCH_PASSWORD = Environment.GetEnvironmentVariable("ELASTICSEARCH_PASSWORD");
#pragma warning restore CA1823

		private StaticConnectionPool m_ConnectionPool;
		private ConnectionSettings m_ElasticSettings;
		public ElasticClient m_ElasticClient { get; }

		public ElasticSearch()
		{
			string[] URIs = URI != null ? URI.Split(' ') : Array.Empty<string>();
			List<Uri> nodes = URIs.Select(URI => new Uri(URI, UriKind.Absolute)).ToList();

			if (nodes.Count > 0)
			{
				m_ConnectionPool = new StaticConnectionPool(nodes);
#if DEBUG
				m_ElasticSettings = new ConnectionSettings(m_ConnectionPool).DefaultIndex(ELASTICSEARCH_INDEX);
#else
                m_ElasticSettings = new ConnectionSettings(m_ConnectionPool).DefaultIndex(ELASTICSEARCH_INDEX).BasicAuthentication(ELASTICSEARCH_USERNAME, ELASTICSEARCH_PASSWORD).ServerCertificateValidationCallback(CertificateValidations.AllowAll);
#endif
				m_ElasticClient = new ElasticClient(m_ElasticSettings);
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("[ELASTICSEARCH] Elasticsearch not configured.");
				Console.ResetColor();
			}
		}

		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool a_CleanupNativeAndManaged)
		{
		}
	}
	public static class ESFunctions
	{
		private static ElasticSearch m_ElasticInst = new ElasticSearch();
		private const int QUERY_SIZE = 50;
		public static void GetVehicleActionsById(long vehicleID, Action<List<CVehicleAction>> onCompletion)
		{
			if (m_ElasticInst?.m_ElasticClient == null)
			{
				onCompletion.Invoke(new List<CVehicleAction>());
			}
			else
			{
				try
				{
					m_ElasticInst.m_ElasticClient.SearchAsync<Log>(s => s.Query(q => q.Term(p => p.action, ELogType.VehicleRelated) && q.Term(t => t.Field(p => p.vehicles).Value(vehicleID))).Size(QUERY_SIZE)).ContinueOnMainThread((logResult) =>
					{
						List<CVehicleAction> vehicleActions = new List<CVehicleAction>();

						if (logResult != null && logResult.ApiCall.Success && logResult.Hits.Count > 0)
						{
							foreach (var hit in logResult.Hits)
							{
								DateTimeOffset date = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(hit.Source.date));

								Database.Functions.Characters.GetCharacterNameFromDBID(hit.Source.origin, (string strCharacterName) =>
								{
									vehicleActions.Add(new CVehicleAction(date.ToString(), vehicleID, hit.Source.content, strCharacterName));

									// are we done? call callback
									if (vehicleActions.Count == logResult.Hits.Count)
									{
										onCompletion.Invoke(vehicleActions);
									}
								});
							}
						}
						else
						{
							onCompletion.Invoke(new List<CVehicleAction>());
						}
					});
				}
				catch (Exception ex)
				{
					SentryEvent logEvent = new SentryEvent(ex);
					SentrySdk.CaptureEvent(logEvent);
				}
			}
		}

		public static void GetPropertyActionsById(long propertyID, Action<List<CPropertyAction>> onCompletion)
		{
			if (m_ElasticInst?.m_ElasticClient == null)
			{
				onCompletion.Invoke(new List<CPropertyAction>());
			}
			else
			{
				try
				{
					m_ElasticInst.m_ElasticClient.SearchAsync<Log>(s => s.Query(q => q.Term(p => p.action, ELogType.PropertyRelated) && q.Term(t => t.Field(p => p.properties).Value(propertyID))).Size(QUERY_SIZE)).ContinueOnMainThread((logResult) =>
					{
						List<CPropertyAction> propertyActions = new List<CPropertyAction>();

						foreach (var hit in logResult.Hits)
						{
							DateTimeOffset date = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(hit.Source.date));

							Database.Functions.Characters.GetCharacterNameFromDBID(hit.Source.origin, (string strCharacterName) =>
							{
								propertyActions.Add(new CPropertyAction(date.ToString(), propertyID, hit.Source.content, strCharacterName));

								// are we done? call callback
								if (propertyActions.Count == logResult.Hits.Count)
								{
									onCompletion.Invoke(propertyActions);
								}
							});
						}

						onCompletion.Invoke(propertyActions);
					});
				}
				catch (Exception ex)
				{
					SentryEvent logEvent = new SentryEvent(ex);
					SentrySdk.CaptureEvent(logEvent);
				}
			}
		}
	}
}

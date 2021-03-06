﻿using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;

namespace BPUtil.SimpleHttp
{
	public class ServerNameCertificateSelector : ICertificateSelector
	{
		/// <summary>
		/// Dictionary which maps lower-case domain names to certificates. To specify a default certificate, use <see cref="string.Empty"/> as the key.
		/// </summary>
		public ConcurrentDictionary<string, X509Certificate> allCertsByDomain = new ConcurrentDictionary<string, X509Certificate>();

		/// <summary>
		/// Returns an X509Certificate appropriate for the specified serverName, or null.
		/// </summary>
		/// <param name="serverName">The server name as indicated by ServerNameIndication. May be null or empty.</param>
		/// <returns>an X509Certificate or null</returns>
		public X509Certificate GetCertificate(string serverName)
		{
			if (serverName == null)
				serverName = "";
			else
				serverName = serverName.ToLower();

			X509Certificate cert;
			if (allCertsByDomain.TryGetValue(serverName, out cert))
				return cert;
			else if (allCertsByDomain.TryGetValue("", out cert)) // Fallback to default
				return cert;
			else
				return null; // Fallback to null
		}

		/// <summary>
		/// Sets the certificate for the specified server name.
		/// </summary>
		/// <param name="serverName">The domain name of the server.  The lower-case form of this will be used as the dictionary key.  To specify a default certificate, use <see cref="string.Empty"/> as the serverName.  Null is treated as empty string.</param>
		/// <param name="cert">The certificate.</param>
		public void SetCertificate(string serverName, X509Certificate cert)
		{
			if (serverName == null)
				serverName = "";
			else
				serverName = serverName.ToLower();

			allCertsByDomain[serverName] = cert;
		}
	}
}
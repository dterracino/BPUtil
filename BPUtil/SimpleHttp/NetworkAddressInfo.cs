﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace BPUtil.SimpleHttp
{
	public class NetworkAddressInfo
	{
		/// <summary>
		/// A list of IPv4 addresses belonging to this server.  Each item in this list has a corresponding item in the `localIPv4Masks` list at the same index.
		/// </summary>
		public List<byte[]> localIPv4Addresses = new List<byte[]>();
		/// <summary>
		/// A list of IPv4 subnet masks belonging to this server.  Each item in this list has a corresponding item in the `localIPv4Addresses` list at the same index.
		/// </summary>
		public List<byte[]> localIPv4Masks = new List<byte[]>();
		public NetworkAddressInfo()
		{
		}

		public NetworkAddressInfo(NetworkInterface[] networkInterfaces)
		{
			foreach (NetworkInterface adapter in networkInterfaces)
			{
				IPInterfaceProperties ipProp = adapter.GetIPProperties();
				if (ipProp == null)
					continue;
				foreach (UnicastIPAddressInformation addressInfo in ipProp.UnicastAddresses)
				{
					if (addressInfo.Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
						continue;
					AddAddress(addressInfo.Address.GetAddressBytes(), addressInfo.IPv4Mask.GetAddressBytes());
				}
			}
		}

		private void AddAddress(byte[] address, byte[] mask)
		{
			if (address.Length != 4 || mask.Length != 4)
				return;
			localIPv4Addresses.Add(address);
			localIPv4Masks.Add(mask);
		}
		
		/// <summary>
		/// Returns true if the specified address is the same as any of this server's addresses.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public bool IsSameMachine(IPAddress address)
		{
			return IsSameMachine(address.GetAddressBytes());
		}
		/// <summary>
		/// Returns true if the specified address is the same as any of this server's addresses.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public bool IsSameMachine(byte[] address)
		{
			if (address.Length != 4)
				return false;
			foreach (byte[] localAddress in localIPv4Addresses)
				if (ByteUtil.ByteArraysMatch(address, localAddress))
					return true;
			return false;
		}

		/// <summary>
		/// Returns true if the specified address is in the same subnet as any of this server's addresses.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public bool IsSameLAN(IPAddress address)
		{
			return IsSameLAN(address.GetAddressBytes());
		}
		/// <summary>
		/// Returns true if the specified address is in the same subnet as any of this server's addresses.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public bool IsSameLAN(byte[] address)
		{
			if (address.Length != 4)
				return false;
			for (int i = 0; i < localIPv4Addresses.Count; i++)
			{
				byte[] localAddress = localIPv4Addresses[i];
				byte[] mask = localIPv4Masks[i];
				if (ByteUtil.CompareWithMask(localAddress, address, mask))
					return true;
			}
			return false;
		}
	}
}
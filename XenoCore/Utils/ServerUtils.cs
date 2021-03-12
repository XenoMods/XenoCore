using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace XenoCore.Utils {
	public static class ServerUtils {
		private static readonly List<LocalGameServer> LocalGameServers = new List<LocalGameServer>();

		public static void AddLocalServer(string Name, string Ip) {
			LocalGameServers.Add(LocalGameServer.Static(Ip, Name));
		}
		
		public static void AddRegion(string Name, string Ip, ushort Port = 22023) {
			var Regions = ServerManager.DefaultRegions.ToList();

			Regions.Insert(0, new RegionInfo(
				Name, Ip, new[] {
					new ServerInfo(Name, Ip, Port)
				})
			);

			ServerManager.DefaultRegions = Regions.ToArray();
		}
		
		[HarmonyPatch(typeof(GameDiscovery), nameof(GameDiscovery.Start))]
		private static class LocalGameDiscoveryPatch {
			public static void Postfix(GameDiscovery __instance) {
				foreach (var Server in LocalGameServers) {
					AddServer(__instance, Server.Address, Server.Name);
				}
			}

			private static void AddServer(GameDiscovery Discovery, string Address, string Name) {
				var joinGameButton = Object.Instantiate(Discovery.ButtonPrefab, Discovery.ItemLocation);
				joinGameButton.transform.localPosition = new Vector3(0f,
					Discovery.YStart + (Discovery.ItemLocation.childCount - 1) * Discovery.YOffset, -1f);
				joinGameButton.netAddress = Address;
				joinGameButton.timeRecieved = float.MaxValue;
				joinGameButton.gameNameText.Text = Name;
				joinGameButton.GetComponentInChildren<MeshRenderer>().material.SetInt(Globals.MASK, 4);

				Discovery.received["Xeno://" + Address] = joinGameButton;
			}
		}
		
		private sealed class LocalGameServer {
			public readonly string Address;
			public readonly string Name;

			private LocalGameServer(string Address, string Name) {
				this.Address = Address;
				this.Name = Name;
			}

			public static LocalGameServer Static(string Address, string Name) {
				return new LocalGameServer(Address, $"{Name} ({Address})");
			}
		}
	}
}
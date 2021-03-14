using System;
using System.Collections.Generic;
using HarmonyLib;
using Hazel;
using XenoCore.Events;
using XenoCore.Utils;

namespace XenoCore.Core {
	public abstract class Message {
		internal const byte RPCId = 100;
		
		internal XenoMod FromMod;
		internal int MessageId;

		protected PlayerControl OwnerPlayer;
		protected MessageReader Reader;
		private MessageWriter Writer;

		internal void PreHandle(PlayerControl Player, MessageReader MessageReader) {
			OwnerPlayer = Player;
			Reader = MessageReader;
			Handle();
		}

		protected PlayerControl ReadPlayer() {
			return PlayerTools.GetPlayerById(Reader.ReadByte());
		}

		protected void WriteLocalPlayer() {
			Writer.Write(PlayerControl.LocalPlayer.PlayerId);
		}

		protected void WritePlayer(PlayerControl Player) {
			Writer.Write(Player.PlayerId);
		}
		
		protected abstract void Handle();

		protected void Write(Action<MessageWriter> Data = null) {
			Writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
				RPCId, SendOption.Reliable, -1);
			Writer.Write(FromMod.InternalId);
			Writer.Write(MessageId);
			Data?.Invoke(Writer);
			AmongUsClient.Instance.FinishRpcImmediately(Writer);
		}
	}
	
	public abstract class DatalessMessage : Message {
		public void Send() {
			Write();
		}
	}

	internal class NetworkRegistry {
		private readonly Dictionary<int, Message> Messages = new Dictionary<int, Message>();
		private readonly XenoMod Mod;
		private int CurrentId;

		public NetworkRegistry(XenoMod Mod) {
			this.Mod = Mod;
		}

		internal void Register(Message Message) {
			Message.FromMod = Mod;
			Message.MessageId = CurrentId;
			CurrentId++;
			
			Messages.Add(Message.MessageId, Message);
		}

		public Message ById(int MessageId) {
			return Messages[MessageId];
		}
	}

	[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
	public class HandleRpcPatch {
		public static void Postfix(PlayerControl __instance, byte HKHMBLJFLMC,
			MessageReader ALMCIJKELCP) {
			if (HKHMBLJFLMC != Message.RPCId) return;

			var ModId = ALMCIJKELCP.ReadInt32();
			var MessageId = ALMCIJKELCP.ReadInt32();

			XenoMods.ById(ModId)
				.MessageById(MessageId)
				.PreHandle(__instance, ALMCIJKELCP);
		}
	}
	
	internal class ResetAllMessage : DatalessMessage {
		public static readonly ResetAllMessage INSTANCE = new ResetAllMessage();

		private ResetAllMessage() {
		}
		
		protected override void Handle() {
			EventsController.RESET_ALL.Post();
		}
	}
}
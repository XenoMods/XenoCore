using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using Reactor;

namespace XenoCore.CustomOptions {
	public partial class CustomOption {
		[RegisterCustomRpc]
		private protected class Rpc : PlayerCustomRpc<XenoPlugin, Rpc.Data> {
			public static Rpc Instance => Rpc<Rpc>.Instance;

			public Rpc(XenoPlugin plugin) : base(plugin) {
			}

			public readonly struct Data {
				public readonly (string ID, CustomOptionType Type, object Value)[] Options;

				public Data(CustomOption option) {
					Options = new[] {OptionToData(option)};
				}

				public Data(IEnumerable<CustomOption> options) {
					Options = options.OrderBy(GetOptionOrder).Select(OptionToData).ToArray();
				}

				private static (string, CustomOptionType, object) OptionToData(CustomOption option) {
					return (option.ID, option.Type, option.GetValue<object>());
				}

				public Data(IEnumerable<(string, CustomOptionType, object)> options) {
					Options = options.ToArray();
				}
			}

			public override RpcLocalHandling LocalHandling => RpcLocalHandling.None;

			public override void Write(MessageWriter writer, Data data) {
				foreach (var (id, type, value) in data.Options) {
					writer.Write(id);
					writer.Write((int) type);
					
					// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
					switch (type) {
						case CustomOptionType.Toggle:
							writer.Write((bool) value);
							break;
						case CustomOptionType.Number:
							writer.Write((float) value);
							break;
						case CustomOptionType.String:
							writer.Write((int) value);
							break;
					}
				}
			}

			public override Data Read(MessageReader reader) {
				var options = new List<(string, CustomOptionType, object)>();
				while (reader.BytesRemaining > 0) {
					var id = reader.ReadString();
					var type = (CustomOptionType) reader.ReadInt32();
					object value = null;
					if (type == CustomOptionType.Toggle) value = reader.ReadBoolean();
					else if (type == CustomOptionType.Number) value = reader.ReadSingle();
					else if (type == CustomOptionType.String) value = reader.ReadInt32();

					options.Add((id, type, value));
				}

				return new Data(options);
			}

			public override void Handle(PlayerControl innerNetObject, Data data) {
				if (innerNetObject?.Data == null || data.Options == null) return;
				
				foreach (var option in data.Options) {
					var customOption = Options.FirstOrDefault(o => o.ID
						.Equals(option.ID, StringComparison.Ordinal));

					customOption?.SetValue(option.Value, true);
				}
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XenoCore.Events;
using XenoCore.Override.Vents;
using XenoCore.Utils;

namespace XenoCore.Override.Map.Components {
	public abstract class PseudoComponent {
		public Vector3 Position => GameObject.transform.position;
		public GameObject GameObject;

		public virtual void Awake() {
		}
	}
	
	public abstract class PseudoComponentBuilder {
		public abstract string TypeId { get; }

		public abstract PseudoComponent Build(Dictionary<string, string> Options);

		protected bool ToBool(string Value) {
			return Value == "1";
		}
	}

	public static class PseudoComponentsRegistry {
		private const string TYPE = "type";

		private static readonly Dictionary<string, PseudoComponentBuilder> Builders
			= new Dictionary<string, PseudoComponentBuilder>();

		internal static void Init() {
			Builders.Clear();
			
			EventsController.GAME_INIT.Register(() => {
				Register(new SpawnComponentBuilder());
				Register(new VentComponentBuilder());
			});
		}

		public static void Register(PseudoComponentBuilder Builder) {
			Builders.Add(Builder.TypeId, Builder);
		}

		internal static PseudoComponent Build(Text Text) {
			return Build(Text.gameObject, new InternalComponent().Parse(Text.text));
		}

		internal static PseudoComponent Build(GameObject GameObject, InternalComponent Internal) {
			var Options = Internal.Options;
			if (!Options.ContainsKey(TYPE)) {
				throw new Exception("PseudoComponent doesn't have a type parameter");
			}
			
			var Key = Options[TYPE];

			if (!Builders.ContainsKey(Key)) {
				throw new Exception($"Builder for PseudoComponent of type {Key} isn't registered!");
			}
			var Builder = Builders[Key];

			var Component = Builder.Build(Options);
			Component.GameObject = GameObject;
			
			return Component;
		}
	}

	internal sealed class InternalComponent {
		public readonly Dictionary<string, string> Options = new Dictionary<string, string>();

		public InternalComponent Parse(string Text) {
			Options.Clear();
			var Lines = Text.Split('\n');
			
			foreach (var Line in Lines) {
				if (string.IsNullOrWhiteSpace(Line)) continue;
				
				var Parts = Line.Split('=');
				Options.Add(Parts[0], string.Join("=", Parts.Skip(1)));				
			}

			return this;
		}
	}
}
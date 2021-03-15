using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnhollowerRuntimeLib;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace XenoCore.Utils {
	public static class Extensions {
		private static readonly Random RANDOM = new Random();

		public static bool IsSabotage(this TaskTypes TaskType) {
			switch (TaskType) {
				case TaskTypes.FixLights:
				case TaskTypes.RestoreOxy:
				case TaskTypes.ResetReactor:
				case TaskTypes.ResetSeismic:
				case TaskTypes.FixComms:
					return true;
				default:
					return false;
			}
		}

		public static bool Compare(this PlayerControl First, PlayerControl Second) {
			return Second != null && Second.PlayerId == First.PlayerId;
		}

		public static void PlayPositioned(this AudioClip Clip, Vector3 Position, float Volume) {
			var gameObject = new GameObject("One shot audio");
			var Source = gameObject.AddComponent(Il2CppSystem.Type
				.GetTypeFromHandle(RuntimeReflectionHelper
					.GetRuntimeTypeHandle<AudioSource>())).Cast<AudioSource>();
			Source.clip = Clip;
			Source.spatialBlend = 1f;
			Source.rolloffMode = AudioRolloffMode.Linear;
			Source.transform.position = Position;
			Source.volume = Volume;
			Source.Play();
			Object.Destroy(gameObject,
				Clip.length * ((double) Time.timeScale < 0.00999999977648258
					? 0.01f
					: Time.timeScale));
		}

		public static void PlayGlobally(this AudioClip Clip, float Volume, float Pitch = 1f) {
			var gameObject = new GameObject("One shot audio");
			var Source = gameObject.AddComponent(Il2CppSystem.Type
				.GetTypeFromHandle(RuntimeReflectionHelper
					.GetRuntimeTypeHandle<AudioSource>())).Cast<AudioSource>();
			Source.clip = Clip;
			Source.spatialBlend = 0f;
			Source.volume = Volume;
			Source.pitch = Pitch;
			Source.Play();
			Object.Destroy(gameObject,
				Clip.length * ((double) Time.timeScale < 0.00999999977648258
					? 0.01f
					: Time.timeScale));
		}

		public static bool TryGetPlayerById(byte Id, out PlayerControl Player) {
			foreach (var SomePlayer in PlayerControl.AllPlayerControls) {
				if (SomePlayer.PlayerId == Id) {
					Player = SomePlayer;
					return true;
				}
			}

			Player = null;
			return false;
		}

		private static string ToHex(float ColorPart) {
			var SomeByte = (byte) (ColorPart * byte.MaxValue);
			return SomeByte.ToString("x2");
		}

		public static string ToHexRGB(this Color Color) {
			return $"{ToHex(Color.r)}{ToHex(Color.g)}{ToHex(Color.b)}";
		}

		public static string ToHexRGBA(this Color Color) {
			return $"{ToHex(Color.r)}{ToHex(Color.g)}{ToHex(Color.b)}{ToHex(Color.a)}";
		}

		public static IList<T> Shuffle<T>(this IList<T> Source) {
			var Count = Source.Count;

			while (Count > 1) {
				Count--;
				var Index = RANDOM.Next(Count + 1);
				var Value = Source[Index];
				Source[Index] = Source[Count];
				Source[Count] = Value;
			}

			return Source;
		}

		public static T RandomItem<T>(this IList<T> Source) {
			return Source[RANDOM.Next(Source.Count)];
		}

		public static string Argumentize(this string Source,
			Dictionary<string, Func<string>> Arguments) {
			if (Arguments == null) return Source;

			return Arguments.Aggregate(Source, (Current, Pair)
				=> Current.Replace(Pair.Key, Pair.Value()));
		}

		public static void DestroyAfterAnimation(this GameObject Instance) {
			Object.Destroy(Instance, Instance.GetComponent<Animator>()
				.GetCurrentAnimatorStateInfo(0).length);
		}

		public static void OneTimeAnimate(this GameObject Instance, Transform TargetTransform) {
			Object.Instantiate(Instance, TargetTransform).DestroyAfterAnimation();
		}

		public static void DestroyAllOfType<T>() where T : MonoBehaviour {
			var Doors = Object.FindObjectsOfType<T>();
			foreach (var Door in Doors) {
				Object.Destroy(Door.gameObject);
			}
		}

		public static List<Transform> FindByNameRecursive(this Transform Parent, string Name) {
			var Result = new List<Transform>();
			FindNameInner(Parent, Name, Result);
			return Result;
		}

		private static void FindNameInner(Transform Parent, string Name, ICollection<Transform> Result) {
			for (var Index = 0; Index < Parent.childCount; Index++) {
				var Child = Parent.GetChild(Index);

				if (Child.name == Name) {
					Result.Add(Child);
				}

				FindNameInner(Child, Name, Result);
			}
		}

		public static List<T> FindByTypeRecursive<T>(this Transform Parent) where T : Component {
			var Result = new List<T>();
			FindTypeInner(Parent, Result);
			return Result;
		}

		private static void FindTypeInner<T>(Transform Parent, ICollection<T> Result) {
			for (var Index = 0; Index < Parent.childCount; Index++) {
				var Child = Parent.GetChild(Index);

				var Component = Child.GetComponent<T>();
				if (Component != null) {
					Result.Add(Component);
				}

				FindTypeInner(Child, Result);
			}
		}

		public static void LookAt2d(this Transform self, Vector3 target) {
			var val = target - self.transform.position;
			val.Normalize();
			var num = Mathf.Atan2(val.y, val.x);
			if (self.transform.lossyScale.x < 0f) {
				num += (float) Math.PI;
			}

			self.transform.rotation = Quaternion.Euler(0f, 0f, num * 57.29578f);
		}
		
		public static void LookAt2d(this Transform self, Transform target) {
			self.LookAt2d(target.transform.position);
		}
	}

	public static class DebugTools {
		public static bool DEBUG = true;

		public static void DebugWin(PlayerControl Winner) {
			if (!DEBUG) return;
			if (Winner == null) return;

			foreach (var SomePlayer in PlayerControl.AllPlayerControls) {
				if (SomePlayer != Winner) {
					SomePlayer.RemoveInfected();
					SomePlayer.MurderPlayer(SomePlayer);
					SomePlayer.Data.IsDead = true;
					SomePlayer.Data.IsImpostor = false;
				} else {
					SomePlayer.Revive();
					SomePlayer.Data.IsDead = false;
					SomePlayer.Data.IsImpostor = true;
				}
			}
		}
	}
	
	public static class LogLevels {
		public static readonly LogLevel LIGHT = new LogLevel("LIGHT",
			ConsoleColor.DarkGreen, false, true);
		
		public static readonly LogLevel INFO = new LogLevel("INFO",
			ConsoleColor.Yellow, true, true);
		
		public static readonly LogLevel ERROR = new LogLevel("ERROR",
			ConsoleColor.Red, true, true);
		
		public class LogLevel {
			public readonly string Name;
			public readonly ConsoleColor Color;
			public readonly bool PrintChat;
			public readonly bool PrintConsole;

			public LogLevel(string Name, ConsoleColor Color, bool PrintChat, bool PrintConsole) {
				this.Name = Name;
				this.Color = Color;
				this.PrintChat = PrintChat;
				this.PrintConsole = PrintConsole;
			}
		}
	}

	public static class ConsoleTools {
		public static string ToHexString(this byte[] Data) {
			return BitConverter.ToString(Data).Replace("-", "");
		}

		public static string ToFloatString(this IEnumerable<float> Data) {
			return string.Join(' ', Data);
		}

		public static void AddSimpleChat(this ChatController Controller, string Message) {
			Controller.AddChat(PlayerControl.LocalPlayer, Message);
		}

		public static void AddChat(this ChatController Controller, string Message) {
			var Stamp = DateTime.Now.ToString("HH:mm:ss");

			Controller.AddChat(PlayerControl.LocalPlayer, $"({Stamp}) {Message}");
		}

		private static void AddChat(LogLevels.LogLevel Level, string Message) {
			var Text = $"({Level.Name}) {Message}";

			if (Level.PrintConsole) {
				System.Console.ForegroundColor = Level.Color;
				System.Console.WriteLine(Text);
				System.Console.ForegroundColor = ConsoleColor.White;
			}

			if (!Level.PrintChat || HudManager.Instance == null) return;
			if (HudManager.Instance.Chat != null) {
				HudManager.Instance.Chat.AddChat(Text);
			}
		}

		public static void Log(LogLevels.LogLevel Level, string Message) {
			AddChat(Level, Message);
		}
		
		public static void Light(string Message) {
			AddChat(LogLevels.LIGHT, Message);
		}

		public static void Info(string Message) {
			AddChat(LogLevels.INFO, Message);
		}

		public static void Error(string Message) {
			AddChat(LogLevels.ERROR, Message);
		}

		public static void Except(Exception Exception) {
			if (Exception is RuntimeWrappedException RuntimeWrappedException) {
				var Wrapped = RuntimeWrappedException.WrappedException;

				if (Wrapped is Il2CppSystem.Exception Inner) {
					Error(Inner.ToString());
					Error(Inner.Message);
					return;
				}

				Error(Wrapped.ToString());
				return;
			}

			Error(Exception.ToString());
		}
	}
}
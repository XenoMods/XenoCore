using System.Reflection;
using Reactor.Extensions;
using Reactor.Unstrip;
using UnityEngine;
using XenoCore.Skin;

namespace XenoCore.Utils {
	public class ResourceLoader {
		private readonly string Path;
		private readonly Assembly From;

		public ResourceLoader(string Path, Assembly From) {
			this.Path = Path;
			this.From = From;
		}

		public ResourceDescriptor Load(string Name) {
			return new ResourceDescriptor(Name, Path, From);
		}

		public Sprite Sprite(string Name) {
			return Load(Name).ToSprite();
		}
		
		public BundleDefinition Bundle(string Name) {
			return Load(Name).ToBundle();
		}
	}
	
	public class BundleDefinition {
		private readonly AssetBundle AssetBundle;

		public BundleDefinition(AssetBundle AssetBundle) {
			this.AssetBundle = AssetBundle;
		}

		public GameObject Object(string Name) {
			return AssetBundle.LoadAsset<GameObject>(Name).DontUnload();
		}
			
		public Sprite Sprite(string Name) {
			return AssetBundle.LoadAsset<Sprite>(Name).DontUnload();
		}
		
		public AnimationClip Animation(string Name) {
			return AssetBundle.LoadAsset<AnimationClip>(Name).DontUnload();
		}
		
		public AudioClip Audio(string Name) {
			return AssetBundle.LoadAsset<AudioClip>(Name).DontUnload();
		}

		public PetDefinition SkinPet(string Name, float YOffset = -0.1f) {
			return new PetDefinition(this, Name, YOffset);
		}
		
		public HatDefinition SkinHat(string Name, Vector2 ChipOffset, bool InFront = true) {
			return new HatDefinition(this, Name, ChipOffset, InFront);
		}
		
		public HatDefinition SkinHatSimple(string Name, Vector2 ChipOffset, bool InFront = true) {
			return new HatDefinitionSimple(this, Name, ChipOffset, InFront);
		}
	}
	
	public class ResourceDescriptor {
		private readonly byte[] Data;

		public ResourceDescriptor(string Name, string Path, Assembly From) {
			using var Stream = From.GetManifestResourceStream(Path + Name);
			Data = Stream.ReadFully();
		}

		public Texture2D ToTexture() {
			var Result = GUIExtensions.CreateEmptyTexture();
			Result.LoadImage(Data);
			return Result;
		}

		public Sprite ToSprite() {
			return ToTexture().CreateSprite().DontUnload();
		}

		public AssetBundle ToRawBundle() {
			return AssetBundle.LoadFromMemory(Data);
		}

		public BundleDefinition ToBundle() {
			return new BundleDefinition(ToRawBundle());
		}
	}
}
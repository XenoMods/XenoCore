namespace XenoCore.Locale {
	public class LanguageEntry {
		public string Id;

		public string English;
		public string Spanish;
		public string Portuguese;
		public string Korean;
		public string Russian;

		public string Get() {
			return LanguageManager.Get(Id);
		}
	}
}
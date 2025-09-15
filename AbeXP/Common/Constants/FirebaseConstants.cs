namespace AbeXP.Common.Constants
{
	public class FirebaseConstants
	{
		public const string KEY = "AIzaSyCmEa3on-XFanPtdFpfxdLQtjfJn6EgzeY";
		public const string PROJECTID = "abexp-45259";
        public const string DOMAIN = "abexp-45259.firebaseapp.com";

		public const string EXPENSES_COLLECTION = "expenses";

		public const string FIREBASE_API_URL = "https://firestore.googleapis.com";
        public static string GetCollectionStoreURL(string collection) => $"/v1/projects/{PROJECTID}/databases/(default)/documents/{collection}";
    }
}


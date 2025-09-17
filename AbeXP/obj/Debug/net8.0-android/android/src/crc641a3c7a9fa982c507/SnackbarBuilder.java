package crc641a3c7a9fa982c507;


public class SnackbarBuilder
	extends com.google.android.material.snackbar.Snackbar.Callback
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onShown:(Lcom/google/android/material/snackbar/Snackbar;)V:GetOnShown_Lcom_google_android_material_snackbar_Snackbar_Handler\n" +
			"n_onDismissed:(Lcom/google/android/material/snackbar/Snackbar;I)V:GetOnDismissed_Lcom_google_android_material_snackbar_Snackbar_IHandler\n" +
			"";
		mono.android.Runtime.register ("Controls.UserDialogs.Maui.SnackbarBuilder, Controls.UserDialogs.Maui", SnackbarBuilder.class, __md_methods);
	}


	public SnackbarBuilder ()
	{
		super ();
		if (getClass () == SnackbarBuilder.class) {
			mono.android.TypeManager.Activate ("Controls.UserDialogs.Maui.SnackbarBuilder, Controls.UserDialogs.Maui", "", this, new java.lang.Object[] {  });
		}
	}


	public void onShown (com.google.android.material.snackbar.Snackbar p0)
	{
		n_onShown (p0);
	}

	private native void n_onShown (com.google.android.material.snackbar.Snackbar p0);


	public void onDismissed (com.google.android.material.snackbar.Snackbar p0, int p1)
	{
		n_onDismissed (p0, p1);
	}

	private native void n_onDismissed (com.google.android.material.snackbar.Snackbar p0, int p1);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}

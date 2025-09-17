package crc641a3c7a9fa982c507;


public class BottomSheetDialogFragment
	extends crc641a3c7a9fa982c507.AbstractAppCompatDialogFragment_1
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCancel:(Landroid/content/DialogInterface;)V:GetOnCancel_Landroid_content_DialogInterface_Handler\n" +
			"";
		mono.android.Runtime.register ("Controls.UserDialogs.Maui.BottomSheetDialogFragment, Controls.UserDialogs.Maui", BottomSheetDialogFragment.class, __md_methods);
	}


	public BottomSheetDialogFragment ()
	{
		super ();
		if (getClass () == BottomSheetDialogFragment.class) {
			mono.android.TypeManager.Activate ("Controls.UserDialogs.Maui.BottomSheetDialogFragment, Controls.UserDialogs.Maui", "", this, new java.lang.Object[] {  });
		}
	}


	public BottomSheetDialogFragment (int p0)
	{
		super (p0);
		if (getClass () == BottomSheetDialogFragment.class) {
			mono.android.TypeManager.Activate ("Controls.UserDialogs.Maui.BottomSheetDialogFragment, Controls.UserDialogs.Maui", "System.Int32, System.Private.CoreLib", this, new java.lang.Object[] { p0 });
		}
	}


	public void onCancel (android.content.DialogInterface p0)
	{
		n_onCancel (p0);
	}

	private native void n_onCancel (android.content.DialogInterface p0);

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

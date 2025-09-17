package crc641a3c7a9fa982c507;


public class LetterSpacingSpan
	extends android.text.style.MetricAffectingSpan
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_updateDrawState:(Landroid/text/TextPaint;)V:GetUpdateDrawState_Landroid_text_TextPaint_Handler\n" +
			"n_updateMeasureState:(Landroid/text/TextPaint;)V:GetUpdateMeasureState_Landroid_text_TextPaint_Handler\n" +
			"";
		mono.android.Runtime.register ("Controls.UserDialogs.Maui.LetterSpacingSpan, Controls.UserDialogs.Maui", LetterSpacingSpan.class, __md_methods);
	}


	public LetterSpacingSpan ()
	{
		super ();
		if (getClass () == LetterSpacingSpan.class) {
			mono.android.TypeManager.Activate ("Controls.UserDialogs.Maui.LetterSpacingSpan, Controls.UserDialogs.Maui", "", this, new java.lang.Object[] {  });
		}
	}

	public LetterSpacingSpan (float p0)
	{
		super ();
		if (getClass () == LetterSpacingSpan.class) {
			mono.android.TypeManager.Activate ("Controls.UserDialogs.Maui.LetterSpacingSpan, Controls.UserDialogs.Maui", "System.Single, System.Private.CoreLib", this, new java.lang.Object[] { p0 });
		}
	}


	public void updateDrawState (android.text.TextPaint p0)
	{
		n_updateDrawState (p0);
	}

	private native void n_updateDrawState (android.text.TextPaint p0);


	public void updateMeasureState (android.text.TextPaint p0)
	{
		n_updateMeasureState (p0);
	}

	private native void n_updateMeasureState (android.text.TextPaint p0);

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

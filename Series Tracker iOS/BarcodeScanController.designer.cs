// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Series_Tracker_iOS
{
	[Register ("BarcodeScanController")]
	partial class BarcodeScanController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton b_Scan { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton b_Submit { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextView Label { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField t_ISBN { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (b_Scan != null) {
				b_Scan.Dispose ();
				b_Scan = null;
			}
			if (b_Submit != null) {
				b_Submit.Dispose ();
				b_Submit = null;
			}
			if (Label != null) {
				Label.Dispose ();
				Label = null;
			}
			if (t_ISBN != null) {
				t_ISBN.Dispose ();
				t_ISBN = null;
			}
		}
	}
}


// This file has been generated by the GUI designer. Do not modify.
namespace Protractor
{
	public partial class Launcher
	{
		private global::Gtk.UIManager UIManager;
		
		private global::Gtk.Action FileAction;
		
		private global::Gtk.Action GestureAction;
		
		private global::Gtk.Action HelpAction;
		
		private global::Gtk.Action AboutAction;
		
		private global::Gtk.Action FileAction1;
		
		private global::Gtk.Action QuitAction;
		
		private global::Gtk.Action GestureAction1;
		
		private global::Gtk.Action OpenAction;
		
		private global::Gtk.Action SaveAction;
		
		private global::Gtk.Action HelpAction1;
		
		private global::Gtk.Action AboutAction1;
		
		private global::Gtk.Action AboutAction2;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Protractor.Launcher
			this.UIManager = new global::Gtk.UIManager ();
			global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
			this.FileAction = new global::Gtk.Action ("FileAction", global::Mono.Unix.Catalog.GetString ("File"), null, null);
			this.FileAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("File");
			w1.Add (this.FileAction, null);
			this.GestureAction = new global::Gtk.Action ("GestureAction", global::Mono.Unix.Catalog.GetString ("Gesture"), null, null);
			this.GestureAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Gesture");
			w1.Add (this.GestureAction, null);
			this.HelpAction = new global::Gtk.Action ("HelpAction", global::Mono.Unix.Catalog.GetString ("Help"), null, null);
			this.HelpAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Help");
			w1.Add (this.HelpAction, null);
			this.AboutAction = new global::Gtk.Action ("AboutAction", global::Mono.Unix.Catalog.GetString ("About"), null, null);
			this.AboutAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("About");
			w1.Add (this.AboutAction, null);
			this.FileAction1 = new global::Gtk.Action ("FileAction1", global::Mono.Unix.Catalog.GetString ("File"), null, null);
			this.FileAction1.ShortLabel = global::Mono.Unix.Catalog.GetString ("File");
			w1.Add (this.FileAction1, null);
			this.QuitAction = new global::Gtk.Action ("QuitAction", global::Mono.Unix.Catalog.GetString ("Quit"), null, null);
			this.QuitAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Quit");
			w1.Add (this.QuitAction, null);
			this.GestureAction1 = new global::Gtk.Action ("GestureAction1", global::Mono.Unix.Catalog.GetString ("Gesture"), null, null);
			this.GestureAction1.ShortLabel = global::Mono.Unix.Catalog.GetString ("Gesture");
			w1.Add (this.GestureAction1, null);
			this.OpenAction = new global::Gtk.Action ("OpenAction", global::Mono.Unix.Catalog.GetString ("Open"), null, null);
			this.OpenAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Open");
			w1.Add (this.OpenAction, null);
			this.SaveAction = new global::Gtk.Action ("SaveAction", global::Mono.Unix.Catalog.GetString ("Save"), null, null);
			this.SaveAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Save");
			w1.Add (this.SaveAction, null);
			this.HelpAction1 = new global::Gtk.Action ("HelpAction1", global::Mono.Unix.Catalog.GetString ("Help"), null, null);
			this.HelpAction1.ShortLabel = global::Mono.Unix.Catalog.GetString ("Help");
			w1.Add (this.HelpAction1, null);
			this.AboutAction1 = new global::Gtk.Action ("AboutAction1", global::Mono.Unix.Catalog.GetString ("About"), null, null);
			this.AboutAction1.ShortLabel = global::Mono.Unix.Catalog.GetString ("About");
			w1.Add (this.AboutAction1, null);
			this.AboutAction2 = new global::Gtk.Action ("AboutAction2", global::Mono.Unix.Catalog.GetString ("About"), null, null);
			this.AboutAction2.ShortLabel = global::Mono.Unix.Catalog.GetString ("About");
			w1.Add (this.AboutAction2, null);
			this.UIManager.InsertActionGroup (w1, 0);
			this.AddAccelGroup (this.UIManager.AccelGroup);
			this.WidthRequest = 500;
			this.HeightRequest = 500;
			this.Name = "Protractor.Launcher";
			this.Title = global::Mono.Unix.Catalog.GetString ("Protactor");
			this.WindowPosition = ((global::Gtk.WindowPosition)(3));
			this.DefaultWidth = 500;
			this.DefaultHeight = 500;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Show ();
			this.QuitAction.Activated += new global::System.EventHandler (this.quitProgram);
			this.OpenAction.Activated += new global::System.EventHandler (this.openGesture);
			this.SaveAction.Activated += new global::System.EventHandler (this.saveGesture);
			this.AboutAction2.Activated += new global::System.EventHandler (this.aboutScreen);
		}
	}
}
/**
 * The Protractor Recognizer (C# version)
 *
 *		Justin Gruber, Undergraduate
 *	    University of Guelph
 *		Software Engineering Department
 *		50 Stone Road East
 *		Guelph, ON N1G 2W1
 *		jgruber@mail.uoguelph.ca
 *
 *	This file is part of Protractor.
 *
 *  Protractor is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Protractor is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Protractor.  If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System;
using Gtk;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Drawing;
using Cairo;
using System.Collections.Generic;
namespace Protractor
{
	public partial class Launcher : Gtk.Window
	{

		#region window variables

		float divisions = 8;

		//Colour code
		float R=1, G=1, B=1, A=1;
		public ImageSurface surface;

		//Setup Graphing Parameters
		public int param_Max_x	=  6;
		public int param_Min_x	= -6;

		public int param_Max_y	=  10;
		public int param_Min_y	= -10;

		//Bool used if the drawn points need to be cleared from the screen
		bool clear = false;

		DrawingArea dArea;
		Statusbar sBar;
		Entry batchFunctionName;

		//Global variables used for drawing
		PointD Start, End;
		PointD prevDot = new PointD(0,0);

		public bool isDrawing;
		public bool isDrawingPoint;
		public bool isDot = true;
		public static bool startRecordPoints = false;
		private static int count = 0;
		//Delegate for the drawing functions
		//Note: PointD end was used when only lines were being drawn, is not used on dots
		//and start is passed in twice instead
		//Was kept in, in the event lines were reintroduced as an option
		delegate void DrawShape(Cairo.Context ctx, PointD start, PointD end);

		DrawShape Painter;
		static DrawShape myoPainter;

		//Accessor works by creating a value, and inside we choose what is returned
		//and what is set by using the get,set, and value keywords.
		public PointD paint_Start{
			set{ Start = value; }
			get{ return Start;}
		}

		public PointD paint_End{
			set{ End = value; }
			get{ return End;}
		}
			
		public DrawingArea paint_Area{
			get{ return dArea;}
		}
		private bool isRed = true;

		//List of type PointD which contains all of the user-drawn points
		private List<PointD> currentDrawnPoints = new List<PointD> ();

		//Instance of the recognizer
		Recognizer recognizer = new Recognizer();
		#endregion

		//Constructor for the class which uses the setup for the window
		public Launcher () : base (Gtk.WindowType.Toplevel)
		{
			setupWindow ();
			this.Build ();
		}

		#region Window setup

		//Contains all of the components which are on the window
		private void setupWindow(){

			//Vertical box (3 sections) which is the main container
			VBox MainVBox = new VBox(false, 6);

			/*********************Start of menu bar components*********************/
						
			//Note: Flow of menus are:
			//Menubar contains MenuItems (e.g. File, Edit, About, etc...)
			//MenuItems contains one Menu (i.e. a Submenu)
			//Menu contains multiple MenuItems (e.g. Save, Save As, Load, etc...)
			MenuBar MainMenuBar = new MenuBar ();

			Menu FileMenu = new Menu ();
			MenuItem File = new MenuItem ("File");
			File.Submenu = FileMenu;

			MenuItem ExitItem = new MenuItem ("Exit");
			ExitItem.Activated += quitProgram;
			FileMenu.Append (ExitItem);

			Menu GestureMenu = new Menu ();
			MenuItem Gesture = new MenuItem ("Gestures");
			Gesture.Submenu = GestureMenu;

			MenuItem LoadGesture = new MenuItem ("Load Gesture");
			LoadGesture.Activated += openGesture;
			GestureMenu.Append (LoadGesture);

			MenuItem SaveGestureItem = new MenuItem ("Save Gesture");
			SaveGestureItem.Activated += createGesture;
			GestureMenu.Append (SaveGestureItem);

			Menu PointsMenu = new Menu ();
			MenuItem Points = new MenuItem ("Data Points");
			Points.Submenu = PointsMenu;

			MenuItem LoadPointsItem = new MenuItem ("Load Points");
			LoadPointsItem.Activated += loadDataPoints;
			PointsMenu.Append (LoadPointsItem);

			MenuItem RecordPointsItem = new MenuItem ("Record Points");
			RecordPointsItem.Activated += recordPoints;
			PointsMenu.Append (RecordPointsItem);

			MenuItem BatchRecognizeItem = new MenuItem ("Batch Recognize");
			BatchRecognizeItem.Activated += batchRecognize;
			PointsMenu.Append (BatchRecognizeItem);

			Menu HelpMenu = new Menu ();
			MenuItem Help = new MenuItem ("Help");
			Help.Submenu = HelpMenu;

			MenuItem AboutMenuItem = new MenuItem("About");
			HelpMenu.Append (AboutMenuItem);

			MainMenuBar.Append (File);
			MainMenuBar.Append (Gesture);
			MainMenuBar.Append (Points);
			MainMenuBar.Append (Help);

			/*********************End of menu bar components*********************/

			//Drawing area which is the core component of the application
			dArea = new DrawingArea ();
			dArea.SetSizeRequest (500, 500);

			//Horizontal box (4 sections) which contains all of the buttons along the
			//bottom of the window
			HBox ButtonHBox = new HBox (false, 6);

			/*********************Start of Buttons*********************/
						
			Button BtnCreateGesture = new Button ("Create");
			Button BtnRecognizeGesture = new Button ("Recognize");
			Button BtnClearScreen = new Button ("Clear");
			Button BtnRecordPoints = new Button ("Record points");
			Button BtnChangeColour = new Button ("Change colour");

			//Button functions
			BtnCreateGesture.Clicked += new EventHandler (createGesture);
			BtnRecognizeGesture.Clicked += new EventHandler (recognizeGesture);
			BtnClearScreen.Clicked += new EventHandler (clearScreen);
			BtnRecordPoints.Clicked += new EventHandler (recordPoints);
			BtnChangeColour.Clicked += changeColour;

			//Adding buttons to the current horizontal box
			ButtonHBox.PackStart (BtnCreateGesture, true, false, 0);
			ButtonHBox.PackStart (BtnRecognizeGesture, true, false, 0);
			ButtonHBox.PackStart (BtnClearScreen, true, false, 0);
			ButtonHBox.PackStart (BtnRecordPoints, true, false, 0);
			ButtonHBox.PackStart (BtnChangeColour, true, false, 0);
			/*********************End of Buttons*********************/

			//Status bar which shows the score and recognized gesture
			sBar = new Statusbar ();
			sBar.Push (1, "Ready");

			//Entry box for batch function name to be used on the files
			batchFunctionName = new Entry ("Recorded points function name");
			batchFunctionName.AddEvents (
				(int)Gdk.EventMask.ButtonPressMask
			);
			batchFunctionName.ButtonPressEvent += clearTextBox;

			//Adding all components to the Vertical box
			MainVBox.PackStart (MainMenuBar, false, false, 0);
			MainVBox.PackStart (dArea, false, false, 0);
			MainVBox.PackStart (ButtonHBox, false, false, 0);
			MainVBox.PackStart (batchFunctionName, false, false, 0);
			MainVBox.PackStart (sBar, false, false, 0);

			Add (MainVBox);

			ShowAll ();

			//Surface 'pattern' for area to be covered
			surface = new ImageSurface(Format.Argb32, 500, 500);

			//Adding mouse events to the drawing area and assigning functions
			dArea.AddEvents(
				//Mouse Related Events
				(int)Gdk.EventMask.PointerMotionMask
				|(int)Gdk.EventMask.ButtonPressMask
				|(int)Gdk.EventMask.ButtonReleaseMask
			);

			//Repaint the Canvas Internally.
			dArea.ExposeEvent += OnDrawingAreaExposed;
			//Do this on MousePress inside Area
			dArea.ButtonPressEvent += OnMousePress;
			//Do this on MouseReleased inside Area
			dArea.ButtonReleaseEvent += OnMouseRelease;
			//Do this if a Motion Occurs inside the Drawing Area
			dArea.MotionNotifyEvent += OnMouseMotion2;

			//Assigning close function to the window
			DeleteEvent += delegate { Application.Quit(); };

			//Checking to see if bool for using the dot function is true
			//And assigning the required function as the delegate's operation
			//Note: This will always be true, no current way to switch while
			//application is running, has not been needed as of yet
			if (isDot) {
				Painter = new DrawShape (DrawDot);
				myoPainter = new DrawShape (DrawDot);
			} else {
				Painter = new DrawShape (DrawLine);			
			}
		}			
		#endregion
				

		#region Events for Drawing Area
		void OnDrawingAreaExposed(object source, ExposeEventArgs args)
		{
			Cairo.Context ctx;
			using (ctx = Gdk.CairoHelper.Create(dArea.GdkWindow))
			{
				//Set Background Color
				ctx.SetSourceRGB(0, 0, 0);
				ctx.Rectangle (0, 0, 500, 500);
				ctx.Fill();

				//Draw Axis Lines
				for (int i = 0; i < (int)(divisions); i++) 
				{
					float curX = Math.Abs(param_Min_x) + Math.Abs(param_Max_x);

					float divCount = (500) / divisions;
					divCount *= i;

					curX = divCount;

					//If middle colour Green
					if( i == ((int)divisions/2)){
						R = 0;
						G = 1;
						B = 0;
					} else {
						R = 0.5F;
						G = 0.5F;
						B = 0.5F;
					}

					//Draw Vertical Lines
					DrawLine (ctx, new PointD ((int)(curX), 0), new PointD ((int)(curX), 500));

					//Draw Horizontal Lines
					DrawLine (ctx, new PointD (0, (int)(curX)), new PointD (500, (int)(curX)));
				}

				//Applying the surface pattern for how much of the area should be covered
				ctx.SetSource(new SurfacePattern(surface));
				ctx.Paint();

				//Dispose event for the creation of Cairo's context
				//Note: Needs to be done every time a context is created otherwise leaks will happen
				((IDisposable) ctx.GetTarget()).Dispose ();
				((IDisposable)ctx).Dispose ();
			}

			//isDrawing is a bool which is set upon on mouse button press
			//and triggers the start of the drawing
			if (isDrawing)
			{
				//This is responsible for drawing the dots on screen
				using (ctx = Gdk.CairoHelper.Create(dArea.GdkWindow))
				{
					Painter(ctx, Start, End);

					((IDisposable) ctx.GetTarget()).Dispose ();
					((IDisposable)ctx).Dispose ();
				}
			}
		}

		//Getting the initial mouse location on screen
		void OnMousePress(object source, ButtonPressEventArgs args)
		{
			Start.X = args.Event.X;
			Start.Y = args.Event.Y;

			End.X = args.Event.X;
			End.Y = args.Event.Y;

			isDrawing = true;
			dArea.QueueDraw();
		}


		//Getting end of the would-be line, kept for possible re-use of lines
		void OnMouseRelease(object source, ButtonReleaseEventArgs args)
		{
			End.X = args.Event.X;
			End.Y = args.Event.Y;

			isDrawing = false;

			using (Context ctx = new Context(surface))
			{
				Painter(ctx, Start, End);
				((IDisposable) ctx.GetTarget()).Dispose ();
				((IDisposable)ctx).Dispose ();
			}

			dArea.QueueDraw();
		}

		//Updates every time the mouse is moved while button is pressed
		void OnMouseMotion(object source, MotionNotifyEventArgs args)
		{
			if (isDrawing)
			{
				End.X = args.Event.X;
				End.Y = args.Event.Y;

				using (Context ctx = new Context (surface)) {
					//Reset of end's points so there are no accidental 
					if (isDot) {
						Start.X = args.Event.X;
						Start.Y = args.Event.Y;
					}
					Painter (ctx, Start, End);
					((IDisposable) ctx.GetTarget()).Dispose ();
					((IDisposable)ctx).Dispose ();
				}
				
				dArea.QueueDraw();
			}
		}

		//Wrapper for default onmousemotion which is used to clear the drawing from the window
		void OnMouseMotion2(object source, MotionNotifyEventArgs args){

			//Calls the initial on mouse motion which is used for drawing new dots on screen
			OnMouseMotion (source, args);

			removeDotsFromScreen ();
		}


		#endregion

		#region Drawing functions
		//Original function which draws a straight line from start to end
		void DrawLine(Cairo.Context ctx, PointD start, PointD end)
		{
			ctx.SetSourceRGBA(R,G,B,A);
			ctx.MoveTo(start);
			ctx.LineTo(end);
			ctx.Stroke();
		}

		//Draws the dots on the screen, end is a moot parameter
		public void DrawDot (Cairo.Context ctx, PointD start, PointD end)
		{
			if (!start.Equals (prevDot)) {
					
					ctx.SetSourceRGBA (R, G, B, A);
					ctx.Arc (start.X, start.Y, 1, 0, 2 * Math.PI);
					ctx.StrokePreserve ();
					ctx.Fill ();
				if (startRecordPoints) {

					currentDrawnPoints.Add (start);

				}
				prevDot = start;

			}
		}

		protected void removeDotsFromScreen(){
			//Redraw all of the grid
			if (clear) {
				using (Context ctx = new Context (surface)) {

					//Set Background Color
					ctx.SetSourceRGB (0, 0, 0);
					ctx.Rectangle (0, 0, 500, 500);
					ctx.Fill ();

					//Draw Axis Lines
					for (int i = 0; i < (int)(divisions); i++) {
						float curX = Math.Abs (param_Min_x) + Math.Abs (param_Max_x);

						float divCount = (500) / divisions;
						divCount *= i;

						curX = divCount;

						//If Middle Color Green
						if (i == ((int)divisions / 2)) {
							R = 0;
							G = 1;
							B = 0;
						} else {
							R = 0.5F;
							G = 0.5F;
							B = 0.5F;
						}
						//Draw Vertical Lines
						DrawLine (ctx, new PointD ((int)(curX), 0), new PointD ((int)(curX), 500));

						//Draw Horizontal Lines
						DrawLine (ctx, new PointD (0, (int)(curX)), new PointD (500, (int)(curX)));
					}
				}
				clear = false;
				dArea.QueueDraw ();
			}
		}
		#endregion

		#region Menubar functions
		protected void recognizeGesture (object sender, EventArgs e)
		{
			string statusBarMessage = "";

			//Removing current message from the status bar
			sBar.Pop (1);

			//Copying the list of drawn points because of pass by reference
			List<PointD> tempList = new List <PointD> ();
			foreach (PointD pt in currentDrawnPoints) {
				tempList.Add (new PointD(pt.X, pt.Y));
			}

			Tuple<string, double> result = recognizer.findMatchingTemplate (tempList);
			Console.WriteLine ("Match: {0}, maxScore: {1}", result.Item1, (int)((result.Item2)));

			//Updating status bar
			statusBarMessage = "Match: " + result.Item1 + "\t\t Score: " + result.Item2;
			sBar.Push(1, statusBarMessage);
		}

		//Sets the bool for clearing the screen and removes all of the points in the list
		protected void clearScreen (object sender, EventArgs e)
		{
			clear = true;
			currentDrawnPoints.Clear ();
			dArea.QueueDraw ();

			//drawGraphs (); //Here as a quick way to draw new graphs and make an new template

		}

		//Starting the recording of new points, if current points are drawn, they will still 
		//be added to the list of points sent as it sends the entire list of drawn points
		//StartRecordPoints is a bool for the recording of points from the Myo armband input
		protected void recordPoints(object sender, EventArgs e){
			string functionName = "";

			//Record button has already been pushed
			if (startRecordPoints == true) {
				startRecordPoints = false;
				count += 1;
				Console.WriteLine ("Recording: " + count);
				if (batchFunctionName.Text != "Recorded points function name") {
					functionName = batchFunctionName.Text;
				}

				CustomFunctions.saveRecordedPoints (currentDrawnPoints, functionName);
				currentDrawnPoints.Clear();
				clearScreen(sender, e);

			} else {
				currentDrawnPoints.Clear();
				removeDotsFromScreen ();
				startRecordPoints = true;
			}
		}

		//Loads in the data points as the current drawn points to visualize what data set 
		//is being recognized
		protected void loadDataPoints(object sender, EventArgs e){
			List<List<PointD>> pointsToDraw = CustomFunctions.loadPoints ();

			Cairo.Context ctx;
			if (pointsToDraw == null) {
				return;
			}

			if (pointsToDraw.Count == 0) {
				return;
			}

			foreach (List<PointD> li in pointsToDraw) {
				foreach (PointD pt in li) {
					using (ctx = Gdk.CairoHelper.Create (dArea.GdkWindow)) {
						int R2 = 0;
						int G2 = 0;

						if (isRed) {
							R2 = 255;
							G2 = 0;
						} else {
							R2 = 0;
							G2 = 255;
						}
						ctx.SetSourceRGBA (R2, G2, 0, A);
						ctx.Arc (pt.X, pt.Y, 1, 0, 2 * Math.PI);
						ctx.StrokePreserve ();
						ctx.Fill ();
						((IDisposable)ctx.GetTarget ()).Dispose ();
						((IDisposable)ctx).Dispose ();
					}
				}
			}
		}

		//Changes the colour of the lines that get drawn in when loading previously 
		//drawn functions
		private void changeColour(object sender, EventArgs e){

			if (isRed == true) {
				isRed = false;
			} else {
				isRed = true;
			}
		}

		//Wrapper to open the save dialog
		protected void createGesture (object sender, EventArgs e)
		{
			CustomFunctions.openSaveDialogue (currentDrawnPoints);
		}

		//Quits the application
		protected void quitProgram (object sender, EventArgs e)
		{
			Application.Quit ();
		}
	
		//Wrapper for opening the load gesture dialog
		protected void openGesture (object sender, EventArgs e)
		{
			CustomFunctions.openLoadDialogue ();
		}

		//This is still here because of a build issue that occurs when it is not present
		protected void saveGesture (object sender, EventArgs e)
		{
			throw new NotImplementedException ();
		}

		//Shows the about screen - not yet implemented
		protected void aboutScreen (object sender, EventArgs e)
		{
			throw new NotImplementedException ();
		}

		protected void clearTextBox(object sender, ButtonPressEventArgs e){
			if (batchFunctionName.Text == "Recorded points function name"){
				batchFunctionName.DeleteText (0, batchFunctionName.Text.Length);
			} else if (batchFunctionName.Text == ""){
				batchFunctionName.InsertText ("Recorded points function name");
			}
		}

		protected void batchRecognize(object sender, EventArgs e){
			string functionName = "";
			List<Tuple<string,List<PointD>>> loadedPoints = CustomFunctions.loadBatchPoints ();
			List<Tuple<string,double>> batchResultList = new List<Tuple<string, double>> ();

			foreach (Tuple<string, List<PointD>> resultTuple in loadedPoints) {
				Tuple<string, double> batchResultTuple = recognizer.findMatchingTemplate (resultTuple.Item2);
				batchResultList.Add (batchResultTuple);
			}

			if (batchFunctionName.Text != "Recorded points function name") {
				functionName = batchFunctionName.Text;
			}

			CustomFunctions.writeBatchTestingResult (batchResultList, batchFunctionName.Text);
			sBar.Pop (1);
			sBar.Push (1, "Batch recognition done, result in gesture folder.");
		}
		#endregion		


		//Function is only here to draw pre-defined functions in te Graphing
		//class so that I could create an accurate template for them
		//Is not hooked up to anything at the moment, was only used for 
		//gesture creation, was left in if there was ever a need for it again
		public void drawGraphs(){
			//Change the Graphing.--- to whatever function you want drawn
			List<PointD> toDraw = Graphing.xSquaredTwoXplusOne();
			Cairo.Context ctx;

			foreach (PointD pt in toDraw) {
				currentDrawnPoints.Add (pt);
				using (ctx = Gdk.CairoHelper.Create(dArea.GdkWindow))
				{
					ctx.SetSourceRGBA (R, G, B, A);
					ctx.Arc (pt.X, pt.Y, 1, 0, 2*Math.PI);
					ctx.StrokePreserve();
					ctx.Fill();
					((IDisposable) ctx.GetTarget()).Dispose ();
					((IDisposable)ctx).Dispose ();
				}
			}
		}
	}
}
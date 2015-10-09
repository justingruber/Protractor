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
using System.Threading; // For Threading
using System.Collections.Generic;
using Cairo;

namespace Protractor
{
	public class MyoInitialize
	{
		//Instance of the launcher so the Myo can use it's paint appliction
		private Launcher painter;

		//Initialization of class
		public MyoInitialize (Launcher drawer)
		{
			//Starting the Myo application
			MyoWrapper.Myo_Start ();
			painter = drawer;
		}

		//Secondary thread which was intialized in the Program class
		public void Thread_Start()
		{
			//Have the thread sleep for 1 second at a time until myo start sequence occurs
			while (!MyoWrapper.myo_startSequence) {
				Thread.Sleep (1000);
			}
			Console.WriteLine ("Start sequence used");

			while (true) {
				//if (Launcher.startRecordPoints) {
					Thread.Sleep (10);
					double myo_Pitch, myo_Yaw;

					//Notes about normalizing:
					//1) USB connector should be facing elbow, if not then the Y and X values get mixed 
					//   for their directional values
					//2) Myo apparently has a drifting Yaw value the longer the armband is worn
					//   found at: https://developer.thalmic.com/forums/topic/813/
					//   I noticed it after an hour or so of use, it was off by about 90 degrees and caused
					//   a looping in the upper and lower bound of the myo's Yaw

					//Normalizing the Y position from the range 0-1 to 0-500
					myo_Pitch = MyoWrapper.myo_Pitch;
					myo_Pitch += 0.5;
					myo_Pitch *= 500;

					//Normalizing the X position from the range 0-1 to 0-500
					myo_Yaw = MyoWrapper.myo_Yaw;
					myo_Yaw += 0.5;
					myo_Yaw *= -500;

					//Having X position normalized within the range 0-500 because of 
					//the 360 range of motion for the Myo, it would sometimes go off 
					//the area bounds
					if (myo_Yaw > 500 || myo_Yaw < 0) {
						myo_Yaw = (myo_Yaw % 500 + 500) % 500;
					}

					//Console.WriteLine ("X: {0}, Y: {1}", myo_Yaw, myo_Pitch);

					PointD pt = new PointD (myo_Yaw, myo_Pitch);

					Cairo.Context ctx;
					using (ctx = Gdk.CairoHelper.Create (painter.paint_Area.GdkWindow)) {
						painter.DrawDot (ctx, pt, pt);
						((IDisposable)ctx.GetTarget ()).Dispose ();
						((IDisposable)ctx).Dispose ();
					}
				//}
			}
		}
	}
}
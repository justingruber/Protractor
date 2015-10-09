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
using Cairo;
namespace Protractor
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();

			//Window setup
			Launcher sc = new Launcher ();

			/*Test Application is the main component which mostly controls the Myo
			  Note: If using the Myo Armband, you must have the application "Myo Connect" 
			  running and have the bluetooth dongle attached so that the Myo is able to 
			  send gestures and orientation data to the computer

			  Uncomment if you want Myo functionality*/

			/*MyoInitialize tmp = new MyoInitialize(sc);

			//Setup the second thread by assigning the target to enter on
			Thread secondaryThread = new Thread(tmp.Thread_Start);

			//Run the Thread by using the predefined Start() function
			secondaryThread.Start ();

			//Assigning second thread to background so it closes properly when the application ends
			secondaryThread.IsBackground = true;
			*/
			Application.Run ();
		}
	}
}
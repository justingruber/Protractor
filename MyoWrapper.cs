/**
 * The Protractor Recognizer (C# version)
 *
 *		Justin Gruber, Undergraduate
 *		University of Guelph
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
using MyoSharp.Communication;
using MyoSharp.Device;
using MyoSharp.Exceptions;
using MyoSharp.Poses;

namespace Protractor
{
	public class MyoWrapper
	{
		#region Methods
		static OrientationDataEventArgs cachedEvent;
		static bool startSequence=false;
		static double currentPitch;
		static double currentYaw;
		static double currentRoll;

		//Y-position of Myo on grid
		public static double myo_Pitch{
			get{ return  currentPitch;}
		}

		//X-position of Myo on grid
		public static double myo_Yaw {
			get { return currentYaw; }
		}

		//Is unlock then wave-out
		public static bool myo_startSequence{
			get{ return  startSequence; }
			set{ startSequence = value; }
		}

		//Z-position - not used
		public static double myo_Roll {
			get { return currentRoll; }
		}

		//Modified code from the Myo Sharp tutorial on GitHub
		//Available at: https://github.com/tayfuzun/MyoSharp
		public static void Myo_Start ()
		{
			//Create a hub that will manage Myo devices for us
			var channel = Channel.Create (
				ChannelDriver.Create (ChannelBridge.Create (),
					MyoErrorHandlerDriver.Create (MyoErrorHandlerBridge.Create ())));

			IHub hub = Hub.Create (channel);

			//Listen for when the Myo connects
			hub.MyoConnected += (sender, e) =>
			{
				Console.WriteLine("Myo {0} has connected!", e.Myo.Handle);
				e.Myo.Vibrate(VibrationType.Short);
				e.Myo.PoseChanged += Myo_PoseChanged;
				e.Myo.OrientationDataAcquired += Myo_OrientationDataAcquired;

				var pose = HeldPose.Create(e.Myo, Pose.Fist);
				pose.Interval = TimeSpan.FromSeconds(0.5);

				//For every Myo that connects, listen for special sequences
				var sequence = PoseSequence.Create(e.Myo,Pose.WaveOut,Pose.WaveIn);
				sequence.PoseSequenceCompleted += Sequence_PoseSequenceCompleted;
			};

			//Listen for when the Myo disconnects
			hub.MyoDisconnected += (sender, e) =>
			{
				Console.WriteLine("Oh no! It looks like {0} arm Myo has disconnected!", e.Myo.Arm);
				e.Myo.PoseChanged -= Myo_PoseChanged;
				e.Myo.OrientationDataAcquired -= Myo_OrientationDataAcquired;
			};

			//Start listening for Myo data
			channel.StartListening();
			Console.WriteLine ("Listening to channel...");

			//Wait on user input
			ConsoleHelper.UserInputLoop(hub);
		}
		#endregion

		#region Event Handlers
		//Called when a pose is detected and outputs the pose to console,
		//also sets the start sequence
		private static void Myo_PoseChanged(object sender, PoseEventArgs e)
		{
			Console.WriteLine("{0} arm Myo detected {1} pose!", e.Myo.Arm, e.Myo.Pose);
			Console.WriteLine ("Pose = " + e.Myo.Pose);

			//Start sequence is wave out
			if (e.Myo.Pose == Pose.WaveOut)
				startSequence = true; 
			if (e.Myo.Pose == Pose.WaveIn)
				startSequence = false;
		}

		private static void Myo_Unlocked(object sender, MyoEventArgs e)
		{
			Console.WriteLine("{0} arm Myo has unlocked!", e.Myo.Arm);
		}

		private static void Myo_Locked(object sender, MyoEventArgs e)
		{
			Console.WriteLine("{0} arm Myo has locked!", e.Myo.Arm);
		}

		private static void Sequence_PoseSequenceCompleted(object sender, PoseSequenceEventArgs e)
		{
			Console.WriteLine("{0} arm Myo has performed a pose sequence!", e.Myo.Arm);
			e.Myo.Vibrate(VibrationType.Medium);
			startSequence = true;
		}

		public static void VibrateMyo()
		{
			cachedEvent.Myo.Vibrate (VibrationType.Short);
		}

		//Contains all of the orientation data which is received
		private static void Myo_OrientationDataAcquired(object sender, OrientationDataEventArgs e)
		{
			cachedEvent = e;
			//const float PI = (float)System.Math.PI; // Only needed if using conversion

			//Commented math will convert the values to a 0-9 scale (for easier digestion/understanding)
			var roll = e.Roll;   //(int)((e.Roll + PI) / (PI * 2.0f) * 10);
			var pitch = e.Pitch; //(int)((e.Pitch + PI) / (PI * 2.0f) * 10);
			var yaw = e.Yaw;     //(int)((e.Yaw + PI) / (PI * 2.0f) * 10);

			currentPitch = pitch;
			currentYaw = yaw;
			currentRoll = roll;
			//Console.WriteLine(@"Roll: {0}", roll);
			//Console.WriteLine(@"Pitch: {0}", pitch);
			//Console.WriteLine(@"Yaw: {0}", yaw);
		}
		#endregion
	}
}
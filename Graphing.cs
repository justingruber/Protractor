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
using Cairo;
using System.Collections.Generic;

namespace Protractor
{

	//This class contains the functions which will create the plot points
	//(albeit a bit stretched due to having a 500x500 area, but they look as they should)
	//that are continious functions for the recognizer to, well, recognize
	//Functions that are being done are in the comments above the functions
	public class Graphing
	{

		// y = x
		public static List<PointD> yEqualsX ()
		{
			int i;
			List<PointD> graph = new List<PointD> ();

			for (i = 0; i <= 500; i+=5) {
				PointD pt = new PointD(i, 500 - i);
				Console.WriteLine ("pt.X: {0}, pt.Y: {1}", pt.X, pt.Y);
				graph.Add (pt);
			}
			return graph;
		}

		// y = x^2 + 2x + 1
		public static List<PointD> xSquaredTwoXplusOne ()
		{
			double i;
			List<PointD> graph = new List<PointD> ();

			for (i = -18; i <= 18; i+=0.25) {
				double tmpVar = (Math.Pow (i, 2) + 2 * i + 1);
				if (tmpVar < 0) {
					tmpVar = Math.Abs (tmpVar - 2 * tmpVar);
				}
				Console.WriteLine ("tmp: " + tmpVar);
				if (tmpVar > 500) {
					continue;
				}
				PointD pt = new PointD(250 + 10*i, 250 - tmpVar);
				Console.WriteLine ("pt.X: {0}, pt.Y: {1}", pt.X, pt.Y);
				graph.Add (pt);
			}
			return graph;
		}

		// y = -x^2 + 2x + 1
		public static List<PointD> negXSquaredTwoXplusOne ()
		{
			int i;
			List<PointD> graph = new List<PointD> ();

			for (i = -18; i <= 18; i++) {
				double tmpVar = ((-1 * Math.Pow (i, 2)) + 2 * i + 1);

				if (tmpVar > 500) {
					continue;
				}
				PointD pt = new PointD(250 + 10*i, 250 - tmpVar);
				Console.WriteLine ("pt.X: {0}, pt.Y: {1}", pt.X, pt.Y);
				graph.Add (pt);
			}
			return graph;

		}

		// y = 2(x - 3)^2 + 4
		public static List<PointD> TwoXMinThreeSqPlusfour()
		{
			int i;
			List<PointD> graph = new List<PointD> ();

			for (i = -18; i <= 18; i++) {
				double tmpVar = ((2 * Math.Pow (i - 3, 2)) + 4);

				if (tmpVar > 500) {
					continue;
				}
				PointD pt = new PointD(270 + 5*i, 230 - tmpVar);
				Console.WriteLine ("pt.X: {0}, pt.Y: {1}", pt.X, pt.Y);
				graph.Add (pt);
			}
			return graph;

		}

		// y = 2(x + 3)^2 + 4
		public static List<PointD> TwoXPlusThreeSqPlusfour()
		{
			int i;
			List<PointD> graph = new List<PointD> ();

			for (i = -18; i <= 18; i++) {
				double tmpVar = ((2 * Math.Pow (i + 3, 2)) + 4);

				if (tmpVar > 500) {
					continue;
				}
				PointD pt = new PointD(230 + 5*i, 270 - tmpVar);
				Console.WriteLine ("pt.X: {0}, pt.Y: {1}", pt.X, pt.Y);
				graph.Add (pt);
			}
			return graph;

		}

		// x^2 = 8y  === y = (x^2)/8
		public static List<PointD> xSqDiv8()
		{
			int i;
			List<PointD> graph = new List<PointD> ();

			for (i = -45; i <= 45; i++) {
				double tmpVar = ((Math.Pow (i, 2) / 8));

				if (tmpVar > 500) {
					continue;
				}
				PointD pt = new PointD(250 + 5*i, 250 - tmpVar);
				Console.WriteLine ("pt.X: {0}, pt.Y: {1}", pt.X, pt.Y);
				graph.Add (pt);
			}
			return graph;

		}

		// y = x^3
		public static List<PointD> xCubed()
		{
			double i;
			List<PointD> graph = new List<PointD> ();

			for (i = -15; i <= 15; i+=0.25) {
				double tmpVar = (Math.Pow (i, 3));


				PointD pt = new PointD(250 + 10*i, 250 - 0.10*tmpVar);
				Console.WriteLine ("pt.X: {0}, pt.Y: {1}", pt.X, pt.Y);
				graph.Add (pt);
			}
			return graph;

		}

		// y = sin(x)/x
		public static List<PointD> sinXdivX()
		{
			double i;
			List<PointD> graph = new List<PointD> ();

			for (i = -25; i <= 25; i+=0.25) {
				double tmpVar = (Math.Sin(i) / i);


				PointD pt = new PointD(250 + 10*i, 250 - 80*tmpVar);
				Console.WriteLine ("pt.X: {0}, pt.Y: {1}", pt.X, pt.Y);
				graph.Add (pt);
			}
			return graph;

		}

		//y = sin(x)
		public static List<PointD> sinX()
		{
			double i;
			List<PointD> graph = new List<PointD> ();

			for (i = -50; i <= 50; i+= 0.25) {
				double tmpVar = (Math.Sin(i));


				PointD pt = new PointD(250 + 10*i, 250 - 30*tmpVar);
				Console.WriteLine ("pt.X: {0}, pt.Y: {1}", pt.X, pt.Y);
				graph.Add (pt);
			}
			return graph;

		}


		//y = -sin(x)
		public static List<PointD> NegSinX()
		{
			double i;
			List<PointD> graph = new List<PointD> ();

			for (i = -50; i <= 50; i+= 0.25) {
				double tmpVar = -(Math.Sin(i));


				PointD pt = new PointD(250 + 10*i, 250 - 30*tmpVar);
				Console.WriteLine ("pt.X: {0}, pt.Y: {1}", pt.X, pt.Y);
				graph.Add (pt);
			}
			return graph;

		}

		//y = cos(x)
		public static List<PointD> cosX()
		{
			double i;
			List<PointD> graph = new List<PointD> ();

			for (i = -50; i <= 50; i+= 0.25) {
				double tmpVar = (Math.Cos(i));


				PointD pt = new PointD(250 + 10*i, 250 - 30*tmpVar);
				Console.WriteLine ("pt.X: {0}, pt.Y: {1}", pt.X, pt.Y);
				graph.Add (pt);
			}
			return graph;

		}


		//y = e^x
		public static List<PointD> eToX()
		{
			double i;
			List<PointD> graph = new List<PointD> ();

			for (i = -30; i <= 10; i+= 0.15) {
				
				double tmpVar = (Math.Pow(Math.E,i));

				if (10*tmpVar > 500 || 10*tmpVar < -500) {
					continue;
				}
				PointD pt = new PointD(250 + 10*i, 250 - 10*tmpVar);
				Console.WriteLine ("pt.X: {0}, pt.Y: {1}", pt.X, pt.Y);
				graph.Add (pt);
			}
			return graph;

		}


		//y = |x|
		public static List<PointD> absX()
		{
			double i;
			List<PointD> graph = new List<PointD> ();

			for (i = -10; i <= 10; i+= 0.25) {
				double tmpVar = (Math.Abs(i));
				if (tmpVar > 500) {
					continue;
				}

				PointD pt = new PointD(250 + 10*i, 250 - 30*tmpVar);
				Console.WriteLine ("pt.X: {0}, pt.Y: {1}", pt.X, pt.Y);
				graph.Add (pt);
			}
			return graph;

		}


	}
}


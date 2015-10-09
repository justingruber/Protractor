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
using System.Collections.Generic;
using Cairo;

//Most of the code is a C# implementation of the Protector Pseudo code found at
//http://yangl.org/protractor/protractor.pdf
namespace Protractor
{
	
	public class Recognizer
	{
		//List of templates to check against
		public static List<GestureTemplate.template> templateList = new List<GestureTemplate.template> ();

		//Is called once upon startup to clear out the list incase there is any garbage in it
		public static void setup(){
			if (templateList != null) {
				templateList.Clear ();
			}
		}

		//Entry point of the application to begin recognizing
		public Tuple<string,double> findMatchingTemplate(List<PointD> points)
		{
			//If no drawn points, return value not present
			if (points == null || points.Count == 0)
				return new Tuple<string, double> ("N/A", 0.0);

			//Resampling thhe drawn points into 64 evenly spaced points
			//Note: Number of points will change the accuracy, 64 was a bit better
			//even though the pseudo code says 16 is optimal
			List<PointD> resampledPoints = Resample (points, 64);

			//Turn the resampled points into a list of vectors
			List<double> vectoredPoints = Vectorize (resampledPoints, true);

			//Pass the vectorized list into the recognizer to be tested against the templates
			Tuple<string,double> result = Recognize (vectoredPoints);

			return result;
		}

		//C# implemtation of the pseudo code from http://yangl.org/protractor/protractor.pdf
		List<PointD> Resample (List<PointD> old_pts, int n)
		{
			int i;
			double len, dist, d = 0;
			List<PointD> pts = cloneList_PointD (old_pts);
			List<PointD> newPoints = new List<PointD>();
			newPoints.Add(pts[0]);

			len = PathLength (pts) / (n - 1);


			for(i = 1; i < pts.Count; i++)
			{
				dist = CalculateDistance (pts[i-1], pts[i]);

				if (dist + d >= len) {
					PointD q = new PointD();
					q.X = pts[i-1].X + (((len-d) / dist) * (pts[i].X - pts[i-1].X));
					q.Y = pts[i-1].Y + (((len-d) / dist) * (pts[i].Y - pts[i-1].Y));

					newPoints.Add(q);
					pts.Insert(i,q);
					d = 0;
				} else {
					d = dist + d;
				}
			}
			return newPoints;
		}

		//C# implemtation of the pseudo code from http://yangl.org/protractor/protractor.pdf
		double PathLength(List<PointD> A)
		{
			double dist = 0;
			int i;

			for(i = 1; i < A.Count; i++)
			{
				dist += (CalculateDistance(A[i-1],A[i]));
			}
			return dist;
		}

		List<double> Vectorize (List<PointD> pts, bool oSensitive)
		{
			PointD centroid = Centroid (pts);
			List<PointD> translatedPoints = Translate (pts, centroid);
			List<double> vector = new List<double>();
			double indicativeAngle = Math.Atan2 (translatedPoints [0].Y, translatedPoints [0].X);
			double delta, sum, magnitude;
			double baseOrientation = 0;
			int i;

			if(oSensitive){
				baseOrientation = (Math.PI / 4) * Math.Floor (indicativeAngle + (Math.PI / 8) / (Math.PI / 4));
				delta = baseOrientation - indicativeAngle;
			} else {
				delta = -1 * indicativeAngle;
			}
			sum = 0;

			foreach (PointD pt in translatedPoints) {
				double newX = pt.X * Math.Cos (delta) - pt.Y * Math.Sin (delta);
				double newY = pt.Y * Math.Cos (delta) + pt.X * Math.Sin (delta);
				vector.Add (newX);
				vector.Add (newY);
				sum = sum + (newX * newX) + (newY * newY);
			}

			magnitude = Math.Sqrt (sum);

			for (i = 0; i < vector.Count; i++) {
				vector[i] = vector[i] / magnitude;
			}
			return vector;
		}

		Tuple<string,double> Recognize (List<double> vector)
		{
			double maxScore = 0, currentScore = 0;
			double distance;
			int i;
			string match = "";

			for (i = 0; i < templateList.Count; i++) {
				distance = Optimal_Cosine_Distance (templateList [i].points, vector);
				//Console.Write ("Distance: {0}, ", distance);
				currentScore = 1 / distance;
				//Console.WriteLine ("currentScore: {0}, currentTemplate: {1}", currentScore, templateList[i].name);
				if (currentScore > maxScore) {
					maxScore = currentScore;
					match = templateList [i].name;
				}
			}

			Tuple<string,double> result = new Tuple<string, double>(match, maxScore);
			return result;
		}

		double Optimal_Cosine_Distance(List<PointD> templateVector, List<double> drawnVector)
		{
			double a = 0, b = 0;
			int i;
			int len;
			
			List<double> vectorizedTemplate = Vectorize (templateVector, true);

			//Used so that way there is no out of index error when a user has drawn more or
			//less points then that of the template
			if (vectorizedTemplate.Count < drawnVector.Count) {
				len = vectorizedTemplate.Count;
			} else {
				len = drawnVector.Count;
			}

			for (i = 0; i < len - 1; i += 2) {
				a += vectorizedTemplate [i] * drawnVector [i] + vectorizedTemplate [i + 1] * drawnVector [i + 1];
				b += vectorizedTemplate [i] * drawnVector [i + 1] - vectorizedTemplate [i + 1] * drawnVector [i];
			}

			if (a != 0) {

				double angle = Math.Atan (b/a);
				double cosine = Math.Cos (angle);
				double sine = Math.Sin (angle);

				return Math.Acos(a * cosine + b * sine);
			} else {
				return (Math.PI / 2);
			}
		}

		double CalculateDistance(PointD A, PointD B)
		{
			return (
				Math.Sqrt(
					Math.Pow(B.X - A.X, 2) + Math.Pow(B.Y - A.Y, 2)
				)
			);
		}

		PointD Centroid ( List<PointD> old_pts)
		{
			List<PointD> pts = cloneList_PointD (old_pts);

			PointD centroid = new PointD() { X = 0.0, Y = 0.0 };
			double signedArea = 0.0;
			double x0 = 0.0; // Current vertex X
			double y0 = 0.0; // Current vertex Y
			double x1 = 0.0; // Next vertex X
			double y1 = 0.0; // Next vertex Y
			double a = 0.0;  // Partial signed area

			// For all pts except last
			int i;
			for (i = 0; i < pts.Count - 1; ++i)
			{
				x0 = pts[i].X;
				y0 = pts[i].Y;
				x1 = pts[i+1].X;
				y1 = pts[i+1].Y;
				a = x0*y1 - x1*y0;
				signedArea += a;
				centroid.X += (x0 + x1)*a;
				centroid.Y += (y0 + y1)*a;
			}

			// Do last vertex
			x0 = pts[i].X;
			y0 = pts[i].Y;
			x1 = pts[0].X;
			y1 = pts[0].Y;
			a = x0*y1 - x1*y0;
			signedArea += a;
			centroid.X += (x0 + x1)*a;
			centroid.Y += (y0 + y1)*a;

			signedArea *= 0.5;

			centroid.X /= (6*signedArea);
			centroid.Y /= (6*signedArea);
			//Console.WriteLine ("x: " + centroid.X + " y: " + centroid.Y);
			return centroid;
		}

		List<PointD> Translate (List<PointD> old_pts, PointD centroid)
		{
			List<PointD> pts = cloneList_PointD (old_pts);
			int size = pts.Count;
			int i;
			if (size % 2 == 1) {
				size -= 1;
			}
			
			for (i = 0; i < size; i += 2) {
				PointD tmpPointX = new PointD (pts[i].X + centroid.X, pts[i].Y);
				PointD tmpPointY = new PointD (pts[i].X, pts[i].Y + centroid.Y);
				pts [i] = tmpPointX;
				pts [i + 1] = tmpPointY;
			}
			return pts;
		}

		//Used to clone list with type PointD because of C# passing by refence and not value
		private List<PointD> cloneList_PointD(List<PointD> toCopy){
			List<PointD> tempList = new List <PointD> ();
			foreach (PointD pt in toCopy) {
				tempList.Add (new PointD(pt.X, pt.Y));
			}
			return tempList;
		}

		//Used to clone list with type double because of C# passing by refence and not value
		private List<double> cloneList_Double(List<double> toCopy){
			List<double> tempList = new List <double> ();
			foreach (double pt in toCopy) {
				tempList.Add (pt);
			}
			return tempList;
		}
	}
}
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
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using Cairo;

namespace Protractor
{
	public class XMLParser
	{
		//Loads in gestures from the given file and returns a template struct
		public static GestureTemplate.template LoadGesture (string fileName)
		{
			List<PointD> points = new List<PointD> ();
			XmlTextReader xmlReader = null;
			string gestureName = "";

			try
			{
				//Initialize XML reader
				xmlReader = new XmlTextReader(File.OpenText(fileName));

				while (xmlReader.Read())
				{
					if (xmlReader.NodeType != XmlNodeType.Element) continue;
					switch(xmlReader.Name)
					{
						case "Gesture":
							gestureName = xmlReader["Name"];
							break;
						case "Point":
							points.Add(new PointD(
							float.Parse(xmlReader["X"]),
							float.Parse(xmlReader["Y"])
							));
							break;
					}
				}

				//Output saying name of template and how many points it has in it
				Console.WriteLine("Name: {0}, Points: {1} was loaded", gestureName, points.Count);
			} finally{
				if (xmlReader != null) {
					xmlReader.Close ();
				}
			}
			return new GestureTemplate.template (gestureName, points);
		}

		//For saving a drawn set of points into a new template file
		public static void WriteGesture(List<PointD> points, string gestureName, string fileName)
		{
			using (StreamWriter sw = new StreamWriter(fileName))
			{
				int i;
				//Header for XML
				sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>");
				
				sw.WriteLine("<Gesture Name=\"{0}\">", gestureName);
				sw.WriteLine ("\t<Stroke>");
				for (i = 0; i < points.Count; i++) {
					sw.WriteLine("\t\t<Point X=\"{0}\" Y=\"{1}\" />",
						points[i].X, points[i].Y);
				}
				sw.WriteLine("\t</Stroke>");
				sw.WriteLine("</Gesture>");
				sw.Close ();
			}
		}

		//Loads in points from any properly formatted XML file and returns the points to be drawn
		public static List<PointD> LoadPlotPoints (string fileName)
		{
			List<PointD> points = new List<PointD> ();
			XmlTextReader xmlReader = null;
			string gestureName = "";

			try
			{
				xmlReader = new XmlTextReader(File.OpenText(fileName));

				while (xmlReader.Read())
				{
					if (xmlReader.NodeType != XmlNodeType.Element) continue;
					switch(xmlReader.Name)
					{
					case "Gesture":
						gestureName = xmlReader["Name"];
						break;
					case "Point":
						points.Add(new PointD(
							float.Parse(xmlReader["X"]),
							float.Parse(xmlReader["Y"])
						));
						break;
					}
				}
				Console.WriteLine("Name: {0}, Points: {1} was loaded", gestureName, points.Count);
			} finally{
				if (xmlReader != null) {
					xmlReader.Close ();
				}
			}
			return points;
		}

		//Same as load plot points, except returns a tuple with the gesture name for 
		//the file print out for batch testing
		public static Tuple<string,List<PointD>> BatchLoadPoints (string fileName)
		{
			List<PointD> points = new List<PointD>();

			XmlTextReader xmlReader = null;
			string gestureName = "";

			try
			{
				xmlReader = new XmlTextReader(File.OpenText(fileName));

				while (xmlReader.Read())
				{
					if (xmlReader.NodeType != XmlNodeType.Element) continue;
					switch(xmlReader.Name)
					{
					case "Gesture":
						gestureName = xmlReader["Name"];
						break;
					case "Point":
						points.Add(new PointD(
							float.Parse(xmlReader["X"]),
							float.Parse(xmlReader["Y"])
						));
						break;
					}
				}
				//Console.WriteLine("Name: {0}, Points: {1} was loaded", gestureName, points.Count);
			} finally{
				if (xmlReader != null) {
					xmlReader.Close ();
				}
			}

			Tuple<string,List<PointD>> result = new Tuple<string, List<PointD>>(gestureName, points);

			return result;
		}

		//For the quick-save of recorded points to make a quick record of points to be used for batch
		//testing 
		public static void savePlotPoints(List<PointD> points, string functionName){
			string tmp = System.IO.Directory.GetCurrentDirectory();
			string newTmp = tmp + "/../../Gestures/RecordedDataPoints/";
			System.IO.Directory.SetCurrentDirectory (newTmp);

			string time = DateTime.Now.ToString ("yyyy-MMMMM-dd_hh-mm-ss");

			if (functionName != "")
				functionName += "_";

			string fileName = System.IO.Directory.GetCurrentDirectory() + "/" + functionName + "Custom_Points_" + time + ".xml";

			//Create the file and immediately close the file stream because create returns an 
			//open file stream which causes errors when trying to create a new stream writer
			FileStream f = File.Create (fileName);
			f.Close ();

			using (StreamWriter sw = new StreamWriter(fileName))
			{
				int i;
				sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>");
				sw.WriteLine("<Gesture Name=\"{0}\" timestamp={1}>", "Custom_Points", "\"" + time + "\"");
				sw.WriteLine ("\t<Stroke>");
				for (i = 0; i < points.Count; i++) {
					sw.WriteLine("\t\t<Point X=\"{0}\" Y=\"{1}\" />",
						points[i].X, points[i].Y);
				}
				sw.WriteLine("\t</Stroke>");
				sw.WriteLine("</Gesture>");
				sw.Close ();
			}
		}

		//Writes the output from having batch results in the pre-named file
		public static void writeBatchResult(List<Tuple<string, double>> results,  string functionName){
			string tmp = System.IO.Directory.GetCurrentDirectory();
			Console.WriteLine (tmp);
			string newTmp = tmp + "/../../Gestures/RecordedDataPoints/";
			System.IO.Directory.SetCurrentDirectory (newTmp);

			string time = DateTime.Now.ToString ("yyyy-MMMMM-dd_hh-mm-ss");

			if (functionName == "Recorded points function name") {
				functionName = "";
			}

			if (functionName != "")
				functionName += "_";

			string fileName = System.IO.Directory.GetCurrentDirectory() + "/Batch-Result_" + functionName + time + ".xml";

			//Create the file and immediately close the file stream because create returns an 
			//open file stream which causes errors when trying to create a new stream writer
			FileStream f = File.Create (fileName);
			f.Close ();

			using (StreamWriter sw = new StreamWriter(fileName))
			{
				int i;
				sw.WriteLine ("Protractor results for function: " + functionName);
				for (i = 0; i < results.Count; i++) {
					sw.WriteLine(i + ": recognized as: " + results[i].Item1 + " score: " + results[i].Item2);
				}
				sw.Close ();
			}
		}
	}
}
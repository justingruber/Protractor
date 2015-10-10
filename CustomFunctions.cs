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
using Gtk;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Drawing;
using Cairo;
using System.Collections.Generic;

namespace Protractor
{
	public class CustomFunctions
	{

		/** Opens the file dialogue which is used to load in new templates
		 *  for drawings to be matched against. It loads in the XML file and adds to the 
		 *  template list.
		 * 
		 * Note: The file paths have only been tested on Windows
		 */ 

		public static void openLoadDialogue()
		{
			//Initial setup of the dialogue with settings
			FileChooserDialog fileChooser = new FileChooserDialog ("Open File", null, FileChooserAction.Open);
			fileChooser.AddButton (Stock.Cancel, ResponseType.Cancel);
			fileChooser.AddButton (Stock.Open, ResponseType.Ok);

			fileChooser.Filter = new FileFilter ();
			fileChooser.Filter.AddPattern ("*.xml");

			string tmp = System.IO.Directory.GetCurrentDirectory();
			string newTmp = tmp + "/../../Gestures/";
			fileChooser.SetCurrentFolder (newTmp);

			fileChooser.SelectMultiple = true;

			ResponseType retVal = (ResponseType)fileChooser.Run ();

			//If the user preses 'Ok', it calls the XML parser for loading in files 
			//and adds to the list.
			if (retVal == ResponseType.Ok) {
				foreach (string file in fileChooser.Filenames) {
					Recognizer.templateList.Add (XMLParser.LoadGesture (file));
				}
				//Uncomment if printout of full list of loaded gestures is wanted
				/*foreach (GestureTemplate.template tmpl in Recognizer.templateList){
					Console.WriteLine ("name: {0}", tmpl.name);
				}*/
			}

			fileChooser.Destroy ();
		}


		/** Opens the save dialogue and takes in a list of type PointD and saves
		 * the list of points to the specified XML file. 
		 * 
		 * Points: List of user-drawn points on the drawing area
		 * 
		 * Note: The file paths have only been tested on Windows
		 */
		public static void openSaveDialogue(List<PointD> points)
		{
			//Initial setup of the save dialog
			FileChooserDialog fileChooser = new FileChooserDialog ("Save", null, FileChooserAction.Save);
			fileChooser.AddButton (Stock.Cancel, ResponseType.Cancel);
			fileChooser.AddButton (Stock.Ok, ResponseType.Ok);

			string tmp = System.IO.Directory.GetCurrentDirectory();
			string newTmp = tmp + "/../../Gestures/";
			fileChooser.SetCurrentFolder (newTmp);

			fileChooser.SelectMultiple = false;

			ResponseType retVal = (ResponseType)fileChooser.Run ();

			//String manipluation to get the correct file name being saved
			if (retVal == ResponseType.Ok) {
				string saveFileName = "";
				string[] strippedFileName = { "", "" };

				string newFile = fileChooser.Filename.Trim ();
				if (!newFile.EndsWith(".xml", true, null))
				{
					int nameLength = newFile.Length - newFile.LastIndexOf ("\\");
					saveFileName = newFile.Substring(newFile.LastIndexOf("\\"), nameLength);
					strippedFileName = saveFileName.Split('\\');
					Console.WriteLine (strippedFileName[1]);
					newFile += ".xml";
				}
				if (File.Exists (newFile)) {
					File.Delete (newFile);
				}
				FileStream fs = File.Create (newFile);
				fs.Close ();
				XMLParser.WriteGesture (points, strippedFileName[1], newFile);

				GestureTemplate.template newTemplate = new GestureTemplate.template (strippedFileName[1], points);
				
				Recognizer.templateList.Add (newTemplate);
			}

			fileChooser.Destroy ();
		}

		//Wrapper for XML call to save custom plot points with no specified file name
		public static void saveRecordedPoints(List<PointD> points, string functionName){
			XMLParser.savePlotPoints (points, functionName);
		}


		//Loads data points into the user-drawn list of points
		//Used for loading in multiple sets of data points for bulk-recognizing
		//Returns the data points to be added
		public static List<List<PointD>> loadPoints(){
			
			List<List<PointD>> newLoadedPoints = new List<List<PointD>>();

			//Setup of dialog and limiting it to only XML files
			FileChooserDialog fileChooser = new FileChooserDialog ("Open File", null, FileChooserAction.Open);
			fileChooser.AddButton (Stock.Cancel, ResponseType.Cancel);
			fileChooser.AddButton (Stock.Open, ResponseType.Ok);

			fileChooser.Filter = new FileFilter ();
			fileChooser.Filter.AddPattern ("*.xml");

			string tmp = System.IO.Directory.GetCurrentDirectory();
			string newTmp = tmp + "/../../Gestures/RecordedDataPoints/";
			fileChooser.SetCurrentFolder (newTmp);

			fileChooser.SelectMultiple = true;

			ResponseType retVal = (ResponseType)fileChooser.Run ();

			if (retVal == ResponseType.Ok) {
				foreach (string file in fileChooser.Filenames) {
					newLoadedPoints.Add( XMLParser.LoadPlotPoints (file));
				}
			}

			fileChooser.Destroy ();
			return newLoadedPoints;
		}


		//Will load in multiple files, or a folder of files which will be matched against
		//loaded templates
		public static List<Tuple<string,List<PointD>>> loadBatchPoints(){

			List<Tuple<string,List<PointD>>> result = new List<Tuple<string,List<PointD>>> ();

			//Setup of dialog and limiting it to only XML files
			FileChooserDialog fileChooser = new FileChooserDialog ("Open Files", null, FileChooserAction.Open);
			fileChooser.AddButton (Stock.Cancel, ResponseType.Cancel);
			fileChooser.AddButton (Stock.Open, ResponseType.Ok);

			fileChooser.Filter = new FileFilter ();
			fileChooser.Filter.AddPattern ("*.xml");

			string tmp = System.IO.Directory.GetCurrentDirectory();
			string newTmp = tmp + "/../../Gestures/RecordedDataPoints/";
			fileChooser.SetCurrentFolder (newTmp);

			fileChooser.SelectMultiple = true;

			ResponseType retVal = (ResponseType)fileChooser.Run ();

			if (retVal == ResponseType.Ok) {
				foreach (string file in fileChooser.Filenames) {
					result.Add (XMLParser.BatchLoadPoints (file));
				}
			}

			fileChooser.Destroy ();
			return result;
		}

		//Writes the result to a file using the result, and the function that was tested as the 
		//start of the file name
		public static void writeBatchTestingResult(List<Tuple<string, double>> result, string functionName){
			XMLParser.writeBatchResult (result, functionName);
		}
	}
}
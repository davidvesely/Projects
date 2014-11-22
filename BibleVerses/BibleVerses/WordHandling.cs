using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using BibleVerses.Models;
using Microsoft.Office.Interop.Word;
using WordApplication = Microsoft.Office.Interop.Word.Application;

namespace BibleVerses
{
    public class WordHandling
    {
        private static int startTextIndex = 1;
        private static readonly bool ShouldAppend = false;

        public static void CreateDoc(List<BibleVerse> verseList, StreamWriter fileOutput)
        {
#if DEBUG
            string fileName = "D:\\test.docx";
            Console.WriteLine("Filename is {0}\n", fileName);
#else
            Console.Write("\nPlease enter the destination docx file: ");
            string fileName = Console.ReadLine();
#endif
            bool isNewFile = true;
            WordApplication application = new WordApplication();
            // Delete the file before writing
            if (!ShouldAppend)
                File.Delete(fileName);

            if (File.Exists(fileName))
            {
                application.Documents.Open(fileName);
                isNewFile = false;
            }
            else
            {
                application.Documents.Add();
            }

            object pageBreak = WdBreakType.wdPageBreak;
            Paragraph paragraph;
            for (int i = 0; i < verseList.Count; i++)
            {
                // The number of text
                paragraph = application.ActiveDocument.Content.Paragraphs.Add();
                paragraph.Range.Text = (i + startTextIndex).ToString();
                paragraph.set_Style(application.ActiveDocument.Styles["Heading 1"]);
                paragraph.Range.InsertParagraphAfter();
                // Verse
                paragraph = application.ActiveDocument.Content.Paragraphs.Add();
                paragraph.Range.Text = verseList[i].Text;
                paragraph.Range.InsertParagraphAfter();
                // Place
                paragraph = application.ActiveDocument.Content.Paragraphs.Add();
                paragraph.Range.Text = verseList[i].BiblePlace.FullLocation;
                paragraph.set_Style(application.ActiveDocument.Styles["Heading 2"]);
                paragraph.Range.InsertParagraphAfter();

                // Page Break
                if (i < verseList.Count - 1)
                    paragraph.Range.InsertBreak(ref pageBreak);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(paragraph);

                // Check for overlapping
                BiblePlace currentPlace = verseList[i].BiblePlace;
                var q = verseList.GetRange(0, i).Where(a => a.BiblePlace.IsOverlaping(currentPlace));
                if (q.Any())
                {
                    string output = string.Format("Row {0}: Place {1} is overlapping the following places: {2}\n",
                        i + 1, currentPlace.ToString(), string.Join("; ", q.Select(a => a.BiblePlace)));
                    Console.WriteLine(output);
                    fileOutput.WriteLine(output);
                }
            }

            try
            {
                if (isNewFile)
                    application.ActiveDocument.SaveAs(fileName);
                else
                    application.ActiveDocument.Save();
                application.ActiveDocument.Close();
                application.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(application);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch
            {
                paragraph = null;
                application = null;
            }
        }

        public static void InsertPictures()
        {
            string fileName = "D:\\Obeshtaniq.docx";
            string picName = "D:\\picture.jpg";
            File.Copy("D:\\backup.docx", fileName, true);

            Application application = null;
            try
            {
                application = new Application();
                application.Documents.Open(fileName);
                Document doc = application.ActiveDocument;
                object linkToFile = false;
                object saveWithFile = true;
                object missing = Type.Missing;

                int shapeNum = 1;
                for (int i = 1; i < doc.Paragraphs.Count; i += 4)
                {
                    Range fourth = doc.Paragraphs[i].Range;
                    Shape pic = doc.Shapes.AddPicture(picName, linkToFile, saveWithFile, 0, 315, 326, 165, fourth);
                    pic.Shadow.OffsetX = 17f;
                    pic.Shadow.OffsetY = 17f;
                    pic.Shadow.Style = Microsoft.Office.Core.MsoShadowStyle.msoShadowStyleOuterShadow;
                    pic.Shadow.Transparency = 0.5f;
                    pic.Shadow.Type = Microsoft.Office.Core.MsoShadowType.msoShadow21;
                    pic.SoftEdge.Radius = 7f;
                    if (shapeNum == 44)
                        shapeNum = 1;
                    Console.WriteLine(i);
                    if (i > 135)
                        break;
                }

                application.ActiveDocument.Save();
            }
            finally
            {
                application.ActiveDocument.Close();
                application.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(application);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            System.Diagnostics.Process.Start(fileName);
        }
    }
}
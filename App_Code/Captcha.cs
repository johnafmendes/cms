﻿/* CLASS NAME: Captcha.cs
 * CREATED BY: John Mendes - johnafmendes@gmail.com
 * DATE CREATION: 00/00/0000 - 00:00
 * The objective this class is create and validate captcha function in backend side.
 * ==========================
 * Curiosity: It is a contrived acronym for "Completely Automated Public Turing test to 
 * tell Computers and Humans Apart."
 */

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;

using System.Data;
using System.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Threading;

/// <summary>
/// Summary description for Captcha
/// </summary>
public partial class Captcha : System.Web.UI.Page
{
    public string[] Option = new string[6] {"Type the numbers only", "Type the letters only", 
        "Type the first and the last character", "Type the first 4 character", "type the last 3 character", 
        "Type the full code"};
    private static string _numbers;//List of Numbers
    private static string _letters;//List of Letters
    private static string _fullcode;//List of Numbers and Letters
    private static string _OptionChoose; //Option choosed to ask to user.

    public string numbers { set { _numbers = value; } get { return _numbers; } }
    public string letters { set { _letters = value; } get { return _letters; } }
    public string fullcode { set { _fullcode = value; } get { return _fullcode; } }
    public string OptionChoose { set { _OptionChoose = value; } get { return _OptionChoose; } }

    //Constructor: Clean the variables and create a random number between 0 and 5 as a Option.
	public Captcha()
	{
        _numbers = "";
        _letters = "";
        _fullcode = "";
        _OptionChoose = "";
        Random r = new Random();
        _OptionChoose = r.Next(0, 5).ToString();
    }

    //Return the Option choosed as String
    public string ReturnRequirement()
    {
        return Option[Int32.Parse(_OptionChoose)].ToString();
    }

    /*Reproduce sound. First select whitch kind of Option must to reproduce.
     * Second will be reproduced each character in CodeSound.
     */
    public void reproduceSound(object sender, EventArgs e)
    {
        SpeechLib.SpVoiceClass m_TTS = new SpeechLib.SpVoiceClass();
        string codeSound = "";
        if (_OptionChoose == "0") codeSound = _numbers;
        if (_OptionChoose == "1") codeSound = _letters;
        if (_OptionChoose == "2") codeSound = _fullcode.Substring(0, 1).ToString() + _fullcode.Substring(6, 1).ToString();
        if (_OptionChoose == "3") codeSound = _fullcode.Substring(0, 4).ToString();
        if (_OptionChoose == "4") codeSound = _fullcode.Substring(4, 3).ToString();
        if (_OptionChoose == "5") codeSound = _fullcode;

        for (int i = 0; i < codeSound.Length; i++)
        {
            char codepart = codeSound.Substring(i, 1).ToCharArray()[0];
            string UpperLowerCase = "";
            if (Char.IsLetter(codepart))
            {
                if (Char.IsUpper(codepart)) UpperLowerCase = "Upper Case";
                else UpperLowerCase = "Lower Case";
                m_TTS.Speak(UpperLowerCase, SpeechLib.SpeechVoiceSpeakFlags.SVSFDefault);
            }
            m_TTS.Speak(codeSound.Substring(i, 1).ToString(), SpeechLib.SpeechVoiceSpeakFlags.SVSFlagsAsync);
            m_TTS.WaitUntilDone(Timeout.Infinite);
            //m_TTS.Speak(codeSound.Substring(i, 1).ToString(), SpeechLib.SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechLib.SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
        }
        //Response.Write("");
    }

    /*
     Validade Code typed by user with code generated by system and return true or false.
     */
    public bool validateCode(string Code)
    {
        bool result = false;

        switch (_OptionChoose)
        {
            case "0":
                {
                    if (Code.CompareTo(_numbers) == 0) result = true;
                    else result = false;
                    break;
                }
            case "1":
                {
                    if (Code.CompareTo(_letters) == 0) result = true;
                    else result = false;
                    break;
                }
            case "2":
                {
                    if (Code.CompareTo((_fullcode.Substring(0, 1).ToString() + _fullcode.Substring(6, 1).ToString())) == 0) result = true;
                    else result = false;
                    break;
                }
            case "3":
                {
                    if (Code.CompareTo(_fullcode.Substring(0, 4).ToString()) == 0) result = true;
                    else result = false;
                    break;
                }
            case "4":
                {
                    if (Code.CompareTo(_fullcode.Substring(4, 3).ToString()) == 0) result = true;
                    else result = false;
                    break;
                }
            case "5":
                {
                    if (Code.CompareTo(_fullcode) == 0) result = true;
                    else result = false;
                    break;
                }
        }
        return result;
    }

    /*
     Create the empty Image and write the code generated by the system into it
     * and return the image to page into iframe.
     */
    public void returnImage(HttpContext hc)
    {
        hc.Response.ContentType = "image/jpeg";
        hc.Response.Clear();

        // Buffer response so that page is sent
        // after processing is complete.
        hc.Response.BufferOutput = true;

        // Create a font style.
        Font rectangleFont = new Font(
            "Arial", 14, FontStyle.Bold);

        // Create integer variables.
        int height = 40;
        int width = 110;

        /*int height = 140;
        int width = 310;*/

        // Create a random number generator and create
        // variable values based on it.
        Random r = new Random();
        int x = r.Next(40);
        int a = r.Next(110);
        int x1 = r.Next(50);

        // Create a bitmap and use it to create a
        // Graphics object.
        Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        Graphics g = Graphics.FromImage(bmp);

        //g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Color.Teal);

        // Use the Graphics object to draw three rectangles.
        //g.DrawRectangle(Pens.White, 1, 1, width - 3, height - 3);
        //g.DrawRectangle(Pens.Aquamarine, 2, 2, width - 3, height - 3);
        //g.DrawRectangle(Pens.Black, 0, 0, width, height);

        // Apply color to two of the rectangles.
        /*g.FillRectangle(new SolidBrush(Color.FromArgb(RandomNumber(0, 255), RandomNumber(0, 255),
            RandomNumber(0, 255))), 
            RandomNumber(0, width), RandomNumber(0, height), RandomNumber(0, width), RandomNumber(0, height));

        g.FillRectangle(new LinearGradientBrush(new Point(0, 10), new Point(0, 30),
            Color.FromArgb(RandomNumber(0, 255), RandomNumber(0, 255), RandomNumber(0, 255)),
            Color.FromArgb(RandomNumber(0, 255), RandomNumber(0, 255), RandomNumber(0, 255))), 
            RandomNumber(0, height), RandomNumber(0, width), RandomNumber(0, width), RandomNumber(0, height));
        */
        string fullcode = GetRandonStringKey(7);
        g.DrawString(fullcode.ToString(), rectangleFont, SystemBrushes.Window, new PointF(10, 10));



        /*string fullcode = "A";
        int positiveRotator = 45, negativeRotator = -45, updown = 0;
        for (int i = 0; i < fullcode.Length; i++)
        {*/
        /*if (i % 2 == 0)
        {
            g.RotateTransform(positiveRotator);
            updown = i * -4;
        }
        else
        {
            g.RotateTransform(negativeRotator);
            updown = i * 4;
        }*/
        //g.DrawString(fullcode.Substring(i, 1).ToString(), rectangleFont, SystemBrushes.Window, new PointF((i*(10))+10, 50+updown));
        /*g.RotateTransform(45);
        g.DrawString(fullcode.Substring(i, 1).ToString(), rectangleFont, SystemBrushes.Window, new PointF(50, 30));
        g.RotateTransform(-45);

        g.RotateTransform(45);
        g.DrawString("b", rectangleFont, SystemBrushes.Window, new PointF(60, 20));
        g.RotateTransform(-45);

        g.RotateTransform(45);
        g.DrawString("c", rectangleFont, SystemBrushes.Window, new PointF(70, 10));
        g.RotateTransform(-45);

        g.RotateTransform(45);
        g.DrawString("d", rectangleFont, SystemBrushes.Window, new PointF(80, 0));
        g.RotateTransform(-45);*/
        //if (i % 2 == 0) g.RotateTransform(negativeRotator); else g.RotateTransform(positiveRotator);
        //}
        //Pen myPen = new Pen(Color.White/*Color.FromArgb(RandomNumber(0, 255), RandomNumber(0, 255), RandomNumber(0, 255))*/);
        //g.DrawLine(myPen, RandomNumber(0, width), RandomNumber(0, height), RandomNumber(0, width), RandomNumber(0, height));
        //int xa = RandomNumber(1, width), xb = RandomNumber(1, width), ya = RandomNumber(1, height), yb = RandomNumber(1, height);
        //g.DrawLine(myPen, 106, 8, 100, 16);
        //g.DrawLine(myPen, 110, 40, 0, 10);
        //g.DrawLine(myPen, 106, 8, 100, 16);
        //g.DrawLine(myPen, 106, 8, 100, 16);
        // Save the bitmap to the response stream and
        // convert it to JPEG format.
        bmp.Save(hc.Response.OutputStream, ImageFormat.Jpeg);

        // Release memory used by the Graphics object
        // and the bitmap.
        g.Dispose();
        bmp.Dispose();

        // Send the output to the client.
        hc.Response.Flush();
    }

    //Generate a random number based into min and max range
    private int RandomNumber(int min, int max)
    {
        Random random = new Random();
        return random.Next(min, max);
    }

    //Generate a random alpha/string based into size
    private string RandomString(int size, bool lowerCase)
    {
        StringBuilder builder = new StringBuilder();
        Random random = new Random();
        char ch;
        for (int i = 0; i < size; i++)
        {
            ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
            builder.Append(ch);
        }
        if (lowerCase)
            return builder.ToString().ToLower();
        return builder.ToString();
    }

    //Generate a random alpha/string based into size
    private static string RandomString(int size, Random r)
    {
        string legalChars = "abcdefghijklmnopqrstuvwxzyABCDEFGHIJKLMNOPQRSTUVWXZY";
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < size; i++)
            sb.Append(legalChars.Substring(r.Next(0, legalChars.Length - 1), 1));
        return sb.ToString();
    }

    //Generate a random alpha/number string based into length
    public string GetRandonStringKey(int length)
    {
        StringBuilder builder = new StringBuilder();
        _letters = "";
        _numbers = "";
        while (builder.Length < length)
        {
            string tnumbers = "", tletters = "";
            Thread.Sleep(builder.Length * 10);
            Random r = new Random();
            int Num = r.Next(0, 10);
            if (Num % 2 == 0)
            {
                _letters += tletters = RandomString(1, new Random());
                //letters += tletters = RandomString(1, true);
                builder.Append(tletters);
            }
            else
            {
                _numbers += tnumbers = RandomNumber(0, 9).ToString();
                builder.Append(tnumbers);
            }
        }
        _fullcode = builder.ToString();
        return builder.ToString();
    }
}

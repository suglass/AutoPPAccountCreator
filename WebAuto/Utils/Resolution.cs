
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;

[StructLayout(LayoutKind.Sequential)]
public struct DEVMODE1 
{
	[MarshalAs(UnmanagedType.ByValTStr,SizeConst=32)] public string dmDeviceName;
	public short  dmSpecVersion;
	public short  dmDriverVersion;
	public short  dmSize;
	public short  dmDriverExtra;
	public int    dmFields;

	public short dmOrientation;
	public short dmPaperSize;
	public short dmPaperLength;
	public short dmPaperWidth;

	public short dmScale;
	public short dmCopies;
	public short dmDefaultSource;
	public short dmPrintQuality;
	public short dmColor;
	public short dmDuplex;
	public short dmYResolution;
	public short dmTTOption;
	public short dmCollate;
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string dmFormName;
	public short dmLogPixels;
	public short dmBitsPerPel;
	public int   dmPelsWidth;
	public int   dmPelsHeight;

	public int   dmDisplayFlags;
	public int   dmDisplayFrequency;

	public int   dmICMMethod;
	public int   dmICMIntent;
	public int   dmMediaType;
	public int   dmDitherType;
	public int   dmReserved1;
	public int   dmReserved2;

	public int   dmPanningWidth;
	public int   dmPanningHeight;
};



class User_32
{
	[DllImport("user32.dll")]
	public static extern int EnumDisplaySettings (string deviceName, int modeNum, ref DEVMODE1 devMode );         
	[DllImport("user32.dll")]
	public static extern int ChangeDisplaySettings(ref DEVMODE1 devMode, int flags);

	public const int ENUM_CURRENT_SETTINGS = -1;
	public const int CDS_UPDATEREGISTRY = 0x01;
	public const int CDS_TEST = 0x02;
	public const int DISP_CHANGE_SUCCESSFUL = 0;
	public const int DISP_CHANGE_RESTART = 1;
	public const int DISP_CHANGE_FAILED = -1;
}


namespace Resolution
{
	class CResolution
	{
        static public string ResolutionToString(int width, int height)
        {
            string x = string.Format("{0}x{1}", width, height);
            return x;
        }
        static public void StringToResolution(string screen, out int width, out int height)
        {
            width = 0;
            height = 0;

            try
            {
                string[] arr = screen.Split('x');
                if (arr.Length != 2)
                    throw new Exception("Invalid parameter");

                width = int.Parse(arr[0]);
                height = int.Parse(arr[1]);
            }
            catch (Exception ex)
            {
                width = 0;
                height = 0;
            }
        }
        static public void GetAvailableResolutions(List<int> widths, List<int> heights)
        {
            int mode = 0;

            DEVMODE1 dm = new DEVMODE1();
            dm.dmDeviceName = new String(new char[32]);
            dm.dmFormName = new String(new char[32]);
            dm.dmSize = (short)Marshal.SizeOf(dm);

            while (0 != User_32.EnumDisplaySettings(null, mode++, ref dm))
            {
                int i;
                for (i = 0; i < widths.Count; i++)
                {
                    if (widths[i] == dm.dmPelsWidth && heights[i] == dm.dmPelsHeight)
                        break;
                }
                if (i == widths.Count)
                {
                    widths.Add(dm.dmPelsWidth);
                    heights.Add(dm.dmPelsHeight);
                }
            }
        }
        static public List<string> GetAvailableResolutionsStrings()
        {
            List<string> ret = new List<string>();

            List<int> widths = new List<int>();
            List<int> heights = new List<int>();

            GetAvailableResolutions(widths, heights);

            for (int i = 0; i < widths.Count; i++)
            {
                ret.Add(ResolutionToString(widths[i], heights[i]));
            }

            return ret;
        }
        static public void GetCurrentResolution(out int width, out int height)
        {
            Screen screen = Screen.PrimaryScreen;
            width = screen.Bounds.Width;
            height = screen.Bounds.Height;
        }
        static public string GetCurrentResolutionString()
        {
            int width = 0;
            int height = 0;

            GetCurrentResolution(out width, out height);

            return ResolutionToString(width, height);
        }
        static public bool ChangeResolution(string resolution)
        {
            int width = 0;
            int height = 0;
            StringToResolution(resolution, out width, out height);
            return ChangeResolution(width, height);
        }
        static public bool ChangeResolution(int width, int height)
		{
			Screen screen = Screen.PrimaryScreen;
			
			int iWidth = width;
			int iHeight = height;

			DEVMODE1 dm = new DEVMODE1();
			dm.dmDeviceName = new String (new char[32]);
			dm.dmFormName = new String (new char[32]);
			dm.dmSize = (short)Marshal.SizeOf (dm);

            if (0 == User_32.EnumDisplaySettings(null, User_32.ENUM_CURRENT_SETTINGS, ref dm))
                return false;
				
			dm.dmPelsWidth = iWidth;
			dm.dmPelsHeight = iHeight;

			int iRet = User_32.ChangeDisplaySettings (ref dm, User_32.CDS_TEST);
			if (iRet == User_32.DISP_CHANGE_FAILED)
                return false;

			iRet = User_32.ChangeDisplaySettings (ref dm, User_32.CDS_UPDATEREGISTRY);
            if (iRet == User_32.DISP_CHANGE_SUCCESSFUL)
                return true;

            if (iRet == User_32.DISP_CHANGE_RESTART)
            {
                // To Do.
            }
            return false;
		}
	}
}

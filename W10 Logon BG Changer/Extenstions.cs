﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using DColor = System.Drawing.Color;
using MColor = System.Windows.Media.Color;

namespace W10_Logon_BG_Changer
{
    public static class Extenstions
    {
        public static MColor ToMediaColor(this DColor color)
        {
            return MColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static BitmapSource ToBitmapSource(this Image source)
        {
            var bitmap = new Bitmap(source);

            var bitSrc = bitmap.ToBitmapSource();

            bitmap.Dispose();

            return bitSrc;
        }

        public static BitmapSource ToBitmapSource(this Bitmap source)
        {
            BitmapSource bitSrc;

            var hBitmap = source.GetHbitmap();

            try
            {
                bitSrc = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Win32Exception)
            {
                bitSrc = null;
            }
            finally
            {
                NativeMethods.DeleteObject(hBitmap);
            }

            return bitSrc;
        }

        public static bool IsImage(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            var jpg = new List<string> { "FF", "D8" };
            var bmp = new List<string> { "42", "4D" };
            var gif = new List<string> { "47", "49", "46" };
            var png = new List<string> { "89", "50", "4E", "47", "0D", "0A", "1A", "0A" };
            var imgTypes = new List<List<string>> { jpg, bmp, gif, png };

            var bytesIterated = new List<string>();

            for (var i = 0; i < 8; i++)
            {
                var bit = stream.ReadByte().ToString("X2");
                bytesIterated.Add(bit);

                var isImage = imgTypes.Any(img => !img.Except(bytesIterated).Any());
                if (isImage)
                {
                    return true;
                }
            }

            return false;
        }

        internal static class NativeMethods
        {
            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool DeleteObject(IntPtr hObject);
        }
    }
}
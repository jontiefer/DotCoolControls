/* Copyright © 2016 Jonathan Tiefer - All Rights Reserved
 * You may use, distribute and modify this code under the
 * terms of the GNU Lesser General Public License (LGPL)
 *
 * You should have received a copy of the LGPL license with
 * this file.
 *
 * /

/*  This file is part of DotCoolControls
*
*   DotCoolControls is free software: you can redistribute it and/or modify
*   it under the terms of the GNU Lesser General Public License as published by
*   the Free Software Foundation, either version 3 of the License, or
*    (at your option) any later version.
*
*   DotCoolControls is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*   GNU Lesser General Public License for more details.
*
*  You should have received a copy of the GNU Lesser General Public License
*   along with DotCoolControls.  If not, see <http://www.gnu.org/licenses/>.
*/

using Tiferix.Global;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotCoolControls.Tools
{
    #region Enumerations

    /// <summary>
    /// Style/Pattern of gradient to use in the various set of DotCool controls.
    /// </summary>
    public enum CoolGradientType
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2,
        ForwardDiagonal = 3,
        BackwardDiagonal = 4,
        Ellipsis = 5,
        Circular = 6,
        FourPointPoly = 7,
        TenPointPoly = 8,
        Triangular = 9
    }

    #endregion

    public static class CoolGradient
    {
        #region Gradient Drawing Functions

        /// <summary>
        /// Draws the appropriate style of gradient based on gradient type flag specified in the function parameter.                  
        /// </summary>
        /// <param name="gradType"></param>
        /// <param name="graphics"></param>
        /// <param name="gradColor1"></param>
        /// <param name="gradColor2"></param>
        /// <param name="rectBounds"></param>
        /// <param name="fGradientSpan"></param>
        /// <param name="iOffsetX"></param>
        /// <param name="iOffsetY"></param>
        public static void DrawGradient(CoolGradientType gradType, Graphics graphics, Color gradColor1, Color gradColor2, Rectangle rectBounds,
                                                      float fGradientSpan = 0f, int iOffsetX = 0, int iOffsetY = 0)
        {
            try
            {
                //graphics.SmoothingMode = SmoothingMode.HighQuality;
                //graphics.InterpolationMode = InterpolationMode.High;

                if (fGradientSpan == 0)
                    fGradientSpan = GetDefaultGradientSpan(gradType);

                switch (gradType)
                {
                    case CoolGradientType.Horizontal:
                        DrawLinearGradient(graphics, gradColor1, gradColor2, rectBounds, LinearGradientMode.Horizontal, fGradientSpan, iOffsetX, iOffsetY);

                        break;
                    case CoolGradientType.Vertical:
                        DrawLinearGradient(graphics, gradColor1, gradColor2, rectBounds, LinearGradientMode.Vertical, fGradientSpan, iOffsetX, iOffsetY);

                        break;
                    case CoolGradientType.ForwardDiagonal:
                        DrawLinearGradient(graphics, gradColor1, gradColor2, rectBounds, LinearGradientMode.ForwardDiagonal, fGradientSpan, iOffsetX, iOffsetY);

                        break;
                    case CoolGradientType.BackwardDiagonal:
                        DrawLinearGradient(graphics, gradColor1, gradColor2, rectBounds, LinearGradientMode.BackwardDiagonal, fGradientSpan, iOffsetX, iOffsetY);

                        break;
                    case CoolGradientType.Ellipsis:
                    case CoolGradientType.Circular:
                        DrawEllipsisGradient(graphics, gradColor1, gradColor2, rectBounds, fGradientSpan, iOffsetX, iOffsetY,
                                                      gradType == CoolGradientType.Circular);

                        break;
                    case CoolGradientType.FourPointPoly:
                        DrawPolyGradient(graphics, gradColor1, gradColor2, rectBounds, 4, fGradientSpan, iOffsetX, iOffsetY);

                        break;
                    case CoolGradientType.TenPointPoly:
                        DrawPolyGradient(graphics, gradColor1, gradColor2, rectBounds, 10, fGradientSpan, iOffsetX, iOffsetY);

                        break;
                    case CoolGradientType.Triangular:
                        DrawTriangleGradient(graphics, gradColor1, gradColor2, rectBounds, fGradientSpan, iOffsetX, iOffsetY);

                        break;
                }//end switch           
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawGradient function of CoolGradient class.");
            }
        }

        /// <summary>
        /// Draws a linear style of gradient in the specified region passed to the function.  The linear gradient will be drawn using the
        /// starting and ending colors and linear gradient style specified in the function's parameters.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="gradColor1"></param>
        /// <param name="gradColor2"></param>
        /// <param name="rectBounds"></param>
        /// <param name="mode"></param>
        /// <param name="fGradientSpan"></param>
        /// <param name="iOffsetX"></param>
        /// <param name="iOffsetY"></param>
        public static void DrawLinearGradient(Graphics graphics, Color gradColor1, Color gradColor2, Rectangle rectBounds, LinearGradientMode mode,
                                                               float fGradientSpan = 1f, int iOffsetX = 0, int iOffsetY = 0)
        {
            LinearGradientBrush linGradBrush = null;

            try
            {
                Rectangle rectGrad = new Rectangle(rectBounds.Left + iOffsetX, rectBounds.Top + iOffsetY,
                                                                     Convert.ToInt32(rectBounds.Width * fGradientSpan), Convert.ToInt32(rectBounds.Height * fGradientSpan));

                linGradBrush = new LinearGradientBrush(
                                                        rectGrad, gradColor1, gradColor2, mode);

                graphics.FillRectangle(linGradBrush, rectBounds);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawLinearGradient function of CoolGradient class.");
            }
            finally
            {
                if (linGradBrush != null)
                    linGradBrush.Dispose();
            }
        }

        /// <summary>
        /// Draws an elliptical style of gradient in the specified region passed to the function.  The ellipitical gradient will be drawn using the
        /// starting and ending colors specified in the function's parameters.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="gradColor1"></param>
        /// <param name="gradColor2"></param>
        /// <param name="rectBounds"></param>
        /// <param name="fGradientSpan"></param>
        /// <param name="iOffsetX"></param>
        /// <param name="iOffsetY"></param>
        /// <param name="blDrawCircle"></param>
        public static void DrawEllipsisGradient(Graphics graphics, Color gradColor1, Color gradColor2, Rectangle rectBounds, float fGradientSpan = 2f,
                                                               int iOffsetX = 0, int iOffsetY = 0, bool blDrawCircle = false)
        {
            GraphicsPath path = null;
            PathGradientBrush pathGradBrush = null;

            try
            {
                //The largest dimension of the specified region will be used to determine the diameter of the circle to draw.
                int iCircLen;
                
                Bitmap bmpMemory = null;

                if (!blDrawCircle)
                {
                    bmpMemory = new Bitmap(Convert.ToInt32(rectBounds.Width * fGradientSpan), Convert.ToInt32(rectBounds.Height * fGradientSpan));
                }
                else 
                {
                    if (rectBounds.Width > rectBounds.Height)
                        iCircLen = rectBounds.Width;
                    else
                        iCircLen = rectBounds.Height;

                    bmpMemory = new Bitmap(Convert.ToInt32(iCircLen * fGradientSpan), Convert.ToInt32(iCircLen * fGradientSpan));
                }//end if

                
                Graphics gMemory = Graphics.FromImage(bmpMemory);

                // Create a path that consists of a single ellipse.
                path = new GraphicsPath();
                path.AddEllipse(0, 0, bmpMemory.Width, bmpMemory.Height);                

                // Use the path to construct a brush.
                pathGradBrush = new PathGradientBrush(path);

                // Set the color at the center of the path to first gradient color.
                pathGradBrush.CenterColor = gradColor1;

                // Set the color along the entire boundary of the path to second gradient color.                
                Color[] colors = { gradColor2 };
                pathGradBrush.SurroundColors = colors;

                //Draws the ellipitical gradient in a expanded region in memory, so that the desired region of the ellipsis gradient can be 
                //rendered onto the device context (paintable surface/display) of the control's UI.
                gMemory.FillEllipse(pathGradBrush, 0, 0, bmpMemory.Width, bmpMemory.Height);
                gMemory.Dispose();

                RectangleF rectSrc, rectDest;

                float fSrcX = (bmpMemory.Width - rectBounds.Width) / 2;
                float fSrcY = (bmpMemory.Height - rectBounds.Height) / 2;

                rectSrc = new RectangleF(fSrcX + iOffsetX, fSrcY + iOffsetY, rectBounds.Width, rectBounds.Height);
                rectDest = new RectangleF(rectBounds.Left, rectBounds.Top, rectBounds.Width, rectBounds.Height);

                //Draws a cropped region of the center-most area of the ellipitical gradient in the memory-resident image which will be
                //rendered on the control's UI display.  This will result in producing the desired gradient effect for the control.
                graphics.DrawImage(bmpMemory, rectDest, rectSrc, GraphicsUnit.Pixel);

                bmpMemory.Dispose();
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawEllipsisGradient function of CoolGradient class.");
            }
            finally
            {
                if (path != null)
                    path.Dispose();

                if (pathGradBrush != null)
                    pathGradBrush.Dispose();                
            }
        }

        /// <summary>
        /// Draws a symmetrical circle style of gradient in the specified region passed to the function.  The circular gradient will be drawn using the
        /// starting and ending colors specified in the function's parameters. 
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="gradColor1"></param>
        /// <param name="gradColor2"></param>
        /// <param name="rectBounds"></param>
        /// <param name="fGradientSpan"></param>
        /// <param name="iOffsetX"></param>
        /// <param name="iOffsetY"></param>
        public static void DrawCircleGradient(Graphics graphics, Color gradColor1, Color gradColor2, Rectangle rectBounds, float fGradientSpan = 1.5f,
                                                               int iOffsetX = 0, int iOffsetY = 0)
        {            
            try
            {
                DrawEllipsisGradient(graphics, gradColor1, gradColor2, rectBounds, fGradientSpan, iOffsetX, iOffsetY, true);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawCircleGradient function of CoolGradient class.");
            }            
        }

        /// <summary>
        /// Draws a polygon gradient that uses an array of points in the specified region passed to the function.  The number of points used 
        /// to draw the polygon gradient will be determined by the Points parameter.  A distribution of colors will be calculated based on the 
        /// number of points and the starting and ending colors specified in the function's parameters to produce the appropriate gradient 
        /// effect.  
        /// NOTE: As of now, only 4 and 10 point polygon gradients are supported.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="gradColor1"></param>
        /// <param name="gradColor2"></param>
        /// <param name="rectBounds"></param>
        /// <param name="iPoints">Indicates how many points will be used for drawing the polygon gradient.  The following values are 
        /// allowed: 4 and 10.</param>
        /// <param name="fGradientSpan"></param>
        /// <param name="iOffsetX"></param>
        /// <param name="iOffsetY"></param>
        public static void DrawPolyGradient(Graphics graphics, Color gradColor1, Color gradColor2, Rectangle rectBounds, int iPoints,
                                                            float fGradientSpan = 1f, int iOffsetX = 0, int iOffsetY = 0)
        {
            PathGradientBrush pathGradBrush = null;

            try
            {
                Rectangle rectDraw = new Rectangle(rectBounds.Left + iOffsetX, rectBounds.Top + iOffsetY,
                                                            Convert.ToInt32(rectBounds.Width * fGradientSpan), Convert.ToInt32(rectBounds.Height * fGradientSpan));
                
                Color gradColorLightest;
                Color gradColorLighter;
                Color gradColorDarker;
                Color gradColorDarkest;

                //Indicates if the colors are going from lighter to dark or from darker to light in the gradient path.
                bool blColorAscending = false;

                //Determines whether the starting or ending color is darker and calculates a range of lighter and darker colors based 
                //on whether the polygon gradient will have colors that go from lighter to darker or darker to lighter starting from the 
                //center point of the gradient.
                int iColor1Sum = gradColor1.R + gradColor1.G + gradColor1.B;
                int iColor2Sum = gradColor2.R + gradColor2.G + gradColor2.B;

                if (iColor1Sum < iColor2Sum)
                {
                    gradColorLighter = gradColor2;
                    gradColorDarker = gradColor1;
                    blColorAscending = false;
                }
                else
                {
                    gradColorLighter = gradColor1;
                    gradColorDarker = gradColor2;
                    blColorAscending = true;
                }//end if

                int iLightR = 255, iLightG = 255, iLightB = 255;
                int iDarkR = 0, iDarkG = 0, iDarkB = 0;

                if (gradColorLighter.R + 30 <= 255)
                    iLightR = gradColorLighter.R + 30;

                if (gradColorLighter.G + 30 <= 255)
                    iLightG = gradColorLighter.G + 30;

                if (gradColorLighter.B + 30 <= 255)
                    iLightB = gradColorLighter.B + 30;

                //Another lighter shade of the lighter of the two gradient colors will be calculated, that will be lighter by a factor 30 on the RGB scale.
                gradColorLightest = Color.FromArgb(iLightR, iLightG, iLightB);


                if (gradColorDarker.R - 30 > 0)
                    iDarkR = gradColorDarker.R - 30;

                if (gradColorDarker.G - 30 > 0)
                    iDarkG = gradColorDarker.G - 30;

                if (gradColorDarker.B - 30 > 0)
                    iDarkB = gradColorDarker.B - 30;

                //Another darker shade of the darker of the two gradient colors will be calculated, that will be darker by a factor 30 on the RGB scale.
                gradColorDarkest = Color.FromArgb(iDarkR, iDarkG, iDarkB);

                PointF[] ptsF = null;

                // Construct a path gradient brush based on an array of points.
                if (iPoints == 4)
                {
                    ptsF = new PointF[] {
                                           //new PointF(0, 0),
                                           new PointF(rectDraw.Left, rectDraw.Top),
                                           new PointF(rectDraw.Width, rectDraw.Top),
                                           new PointF(rectDraw.Width, rectDraw.Height),
                                           new PointF(rectDraw.Left, rectDraw.Height)
                                        };
                }
                else if (iPoints == 10)
                {
                    ptsF = new PointF[] {
                                           new PointF(rectDraw.Left, rectDraw.Top),
                                           new PointF(rectDraw.Width / 4f, rectDraw.Top),
                                           new PointF(rectDraw.Width /2f, rectDraw.Top),
                                           new PointF(rectDraw.Width * .75f, rectDraw.Top),
                                           new PointF(rectDraw.Width, rectDraw.Top),

                                           new PointF(rectDraw.Width, rectDraw.Height),
                                           new PointF(rectDraw.Width * .75f, rectDraw.Height),
                                           new PointF(rectDraw.Width /2f, rectDraw.Height),
                                           new PointF(rectDraw.Width /4f, rectDraw.Height),
                                           new PointF(rectDraw.Left, rectDraw.Height)
                                        };
                }//end if

                pathGradBrush = new PathGradientBrush(ptsF);

                // An array of five points was used to construct the path gradient
                // brush. Set the color of each point in that array.
                Color[] colors = null;

                if (blColorAscending)
                {
                    if (iPoints == 4)
                    {
                        colors = new Color[]
                        {
                            gradColorDarkest,  // (0, 0)
                            gradColorDarkest,  // (rectDraw.Width, 0)                             
                            gradColorDarkest, // (rectDraw.Width, rectDraw.Height)
                            gradColorDarkest, // (0, rectDraw.Height)                            
                        };
                    }
                    else if (iPoints == 10)
                    {
                        colors = new Color[]
                        {
                            gradColorDarkest,  // (0, 0)
                            gradColorDarker,  // (rectDraw.Width / 4f, 0) 
                            gradColorLighter, // (rectDraw.Width / 2f, 0) 
                            gradColorDarker, // (rectDraw.Width * .75f, 0)
                            gradColorDarkest, // (rectDraw.Width, 0)

                            gradColorDarkest, //  (rectDraw.Width, rectDraw.Height)                        
                            gradColorDarker,  // (rectDraw.Width * .75f, rectDraw.Height)
                            gradColorLighter,  // (rectDraw.Width / 2f, rectDraw.Height) 
                            gradColorDarker, // (rectDraw.Width / 4f, rectDraw.Height) 
                            gradColorDarkest, // (0, rectDraw.Height)                        
                        };
                    }//end if

                    pathGradBrush.CenterColor = gradColorLightest;
                }
                else
                {
                    if (iPoints == 4)
                    {
                        colors = new Color[]
                        {
                            gradColorLightest,  // (0, 0)
                            gradColorLightest,  // (rectDraw.Width, 0)                             
                            gradColorLightest, // (rectDraw.Width, rectDraw.Height)
                            gradColorLightest, // (0, rectDraw.Height)                            
                        };
                    }
                    else if (iPoints == 10)
                    {
                        colors = new Color[]
                        {
                            gradColorLightest,  // (0, 0)
                            gradColorLighter,  // (rectDraw.Width / 4f, 0) 
                            gradColorDarker, // (rectDraw.Width / 2f, 0) 
                            gradColorLighter, // (rectDraw.Width * .75f, 0)
                            gradColorLightest, // (rectDraw.Width, 0)

                            gradColorLightest, //  (rectDraw.Width, rectDraw.Height)                        
                            gradColorLighter,  // (rectDraw.Width * .75f, rectDraw.Height)
                            gradColorDarker,  // (rectDraw.Width / 2f, rectDraw.Height) 
                            gradColorLighter, // (rectDraw.Width / 4f, rectDraw.Height) 
                            gradColorLightest, // (0, rectDraw.Height)                        
                        };
                    }//end if

                    pathGradBrush.CenterColor = gradColorDarkest;
                }//end if

                pathGradBrush.SurroundColors = colors;

                // Use the path gradient brush to fill a rectangle.
                graphics.FillRectangle(pathGradBrush, rectDraw);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawPolyGradient function of CoolGrid class.");
            }
            finally
            {
                if (pathGradBrush != null)
                    pathGradBrush.Dispose();
            }
        }

        /// <summary>
        /// Draws a triangular style of gradient in the specified region passed to the function.  The triangular gradient will be drawn using the
        /// starting and ending colors specified in the function's parameters.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="gradColor1"></param>
        /// <param name="gradColor2"></param>
        /// <param name="rectBounds"></param>
        /// <param name="fGradientSpan"></param>
        /// <param name="iOffsetX"></param>
        /// <param name="iOffsetY"></param>
        public static void DrawTriangleGradient(Graphics graphics, Color gradColor1, Color gradColor2, Rectangle rectBounds, float fGradientSpan = 3f,
                                                                 int iOffsetX = 0, int iOffsetY = 0)
        {
            PathGradientBrush pathGradBrush = null;

            try
            {
                Bitmap bmpMemory = new Bitmap(Convert.ToInt32(rectBounds.Width * fGradientSpan), Convert.ToInt32(rectBounds.Height * fGradientSpan));
                Graphics gMemory = Graphics.FromImage(bmpMemory);

                // Vertices of the outer triangle.. The triangle is drawn in shape that will allow the center of the gradient and its distribution 
                //of colors to be properly cropped and displayed in the UI of the control having the gradient drawn.
                Point[] points = {
                                           new Point(Convert.ToInt32(bmpMemory.Width / 2), 0),
                                           new Point(Convert.ToInt32(bmpMemory.Width), Convert.ToInt32(bmpMemory.Height * .8)),
                                           new Point(0, Convert.ToInt32(bmpMemory.Height * .8))};

                // No GraphicsPath object is created. The PathGradientBrush object is constructed directly from the array of points.
                pathGradBrush = new PathGradientBrush(points);

                Color[] colors = { gradColor1, gradColor2 };

                float[] relativePositions = { 0f, 1.0f };

                ColorBlend colorBlend = new ColorBlend();
                colorBlend.Colors = colors;
                colorBlend.Positions = relativePositions;
                pathGradBrush.InterpolationColors = colorBlend;

                //Draws the triangle gradient in a expanded region in memory, so that the desired region of the triangle gradient can be 
                //rendered onto the device context (paintable surface/display) of the control's UI.  The portion of the rectangle outside the
                //triangle will not be painted, although this will not matter, since we will be cropping a section within the region of the triangle.
                gMemory.FillRectangle(pathGradBrush, 0, 0, bmpMemory.Width, bmpMemory.Height);
                gMemory.Dispose();


                RectangleF rectSrc, rectDest;

                float fSrcX = (bmpMemory.Width - rectBounds.Width) / 2;
                float fSrcY = (bmpMemory.Height - rectBounds.Height) / 2;

                rectSrc = new RectangleF(fSrcX +iOffsetX, fSrcY + iOffsetY, rectBounds.Width, rectBounds.Height);
                rectDest = new RectangleF(rectBounds.Left, rectBounds.Top, rectBounds.Width, rectBounds.Height);

                //Draws a cropped region of the center-most area of the triangle gradient in the memory-resident image which will be
                //rendered on the control's UI display.  The triangle gradient and points were calculated in a way that the center of the 
                //gradient can be cropped and drawn into the UI control's display.  This will result in producing the desired gradient effect for the control.
                graphics.DrawImage(bmpMemory, rectDest, rectSrc, GraphicsUnit.Pixel);

                bmpMemory.Dispose();
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawTriangleGradient function of CoolGradient class.");
            }
            finally
            {
                if (pathGradBrush != null)
                    pathGradBrush.Dispose();
            }
        }
        #endregion

        #region Gradient Information/Utility Properties, Functions

        /// <summary>
        /// Retrieves the default gradient span setting for the gradient type specified in the function's parameter.  Each various gradient type will have 
        /// a default gradient span that will be considered the optimal size for displaying the specific type of gradient.
        /// </summary>
        /// <param name="gradType"></param>
        /// <returns></returns>
        public static float GetDefaultGradientSpan(CoolGradientType gradType)
        {
            try
            {
                switch (gradType)
                {
                    case CoolGradientType.Horizontal:
                    case CoolGradientType.Vertical:
                    case CoolGradientType.ForwardDiagonal:
                    case CoolGradientType.BackwardDiagonal:
                        return 1f;
                    case CoolGradientType.Ellipsis:
                        return 2f;
                    case CoolGradientType.Circular:
                        return 1.5f;
                    case CoolGradientType.FourPointPoly:
                        return 1f;
                    case CoolGradientType.TenPointPoly:
                        return 1f;
                    case CoolGradientType.Triangular:
                        return 3f;
                }//end switch

                return 1f;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetDefaultGradientSpan function of CoolGradient class.");
                return 1f;                                        
            }
        }

        #endregion
    }
}

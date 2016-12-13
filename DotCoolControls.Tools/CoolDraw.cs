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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tiferix.Global;

namespace DotCoolControls.Tools
{
    /// <summary>
    /// The CoolDraw class will handle a wide variety of GDI+/Painting and Drawing operations, including various image, text, cropping and gradient 
    /// manipulation drawing and rendering in the various DotCool controls.  In addition to function as a toolkit for various drawing and rendering 
    /// operations, the class will contain many calculation, informational and utility type of functions that relate to graphics, texts and images of the 
    /// DotCool controls.
    /// </summary>
    public static class CoolDraw
    {
        #region Shape Drawing Functions

        /// <summary>
        /// Draws the appropriate shape according to the type of shape and border specified in the function's parameters.  In addition to 
        /// drawing the shape, the shape can be filled in with a solid color specified in the FillColor parameter of the function.  
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="graphics"></param>
        /// <param name="rectBounds"></param>
        /// <param name="BorderColor"></param>
        /// <param name="iBorderWidth"></param>
        /// <param name="fXRadius"></param>
        /// <param name="fYRadius"></param>
        /// <param name="FillColor"></param>
        /// <returns></returns>
        public static PointF[] DrawShape(CoolShape shape, Graphics graphics, Rectangle rectBounds, Color BorderColor, int iBorderWidth,
                                                  float fXRadius = 0, float fYRadius = 0, Color? FillColor = null)
        {
            try
            {
                switch (shape)
                {                    
                    case CoolShape.Rectangle:
                    case CoolShape.Square:
                        rectBounds = new Rectangle(rectBounds.Left, rectBounds.Top, rectBounds.Width + 1, rectBounds.Height + 1);
                        return DrawRectangle(graphics, rectBounds, BorderColor, iBorderWidth, FillColor, shape == CoolShape.Square);                        
                    case CoolShape.RoundedRect:
                    case CoolShape.RoundedSquare:
                        if (iBorderWidth == 1)
                            rectBounds = new Rectangle(rectBounds.Left, rectBounds.Top, rectBounds.Width + 1, rectBounds.Height + 1);

                        return DrawRoundedRectangle(graphics, rectBounds, fXRadius, fYRadius, BorderColor, iBorderWidth,
                                                                    FillColor, shape == CoolShape.RoundedSquare);                                                                          
                    case CoolShape.Ellipse:
                    case CoolShape.Circle:
                        if(iBorderWidth == 1)
                            rectBounds = new Rectangle(rectBounds.Left, rectBounds.Top, rectBounds.Width + 1, rectBounds.Height + 1);

                        return DrawEllipse(graphics, rectBounds, BorderColor, iBorderWidth, FillColor, shape == CoolShape.Circle);                                                                    
                    case CoolShape.Diamond:
                    case CoolShape.TriangleLeft:
                    case CoolShape.Triangle:
                    case CoolShape.TriangleRight:
                    case CoolShape.TriangleBottom:
                        if (iBorderWidth == 1)
                            rectBounds = new Rectangle(rectBounds.Left, rectBounds.Top, rectBounds.Width + 1, rectBounds.Height + 1);

                        return DrawPolyShape(shape, graphics, rectBounds, BorderColor, iBorderWidth, FillColor);
                }//end switch        

                return null;                 
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawShape function of CoolDraw class.");
                return null;
            }
        }

        /// <summary>
        /// Draws a rectangle shape graphic with the border (and optionally fill color) specified in the function's parameters.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rectBounds"></param>
        /// <param name="BorderColor"></param>
        /// <param name="iBorderWidth"></param>
        /// <param name="FillColor"></param>
        /// <param name="blDrawSquare"></param>
        /// <returns></returns>
        public static PointF[] DrawRectangle(Graphics graphics, Rectangle rectBounds, Color BorderColor, int iBorderWidth, Color? FillColor = null,
                                                             bool blDrawSquare = false)
        {
            Pen penBorder = null;
            
            try
            {                
                penBorder = new Pen(BorderColor, iBorderWidth);

                Rectangle rectDraw = rectBounds;


                if (blDrawSquare)
                    rectDraw = CalculateSquareRect(rectBounds);
                
                if (FillColor != null)
                {
                    using (Brush brushFill = new SolidBrush(FillColor.Value))
                    {
                        graphics.FillRectangle(brushFill, rectDraw);
                    }//end using
                }//end if

                if(iBorderWidth > 0)
                    graphics.DrawRectangle(penBorder, rectDraw);

                GraphicsPath path = new GraphicsPath();
                path.AddRectangle(rectDraw);
                PointF[] aryPoints = path.PathPoints;
                path.Dispose();

                return aryPoints;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawRectangle function of CoolDraw class.");
                return null;
            }
            finally
            {
                if (penBorder != null)
                    penBorder.Dispose();                
            }
        }

        /// <summary>
        /// Draws a square shape graphic with the border (and optionally fill color) specified in the function's parameters.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rectBounds"></param>
        /// <param name="BorderColor"></param>
        /// <param name="iBorderWidth"></param>
        /// <param name="FillColor"></param>
        /// <returns></returns>
        public static PointF[] DrawSquare(Graphics graphics, Rectangle rectBounds, Color BorderColor, int iBorderWidth, Color? FillColor = null)
        {
            try
            {
                return DrawRectangle(graphics, rectBounds, BorderColor, iBorderWidth, FillColor, true);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawSquare function of CoolDraw class.");
                return null;
            }
        }

        /// <summary>
        /// Draws an ellipse shape graphic with the border (and optionally fill color) specified in the function's parameters.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rectBounds"></param>
        /// <param name="BorderColor"></param>
        /// <param name="iBorderWidth"></param>
        /// <param name="FillColor"></param>
        /// <param name="blDrawCircle"></param>
        /// <returns></returns>
        public static PointF[] DrawEllipse(Graphics graphics, Rectangle rectBounds, Color BorderColor, int iBorderWidth, Color? FillColor = null, 
                                                        bool blDrawCircle = false)
        {
            Pen penBorder = null;
            GraphicsState origGraphState = null;

            try
            {
                // At the beginning of your drawing
                origGraphState = graphics.Save();
                graphics.SmoothingMode = SmoothingMode.HighQuality;

                penBorder = new Pen(BorderColor, iBorderWidth);

                Rectangle rectDraw = rectBounds;

                if(blDrawCircle)
                   rectDraw = CalculateSquareRect(rectBounds);

                if (FillColor != null)
                {
                    using (Brush brushFill = new SolidBrush(FillColor.Value))
                    {
                        graphics.FillEllipse(brushFill, rectDraw);
                    }//end using
                }//end if

                if(iBorderWidth > 0)
                    graphics.DrawEllipse(penBorder, rectDraw);

                GraphicsPath path = new GraphicsPath();
                path.AddEllipse(rectDraw);
                PointF[] aryPoints = path.PathPoints;
                path.Dispose();

                return aryPoints;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawEllipse function of CoolDraw class.");
                return null;
            }
            finally
            {
                if (penBorder != null)
                    penBorder.Dispose();

                // At the end of your drawing
                if (origGraphState != null)
                    graphics.Restore(origGraphState);
            }
        }

        /// <summary>
        /// Draws a circle shape graphic with the border (and optionally fill color) specified in the function's parameters.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rectBounds"></param>
        /// <param name="BorderColor"></param>
        /// <param name="iBorderWidth"></param>
        /// <param name="FillColor"></param>
        /// <returns></returns>
        public static PointF[] DrawCircle(Graphics graphics, Rectangle rectBounds, Color BorderColor, int iBorderWidth, Color? FillColor = null)
        {           
            try
            {
                return DrawEllipse(graphics, rectBounds, BorderColor, iBorderWidth, FillColor, true);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawCircle function of CoolDraw class.");
                return null;
            }           
        }

        /// <summary>
        /// Draws a specific polygon-shaped graphic with the border (and optionally fill color) specified in the function's parameters.  The type of polygon 
        /// shape graphic that is drawn depends on the shape specified in the polyShape parameter of the function.
        /// </summary>
        /// <param name="polyShape"></param>
        /// <param name="graphics"></param>
        /// <param name="rectBounds"></param>
        /// <param name="BorderColor"></param>
        /// <param name="iBorderWidth"></param>
        /// <param name="FillColor"></param>
        /// <returns></returns>
        public static PointF[] DrawPolyShape(CoolShape polyShape, Graphics graphics, Rectangle rectBounds, Color BorderColor, int iBorderWidth,
                                                              Color? FillColor = null)
        {
            Pen penBorder = null;
            GraphicsState origGraphState = null;

            try
            {                
                // At the beginning of your drawing
                origGraphState = graphics.Save();
                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                penBorder = new Pen(BorderColor, iBorderWidth);

                // Vertices of the polygon shape to be drawn.
                Point[] points = GetPolyShapePoints(polyShape, rectBounds, iBorderWidth);

                if (FillColor != null)
                {
                    using (Brush brushFill = new SolidBrush(FillColor.Value))
                    {
                        graphics.FillPolygon(brushFill, points);
                    }//end using
                }//end if

                if(iBorderWidth > 0)
                    graphics.DrawPolygon(penBorder, points);

                GraphicsPath path = new GraphicsPath();
                path.AddPolygon(points);
                PointF[] aryPoints = path.PathPoints;
                path.Dispose();
                
                return aryPoints;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawPolyShape function of CoolDraw class.");
                return null;
            }
            finally
            {
                if (penBorder != null)
                    penBorder.Dispose();

                // At the end of your drawing
                if (origGraphState != null)
                    graphics.Restore(origGraphState);
            }
        }

        /// <summary>
        /// Draws a rounded rectangle shape graphic with the border and corner radius (and optionally fill color) specified in the function's parameters.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rectBounds"></param>
        /// <param name="fXRadius"></param>
        /// <param name="fYRadius"></param>
        /// <param name="BorderColor"></param>
        /// <param name="iBorderWidth"></param>
        /// <param name="FillColor"></param>
        /// <param name="blDrawSquare"></param>
        /// <returns></returns>
        public static PointF[] DrawRoundedRectangle(Graphics graphics, Rectangle rectBounds, float fXRadius, float fYRadius, Color BorderColor,
                                                                          int iBorderWidth, Color? FillColor = null, bool blDrawSquare = false)
        {
            Pen penBorder = null;
            GraphicsState origGraphState = null;

            try
            {
                // At the beginning of your drawing
                origGraphState = graphics.Save();
                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                penBorder = new Pen(BorderColor, iBorderWidth);

                Rectangle rectDraw = rectBounds;

                if (blDrawSquare)
                    rectDraw = CalculateSquareRect(rectBounds);

                if (iBorderWidth % 2 == 0)
                {
                    if (iBorderWidth > 2)                        
                    {
                        rectDraw.X -= 1;
                        rectDraw.Width += 1;
                    }//end if                   

                    rectDraw.Y -= 1;
                    rectDraw.Height += 1;
                }
                else
                {  
                    if(iBorderWidth > 1)                   
                        rectDraw.Height += 1;
                }//end if
                
                GraphicsPath path = GetRoundedRectPath(rectDraw, fXRadius, fYRadius, iBorderWidth);

                if (FillColor != null)
                {                                        
                    using (Brush brushFill = new SolidBrush(FillColor.Value))
                    {
                        graphics.FillPath(brushFill, path);
                    }//end using
                }//end if

                if(iBorderWidth > 0)
                    graphics.DrawPath(penBorder, path);

                PointF[] aryPoints = path.PathPoints;
                path.Dispose();

                return aryPoints;                
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawRoundedRectangle function of CoolDraw class.");
                return null;
            }
            finally
            {
                if (penBorder != null)
                    penBorder.Dispose();

                // At the end of your drawing
                if (origGraphState != null)
                    graphics.Restore(origGraphState);
            }
        }

        /// <summary>
        /// Draws a rounded square shape graphic with the border and corner radius (and optionally fill color) specified in the function's parameter.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rectBounds"></param>
        /// <param name="fXRadius"></param>
        /// <param name="fYRadius"></param>
        /// <param name="BorderColor"></param>
        /// <param name="iBorderWidth"></param>
        /// <param name="FillColor"></param>
        /// <returns></returns>
        public static PointF[] DrawRoundedSquare(Graphics graphics, Rectangle rectBounds, float fXRadius, float fYRadius, Color BorderColor,
                                                                      int iBorderWidth, Color? FillColor = null)
        {
            try
            {
                return DrawRoundedRectangle(graphics, rectBounds, fXRadius, fYRadius, BorderColor, iBorderWidth, FillColor, true);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawRoundedSquare function of CoolDraw class.");
                return null;
            }
        }

        #endregion

        #region Gradient Shape Drawing Functions

        /// <summary>
        /// Draws the appropriate gradient filled shape according to the type of shape, gradient and border specified in the function's parameters.  
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="gradType"></param>
        /// <param name="graphics"></param>
        /// <param name="gradColor1"></param>
        /// <param name="gradColor2"></param>
        /// <param name="rectBounds"></param>
        /// <param name="BorderColor"></param>
        /// <param name="iBorderWidth"></param>
        /// <param name="fGradientSpan"></param>
        /// <param name="iGradOffsetX"></param>
        /// <param name="iGradOffsetY"></param>
        /// <param name="fXRadius"></param>
        /// <param name="fYRadius"></param>
        /// <returns></returns>
        public static PointF[] DrawGradientShape(CoolShape shape, CoolGradientType gradType, Graphics graphics, Color gradColor1, Color gradColor2,
                                                               Rectangle rectBounds, Color BorderColor, int iBorderWidth, float fGradientSpan, int iGradOffsetX = 0, 
                                                               int iGradOffsetY =0, float fXRadius = 0, float fYRadius = 0)
        {
            try
            {
                switch (shape)
                {                    
                    case CoolShape.Rectangle:
                    case CoolShape.Square:
                        return DrawGradientRectangle(gradType, graphics, gradColor1, gradColor2, rectBounds, BorderColor,
                                                                    iBorderWidth, fGradientSpan, iGradOffsetX, iGradOffsetY, shape == CoolShape.Square);                    
                    case CoolShape.RoundedRect:
                    case CoolShape.RoundedSquare:
                        return DrawGradientRoundedRectangle(gradType, graphics, gradColor1, gradColor2, rectBounds, fXRadius, fYRadius,
                                                                            BorderColor, iBorderWidth, fGradientSpan, iGradOffsetX, iGradOffsetY,
                                                                            shape == CoolShape.RoundedSquare);                        
                    case CoolShape.Ellipse:
                    case CoolShape.Circle:
                        return DrawGradientEllipse(gradType, graphics, gradColor1, gradColor2, rectBounds, BorderColor,
                                                      iBorderWidth, fGradientSpan, iGradOffsetX, iGradOffsetY, shape == CoolShape.Circle);
                                               
                    case CoolShape.Triangle:
                    case CoolShape.TriangleLeft:
                    case CoolShape.TriangleRight:
                    case CoolShape.TriangleBottom:
                    case CoolShape.Diamond:
                        return DrawGradientPolyShape(shape, gradType, graphics, gradColor1, gradColor2, rectBounds, BorderColor, iBorderWidth,
                                                           fGradientSpan, iGradOffsetX, iGradOffsetY);                        
                }//end switch          

                return null;         
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawGradientShape function of CoolDraw class.");
                return null;
            }
        }

        /// <summary>
        /// Draws a gradient filled rectangle shape graphic with the border and gradient specified in the function's parameters.
        /// </summary>
        /// <param name="gradType"></param>
        /// <param name="graphics"></param>
        /// <param name="gradColor1"></param>
        /// <param name="gradColor2"></param>
        /// <param name="rectBounds"></param>
        /// <param name="BorderColor"></param>
        /// <param name="iBorderWidth"></param>
        /// <param name="fGradientSpan"></param>
        /// <param name="iOffsetX"></param>
        /// <param name="iOffsetY"></param>
        /// <param name="blDrawSquare"></param>
        /// <returns></returns>
        public static PointF[] DrawGradientRectangle(CoolGradientType gradType, Graphics graphics, Color gradColor1, Color gradColor2,
                                                              Rectangle rectBounds, Color BorderColor, int iBorderWidth, float fGradientSpan = 2f,
                                                              int iOffsetX = 0, int iOffsetY = 0, bool blDrawSquare = false)
        {
            GraphicsState origGraphState = null;

            try
            {
                // At the beginning of your drawing
                origGraphState = graphics.Save();

                Rectangle rectGrad;

                if (!blDrawSquare)
                    rectGrad = new Rectangle(0, 0, rectBounds.Width + 1, rectBounds.Height + 1);
                else
                    rectGrad = CalculateSquareRect(rectBounds);

                Bitmap bmpGrad = new Bitmap(rectGrad.Width, rectGrad.Height);
                Graphics gGrad = Graphics.FromImage(bmpGrad);

                CoolGradient.DrawGradient(gradType, gGrad, gradColor1, gradColor2, rectGrad, fGradientSpan, iOffsetX, iOffsetY);

                gGrad.Dispose();

                Rectangle rectDraw = new Rectangle(rectBounds.X, rectBounds.Y, rectGrad.Width, rectGrad.Height);                

                graphics.CompositingMode = CompositingMode.SourceOver;
                graphics.DrawImage(bmpGrad, rectDraw);
                bmpGrad.Dispose();

                PointF[] aryPoints = DrawRectangle(graphics, rectDraw, BorderColor, iBorderWidth, null, blDrawSquare);
                return aryPoints;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawGradientRectangle function of CoolDraw class.");
                return null;
            }
            finally
            {
                // At the end of your drawing
                if (origGraphState != null)
                    graphics.Restore(origGraphState);
            }
        }

        /// <summary>
        /// Draws a gradient filled square shape graphic with the border and gradient specified in the function's parameters.
        /// </summary>
        /// <param name="gradType"></param>
        /// <param name="graphics"></param>
        /// <param name="gradColor1"></param>
        /// <param name="gradColor2"></param>
        /// <param name="rectBounds"></param>
        /// <param name="BorderColor"></param>
        /// <param name="iBorderWidth"></param>
        /// <param name="fGradientSpan"></param>
        /// <param name="iOffsetX"></param>
        /// <param name="iOffsetY"></param>
        /// <returns></returns>
        public static PointF[] DrawGradientSquare(CoolGradientType gradType, Graphics graphics, Color gradColor1, Color gradColor2,
                                                              Rectangle rectBounds, Color BorderColor, int iBorderWidth, float fGradientSpan = 2f,
                                                              int iOffsetX = 0, int iOffsetY = 0)
        {
            try
            {
                return DrawGradientRectangle(gradType, graphics, gradColor1, gradColor2, rectBounds, BorderColor, iBorderWidth,
                                                            fGradientSpan, iOffsetX, iOffsetY, true);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawGradientSquare function of CoolDraw class.");
                return null;
            }
        }

        /// <summary>
        /// Draws a gradient filled ellipse shape graphic with the border and gradient specified in the function's parameters.
        /// </summary>
        /// <param name="gradType"></param>
        /// <param name="graphics"></param>
        /// <param name="gradColor1"></param>
        /// <param name="gradColor2"></param>
        /// <param name="rectBounds"></param>
        /// <param name="BorderColor"></param>
        /// <param name="iBorderWidth"></param>
        /// <param name="fGradientSpan"></param>
        /// <param name="iOffsetX"></param>
        /// <param name="iOffsetY"></param>
        /// <param name="blDrawCircle"></param>
        public static PointF[] DrawGradientEllipse(CoolGradientType gradType, Graphics graphics, Color gradColor1, Color gradColor2,
                                                              Rectangle rectBounds, Color BorderColor, int iBorderWidth, float fGradientSpan = 1.5f,
                                                              int iOffsetX = 0, int iOffsetY = 0, bool blDrawCircle = false)
        {
            GraphicsState origGraphState = null;

            try
            {
                // At the beginning of your drawing
                origGraphState = graphics.Save();

                Bitmap bmpGrad = new Bitmap(rectBounds.Width, rectBounds.Height);
                Graphics gGrad = Graphics.FromImage(bmpGrad);

                Rectangle rectGrad = new Rectangle(0, 0, bmpGrad.Width + 3, bmpGrad.Height + 3);

                CoolGradient.DrawGradient(gradType, gGrad, gradColor1, gradColor2, rectGrad, fGradientSpan, iOffsetX, iOffsetY);
                gGrad.Dispose();

                Bitmap bmpCropGrad = CropEllipse(bmpGrad, rectBounds, blDrawCircle);                
                bmpCropGrad.MakeTransparent();
                bmpGrad.Dispose();

                Rectangle rectCropBmp = new Rectangle(0, 0, rectBounds.Width + 1, rectBounds.Height + 1);
                graphics.CompositingMode = CompositingMode.SourceOver;
                graphics.DrawImage(bmpCropGrad, rectCropBmp);
                bmpCropGrad.Dispose();
                
                PointF[] aryPoints = DrawEllipse(graphics, rectBounds, BorderColor, iBorderWidth, null, blDrawCircle);
                return aryPoints;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawGradientEllipse function of CoolDraw class.");
                return null;
            }
            finally
            {
                // At the end of your drawing
                if (origGraphState != null)
                    graphics.Restore(origGraphState);
            }
        }

        /// <summary>
        /// Draws a gradient filled circle shape graphic with the border and gradient specified in the function's parameters.
        /// </summary>
        /// <param name="gradType"></param>
        /// <param name="graphics"></param>
        /// <param name="gradColor1"></param>
        /// <param name="gradColor2"></param>
        /// <param name="rectBounds"></param>
        /// <param name="BorderColor"></param>
        /// <param name="iBorderWidth"></param>
        /// <param name="fGradientSpan"></param>
        /// <param name="iOffsetX"></param>
        /// <param name="iOffsetY"></param>
        /// <returns></returns>
        public static PointF[] DrawGradientCircle(CoolGradientType gradType, Graphics graphics, Color gradColor1, Color gradColor2,
                                                              Rectangle rectBounds, Color BorderColor, int iBorderWidth, float fGradientSpan = 1.5f,
                                                              int iOffsetX = 0, int iOffsetY = 0)
        {            
            try
            {
                return DrawGradientEllipse(gradType, graphics, gradColor1, gradColor2, rectBounds, BorderColor, iBorderWidth,
                                              fGradientSpan, iOffsetX, iOffsetY, true);              
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawGradientCircle function of CoolDraw class.");
                return null;
            }            
        }

        /// <summary>
        /// Draws a gradient filled polygon shape graphic with the border and gradient specified in the function's parameters.  The type of polygon 
        /// shape graphic that is drawn depends on the shape specified in the polyShape parameter of the function.
        /// </summary>
        /// <param name="polyShape"></param>
        /// <param name="gradType"></param>
        /// <param name="graphics"></param>
        /// <param name="gradColor1"></param>
        /// <param name="gradColor2"></param>
        /// <param name="rectBounds"></param>
        /// <param name="BorderColor"></param>
        /// <param name="iBorderWidth"></param>
        /// <param name="fGradientSpan"></param>
        /// <param name="iOffsetX"></param>
        /// <param name="iOffsetY"></param>
        /// <returns></returns>
        public static PointF[] DrawGradientPolyShape(CoolShape polyShape, CoolGradientType gradType, Graphics graphics, Color gradColor1, Color gradColor2,
                                                                    Rectangle rectBounds, Color BorderColor, int iBorderWidth, float fGradientSpan = 2f,
                                                                    int iOffsetX = 0, int iOffsetY = 0)
        {
            GraphicsState origGraphState = null;

            try
            {
                // At the beginning of your drawing
                origGraphState = graphics.Save();                

                Bitmap bmpGrad = new Bitmap(rectBounds.Width, rectBounds.Height);
                Graphics gGrad = Graphics.FromImage(bmpGrad);

                Rectangle rectGrad = new Rectangle(0, 0, bmpGrad.Width + 3, bmpGrad.Height + 3);

                CoolGradient.DrawGradient(gradType, gGrad, gradColor1, gradColor2, rectGrad, fGradientSpan, iOffsetX, iOffsetY);                

                gGrad.Dispose();

                Bitmap bmpCropGrad = CropPolyShape(polyShape, bmpGrad, rectBounds);                
                bmpCropGrad.MakeTransparent();
                bmpGrad.Dispose();

                Rectangle rectCropBmp = new Rectangle(0, 0, rectBounds.Width + 1, rectBounds.Height + 1);
                graphics.CompositingMode = CompositingMode.SourceOver;
                graphics.DrawImage(bmpCropGrad, rectCropBmp);
                bmpCropGrad.Dispose();

                PointF[] aryPoints = DrawPolyShape(polyShape, graphics, rectBounds, BorderColor, iBorderWidth);
                return aryPoints;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawGradientPolyShape function of CoolDraw class.");
                return null;
            }
            finally
            {
                // At the end of your drawing
                if (origGraphState != null)
                    graphics.Restore(origGraphState);
            }
        }

        /// <summary>
        /// Draws a gradient filled rounded rectangle shape graphic with the border, corner radius and gradient specified in the function's parameters.
        /// </summary>
        /// <param name="gradType"></param>
        /// <param name="graphics"></param>
        /// <param name="gradColor1"></param>
        /// <param name="gradColor2"></param>
        /// <param name="rectBounds"></param>
        /// <param name="fXRadius"></param>
        /// <param name="fYRadius"></param>
        /// <param name="BorderColor"></param>
        /// <param name="iBorderWidth"></param>
        /// <param name="fGradientSpan"></param>
        /// <param name="iOffsetX"></param>
        /// <param name="iOffsetY"></param>
        /// <param name="blDrawSquare"></param>
        /// <returns></returns>
        public static PointF[] DrawGradientRoundedRectangle(
                                                                    CoolGradientType gradType, Graphics graphics, Color gradColor1, Color gradColor2, Rectangle rectBounds,
                                                                    float fXRadius, float fYRadius, Color BorderColor, int iBorderWidth, float fGradientSpan = 2f, 
                                                                    int iOffsetX = 0, int iOffsetY = 0, bool blDrawSquare = false)
        {
            GraphicsState origGraphState = null;

            try
            {
                // At the beginning of your drawing
                origGraphState = graphics.Save();

                Bitmap bmpGrad = new Bitmap(rectBounds.Width, rectBounds.Height);
                Graphics gGrad = Graphics.FromImage(bmpGrad);

                Rectangle rectGrad = new Rectangle(0, 0, bmpGrad.Width + 3, bmpGrad.Height + 3);

                GraphicsPath pathRoundRect = GetRoundedRectPath(rectGrad, fXRadius, fYRadius);
                CoolGradient.DrawGradient(gradType, gGrad, gradColor1, gradColor2, rectGrad, fGradientSpan, iOffsetX, iOffsetY);                
                gGrad.Dispose();
                
                Bitmap bmpCropGrad = CropRoundedRect(bmpGrad, rectGrad, fXRadius, fYRadius, blDrawSquare);
                bmpCropGrad.MakeTransparent();
                bmpGrad.Dispose();
                
                Rectangle rectCropBmp = new Rectangle(rectBounds.X ,rectBounds.Y, rectBounds.Width, rectBounds.Height);
                graphics.CompositingMode = CompositingMode.SourceOver;

                graphics.DrawImage(bmpCropGrad, rectCropBmp);
                bmpCropGrad.Dispose();                

                PointF[] aryPoints = DrawRoundedRectangle(graphics, rectBounds, fXRadius, fYRadius, BorderColor, iBorderWidth, null, blDrawSquare);
                return aryPoints;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawGradientRoundedRectangle function of CoolDraw class.");
                return null;
            }
            finally
            {
                // At the end of your drawing
                if (origGraphState != null)
                    graphics.Restore(origGraphState);
            }
        }

        /// <summary>
        /// Draws a gradient filled rounded square shape graphic with the border, corner radius and gradient specified in the function's parameters.
        /// </summary>
        /// <param name="gradType"></param>
        /// <param name="graphics"></param>
        /// <param name="gradColor1"></param>
        /// <param name="gradColor2"></param>
        /// <param name="rectBounds"></param>
        /// <param name="fXRadius"></param>
        /// <param name="fYRadius"></param>
        /// <param name="BorderColor"></param>
        /// <param name="iBorderWidth"></param>
        /// <param name="fGradientSpan"></param>
        /// <param name="iOffsetX"></param>
        /// <param name="iOffsetY"></param>
        /// <returns></returns>
        public static PointF[] DrawGradientRoundedSquare(CoolGradientType gradType, Graphics graphics, Color gradColor1, Color gradColor2,
                                                                        Rectangle rectBounds, float fXRadius, float fYRadius, Color BorderColor,
                                                                        int iBorderWidth, float fGradientSpan = 2f, int iOffsetX = 0, int iOffsetY = 0)
        {
            try
            {
                return DrawGradientRoundedRectangle(gradType, graphics, gradColor1, gradColor2, rectBounds, fXRadius, fYRadius, BorderColor, iBorderWidth,
                                                                         fGradientSpan, iOffsetX, iOffsetY, true);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawGradientRoundedSquare function of CoolDraw class.");
                return null;
            }
}

        #endregion

        #region Image Cropping Functions

        /// <summary>
        /// Crops the bitmap passed to the image into an ellipse shape that will fit within the bounds specified in the function's parameter.  The 
        /// cropped image will be returned as a new bitmap image.
        /// </summary>
        /// <param name="bmpSrc"></param>
        /// <param name="rectBounds"></param>
        /// <param name="blCropCircle"></param>
        /// <returns></returns>
        public static Bitmap CropEllipse(Bitmap bmpSrc, Rectangle rectBounds, bool blCropCircle = false)
        {
            try
            {
                Bitmap bmpDest = new Bitmap(rectBounds.Width, rectBounds.Height, bmpSrc.PixelFormat);
                Graphics g = Graphics.FromImage(bmpDest);
                g.SmoothingMode = SmoothingMode.HighQuality;
                using (Brush brTrans = new SolidBrush(Color.Transparent))
                {
                    g.FillRectangle(brTrans, 0, 0, bmpDest.Width, bmpDest.Height);
                }//end using

                Rectangle rectCrop = rectBounds;

                if(blCropCircle)
                    rectCrop = CalculateSquareRect(rectBounds);
                
                GraphicsPath path = new GraphicsPath();
                path.AddEllipse(rectCrop);
                g.SetClip(path);
                g.DrawImage(bmpSrc, 0, 0);
                
                g.Dispose();

                return bmpDest;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in CropEllipse function of CoolDraw class.");
                return null;
            }
        }

        /// <summary>
        /// Crops the bitmap passed to the image into a circle shape that will fit within the bounds specified in the function's parameter.  The 
        /// cropped image will be returned as a new bitmap image.
        /// </summary>
        /// <param name="bmpSrc"></param>
        /// <param name="rectBounds"></param>
        /// <returns></returns>
        public static Bitmap CropCircle(Bitmap bmpSrc, Rectangle rectBounds)
        {
            try
            {
                return CropEllipse(bmpSrc, rectBounds, true);
            }             
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in CropCircle function of CoolDraw class.");
                return null;
            }
        }

        /// <summary>
        /// Crops the bitmap passed to the image into a polygon shape based that will fit within the bounds specified in the function's parameter.
        /// Which polygon shape will be cropped will be determined by the shape passed to the polyShape parameter of the function.  The cropped 
        /// image will be returned as a new bitmap image.
        /// </summary>
        /// <param name="polyShape"></param>
        /// <param name="bmpSrc"></param>
        /// <param name="rectBounds"></param>
        /// <returns></returns>
        public static Bitmap CropPolyShape(CoolShape polyShape, Bitmap bmpSrc, Rectangle rectBounds)
        {
            try
            {
                Bitmap bmpDest = new Bitmap(rectBounds.Width, rectBounds.Height, bmpSrc.PixelFormat);
                Graphics g = Graphics.FromImage(bmpDest);
                g.SmoothingMode = SmoothingMode.HighQuality;
                using (Brush brTrans = new SolidBrush(Color.Transparent))
                {
                    g.FillRectangle(brTrans, 0, 0, bmpDest.Width, bmpDest.Height);
                }//end using

                Point[] aryCropPts = GetPolyShapePoints(polyShape, rectBounds);                

                GraphicsPath path = new GraphicsPath();
                path.AddPolygon(aryCropPts);
                g.SetClip(path);
                g.DrawImage(bmpSrc, 0, 0);

                g.Dispose();

                return bmpDest;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in CropPolyShape function of CoolDraw class.");
                return null;
            }
        }

        /// <summary>
        /// Crops the bitmap passed to the image into a rounded rectangle shape that will fit within the bounds specified in the function's parameter.  The 
        /// cropped image will be returned as a new bitmap image.
        /// </summary>
        /// <param name="bmpSrc"></param>
        /// <param name="rectBounds"></param>
        /// <param name="fXRadius"></param>
        /// <param name="fYRadius"></param>
        /// <param name="blCropSquare"></param>
        /// <returns></returns>
        public static Bitmap CropRoundedRect(Bitmap bmpSrc, Rectangle rectBounds, float fXRadius, float fYRadius, bool blCropSquare = false)
        {
            try
            {
                Bitmap bmpDest = new Bitmap(rectBounds.Width, rectBounds.Height, bmpSrc.PixelFormat);
                Graphics g = Graphics.FromImage(bmpDest);
                g.SmoothingMode = SmoothingMode.HighQuality;
                using (Brush brTrans = new SolidBrush(Color.Transparent))
                {
                    g.FillRectangle(brTrans, 0, 0, bmpDest.Width, bmpDest.Height);
                }//end using

                Rectangle rectCrop = rectBounds;

                if (blCropSquare)
                    rectCrop = CalculateSquareRect(rectCrop);

                GraphicsPath path = GetRoundedRectPath(rectCrop, fXRadius, fYRadius);
                g.SetClip(path);
                g.DrawImage(bmpSrc, path.GetBounds());

                g.Dispose();

                return bmpDest;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in CropRoundedRect function of CoolDraw class.");
                return null;
            }
        }

        /// <summary>
        /// Crops the bitmap passed to the image into a rounded square shape that will fit within the bounds specified in the function's parameter.  The 
        /// cropped image will be returned as a new bitmap image.
        /// </summary>
        /// <param name="bmpSrc"></param>
        /// <param name="rectBounds"></param>
        /// <param name="fXRadius"></param>
        /// <param name="fYRadius"></param>
        /// <returns></returns>
        public static Bitmap CropRoundedSquare(Bitmap bmpSrc, Rectangle rectBounds, float fXRadius, float fYRadius)
        {
            try
            {
                return CropRoundedRect(bmpSrc, rectBounds, fXRadius, fYRadius, true);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in CropRoundedSquare function of CoolDraw class.");
                return null;
            }
        }
        #endregion

        #region Text Drawing Functions

        /// <summary>
        /// Draws the text specified in the function's Text parameter using the various text-related settings passed to the parameters of the function.  
        /// The text will be drawn and rendered onto the graphics object passed to the function.  Optionally, the text can be drawn with a focus rectangle 
        /// around it if the DrawFocusRect parameter is set to true.
        /// </summary>
        /// <param name="strText"></param>
        /// <param name="graphics"></param>
        /// <param name="ptTextOffset"></param>
        /// <param name="szCanvas"></param>
        /// <param name="font"></param>
        /// <param name="ForeColor"></param>
        /// <param name="align"></param>
        /// <param name="blDrawFocusRect"></param>
        /// <param name="blWordBreak"></param>
        public static void DrawText(string strText, Graphics graphics, Point ptTextOffset, Size szCanvas, Font font, Color ForeColor, ContentAlignment align,
                                               bool blDrawFocusRect, bool blWordBreak = true)
        {
            GraphicsState origGraphState = null;

            try
            {
                // At the beginning of your drawing
                origGraphState = graphics.Save();

                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                TextFormatFlags flags = TextFormatFlags.Default;

                if(blWordBreak)
                    flags |= TextFormatFlags.WordBreak;

                flags |= GetTextFormatFlagAlign(align);

                Rectangle rectDraw = new Rectangle(ptTextOffset.X, ptTextOffset.Y, szCanvas.Width, szCanvas.Height);
                TextRenderer.DrawText(graphics, strText, font, rectDraw, ForeColor, flags);
                
                if (blDrawFocusRect)
                {
                    using (Pen penBorder = new Pen(Color.Black))
                    {
                        penBorder.DashStyle = DashStyle.Dot;                        
                        
                        Rectangle rectBorder = GetTextRectangle(strText, graphics,
                                                                                    new Rectangle(0, 0, szCanvas.Width, szCanvas.Height), font, align);

                        rectBorder.Location = new Point(rectBorder.X + ptTextOffset.X, rectBorder.Y + ptTextOffset.Y);
                        graphics.DrawRectangle(penBorder, rectBorder);
                    }//end using
                }//end if
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawText function of CoolDraw class.");
            }
            finally
            {
                // At the end of your drawing
                if (origGraphState != null)
                    graphics.Restore(origGraphState);
            }
        }

        /// <summary>
        /// Gets the TextFormatFlags that can be used for rendering text using GDI operations based on the ContentAlignment enumeration value 
        /// passed to the function.
        /// </summary>
        /// <param name="align"></param>
        /// <returns></returns>
        public static TextFormatFlags GetTextFormatFlagAlign(ContentAlignment align)
        {
            try
            {
                TextFormatFlags flags = TextFormatFlags.Default;

                switch (align)
                {
                    case ContentAlignment.TopLeft:
                        flags = (TextFormatFlags.Top | TextFormatFlags.Left);
                        break;
                    case ContentAlignment.TopCenter:
                        flags = (TextFormatFlags.Top | TextFormatFlags.HorizontalCenter);
                        break;
                    case ContentAlignment.TopRight:
                        flags = (TextFormatFlags.Top | TextFormatFlags.Right);
                        break;
                    case ContentAlignment.MiddleLeft:
                        flags = (TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
                        break;
                    case ContentAlignment.MiddleCenter:
                        flags = (TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
                        break;
                    case ContentAlignment.MiddleRight:
                        flags = (TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
                        break;
                    case ContentAlignment.BottomLeft:
                        flags = (TextFormatFlags.Bottom | TextFormatFlags.Left);
                        break;
                    case ContentAlignment.BottomCenter:
                        flags = (TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter);
                        break;
                    case ContentAlignment.BottomRight:
                        flags = (TextFormatFlags.Bottom | TextFormatFlags.Right);
                        break;
                    default:                        
                        flags = (TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
                        break;
                }//end switch

                return flags;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetTextFormatFlagAlign function of CoolDraw class.");
                return TextFormatFlags.Default;
            }
        }

        /// <summary>
        /// Calculates and returns a rectangle that surrounds the entire region of the displayed text with all formatting settings specified in the function's 
        /// parameters.
        /// </summary>
        /// <param name="strText"></param>
        /// <param name="graphics"></param>
        /// <param name="rectBounds"></param>
        /// <param name="font"></param>
        /// <param name="align"></param>
        /// <returns></returns>
        public static Rectangle GetTextRectangle(string strText, Graphics graphics, Rectangle rectBounds, Font font, ContentAlignment align)
        {
            try
            {
                Rectangle rectText = new Rectangle();
                SizeF szText = graphics.MeasureString(strText, font);

                rectText.Size = new Size(Convert.ToInt32(Math.Truncate(szText.Width) + 1), 
                                                    Convert.ToInt32(Math.Truncate(szText.Height)) + 1);

                switch (align)
                {
                    case ContentAlignment.TopLeft:
                        rectText.Location = new Point(0, 0);                   
                        break;
                    case ContentAlignment.TopCenter:
                        rectText.Location = new Point(Convert.ToInt32((rectBounds.Width - szText.Width) / 2d), 0);
                        break;
                    case ContentAlignment.TopRight:
                        rectText.Location = new Point(Convert.ToInt32(rectBounds.Width - szText.Width), 0);
                        break;
                    case ContentAlignment.MiddleLeft:
                        rectText.Location = new Point(0, Convert.ToInt32((rectBounds.Height - szText.Height) / 2d));
                        break;
                    case ContentAlignment.MiddleCenter:
                        rectText.Location = new Point(Convert.ToInt32((rectBounds.Width - szText.Width) / 2d),
                                                                   Convert.ToInt32((rectBounds.Height - szText.Height) / 2d));
                        break;
                    case ContentAlignment.MiddleRight:
                        rectText.Location = new Point(Convert.ToInt32(rectBounds.Width - szText.Width),
                                                                    Convert.ToInt32((rectBounds.Height - szText.Height) / 2d));
                        break;
                    case ContentAlignment.BottomLeft:
                        rectText.Location = new Point(0, Convert.ToInt32(rectBounds.Height - szText.Height));
                        break;
                    case ContentAlignment.BottomCenter:
                        rectText.Location = new Point(Convert.ToInt32((rectBounds.Width - szText.Width) / 2d),
                                                                    Convert.ToInt32(rectBounds.Height - szText.Height));
                        break;
                    case ContentAlignment.BottomRight:
                        rectText.Location = new Point(Convert.ToInt32(rectBounds.Width - szText.Width),
                                                                    Convert.ToInt32(rectBounds.Height - szText.Height));
                        break;
                }//end switch               

                return rectText;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetTextRectangle function of CoolDraw class.");
                return Rectangle.Empty;
            }
        }

        #endregion

        #region Image Drawing Functions

        /// <summary>
        /// Draws the image onto the graphics object specified in the function's parameter using the various image related settings passed into the 
        /// function's parameters.  If the image is set to Normal size mode, then the various alignment and offset positions can be used to position 
        /// the image around the control.  With other size modes, many of the additional imaging positioning parameters are not used and the image 
        /// will be sized according to the size of the control and image.  Optionally, a focus rectangle can be drawn around the image if the DrawFocusRect 
        /// parameter is set to true.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="graphics"></param>
        /// <param name="rectBounds"></param>
        /// <param name="align"></param>
        /// <param name="ptImageOffset"></param>
        /// <param name="blTransparent"></param>
        /// <param name="TransColor"></param>
        /// <param name="blDrawFocusRect"></param>
        /// <param name="SizeMode"></param>
        public static void DrawImage(Image image, Graphics graphics, Rectangle rectBounds, ContentAlignment align, Point ptImageOffset,
                                                   bool blTransparent = true, Color? TransColor = null, bool blDrawFocusRect = false,
                                                   CoolImageSizeMode SizeMode = CoolImageSizeMode.Normal)
        {
            GraphicsState origGraphState = null;

            try
            {
                // At the beginning of your drawing
                origGraphState = graphics.Save();
                graphics.CompositingQuality = CompositingQuality.HighQuality;

                Rectangle rectImage = Rectangle.Empty;

                if (SizeMode == CoolImageSizeMode.Normal)
                    rectImage = GetImageRectangle(image, rectBounds, align, ptImageOffset);
                else
                    rectImage = GetImageRectangle(image, rectBounds, SizeMode);

                if (image.GetType() == typeof(Bitmap))
                {
                    Bitmap bmp = (Bitmap)image;

                    if (blTransparent)
                    {
                        if (TransColor == null || TransColor == Color.Transparent)
                            bmp.MakeTransparent();
                        else
                            bmp.MakeTransparent(TransColor.Value);
                    }//end if
                }//end if

                graphics.DrawImage(image, rectImage);

                if (blDrawFocusRect)
                {
                    using (Pen penBorder = new Pen(Color.Black))
                    {
                        penBorder.DashStyle = DashStyle.Dot;
                        Rectangle rectBorder = rectImage;

                        if (SizeMode != CoolImageSizeMode.Normal)
                            rectBorder = new Rectangle(rectImage.Left + 4, rectImage.Top + 4, rectImage.Width - 4, rectImage.Height - 4);

                        graphics.DrawRectangle(penBorder, rectBorder);
                    }//end using
                }//end if                                              
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawImage Overload 1 function of CoolDraw class.");
            }
            finally
            {
                // At the end of your drawing
                if (origGraphState != null)
                    graphics.Restore(origGraphState);
            }
        }

        /// <summary>
        /// Draws the image onto the graphics object specified in the function's parameter using the various image related settings passed into the 
        /// function's parameters.  If the image is set to Normal size mode, then the various alignment and offset positions can be used to position 
        /// the image around the control.  With other size modes, many of the additional imaging positioning parameters are not used and the image 
        /// will be sized according to the size of the control and image.  Optionally, a focus rectangle can be drawn around the image if the DrawFocusRect 
        /// parameter is set to true.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="graphics"></param>
        /// <param name="rectBounds"></param>
        /// <param name="SizeMode"></param>
        /// <param name="blTransparent"></param>
        /// <param name="TransColor"></param>
        /// <param name="blDrawFocusRect"></param>
        public static void DrawImage(Image image, Graphics graphics, Rectangle rectBounds, CoolImageSizeMode SizeMode, bool blTransparent,
                                                   Color? TransColor = null, bool blDrawFocusRect = false)
        {
            try
            {
                DrawImage(image, graphics, rectBounds, ContentAlignment.TopLeft, new Point(0, 0), blTransparent, TransColor, blDrawFocusRect, SizeMode);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawImage Overload 2 function of CoolDraw class.");
            }
        }

        /// <summary>
        /// Calculates and returns a rectangle that surrounds the entire region of the image with all positioning settings specified in the function's 
        /// parameters.  In the case, the image is sized to fit across the control, such as being stretched or sized to fit, then the image rectangle will be
        /// calculated to the size of where the image will be displayed based on the size of the control and image.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="rectBounds"></param>
        /// <param name="align"></param>
        /// <param name="ptImageOffset"></param>
        /// <returns></returns>
        public static Rectangle GetImageRectangle(Image image, Rectangle rectBounds, ContentAlignment align, Point ptImageOffset)
        {
            try
            {
                Rectangle rectImage = new Rectangle();

                rectImage.Size = new Size(image.Width, image.Height);
                
                switch (align)
                {
                    case ContentAlignment.TopLeft:
                        rectImage.Location = new Point(0, 0);
                        break;
                    case ContentAlignment.TopCenter:
                        rectImage.Location = new Point(Convert.ToInt32((rectBounds.Width - image.Width) / 2d), 0);
                        break;
                    case ContentAlignment.TopRight:
                        rectImage.Location = new Point(Convert.ToInt32(rectBounds.Width - image.Width), 0);
                        break;
                    case ContentAlignment.MiddleLeft:
                        rectImage.Location = new Point(0, Convert.ToInt32((rectBounds.Height - image.Height) / 2d));
                        break;
                    case ContentAlignment.MiddleCenter:
                        rectImage.Location = new Point(Convert.ToInt32((rectBounds.Width - image.Width) / 2d),
                                                                   Convert.ToInt32((rectBounds.Height - image.Height) / 2d));
                        break;
                    case ContentAlignment.MiddleRight:
                        rectImage.Location = new Point(Convert.ToInt32(rectBounds.Width - image.Width),
                                                                    Convert.ToInt32((rectBounds.Height - image.Height) / 2d));
                        break;
                    case ContentAlignment.BottomLeft:
                        rectImage.Location = new Point(0, Convert.ToInt32(rectBounds.Height - image.Height));
                        break;
                    case ContentAlignment.BottomCenter:
                        rectImage.Location = new Point(Convert.ToInt32((rectBounds.Width - image.Width) / 2d),
                                                                    Convert.ToInt32(rectBounds.Height - image.Height));
                        break;
                    case ContentAlignment.BottomRight:
                        rectImage.Location = new Point(Convert.ToInt32(rectBounds.Width - image.Width),
                                                                    Convert.ToInt32(rectBounds.Height - image.Height));
                        break;
                }//end switch               

                rectImage.Location = new Point(rectImage.X + ptImageOffset.X, rectImage.Y + ptImageOffset.Y);

                return rectImage;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetImageRectangle Overload 1 function of CoolDraw class.");
                return Rectangle.Empty;
            }
        }

        /// <summary>
        /// Calculates and returns a rectangle that surrounds the entire region of the image with all positioning settings specified in the function's 
        /// parameters.  In the case, the image is sized to fit across the control, such as being stretched or sized to fit, then the image rectangle will be
        /// calculated to the size of where the image will be displayed based on the size of the control and image.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="rectBounds"></param>
        /// <param name="SizeMode"></param>
        /// <returns></returns>
        public static Rectangle GetImageRectangle(Image image, Rectangle rectBounds, CoolImageSizeMode SizeMode)
        {
            try
            {
                Rectangle rectImage = new Rectangle();
                
                switch (SizeMode)
                {
                    case CoolImageSizeMode.CenterImage:
                        rectImage.Size = new Size(image.Width, image.Height);
                        rectImage.X = Convert.ToInt32((rectBounds.Width - image.Width) / 2d);
                        rectImage.Y = Convert.ToInt32((rectBounds.Height - image.Height) / 2d);

                        break;
                    case CoolImageSizeMode.StretchImage:
                        rectImage = new Rectangle(rectBounds.Location, rectBounds.Size);

                        break;
                    case CoolImageSizeMode.SizeToFit:
                        rectImage.Size = SizeToFitImage(image, rectBounds);
                        rectImage.X = Convert.ToInt32((rectBounds.Width - rectImage.Width) / 2d);
                        rectImage.Y = Convert.ToInt32((rectBounds.Height - rectImage.Height) / 2d);

                        break;
                    case CoolImageSizeMode.Normal:
                        rectImage = GetImageRectangle(image, rectBounds, ContentAlignment.TopLeft, new Point(0, 0));

                        break;
                }//end switch                  

                return rectImage;          
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetImageRectangle Overload 2 function of CoolDraw class.");
                return Rectangle.Empty;
            }
        }

        /// <summary>
        /// Proportionally resizes the image to fit within the bounds specified in the function.  
        /// </summary>
        /// <param name="rectBounds"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        private static Size SizeToFitImage(Image image, Rectangle rectBounds)
        {
            try
            {
                Size szImageFit = new Size();

                float fImageRatio = image.Width / Convert.ToSingle(image.Height);

                if (fImageRatio >= 1f)
                {
                    szImageFit.Width = rectBounds.Width;
                    szImageFit.Height = Convert.ToInt32(szImageFit.Width / fImageRatio);
                }
                else
                {
                    szImageFit.Height = rectBounds.Height;
                    szImageFit.Width = Convert.ToInt32(szImageFit.Height * fImageRatio);
                }//end if          

                float fSizeDownRatio = 0f;

                if (szImageFit.Width > rectBounds.Width)
                    fSizeDownRatio = rectBounds.Width / Convert.ToSingle(szImageFit.Width);
                else if (szImageFit.Height > rectBounds.Height)
                    fSizeDownRatio = rectBounds.Height / Convert.ToSingle(szImageFit.Height);
                else
                    return szImageFit;

                szImageFit.Width = Convert.ToInt32(szImageFit.Width * fSizeDownRatio);
                szImageFit.Height = Convert.ToInt32(szImageFit.Height * fSizeDownRatio);

                return szImageFit;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in SizeToFitImage function of CoolDraw class.");
                return Size.Empty;
            }
        }

        #endregion

        #region Calculation Functions

        /// <summary>
        /// Calculates and returns a square rectangle based on the dimensions of the rectangle passed to the function.  The square rectangle will be 
        /// calculated based on the smaller of the two dimensions of the rectangle passed to the function.  The smaller dimension will be set for both 
        /// sides of the square rectangle, which will then be returned by the function.
        /// </summary>
        /// <param name="rectBounds"></param>
        /// <returns></returns>
        private static Rectangle CalculateSquareRect(Rectangle rectBounds)
        {
            try
            {
                int iSmallestDim = 0;

                if (rectBounds.Width < rectBounds.Height)
                    iSmallestDim = 1;
                else
                    iSmallestDim = 2;

                Rectangle rectSquare = new Rectangle();

                if (iSmallestDim == 1)
                {
                    rectSquare.Width = rectBounds.Width;
                    rectSquare.Height = rectBounds.Width;
                }
                else
                {
                    rectSquare.Width = rectBounds.Height;
                    rectSquare.Height = rectBounds.Height;
                }//end if                

                rectSquare.X = rectBounds.X;
                rectSquare.Y = rectBounds.Y;

                return rectSquare;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in CalculateSquareRect function of CoolDraw class.");
                return Rectangle.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="polyShape"></param>
        /// <param name="rectBounds"></param>
        /// <param name="iBorderWidth"></param>
        /// <returns></returns>
        private static Point[] GetPolyShapePoints(CoolShape polyShape, Rectangle rectBounds, int iBorderWidth = 0)
        {
            try
            {
                Point[] points = null;
                
                if (polyShape == CoolShape.Triangle)
                {
                    if (iBorderWidth <= 1)
                    {
                        points = new Point[] {
                                    new Point(rectBounds.X + Convert.ToInt32(rectBounds.Width / 2), rectBounds.Y),
                                    new Point(rectBounds.X + rectBounds.Width, rectBounds.Height + rectBounds.Y),
                                    new Point(rectBounds.X, rectBounds.Height + rectBounds.Y) };
                    }
                    else
                    {
                        points = new Point[] {
                                    new Point(rectBounds.X + Convert.ToInt32(rectBounds.Width / 2), rectBounds.Y * 2),
                                    new Point(Convert.ToInt32(rectBounds.X /2)+ rectBounds.Width, rectBounds.Height + rectBounds.Y),
                                    new Point(rectBounds.X * 2, rectBounds.Height + rectBounds.Y) };
                    }//end if
                }
                else if (polyShape == CoolShape.TriangleLeft)
                {
                    if (iBorderWidth <= 1)
                    {
                        points = new Point[] {
                                    new Point(rectBounds.X, rectBounds.Y + Convert.ToInt32(rectBounds.Height / 2)),
                                    new Point(rectBounds.X + rectBounds.Width, rectBounds.Y),
                                    new Point(rectBounds.X + rectBounds.Width, rectBounds.Height + rectBounds.Y) };
                    }
                    else
                    {
                        points = new Point[] {
                                    new Point(rectBounds.X * 2, rectBounds.Y + Convert.ToInt32(rectBounds.Height / 2)),
                                    new Point(Convert.ToInt32(rectBounds.X / 2) + rectBounds.Width, rectBounds.Y * 2),
                                    new Point(Convert.ToInt32(rectBounds.X / 2) + rectBounds.Width, rectBounds.Height + rectBounds.Y) };
                    }//end if
                }
                else if (polyShape == CoolShape.TriangleRight)
                {
                    if (iBorderWidth <= 1)
                    {
                        points = new Point[] {
                                    new Point(rectBounds.X + rectBounds.Width, rectBounds.Y + Convert.ToInt32(rectBounds.Height / 2)),
                                    new Point(rectBounds.X, rectBounds.Y + rectBounds.Height),
                                    new Point(rectBounds.X, rectBounds.Y) };
                    }
                    else
                    {
                        points = new Point[] {
                                    new Point(Convert.ToInt32(rectBounds.X / 1.25) + rectBounds.Width, rectBounds.Y + Convert.ToInt32(rectBounds.Height / 2)),
                                    new Point(Convert.ToInt32(rectBounds.X * 1.25), rectBounds.Y + rectBounds.Height),
                                    new Point(Convert.ToInt32(rectBounds.X * 1.25), Convert.ToInt32(rectBounds.Y * 1.25)) };
                    }//end if
                }
                else  if (polyShape == CoolShape.TriangleBottom)
                {
                    if (iBorderWidth <= 1)
                    {
                        points = new Point[] {
                                    new Point(rectBounds.X + Convert.ToInt32(rectBounds.Width / 2), rectBounds.Y + rectBounds.Height),
                                    new Point(rectBounds.X, rectBounds.Y),
                                    new Point(rectBounds.X + rectBounds.Width, rectBounds.Y) };
                    }
                    else
                    {
                        points = new Point[] {
                                    new Point(rectBounds.X + Convert.ToInt32(rectBounds.Width / 2), rectBounds.Y + rectBounds.Height),
                                    new Point(Convert.ToInt32(rectBounds.X * 1.25), Convert.ToInt32(rectBounds.Y * 1.25)),
                                    new Point(Convert.ToInt32(rectBounds.X / 1.25) + rectBounds.Width, Convert.ToInt32(rectBounds.Y * 1.25)) };
                    }//end if
                }
                else if (polyShape == CoolShape.Diamond)
                {
                    if (iBorderWidth <= 1)
                    {
                        points = new Point[] {
                                    new Point(rectBounds.X + Convert.ToInt32(rectBounds.Width / 2), rectBounds.Y),
                                    new Point(rectBounds.X + rectBounds.Width, Convert.ToInt32(rectBounds.Height / 2) + rectBounds.Y),
                                    new Point(rectBounds.X + Convert.ToInt32(rectBounds.Width / 2), rectBounds.Height + rectBounds.Y),
                                    new Point(rectBounds.X, Convert.ToInt32(rectBounds.Height / 2) + rectBounds.Y) };
                    }
                    else
                    {
                        points = new Point[] {
                                    new Point(rectBounds.X + Convert.ToInt32(rectBounds.Width / 2), rectBounds.Y * 2),
                                    new Point(Convert.ToInt32(rectBounds.X / 2) + rectBounds.Width, Convert.ToInt32(rectBounds.Height / 2) + rectBounds.Y),
                                    new Point(rectBounds.X + Convert.ToInt32(rectBounds.Width / 2), rectBounds.Height + rectBounds.Y),
                                    new Point(rectBounds.X * 2, Convert.ToInt32(rectBounds.Height / 2) + rectBounds.Y) };
                    }//end if
                }//end if

                return points;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetPolyShapePoints function of CoolDraw class.");
                return null;
            }
        }

        /// <summary>
        /// Calculates and generates a gradient path that is used to draw a rounded rectangle based on the parameters that are passed to the function.  
        /// The gradient path containing all the points quired to draw the rounded rectangle will be returned by the function after the path is constructed. 
        /// </summary>
        /// <param name="rectBounds"></param>
        /// <param name="fXRadius"></param>
        /// <param name="fYRadius"></param>
        /// <param name="iBorderWidth"></param>
        /// <returns></returns>
        internal static GraphicsPath GetRoundedRectPath(Rectangle rectBounds, float fXRadius, float fYRadius, int iBorderWidth = 0)
        {
            try
            {
                RectangleF rectCalc = new RectangleF(rectBounds.X, rectBounds.Y, rectBounds.Width, rectBounds.Height);                

                //Make a Graphics path to draw the rectangle.
                PointF point1, point2;
                RectangleF corner;
                GraphicsPath path = new GraphicsPath();
            
                //Upper Left Corner                
                corner = new RectangleF(rectCalc.X, rectCalc.Y, 2 * fXRadius, 2 * fYRadius);
                path.AddArc(corner, 180, 90);
                point1 = new PointF(rectCalc.X + fXRadius, rectCalc.Y);

                //Top Side
                point2 = new PointF(rectCalc.Right - fXRadius, rectCalc.Y);
                path.AddLine(point1, point2);

                //Upper Right Corner
                corner = new RectangleF(rectCalc.Right - 2 * fXRadius, rectCalc.Y, 2 * fXRadius, 2 * fYRadius);
                path.AddArc(corner, 270, 90);
                point1 = new PointF(rectCalc.Right, rectCalc.Y + fYRadius);

                //Right Side
                point2 = new PointF(rectCalc.Right, rectCalc.Bottom - fYRadius);
                path.AddLine(point1, point2);

                //Lower Right Corner
                corner = new RectangleF(rectCalc.Right - 2 * fXRadius, rectCalc.Bottom - 2 * fYRadius, 2 * fXRadius, 2 * fYRadius);
                path.AddArc(corner, 0, 90);
                point1 = new PointF(rectCalc.Right - fXRadius, rectCalc.Bottom);

                //Bottom Side
                point2 = new PointF(rectCalc.X + fXRadius, rectCalc.Bottom);
                path.AddLine(point1, point2);

                //Lower Left Corner
                corner = new RectangleF(rectCalc.X, rectCalc.Bottom - 2 * fYRadius, 2 * fXRadius, 2 * fYRadius);
                path.AddArc(corner, 90, 90);
                point1 = new PointF(rectCalc.X, rectCalc.Bottom - fYRadius);

                //Left Side
                point2 = new PointF(rectCalc.X, rectCalc.Y + fYRadius);
                path.AddLine(point1, point2);                                               
                
                //Join with the start point.
                path.CloseFigure();

                return path;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetRoundedRectPoints function of CoolDraw class.");
                return null;
            }
        }

        #endregion

        #region Information/Region Identification Related Functions

        /// <summary>
        /// The function checks to see if the point passed in the point parameter is contained within the array of points specified in the aryPolygon 
        /// parameter.  If the array of points contained the specified point specified, then the function will return true, else it returns false.
        /// NOTE: The current implementation of this function performs a loop on the array which delays the appropriate mouse effects of the 
        /// DotCoolControls from being displayed when the mouse interacts with the control.  In future versions a hash table will be generated 
        /// with the array points of the control for quicker checks of the region boundaries of the control.
        /// </summary>
        /// <param name="aryPolygon"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool ContainsPoint(PointF[] aryPolygon, PointF point)
        {
            try
            {
                bool blInside = false;

                for (int i = 0, j = aryPolygon.Length - 1; i < aryPolygon.Length; j = i++)
                {
                    if (((aryPolygon[i].Y > point.Y) != (aryPolygon[j].Y > point.Y)) &&
                    (point.X < (aryPolygon[j].X - aryPolygon[i].X) * (point.Y - aryPolygon[i].Y) / (aryPolygon[j].Y - aryPolygon[i].Y) + aryPolygon[i].X))
                    {
                        blInside = !blInside;
                    }
                }
                
                return blInside;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in ContainsPoint function of CoolDraw class.");
                return false;
            }
        }        

        #endregion
    }
}

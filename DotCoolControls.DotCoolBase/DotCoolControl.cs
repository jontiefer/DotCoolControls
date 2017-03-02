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
using DotCoolControls.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotCoolControls.VisualSettings;
using Tiferix.Global.Win32API;
using System.Runtime.InteropServices;
using System.Collections;
using System.Threading;
using System.ComponentModel.Design.Serialization;
using System.IO;

namespace DotCoolControls
{
    /// <summary>
    /// The DotCoolControl class provides the prototype for all controls in the DotCoolControls library.  All Dot Cool controls will be derived from this 
    /// abstract class.
    /// </summary>  
    [ToolboxItemFilter("DotCoolControls.DotCoolBase")]
    public class DotCoolControl : Control
    {
        #region Enumerations 

        public enum ControlSelectBehavior
        {
            Border = 1,
            Control = 2
        }

        public enum FocusRectEnum
        {
            Text = 1,
            Image = 2
        }       

        #endregion

        #region Events

        protected EventHandler DoubleClickHandlers;
        protected int m_iDoubleClickHandlerCount = 0;

        /// <summary>
        /// Occurs when the control is double-clicked.
        /// </summary>
        [Browsable(true), Category("Action"), 
         Description("Occurs when the control is double-clicked.")]
        public new event EventHandler DoubleClick
        {
            add
            {
                if (m_iDoubleClickHandlerCount == 0 || DoubleClickHandlers.GetInvocationList().Contains(value))
                {
                    base.DoubleClick += value;
                    DoubleClickHandlers += value;
                    m_iDoubleClickHandlerCount++;
                }//end if
            }
            remove
            {
                if (DoubleClickHandlers.GetInvocationList().Contains(value))
                {
                    base.DoubleClick -= value;
                    DoubleClickHandlers -= value;
                    m_iDoubleClickHandlerCount--;
                }//end if
            }
        }
        
        #endregion

        #region Member Variables

        protected CancelEvents m_CancelEvents = new CancelEvents();

        internal static bool m_blKeyHookActive = false;

        #endregion

        #region Member Object Variables

        //private Form m_ParentForm = null;        

        private object m_oLock = new object();
          
        #endregion

        #region Focus/Activation Variables

        protected bool m_blFocusOnClick = false;

        private ControlSelectBehavior m_CtlSelBehavior = ControlSelectBehavior.Border;

        private FocusRectEnum m_FocusRect = FocusRectEnum.Text;

        #endregion

        #region Background and General Gradient Variables

        //protected bool m_blDrawBackGrad = false;

        #endregion

        #region Gradient Variables

        /// <summary>
        /// Contains the various gradient settings for the background of the DotCoolControl class.
        /// </summary>
        protected DotCoolCtlGradientSettingsList m_GradBackSettingsList = null;

        #endregion

        #region Border and Region Variables        

        /// <summary>
        /// Contains the various border settings of the DotCoolControl class.
        /// </summary>
        protected DotCoolCtlBorderSettingsList m_BorderSettingsList = null;

        /// <summary>
        /// Contains a reference to the border color settings class that will allow the user to view and modify the control's border color settings for 
        /// each control state/scenario using an expandable property grid.
        /// </summary>
        protected DotCoolCtlBorderColorSettings m_BorderColorSettings = null;

        /// <summary>
        /// Contains a reference to the border width settings class that will allow the user to view and modify the control's border width settings for 
        /// each control state/scenario using an expandable property grid.
        /// </summary>
        protected DotCoolCtlBorderWidthSettings m_BorderWidthSettings = null;

        protected PointF[] m_aryBorderPointsF = null;

        protected Point[] m_aryBorderPoints = null;

        protected IntPtr m_hBorderRgn = IntPtr.Zero;

        protected bool m_blCalcBorderRgn = true;        

        #endregion

        #region Caption Variables        

        /// <summary>
        /// Contains the various text and font settings for the caption displayed in the DotCoolControl class.
        /// </summary>
        protected DotCoolCtlTextSettingsList m_TextSettingsList = null;

        #endregion

        #region Image Variables

        /// <summary>
        /// Contains the various settings for the images displayed in the DotCoolControl class.
        /// </summary>
        protected DotCoolCtlImageSettingsList m_ImageSettingsList = null;

        #endregion

        #region Mouse Position and Mouse Specific Variables

        protected bool m_blMouseOver = false;

        protected bool m_blMouseDown = false;
        
        #endregion

        #region Hot Key Variables
        
        protected KeyboardHook m_KeyHook = null;
        
        protected Keys m_HotKey = Keys.None;

        private ModifierKeys m_HotKeyMod = 0;

        private Keys m_HotKeyVal = Keys.None;

        protected int m_iHotKeyID = 0;

        #endregion

        #region Constructor/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public DotCoolControl()
            : base()
        {
            try
            {                                
                this.SetStyle(ControlStyles.DoubleBuffer, true);
                this.SetStyle(ControlStyles.ResizeRedraw, true);
                this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                this.SetStyle(ControlStyles.UserPaint, true);
                this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

                this.DoubleBuffered = true;

                m_GradBackSettingsList = new DotCoolCtlGradientSettingsList(this);

                m_BorderSettingsList = new DotCoolCtlBorderSettingsList(this);
                m_BorderColorSettings = new DotCoolCtlBorderColorSettings(m_BorderSettingsList);
                m_BorderWidthSettings = new DotCoolCtlBorderWidthSettings(m_BorderSettingsList);

                m_TextSettingsList = new DotCoolCtlTextSettingsList(this);
                m_ImageSettingsList = new DotCoolControls.DotCoolCtlImageSettingsList(this);

                m_blFocusOnClick = true;
                m_CtlSelBehavior = ControlSelectBehavior.Border;

                for (VisualSettingEnum setting = VisualSettingEnum.Normal; setting <= VisualSettingEnum.Indeterminate; setting++)
                {
                    if (setting != VisualSettingEnum.Normal)
                    {
                        m_BorderSettingsList[setting].BorderColor = Color.Transparent;
                        m_ImageSettingsList[setting].EnableImage = false;
                        m_TextSettingsList[setting].EnableText = false;
                    }
                    else
                    {
                        m_ImageSettingsList[setting].EnableImage = true;
                        m_TextSettingsList[setting].EnableText = true;
                    }//end if                    
                }//next setting          

                if (!DesignMode)
                {
                    m_KeyHook = new KeyboardHook();

                    //Register the event that is fired after the key press.
                    m_KeyHook.KeyPressed += new EventHandler<KeyPressedEventArgs>(KeyHook_KeyPressed);                    
                }//end if
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Constructor function of DotCoolControl control.");
            }
        }
                
        /// <summary>
        /// Raises the Control.OnVisibleChanged event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            try
            {
                base.OnVisibleChanged(e);

                if (DesignMode)
                    return;

                UpdateHotKey();                                                
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnVisibleChanged function of DotCoolControl class.");
            }
        }

        #endregion

        #region Destruction/Cleanup

        /// <summary>
        /// Releases the unmanaged resources used by the System.Windows.Forms.Control and its child controls and optionally releases 
        /// the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged
        /// resources.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (m_KeyHook != null)
                {
                    if (m_iHotKeyID > 0)
                        m_KeyHook.UnregisterHotKey(m_iHotKeyID);

                    m_KeyHook.Dispose();
                }//end if

                if(m_hBorderRgn != IntPtr.Zero)
                {
                    WinAPI.DeleteObject(m_hBorderRgn);
                    m_hBorderRgn = IntPtr.Zero;
                }//end if

                base.Dispose(disposing);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Dispose function of DotCoolControl class.");
            }
        }

        #endregion

        #region Overriden Window Procedure/Window Message/Parent Form Event Handling Functions, Event Handlers       

        /// <summary>
        /// Processes Windows messages.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            try
            {                
                base.WndProc(ref m);

                if (DesignMode)
                    return;

                if (!this.IsDisposed)
                {
                    if (m.HWnd == this.Handle && (WM)m.Msg == WM.WM_KEYUP)
                    {
                        if ((VK)m.WParam == VK.VK_SPACE)
                        {
                            PerformClick();
                        }//end if                    
                    }//end if
                }//end if
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in WndProc function of DotCoolControl class.", "", true);
            }
        }
        
        /// <summary>
        /// Processes a mnemonic character.
        /// </summary>
        /// <param name="inputChar"></param>
        /// <returns></returns>
        protected override bool ProcessMnemonic(char inputChar)
        {
            try
            {
                if (CanSelect && IsMnemonic(inputChar, this.Text))
                {                    
                    this.PerformClick();
                    return true;
                }//end if

                return false;                
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in ProcessMnemonic function of DotCoolControl.");
                return false;
            }
        }        

        #endregion

        #region General Control Drawing/Paint/GDI+ Functions, Event Handlers

        /// <summary>
        /// Handles all custom GDI/drawing/painting operations of the DotCoolControl, including the drawing of the control's gradients, text and images.
        /// </summary>
        /// <param name="pevent"></param>
        protected override void OnPaint(PaintEventArgs pevent)
        {
            IntPtr hDC = IntPtr.Zero;
            Bitmap bmpCtlMem = null;

            try
            {                
                hDC = pevent.Graphics.GetHdc();
                Graphics gCtlMem = Graphics.FromHdc(hDC);
                pevent.Graphics.ReleaseHdc();
                hDC = IntPtr.Zero;

                DrawBackGradientImage(gCtlMem);
                DrawControlText(gCtlMem);
                DrawControlImage(gCtlMem);

                bmpCtlMem = new Bitmap(this.Width, this.Height);
                gCtlMem.DrawImage(bmpCtlMem, 0, 0);
                gCtlMem.Dispose();

                pevent.Graphics.DrawImage(bmpCtlMem, 0, 0);                
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnPaint function of DotCoolControl control.");
            }
            finally
            {
                if (hDC != IntPtr.Zero)
                    pevent.Graphics.ReleaseHdc();

                if (bmpCtlMem != null)
                    bmpCtlMem.Dispose();
            }
        }

        #endregion

        #region Background Drawing/Paint/GDI+ Functions, Event Handlers

        /// <summary>
        /// Draws the background gradient image in the control based on the various gradient settings that are set for the control.  All drawing will 
        /// be performed in memory-based device context before rendering onto the control to allow for flicker-free drawing.
        /// </summary>
        protected virtual void DrawBackGradientImage(Graphics gCtl)
        {
            Bitmap bmpGradBackMem = null;
            Graphics gGradBackMem = null;
            GraphicsState origGraphState = null;

            try
            {
                // At the beginning of your drawing
                origGraphState = gCtl.Save();

                bmpGradBackMem = new Bitmap(GetBorderSize().Width, GetBorderSize().Height);
                gGradBackMem = Graphics.FromImage(bmpGradBackMem);

                CoolGradientType BackGradTypeActive = GetBackGradientType();
                Color BackGradColor1Active = GetBackGradientColor(1);
                Color BackGradColor2Active = GetBackGradientColor(2);
                float fGradientSpan = GetBackGradientSpan();
                Point ptGradOffset = GetBackGradientOffset();

                Color BorderColorActive = GetBorderColor();
                int iBorderWidth = GetBorderWidth();

                //if (BackGradTypeActive == CoolGradientType.None)
                //    ClearBackGradientImage();

                Rectangle rectBounds = new Rectangle(Convert.ToInt32(iBorderWidth / 2), Convert.ToInt32(iBorderWidth / 2),
                                                                GetBorderSize().Width - iBorderWidth - 1,
                                                                GetBorderSize().Height - iBorderWidth - 1);

                if (BackGradColor1Active != Color.Transparent && BackGradTypeActive != CoolGradientType.None)
                    m_aryBorderPointsF = CoolDraw.DrawGradientShape(BorderShape, BackGradTypeActive, gGradBackMem, BackGradColor1Active,
                                                                BackGradColor2Active, rectBounds, BorderColorActive, iBorderWidth,
                                                                fGradientSpan, ptGradOffset.X, ptGradOffset.Y,
                                                                BorderRadius.X, BorderRadius.Y);
                else
                {
                    Color? FillColor = null;
                    if (BackGradColor1Active != Color.Transparent)
                        FillColor = BackGradColor1Active;

                    m_aryBorderPointsF = CoolDraw.DrawShape(BorderShape, gGradBackMem, rectBounds, BorderColorActive, iBorderWidth,
                                                                BorderRadius.X, BorderRadius.Y, FillColor);
                }//end if

                Rectangle rectDraw = new Rectangle(GetBorderOffset().X, GetBorderOffset().Y, GetBorderSize().Width, GetBorderSize().Height);
                //gCtl.CompositingMode = CompositingMode.SourceCopy;
                gCtl.DrawImage(bmpGradBackMem, rectDraw);

                CalculateBorderAreaRegion();                
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawBackGradientImage function of DotCoolControl control.");
            }
            finally
            {
                if (origGraphState != null)
                    gCtl.Restore(origGraphState);

                if (gGradBackMem != null)
                    gGradBackMem.Dispose();

                if (bmpGradBackMem != null)
                    bmpGradBackMem.Dispose();
            }
        }        

        #endregion

        #region Image and Text Drawing/Painting Functions, Event Handlers

        /// <summary>
        /// Draws the text or caption set for the control over the background of the control.  The text will be drawn and rendered onto the control based
        /// on the various text settings that are set in the control.
        /// </summary>
        /// <param name="gCtl"></param>
        protected virtual void DrawControlText(Graphics gCtl)
        {
            GraphicsState origGraphState = null;

            try
            {
                if (this.Text.Trim() == "")
                    return;

                origGraphState = gCtl.Save();
                gCtl.CompositingMode = CompositingMode.SourceOver;

                CoolDraw.DrawText(this.Text, gCtl, GetTextOffset(), this.Bounds.Size, GetFont(), GetForeColor(), GetTextAlign(),
                                             this.Focused && GetDrawFocusRect() == FocusRectEnum.Text);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawControlText function of DotCoolButton class.");
            }
            finally
            {
                if (origGraphState != null)
                    gCtl.Restore(origGraphState);
            }
        }

        /// <summary>
        /// Draws the image for the control over the background of the control.  The image will be drawn and rendered onto the control based
        /// on the various image settings that are set in the control.
        /// </summary>
        /// <param name="gCtl"></param>
        protected virtual void DrawControlImage(Graphics gCtl)
        {
            GraphicsState origGraphState = null;

            try
            {
                origGraphState = gCtl.Save();
                gCtl.CompositingMode = CompositingMode.SourceOver;

                Image imgActive = GetImage();

                if (imgActive != null)
                {
                    CoolDraw.DrawImage(GetImage(), gCtl, new Rectangle(0, 0, this.Width, this.Height), GetImageAlign(), GetImageOffset(),
                                                    true, GetImageTransColor(), this.Focused && GetDrawFocusRect() == FocusRectEnum.Image);
                }//end if
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawControlImage function of DotCoolButton class.");
            }
            finally
            {
                if (origGraphState != null)
                    gCtl.Restore(origGraphState);
            }
        }

        #endregion

        #region Control Mouse Interaction Functions, Event Handlers

        /// <summary>
        /// If the control's selection behavior is set to Control, the <seealso cref="Control.MouseEnter"/> event will be raised when it enters 
        /// the physical area of the control (not border).  In the case the selection behavior is set to Border, the MouseEnter event will only be raised when 
        /// the mouse cursor is moved within the boundaries of the control's borders.  In this scenario, the MouseMove event will be used to detect when
        /// the mouse has entered the boundaries of the control and will then raise the MouseEnter event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            try
            {                
                if (GetSelectBehavior() == ControlSelectBehavior.Control)
                {
                    if (!m_blMouseOver)
                    {
                        base.OnMouseEnter(new EventArgs());

                        m_blMouseOver = true;
                        this.Refresh();
                    }//end if
                }//end if                
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnMouseEnter function of DotCoolControl class.", "", true);
            }
        }

        /// <summary>
        /// If the control's selection behavior is set to Control, then raises the MouseLeave event.  In the case the selection behavior is set to Border, the 
        /// MouseLeave event will not be raised by the control unless the mouse was previously moved within the boundaries of the control's borders.
        /// If it was detected that the mouse has moved within the boundaries of the control's border, then the MouseLeave event will be raised and the 
        /// MouseOver flag will be set to false.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            try
            {                
                if (GetSelectBehavior() == ControlSelectBehavior.Control || m_blMouseOver)
                {
                    if (m_blMouseOver)
                    {
                        base.OnMouseLeave(new EventArgs());

                        m_blMouseOver = false;
                        this.Refresh();
                    }//end if
                }//end if
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnMouseLeave function of DotCoolControl class.", "", true);
            }
        }

        /// <summary>
        /// If the control's selection behavior is set to Control, then raises the MouseMove event.  In the case the control's selection behavior is set to 
        /// Border, the MouseMove event will be used to detect if the mouse cursor has been moved within the boundaries of the control's border.  If the 
        /// mouse cursor is detected within the border of the control, then the MouseMove event will be raised.  As well, the MouseEnter event will be 
        /// raised from the MouseMove event when the mouse cursor first enters the boundaries of the control's border.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            try
            {                
                if (GetSelectBehavior() == ControlSelectBehavior.Control)
                    base.OnMouseMove(e);
                else
                {
                    if (m_aryBorderPoints != null && ContainsPoint(e.Location)) //CoolDraw.ContainsPoint(m_aryBorderPoints, e.Location))
                    {                        
                        base.OnMouseMove(e);

                        if (!m_blMouseOver)
                        {
                            base.OnMouseEnter(new EventArgs());

                            m_blMouseOver = true;
                            this.Refresh();                            
                        }//end if                        
                    }
                    else
                    {
                        if (m_blMouseOver)
                        {
                            base.OnMouseLeave(new EventArgs());

                            m_blMouseOver = false;
                            this.Refresh();                            
                        }//end if
                    }//end if                    
                }//end if
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnMouseMove function of DotCoolControl class.", "", true);
            }
        }

        /// <summary>
        /// If the control's selection behavior is set to Control, then raises the MouseDown event.  In the case the selection behavior is set to Border, 
        /// the MouseDown event will be raised only when the mouse button is pushed down while the mouse cursor is located within the boundaries 
        /// of the control's border.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            try
            {                
                if (GetSelectBehavior() == ControlSelectBehavior.Control ||
                    (m_aryBorderPoints != null && ContainsPoint(e.Location)))//CoolDraw.ContainsPoint(m_aryBorderPoints, e.Location)))
                {
                    base.OnMouseDown(e);

                    if (e.Button == MouseButtons.Left)
                    {
                        if (!m_blMouseDown)
                        {
                            m_blMouseDown = true;
                            this.Refresh();                                                      
                        }//end if
                    }//end if
                }//end if                
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnMouseDown function of DotCoolControl class.", "", true);
            }
        }

        /// <summary>
        /// The <seealso cref="Control.MouseUp"/> event will be raised when the mouse button is released after being pushed down while the mouse cursor was located within the 
        /// boundaries of the control's border.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            try
            {                
                if (m_blMouseDown)
                {
                    base.OnMouseUp(e);

                    m_blMouseDown = false;
                    this.Refresh();                    
                }//end if
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnMouseUp function of DotCoolControl class.", "", true);
            }
        }

        /// <summary>
        /// The <seealso cref="Control.Click"/>  Click event will be raised when the mouse button is clicked within the boundaries of the control's border.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClick(EventArgs e)
        {
            if (m_blMouseDown)
            {
                if (m_blFocusOnClick)
                    this.Focus();

                base.OnClick(e);
            }//end if
        }

        /// <summary>
        /// The <seealso cref="Control.DoubleClick"></seealso> event will be raised when the mouse button is double-clicked within the boundaries of the control's border.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDoubleClick(EventArgs e)
        {
            if (m_blMouseDown)
            {
                if (m_blFocusOnClick)
                    this.Focus();

                //If no DoubleClick event handlers are loaded, then the DoubleClick event will raise the Click event instead.
                if (m_iDoubleClickHandlerCount == 0)
                    base.OnClick(e);
                else
                    base.OnDoubleClick(e);
            }//end if
        }

        #endregion

        #region Control Focus Functions, Event Handlers

        /// <summary>
        /// Raises the <seealso cref="Control.GotFocus"/> event of the control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(EventArgs e)
        {
            try
            {
                base.OnGotFocus(e);
                this.Refresh();                
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnGotFocus function of DotCoolControl class.", "", true);
            }
        }

        /// <summary>
        /// Raises the <seealso cref="Control.LostFocus"/> event of the control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(EventArgs e)
        {
            try
            {
                base.OnLostFocus(e);
                this.Refresh();
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnLostFocus function of DotCoolControl class.", "", true);
            }
        }

        #endregion

        #region Current State Border Properties, Functions

        /// <summary>
        /// Gets the appropriate border color of the control based on the control's current state.
        /// </summary>
        /// <returns></returns>
        protected Color GetBorderColor()
        {
            try
            {
                if (!this.Enabled && BorderColorSettings.BorderColor != Color.Transparent)
                {
                    return BorderColorSettings.BorderColor;
                }
                else if (m_blMouseDown && BorderColorSettings.BorderColorMouseDown != Color.Transparent)
                {
                    return BorderColorSettings.BorderColorMouseDown;
                }
                else if (m_blMouseOver && BorderColorSettings.BorderColorMouseOver != Color.Transparent)
                {
                    return BorderColorSettings.BorderColorMouseOver;
                }
                else
                {
                    return BorderColorSettings.BorderColor;
                }//end if
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetBorderColor function of DotCoolControl class.");
                return Color.Black;
            }
        }

        /// <summary>
        /// Retrieves the appropriate border width of the control based on the control's current state.
        /// </summary>
        /// <returns></returns>
        protected int GetBorderWidth()
        {
            try
            {
                if (m_blMouseDown)
                {
                    return BorderWidthSettings.BorderWidthMouseDown;
                }
                else
                {
                    return BorderWidthSettings.BorderWidth;
                }//end if
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetBorderWidth function of DotCoolControl class.");
                return 0;
            }
        }

        #endregion

        #region Border (All States) Appearance Properties, Functions

        /// <summary>
        /// Gets or sets the shape of the control.
        /// </summary>
        [Browsable(true), Category("CoolBorder"), DefaultValue(CoolShape.Rectangle),
         Description("Gets or sets the shape of the control.")]
        public virtual CoolShape BorderShape
        {
            get
            {
                return m_BorderSettingsList[VisualSettingEnum.Normal].BorderShape;
            }
            set
            {
                m_BorderSettingsList[VisualSettingEnum.Normal].BorderShape = value;
                this.Refresh();
            }
        }

        #endregion

        #region Border Color (State Specific) Appearance Properties, Functions

        /// <summary>
        /// Gets or sets the color of the control's border for each associated control state.
        /// </summary>
        [Browsable(true), Category("CoolBorder"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Gets or sets the color of the control's border for each associated control state.")]
        public virtual DotCoolCtlBorderColorSettings BorderColorSettings
        {
            get
            {
                return m_BorderColorSettings;
            }
        }

        #endregion

        #region Border (All States) Size and Position Properties, Functions

        /// <summary>
        /// Gets the size of the control's border boundaries.  
        /// </summary>
        /// <returns></returns>
        protected virtual Size GetBorderSize()
        {
            try
            {
                return m_BorderSettingsList[VisualSettingEnum.Normal].BorderSize;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetBorderSize function of DotCoolControl class.");
                return Size.Empty;
            }
        }

        /// <summary>
        /// Sets the size of the control's border boundaries.  This method will be used by inherited classes to manipulate the size of the
        /// of the control's borders, whether internally or directly by the user via properties.
        /// </summary>
        protected virtual void SetBorderSize(Size szBorder)
        {
            try
            {
                m_BorderSettingsList[VisualSettingEnum.Normal].BorderSize = szBorder;
                m_blCalcBorderRgn = true;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in SetBorderSize function of DotCoolControl class.");
            }
        }

        /// <summary>
        /// Gets or sets the radius of the border's corners when drawing shapes that have curved corners or edges.
        /// </summary>
        [Browsable(true), Category("CoolBorder"),
         Description("Gets or sets the radius of the border's corners when drawing shapes that have curved corners or edges.")]
        public virtual Point BorderRadius
        {
            get
            {
                return m_BorderSettingsList[VisualSettingEnum.Normal].BorderRadius;
            }
            set
            {
                m_BorderSettingsList[VisualSettingEnum.Normal].BorderRadius = value;
                m_blCalcBorderRgn = true;
            }
        }
        
        /// <summary>
        /// Sets the X and Y offset positions of the control's border boundaries.  This method will be used by inherited classes to manipulate the position 
        /// of the control's borders, whether internally or directly by the user via properties.
        /// </summary>
        protected virtual void SetBorderOffset(Point ptOffset)
        {
            try
            {
                m_BorderSettingsList[VisualSettingEnum.Normal].BorderOffset = ptOffset;
                m_blCalcBorderRgn = true;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in SetBorderOffset function of DotCoolControl class.");
            }
        }
        
        /// <summary>
        /// Gets the X and Y offset positions of the control's border boundaries.
        /// </summary>
        /// <returns></returns>
        protected virtual Point GetBorderOffset()
        {
            try
            {
                return m_BorderSettingsList[VisualSettingEnum.Normal].BorderOffset;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetBorderOffset function of DotCoolControl class.");
                return Point.Empty;
            }
        }
        
        protected virtual void CalculateBorderAreaRegion()
        {
            try
            {                
                if (!m_blCalcBorderRgn)
                    return;
                                  
                if (m_hBorderRgn != IntPtr.Zero)
                {
                    WinAPI.DeleteObject(m_hBorderRgn);
                    m_hBorderRgn = IntPtr.Zero;
                }//end if

                m_aryBorderPoints = m_aryBorderPointsF.Select(a => new Point(Convert.ToInt32(a.X), Convert.ToInt32(a.Y))).ToArray();                
                m_hBorderRgn = WinAPI.CreatePolygonRgn(m_aryBorderPoints, m_aryBorderPoints.Length, 1);

                m_blCalcBorderRgn = false;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in CalculateBorderAreaRegion function of DotCoolControl class.");
            }
        }

        /// <summary>
        /// The function checks to see if the point passed in the point parameter is contained within the array of points that compose the border region 
        /// of the control.  If the array of points contained the specified point specified, then the function will return true, else it returns false.        
        /// </summary>        
        /// <param name="point"></param>
        /// <returns></returns>
        protected virtual bool ContainsPoint(Point point)
        {
            try
            {                
                if (m_hBorderRgn != IntPtr.Zero)
                {
                    return WinAPI.PtInRegion(m_hBorderRgn, point.X, point.Y);
                }//end if

                return false;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in ContainsPoint function of DotCoolControl class.");
                return false;
            }
        }

        #endregion

        #region Border Width (State-Specific) Properties, Functions

            /// <summary>
            /// Gets or sets the width (thickness) of the control's border for each associated control state.
            /// </summary>
        [Browsable(true), Category("CoolBorder"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Gets or sets the width (thickness) of the control's border for each associated control state.")]
        public virtual DotCoolCtlBorderWidthSettings BorderWidthSettings
        {
            get
            {                
                return m_BorderWidthSettings;                
            }
        }

        #endregion

        #region Current State Background Color/Gradient Properties, Functions     

        /// <summary>
        /// Gets the appropriate background gradient start or ending color of the control based on the control's current state.
        /// </summary>
        /// <param name="iColorIndex">1 = Gradient Starting Color, 2 = Gradient Ending Color</param>
        /// <returns></returns>
        protected virtual Color GetBackGradientColor(int iColorIndex)
        {
            try
            {
                if (!this.Enabled && BackGradientSettingsDisabled.GradientColor1 != Color.Transparent)
                {
                    if (iColorIndex == 1)
                        return BackGradientSettingsDisabled.GradientColor1;
                    else
                        return BackGradientSettingsDisabled.GradientColor2;
                }
                else if (m_blMouseDown && GetBackGradientSettingsMouseDown().GradientColor1 != Color.Transparent)
                {
                    if (iColorIndex == 1)
                        return GetBackGradientSettingsMouseDown().GradientColor1;
                    else
                        return GetBackGradientSettingsMouseDown().GradientColor2;
                }
                else if (m_blMouseOver && BackGradientSettingsMouseOver.GradientColor1 != Color.Transparent)
                {
                    if (iColorIndex == 1)
                        return BackGradientSettingsMouseOver.GradientColor1;
                    else
                        return BackGradientSettingsMouseOver.GradientColor2;
                }
                else
                {
                    if (iColorIndex == 1)
                        return BackGradientSettings.GradientColor1;
                    else
                        return BackGradientSettings.GradientColor2;
                }//end if
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetBackGradientColor function of DotCoolControl control.");
                return Color.Black;
            }
        }

        /// <summary>
        /// Gets the appropriate background gradient type of the control based on the control's current state.
        /// </summary>
        /// <returns></returns>
        protected virtual CoolGradientType GetBackGradientType()
        {
            try
            {
                if (!this.Enabled && BackGradientSettingsDisabled.GradientColor1 != Color.Transparent)
                    return BackGradientSettingsDisabled.GradientType;
                else if (m_blMouseDown && GetBackGradientSettingsMouseDown().GradientColor1 != Color.Transparent)
                    return GetBackGradientSettingsMouseDown().GradientType;
                else if (m_blMouseOver && BackGradientSettingsMouseOver.GradientColor1 != Color.Transparent)
                    return BackGradientSettingsMouseOver.GradientType;
                else
                    return BackGradientSettings.GradientType;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetBackGradientType function of DotCoolControl control.");
                return CoolGradientType.None;
            }
        }

        /// <summary>
        /// Gets the appropriate background gradient span value of the control based on the control's current state.
        /// </summary>
        /// <returns></returns>
        protected virtual float GetBackGradientSpan()
        {
            try
            {
                if (!this.Enabled && BackGradientSettingsDisabled.GradientColor1 != Color.Transparent)
                {
                    if (BackGradientSettingsDisabled.UseDefaultGradientSpan)
                        return CoolGradient.GetDefaultGradientSpan(BackGradientSettingsDisabled.GradientType);
                    else
                        return BackGradientSettingsDisabled.GradientSpan;
                }
                if (m_blMouseDown && GetBackGradientSettingsMouseDown().GradientColor1 != Color.Transparent)
                {
                    if (GetBackGradientSettingsMouseDown().UseDefaultGradientSpan)
                        return CoolGradient.GetDefaultGradientSpan(GetBackGradientSettingsMouseDown().GradientType);
                    else
                        return GetBackGradientSettingsMouseDown().GradientSpan;
                }
                else if (m_blMouseOver && BackGradientSettingsMouseOver.GradientColor1 != Color.Transparent)
                {
                    if (BackGradientSettingsMouseOver.UseDefaultGradientSpan)
                        return CoolGradient.GetDefaultGradientSpan(BackGradientSettingsMouseOver.GradientType);
                    else
                        return BackGradientSettingsMouseOver.GradientSpan;
                }
                else
                {
                    if (BackGradientSettings.UseDefaultGradientSpan)
                        return CoolGradient.GetDefaultGradientSpan(BackGradientSettings.GradientType);
                    else
                        return BackGradientSettings.GradientSpan;
                }//end if
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetBackGradientSpan function of DotCoolControl control.");
                return 0f;
            }
        }

        /// <summary>
        /// Gets the appropriate background gradient X and Y offset position of the control based on the control's current state.
        /// </summary>
        /// <returns></returns>
        protected virtual Point GetBackGradientOffset()
        {
            try
            {
                if (!this.Enabled && BackGradientSettingsDisabled.GradientColor1 != Color.Transparent)
                    return BackGradientSettingsDisabled.GradientOffset;
                if (m_blMouseDown && GetBackGradientSettingsMouseDown().GradientColor1 != Color.Transparent)
                    return GetBackGradientSettingsMouseDown().GradientOffset;
                else if (m_blMouseOver && BackGradientSettingsMouseOver.GradientColor1 != Color.Transparent)
                    return BackGradientSettingsMouseOver.GradientOffset;
                else
                    return BackGradientSettings.GradientOffset;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetBackGradientOffset function of DotCoolControl control.");
                return new Point(0, 0);
            }
        }

        #endregion

        #region Background Color/Gradient Properties, Functions

        /// <summary>
        /// Gets the set of gradient background control settings for the control state specified in the function's setting parameter.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public virtual DotCoolCtlGradientSettings GetBackGradientSettings(VisualSettingEnum setting)
        {
            try
            {
                return m_GradBackSettingsList[setting];
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetBackGradientSettings function of DotCoolControl class.");
                return null;
            }
        }

        /// <summary>
        /// Background gradient settings of the DotCoolControl when the control is in its normal state.
        /// </summary>
        [Browsable(true), Category("CoolBackground"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
         Description("Background gradient settings of the DotCoolControl when the control is in its normal state.")]
        public virtual DotCoolCtlGradientSettings BackGradientSettings
        {
            get
            {

                return GetBackGradientSettings(VisualSettingEnum.Normal);
            }
        }

        /// <summary>
        /// Background gradient settings of the DotCoolControl when the control is in its disabled state.
        /// </summary>
        [Browsable(true), Category("CoolBackground"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Background gradient settings of the DotCoolControl when the control is in its disabled state.")]
        public virtual DotCoolCtlGradientSettings BackGradientSettingsDisabled
        {
            get
            {

                return GetBackGradientSettings(VisualSettingEnum.Disabled);
            }
        }
        
        /// <summary>
        /// Background gradient settings of the DotCoolControl when the mouse cursor is over the control.
        /// </summary>
        [Browsable(true), Category("CoolBackground"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Background gradient settings of the DotCoolControl when the mouse cursor is over the control.")]
        public virtual DotCoolCtlGradientSettings BackGradientSettingsMouseOver
        {
            get
            {

                return GetBackGradientSettings(VisualSettingEnum.MouseOver);
            }
        }

        /// <summary>
        /// Gets the background gradient settings of the DotCoolControl when the mouse button is pushed down on the control.
        /// Certain derived controls will not have a background gradient settings for the MouseDown event and these settings will not be
        /// displayed in those controls, therefore every derived class will have to implement its own BackGradientSettingsMouseDown property if it 
        /// wants to be able to modify and access the set of Mouse Down state properties for the BackGradientSettings.
        /// </summary>
        /// <returns></returns>
        protected virtual DotCoolCtlGradientSettings GetBackGradientSettingsMouseDown()
        {
            try
            {
                return GetBackGradientSettings(VisualSettingEnum.MouseDown);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetBackGradientSettingsMouseDown function of DotCoolControl class.");
                return null;
            }
        }
        
        /* NOT USED
        /// <summary>
        /// Background gradient settings of the DotCoolControl when the mouse button is pushed down on the control.
        /// </summary>
        [Browsable(true), Category("CoolBackground"), TypeConverter(typeof(ExpandableObjectConverter)),
         Description("Background gradient settings of the DotCoolControl when the mouse button is pushed down on the control.")]
        public virtual DotCoolCtlGradientSettings BackGradientSettingsMouseDown
        {
            get
            {

                return GetBackGradientSettings(VisualSettingEnum.MouseDown);
            }
        }        
        */

        #endregion

        #region General Text/Caption Properties, Functions

        /// <summary>
        /// Gets or sets the text to be displayed in the control.
        /// </summary>
        [Browsable(true), Category("CoolText"),
         Description("Gets or sets the text to be displayed in the control.")]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                this.Refresh();                                
            }
        }

        #endregion

        #region Current State Text/Caption Properties, Functions

        /// <summary>
        /// Gets the appropriate text alignment setting of the displayed text in the control based on the control's current state.
        /// </summary>
        /// <returns></returns>
        protected virtual ContentAlignment GetTextAlign()
        {
            try
            {
                if (m_blMouseDown && GetTextSettingsMouseDown().EnableText)
                    return GetTextSettingsMouseDown().TextAlign;
                else if (m_blMouseOver && TextSettingsMouseOver.EnableText)
                    return TextSettingsMouseOver.TextAlign;
                else
                    return TextSettings.TextAlign;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetTextAlign function of DotCoolControl class.");
                return ContentAlignment.TopLeft;
            }
        }

        /// <summary>
        /// Gets the appropriate X and Y text offset setting of the displayed text in the control based on the control's current state.
        /// </summary>
        /// <returns></returns>
        protected virtual Point GetTextOffset()
        {
            try
            {
                if (m_blMouseDown && GetTextSettingsMouseDown().EnableText)
                    return GetTextSettingsMouseDown().TextOffset;
                else if (m_blMouseOver && TextSettingsMouseOver.EnableText)
                    return TextSettingsMouseOver.TextOffset;
                else
                    return TextSettings.TextOffset;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetTextOffset function of DotCoolControl class.");
                return new Point(0, 0);
            }
        }

        /// <summary>
        /// Gets the appropriate font setting of the displayed text in the control based on the control's current state.
        /// </summary>
        /// <returns></returns>
        protected virtual Font GetFont()
        {
            try
            {
                if (!this.Enabled && TextSettingsDisabled.EnableText)
                    return TextSettingsDisabled.Font;
                else if (m_blMouseDown && GetTextSettingsMouseDown().EnableText)
                    return GetTextSettingsMouseDown().Font;
                else if (m_blMouseOver && TextSettingsMouseOver.EnableText)
                    return TextSettingsMouseOver.Font;
                else
                    return TextSettings.Font;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetFont function of DotCoolControl class.");
                return null;
            }
        }

        /// <summary>
        /// Gets the appropriate fore color setting of the displayed text in the control based on the control's current state.
        /// </summary>
        /// <returns></returns>
        protected virtual Color GetForeColor()
        {
            try
            {
                if (!this.Enabled && TextSettingsDisabled.EnableText)
                    return TextSettingsDisabled.ForeColor;
                else if (m_blMouseDown && GetTextSettingsMouseDown().EnableText)
                    return GetTextSettingsMouseDown().ForeColor;
                else if (m_blMouseOver && TextSettingsMouseOver.EnableText)
                    return TextSettingsMouseOver.ForeColor;
                else
                    return TextSettings.ForeColor;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetForeColor function of DotCoolControl class.");
                return Color.Black;
            }
        }

        #endregion

        #region Text/Caption Properties, Functions

        /// <summary>
        /// Gets the set of control text/caption settings for the control state specified in the function's setting parameter.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public virtual DotCoolCtlTextSettings GetTextSettings(VisualSettingEnum setting)
        {
            try
            {
                return m_TextSettingsList[setting];
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetTextSettings function of DotCoolControl class.");
                return null;
            }
        }

        /// <summary>
        /// Text/Caption display settings of the DotCoolControl when the control is in its normal state.
        /// </summary>
        [Browsable(true), Category("CoolText"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Text/Caption display settings of the DotCoolControl when the control is in its normal state.")]
        public virtual DotCoolCtlTextSettings TextSettings
        {
            get
            {

                return GetTextSettings(VisualSettingEnum.Normal);
            }
        }

        /// <summary>
        /// Text/Caption display settings of the DotCoolControl when the control is in its disabled state.
        /// </summary>
        [Browsable(true), Category("CoolText"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Text/Caption display settings of the DotCoolControl when the control is in its disabled state.")]
        public DotCoolCtlTextSettings TextSettingsDisabled
        {
            get
            {

                return GetTextSettings(VisualSettingEnum.Disabled);
            }
        }        

        /// <summary>
        /// Text/Caption display settings of the DotCoolControl when the mouse cursor is over the control.
        /// </summary>
        [Browsable(true), Category("CoolText"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Text/Caption display settings of the DotCoolControl when the mouse cursor is over the control.")]
        public virtual DotCoolCtlTextSettings TextSettingsMouseOver
        {
            get
            {

                return GetTextSettings(VisualSettingEnum.MouseOver);
            }
        }

        /// <summary>
        /// Gets the Text/Caption display settings of the DotCoolControl when the mouse button is pushed down on the control.
        /// Certain derived controls will not have a background gradient settings for the MouseDown event and these settings will not be
        /// displayed in those controls, therefore every derived class will have to implement its own TextSettingsMouseDown property if it 
        /// wants to be able to modify and access the set of Mouse Down state properties for the TextSettings.
        /// </summary>
        /// <returns></returns>
        protected virtual DotCoolCtlTextSettings GetTextSettingsMouseDown()
        {
            try
            {
                return GetTextSettings(VisualSettingEnum.MouseDown);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetTextSettingsMouseDown function of DotCoolControl class.");
                return null;
            }
        }

        /* NOT USED
        /// <summary>
        /// Text/Caption display settings of the DotCoolControl when the mouse button is pushed down on the control.
        /// </summary>
        [Browsable(true), Category("CoolText"), TypeConverter(typeof(ExpandableObjectConverter)),
         Description("Text/Caption display settings of the DotCoolControl when the mouse button is pushed down on the control.")]
        public virtual DotCoolCtlTextSettings TextSettingsMouseDown
        {
            get
            {

                return GetTextSettings(VisualSettingEnum.MouseDown);
            }
        }
        */

        #endregion                    

        #region Current State Image Properties, Functions

        /// <summary>
        /// Gets the appropriate image that will be displayed in the control based on the control's current state.
        /// </summary>
        /// <returns></returns>
        /// <param name="delStateCheckFunc">Delegate to a function containing additional logic to add to the function.  The function will be usually 
        /// passed from children classes to the base DotCoolControl class to embed additional criteria to a visual setting state checking function.</param>
        protected virtual Image GetImage(Func<Image> delStateCheckFunc)
        {
            try
            {
                if (!this.Enabled && ImageSettingsDisabled.EnableImage && ImageSettingsDisabled.Image != null)
                    return ImageSettingsDisabled.Image;
                else if (m_blMouseDown && GetImageSettingsMouseDown().EnableImage && GetImageSettingsMouseDown().Image != null)
                    return GetImageSettingsMouseDown().Image;
                else if (m_blMouseOver && ImageSettingsMouseOver.EnableImage && ImageSettingsMouseOver.Image != null)
                    return ImageSettingsMouseOver.Image;
                else if(delStateCheckFunc != null)
                {
                    Image imgResult = delStateCheckFunc.Invoke();

                    if (imgResult != null)
                        return imgResult;
                    else
                        return ImageSettings.Image;
                }
                else
                    return ImageSettings.Image;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetImage function of DotCoolControl control.");
                return null;
            }
        }

        /// <summary>
        /// Gets the appropriate image that will be displayed in the control based on the control's current state.
        /// </summary>
        /// <returns></returns>        
        protected virtual Image GetImage()
        {
            return GetImage(null);
        }

        /// <summary>
        /// Gets the appropriate image alignment of the image displayed in the control based on the control's current state.
        /// </summary>
        /// <returns></returns>
        /// <param name="delStateCheckFunc">Delegate to a function containing additional logic to add to the function.  The function will be usually 
        /// passed from children classes to the base DotCoolControl class to embed additional criteria to a visual setting state checking function.</param>
        protected virtual ContentAlignment GetImageAlign(Func<ContentAlignment?> delStateCheckFunc)
        {
            try
            {
                if (m_blMouseDown && GetImageSettingsMouseDown().EnableImage && GetImageSettingsMouseDown().Image != null)
                    return GetImageSettingsMouseDown().ImageAlign;
                else if (m_blMouseOver && ImageSettingsMouseOver.EnableImage && ImageSettingsMouseOver.Image != null)
                    return ImageSettingsMouseOver.ImageAlign;
                else if (delStateCheckFunc != null)
                {
                    ContentAlignment? alignResult = delStateCheckFunc.Invoke();

                    if (alignResult != null)
                        return alignResult.Value;
                    else
                        return ImageSettings.ImageAlign;
                }
                else
                    return ImageSettings.ImageAlign;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetImageAlign function of DotCoolControl control.");
                return ContentAlignment.TopLeft;
            }
        }

        /// <summary>
        /// Gets the appropriate image alignment of the image displayed in the control based on the control's current state.
        /// </summary>
        /// <returns></returns>        
        protected virtual ContentAlignment GetImageAlign()
        {
            return GetImageAlign(null);
        }

        /// <summary>
        /// Gets the appropriate X and Y offset positions of the image displayed in the control based on the control's current state.
        /// </summary>
        /// <returns></returns>
        /// <param name="delStateCheckFunc">Delegate to a function containing additional logic to add to the function.  The function will be usually 
        /// passed from children classes to the base DotCoolControl class to embed additional criteria to a visual setting state checking function.</param>
        protected virtual Point GetImageOffset(Func<Point?> delStateCheckFunc)
        {
            try
            {
                if (m_blMouseDown && GetImageSettingsMouseDown().EnableImage && GetImageSettingsMouseDown().Image != null)
                    return GetImageSettingsMouseDown().ImageOffset;
                else if (m_blMouseOver && ImageSettingsMouseOver.EnableImage && ImageSettingsMouseOver.Image != null)
                    return ImageSettingsMouseOver.ImageOffset;
                else if (delStateCheckFunc != null)
                {
                    Point? ptResult = delStateCheckFunc.Invoke();

                    if (ptResult != null)
                        return ptResult.Value;
                    else
                        return ImageSettings.ImageOffset;
                }
                else
                    return ImageSettings.ImageOffset;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetImageOffset function of DotCoolControl control.");
                return new Point(0, 0);
            }
        }

        /// <summary>
        /// Gets the appropriate X and Y offset positions of the image displayed in the control based on the control's current state.
        /// </summary>
        /// <returns></returns>
        protected virtual Point GetImageOffset()
        {
            return GetImageOffset(null);
        }

        /// <summary>
        /// Gets the appropriate transparent color of the image displayed in the control based on the control's current state.
        /// </summary>
        /// <returns></returns>
        /// <param name="delStateCheckFunc">Delegate to a function containing additional logic to add to the function.  The function will be usually 
        /// passed from children classes to the base DotCoolControl class to embed additional criteria to a visual setting state checking function.</param>
        protected virtual Color GetImageTransColor(Func<Color?> delStateCheckFunc)
        {
            try
            {
                if (!this.Enabled && ImageSettingsDisabled.EnableImage && ImageSettingsDisabled.ImageTransColor != null)
                    return ImageSettingsDisabled.ImageTransColor;
                else if (m_blMouseDown && GetImageSettingsMouseDown().EnableImage && GetImageSettingsMouseDown().Image != null)
                    return GetImageSettingsMouseDown().ImageTransColor;
                else if (m_blMouseOver && ImageSettingsMouseOver.EnableImage && ImageSettingsMouseOver.Image != null)
                    return ImageSettingsMouseOver.ImageTransColor;
                else if (delStateCheckFunc != null)
                {
                    Color? colorResult = delStateCheckFunc.Invoke();

                    if (colorResult != null)
                        return colorResult.Value;
                    else
                        return ImageSettings.ImageTransColor;
                }
                else
                    return ImageSettings.ImageTransColor;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetImageTransColor function of DotCoolControl control.");
                return Color.Transparent;
            }
        }

        /// <summary>
        /// Gets the appropriate transparent color of the image displayed in the control based on the control's current state.
        /// </summary>
        /// <returns></returns>
        protected virtual Color GetImageTransColor()
        {
            return GetImageTransColor(null);
        }

        #endregion

        #region Image Properties, Functions

        /// <summary>
        /// Gets the set of image display settings for the control state specified in the function's setting parameter.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public virtual DotCoolCtlImageSettings GetImageSettings(VisualSettingEnum setting)
        {
            try
            {
                return m_ImageSettingsList[setting];
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetImageSettings function of DotCoolControl class.");
                return null;
            }
        }

        /// <summary>
        /// Image display settings of the DotCoolControl when the control is in its normal state.
        /// </summary>
        [Browsable(true), Category("CoolImage"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Image display settings of the DotCoolControl when the control is in its normal state.")]
        public virtual DotCoolCtlImageSettings ImageSettings
        {
            get
            {

                return GetImageSettings(VisualSettingEnum.Normal);
            }
        }

        /// <summary>
        /// Image display settings of the DotCoolControl when the control is in its disabled state.
        /// </summary>
        [Browsable(true), Category("CoolImage"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Image display settings of the DotCoolControl when the control is in its disabled state.")]
        public virtual DotCoolCtlImageSettings ImageSettingsDisabled
        {
            get
            {

                return GetImageSettings(VisualSettingEnum.Disabled);
            }
        }
        
        /// <summary>
        /// Image display settings of the DotCoolControl when the mouse cursor is over the control.
        /// </summary>
        [Browsable(true), Category("CoolImage"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Image display settings of the DotCoolControl when the mouse cursor is over the control.")]
        public virtual DotCoolCtlImageSettings ImageSettingsMouseOver
        {
            get
            {

                return GetImageSettings(VisualSettingEnum.MouseOver);
            }
        }

        /// <summary>
        /// Gets the image display settings of the DotCoolControl when the mouse button is pushed down on the control.
        /// Certain derived controls will not have image settings for the MouseDown event and these settings will not be
        /// displayed in those controls. Therefore, every derived class will have to implement its own ImageSettingsMouseDown property if it 
        /// wants to be able to modify and access the set of Mouse Down state properties for the ImageSettings.
        /// </summary>
        /// <returns></returns>
        protected virtual DotCoolCtlImageSettings GetImageSettingsMouseDown()
        {
            try
            {
                return GetImageSettings(VisualSettingEnum.MouseDown);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetImageSettingsMouseDown function of DotCoolControl class.");
                return null;
            }
        }

        /* NOT USED
        /// <summary>
        /// Image display settings of the DotCoolControl when the mouse button is pushed down on the control.
        /// </summary>
        [Browsable(true), Category("CoolImage"), TypeConverter(typeof(ExpandableObjectConverter)),
         Description("Image display settings of the DotCoolControl when the mouse button is pushed down on the control.")]
        public virtual DotCoolCtlImageSettings ImageSettingsMouseDown
        {
            get
            {

                return GetImageSettings(VisualSettingEnum.MouseDown);
            }
        }
        */

        #endregion

        #region Control Focus/Activation Properties, Functions

        /// <summary>
        /// Indicates if the button control will receive the focus after it is clicked by the user.
        /// </summary>
        [Browsable(true), Category("Behavior"), DefaultValue(true),
         Description("Indicates if the control will receive the focus after it is clicked by the user.")]
        public virtual bool FocusOnClick
        {
            get
            {
                return m_blFocusOnClick;
            }
            set
            {
                m_blFocusOnClick = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Simulates a user clicking on the control.
        /// </summary>
        public virtual void PerformClick()
        {
            try
            {
                m_blMouseDown = true;
                OnClick(new EventArgs());
                m_blMouseDown = false;
                this.Refresh();
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in PerformClick function of DotCoolControl class.");
            }
        }

        /// <summary>
        /// Gets a value that specifies where in the control the focus rectangle will be drawn when the control has focus.
        /// </summary>
        /// <returns></returns>
        protected FocusRectEnum GetDrawFocusRect()
        {
            try
            {
                return m_FocusRect;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetDrawFocusRect function of DotCoolControl class.");
                return FocusRectEnum.Text;
            }
        }

        /// <summary>
        /// Sets a value that specifies where in the control the focus rectangle will be drawn when the control has focus.  Inherited controls will use 
        /// this value to either modify the draw focus rectangle settings internally or to allow the user to directly modify the settings through properties.
        /// </summary>
        /// <returns></returns>
        protected void SetDrawFocusRect(FocusRectEnum drawFocusRect)
        {
            try
            {
                m_FocusRect = drawFocusRect;
                this.Refresh();
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in SetDrawFocusRect function of DotCoolControl class.");
            }
        }

        /// <summary>
        /// Gets a value that indicates if the check control is selected by clicking specifically within the border boundaries of the control or any part of
        /// the control.
        /// </summary>        
        /// <returns></returns>
        protected ControlSelectBehavior GetSelectBehavior()
        {
            try
            {
                return m_CtlSelBehavior;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetSelectBehavior function of DotCoolControl class.");
                return ControlSelectBehavior.Border;
            }
        }

        /// <summary>
        /// Sets a value that indicates if the control is selected by clicking specifically within the border boundaries of the control or any part of
        /// the control.  Inherited controls will use this value to either modify the control selection behavior settings internally or to allow the user
        /// to directly modify the settings through properties.
        /// </summary>
        /// <param name="ctlSelBehavior"></param>
        /// <returns></returns>
        protected void SetSelectBehavior(ControlSelectBehavior ctlSelBehavior)
        {
            try
            {
                m_CtlSelBehavior = ctlSelBehavior;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in SetSelectBehavior function of DotCoolControl class.");
            }
        }

        #endregion

        #region Hot Key Properties, Functions, Event Handlers

        /// <summary>
        /// Handles all key events that are registered at the form level and need to be processed by the DotCoolControl.  A key event hook of the form 
        /// containing the DotCoolControl will be used for various operations like handling hot keys and accelerators and performing the appropriate 
        /// actions in response to them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyHook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            try
            {                
                if (m_blKeyHookActive || !CanSelect)
                    return;

                IntPtr hForegroundWnd = WinAPI.GetForegroundWindow();

                if (hForegroundWnd == IntPtr.Zero)
                {
                    // No window is currently activated
                    return;      
                }

                int iCurProcId = Process.GetCurrentProcess().Id;
                uint iActiveProcId;
                WinAPI.GetWindowThreadProcessId(hForegroundWnd, out iActiveProcId);

                if (iActiveProcId != iCurProcId)
                    return;


                m_blKeyHookActive = true;
                
                if (e.Modifier == m_HotKeyMod && e.Key == m_HotKeyVal)
                {
                    PerformClick();
                }//end if

                m_blKeyHookActive = false;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in KeyHook_KeyPressed function of DotCoolControl class.", "", true);
                m_blKeyHookActive = false;
            }
        }       

        /// <summary>
        /// The key value that will be used as the hot key of the control.
        /// </summary>
        [Browsable(true), Category("Behavior"), DefaultValue(Keys.None),
         Description("The key value that will be used as the hot key of the control.")]
        public virtual Keys HotKey
        {
            get
            {
                return m_HotKey;
            }
            set
            {
                m_HotKey = value;

                m_HotKeyVal = m_HotKey;
                m_HotKeyMod = 0;

                if ((m_HotKeyVal & Keys.Alt) == Keys.Alt)
                {
                    m_HotKeyVal ^= Keys.Alt;
                    m_HotKeyMod |= Tiferix.Global.ModifierKeys.Alt;
                }//end if

                if ((m_HotKeyVal & Keys.Control) == Keys.Control)
                {
                    m_HotKeyVal ^= Keys.Control;
                    m_HotKeyMod |= Tiferix.Global.ModifierKeys.Control;
                }//end if

                if ((m_HotKeyVal & Keys.Shift) == Keys.Shift)
                {
                    m_HotKeyVal ^= Keys.Shift;
                    m_HotKeyMod |= Tiferix.Global.ModifierKeys.Shift;
                }//end if
            }
        }

        /// <summary>
        /// Registers a new hot key using the current hot key settings in the control.  Any previous hot keys will be disabled and deleted from memory.
        /// </summary>
        public virtual void UpdateHotKey()
        {
            try
            {
                if(m_iHotKeyID > 0)
                    m_KeyHook.UnregisterHotKey(m_iHotKeyID);
                
                if (m_HotKeyVal != Keys.None)
                    m_iHotKeyID = m_KeyHook.RegisterHotKey(m_HotKeyMod, m_HotKeyVal);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in UpdateHotKey function of DotCoolControl class.");
            }
        }

        #endregion

        #region Control Cloning and Serialization Properties, Functions

        /// <summary>
        /// Copies all of the properties and settings that are specific to the base DotCoolControl class from one derived DotCoolControl control to another 
        /// derived DotCoolControl control.
        /// </summary>
        /// <param name="ctlClone">The DotCoolControl dervied control that will be cloned from the current DotCoolControl.</param>
        protected virtual void CloneBase(DotCoolControl ctlClone)
        {
            try
            {                
                //Border Shape
                ctlClone.BorderShape = BorderShape;

                //Border Color Settings
                ctlClone.BorderColorSettings.BorderColor = BorderColorSettings.BorderColor;
                ctlClone.BorderColorSettings.BorderColorDisabled = BorderColorSettings.BorderColorDisabled;
                ctlClone.BorderColorSettings.BorderColorMouseDown = BorderColorSettings.BorderColorMouseDown;
                ctlClone.BorderColorSettings.BorderColorMouseOver = BorderColorSettings.BorderColorMouseOver;

                //Border Size and Position
                ctlClone.SetBorderSize(GetBorderSize());
                ctlClone.BorderRadius = BorderRadius;
                ctlClone.SetBorderOffset(GetBorderOffset());

                //Border Width
                ctlClone.BorderWidthSettings.BorderWidth = BorderWidthSettings.BorderWidth;
                ctlClone.BorderWidthSettings.BorderWidthMouseDown = BorderWidthSettings.BorderWidthMouseDown;

                //Background Gradient Settings
                for (VisualSettingEnum setting = VisualSettingEnum.Normal; setting <= VisualSettingEnum.Indeterminate; setting++)
                {
                    DotCoolCtlGradientSettings gradSettingsClone = ctlClone.GetBackGradientSettings(setting);
                    DotCoolCtlGradientSettings gradSettings = GetBackGradientSettings(setting);

                    gradSettingsClone.GradientColor1 = gradSettings.GradientColor1;
                    gradSettingsClone.GradientColor2 = gradSettings.GradientColor2;
                    gradSettingsClone.GradientOffset = gradSettings.GradientOffset;
                    gradSettingsClone.GradientSpan = gradSettings.GradientSpan;
                    gradSettingsClone.GradientType = gradSettings.GradientType;
                    gradSettingsClone.UseDefaultGradientSpan = gradSettings.UseDefaultGradientSpan;
                }//next setting

                //Text/Caption Settings
                ctlClone.Text = this.Text;

                for (VisualSettingEnum setting = VisualSettingEnum.Normal; setting <= VisualSettingEnum.Indeterminate; setting++)
                {
                    DotCoolCtlTextSettings txtSettingsClone = ctlClone.GetTextSettings(setting);
                    DotCoolCtlTextSettings txtSettings = GetTextSettings(setting);

                    txtSettingsClone.EnableText = txtSettings.EnableText;
                    txtSettingsClone.Font = txtSettings.Font;
                    txtSettingsClone.ForeColor = txtSettings.ForeColor;
                    txtSettingsClone.TextAlign = txtSettings.TextAlign;
                    txtSettingsClone.TextOffset = txtSettings.TextOffset;                    
                }//next setting

                //Image Settings
                for (VisualSettingEnum setting = VisualSettingEnum.Normal; setting <= VisualSettingEnum.Indeterminate; setting++)
                {
                    DotCoolCtlImageSettings imgSettingsClone = ctlClone.GetImageSettings(setting);
                    DotCoolCtlImageSettings imgSettings = GetImageSettings(setting);

                    imgSettingsClone.EnableImage = imgSettings.EnableImage;

                    if(imgSettings.Image != null)
                        imgSettingsClone.Image = new Bitmap(imgSettings.Image);

                    imgSettingsClone.ImageAlign = imgSettings.ImageAlign;
                    imgSettingsClone.ImageOffset = imgSettings.ImageOffset;
                    imgSettingsClone.ImageTransColor = imgSettings.ImageTransColor;                    
                }//next setting

                ctlClone.Enabled = true;                          
                ctlClone.Bounds = this.Bounds;                                               
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in CloneBase function of DotCoolControl class.");                
            }
        }

        #endregion

        #region DotCoolControl Specific Control Settings Classes

        /// <summary>
        /// The class will be used to view and modify the state-specific border width settings of the DotCoolControl from the control's property grid
        /// in the designer as well as at run-time.
        /// </summary>
        public class DotCoolCtlBorderWidthSettings
        {
            #region Member Variables
            #endregion

            #region Member Object Variables

            protected DotCoolCtlBorderSettingsList m_BorderSettingsList = null;

            #endregion

            #region Construction/Initialization

            /// <summary>
            /// Constructor
            /// </summary>
            public DotCoolCtlBorderWidthSettings(DotCoolCtlBorderSettingsList BorderSettingsList)
            {
                try
                {
                    m_BorderSettingsList = BorderSettingsList;
                }
                catch (Exception err)
                {
                    ErrorHandler.ShowErrorMessage(err, "Error in Constructor function of DotCoolCtlBorderWidthSettings class.");
                }
            }

            #endregion

            #region General Overriden Properties, Functions

            /// <summary>
            /// Overrides the ToString function which will allow an appropriate caption to be displayed in the property grid for the specific type of control 
            /// visual setting.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                try
                {
                    return "Border Width Settings";
                }
                catch (Exception err)
                {
                    ErrorHandler.ShowErrorMessage(err, "Error in ToString function of DotCoolCtlBorderWidthSettings class.");
                    return "Error";
                }
            }

            #endregion

            #region Border Width (State-Specific) Properties, Functions

            /// <summary>
            /// Gets or sets the width (thickness) of the control's border.
            /// </summary>
            [Browsable(true), Category("CoolBorder"), DefaultValue(1),
             Description("Gets or sets the width (thickness) of the control's border.")]
            public virtual int BorderWidth
            {
                get
                {
                    return m_BorderSettingsList[VisualSettingEnum.Normal].BorderWidth;
                }
                set
                {
                    m_BorderSettingsList[VisualSettingEnum.Normal].BorderWidth = value;
                }
            }

            /// <summary>
            /// Gets or sets the width (thickness) of the control's border when the mouse button is pushed down on the control.
            /// </summary>
            [Browsable(true), Category("CoolBorder"), DefaultValue(2),
             Description("Gets or sets the width (thickness) of the control's border when the mouse button is pushed down on the control.")]
            public virtual int BorderWidthMouseDown
            {
                get
                {
                    return m_BorderSettingsList[VisualSettingEnum.MouseDown].BorderWidth;
                }
                set
                {
                    m_BorderSettingsList[VisualSettingEnum.MouseDown].BorderWidth = value;
                }
            }

            #endregion
        }

        #endregion
    }
}

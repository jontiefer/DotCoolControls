/* Copyright © 2016 Jonathan Tiefer - All Rights Reserved
 * You may use, distribute and modify this code under the
 * terms of the GNU Lesser General Public License (LGPL)
 *
 * You should have received a copy of the LGPL license with
 * this file.
 *
 * /

/*  This file is part of DotCoolPanels
*
*   DotCoolPanels is free software: you can redistribute it and/or modify
*   it under the terms of the GNU Lesser General Public License as published by
*   the Free Software Foundation, either version 3 of the License, or
*    (at your option) any later version.
*
*   DotCoolPanels is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*   GNU Lesser General Public License for more details.
*
*  You should have received a copy of the GNU Lesser General Public License
*   along with DotCoolPanels.  If not, see <http://www.gnu.org/licenses/>.
*/

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
using Tiferix.Global;
using DotCoolControls.Tools;
using DotCoolControls.VisualSettings;

namespace DotCoolControls.WinForms
{
    /// <summary>
    /// An expansion of the native .NET panel control that displays stunning gradient effects.  Both the effects and functionality of the panel can be made to 
    /// be state-specific, such as producing various effects for when the panel is in a Normal or Disabled state, as well various types of mouse 
    /// interactions, such as MouseDown and MouseOver events.  The DotCoolPanel can also be used to display gradient backgrounds for 
    /// DotCoolControls contained within the panel.
    /// </summary>
    public class DotCoolPanel : Panel 
    {
        #region Member Variables

        protected CancelEvents m_CancelEvents = new CancelEvents();

        #endregion

        #region Member Object Variables
        #endregion

        #region Gradient Variables

        /// <summary>
        /// Contains the various gradient settings for the background of the DotCoolPanel class.
        /// </summary>
        protected DotCoolCtlGradientSettingsList m_GradBackSettingsList = null;

        #endregion

        #region Mouse Position Variables

        protected bool m_blMouseOver = false;

        protected bool m_blMouseDown = false;

        #endregion

        #region Construction/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public DotCoolPanel()
            : base()
        {
            try
            {
                this.SetStyle(ControlStyles.DoubleBuffer, true);
                this.SetStyle(ControlStyles.ResizeRedraw, true);
                this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                this.SetStyle(ControlStyles.UserPaint, true);
                this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
                this.SetStyle(ControlStyles.ContainerControl, true);                

                this.DoubleBuffered = true;

                m_GradBackSettingsList = new DotCoolCtlGradientSettingsList(this);                
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Constructor function of DotCoolPanel class.");
            }
        }

        /// <summary>
        /// Gets the required creation parameters when the control handle is created.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {                
                CreateParams cp = base.CreateParams;

                if(!DesignMode)
                    cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED

                return cp;
            }
        }

        #endregion

        #region General Overriden/Shadowed Panel Properties, Functions
        
        /* NOT USED
        /// <summary>                
        /// Indicates the border style for the control.                        
        /// </summary>        
        [DefaultValue(BorderStyle.None)]
        [Browsable(true), Category("Appearance"),
         Description("Indicates the border style for the control.")]       
        public new BorderStyle BorderStyle 
        {
            get
            {
                return base.BorderStyle;            
            }
            set
            {
                base.BorderStyle = value;
                this.Refresh();
            }
        }
        */

        #endregion

        #region General Control Drawing/Paint/GDI+ Functions, Event Handlers

        /// <summary>
        /// Handles all custom GDI/drawing/painting operations of the DotCoolPanel, including the drawing of the control's gradients.
        /// </summary>
        /// <param name="pevent"></param>
        protected override void OnPaint(PaintEventArgs pevent)
        {
            IntPtr hDC = IntPtr.Zero;

            try
            {                
                if (this.BackgroundImage != null)
                {
                    base.OnPaint(pevent);
                    return;
                }//end if                

                hDC = pevent.Graphics.GetHdc();
                Graphics gCtlMem = Graphics.FromHdc(hDC);
                pevent.Graphics.ReleaseHdc();
                hDC = IntPtr.Zero;

                DrawBackGradientImage(gCtlMem);

                Bitmap bmpCtlMem = new Bitmap(this.Width, this.Height);
                gCtlMem.DrawImage(bmpCtlMem, 0, 0);
                gCtlMem.Dispose();

                pevent.Graphics.DrawImage(bmpCtlMem, 0, 0);

                base.OnPaint(pevent);                
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnPaint function of DotCoolPanel control.");
            }
            finally
            {
                if (hDC != IntPtr.Zero)
                    pevent.Graphics.ReleaseHdc();
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

                bmpGradBackMem = new Bitmap(this.Width, this.Height);
                gGradBackMem = Graphics.FromImage(bmpGradBackMem);

                CoolGradientType BackGradTypeActive = GetBackGradientType();
                Color BackGradColor1Active = GetBackGradientColor(1);
                Color BackGradColor2Active = GetBackGradientColor(2);
                float fGradientSpan = GetBackGradientSpan();
                Point ptGradOffset = GetBackGradientOffset();

                Rectangle rectBounds = new Rectangle(0, 0, this.Width, this.Height);

                if (BackGradColor1Active != Color.Transparent && BackGradTypeActive != CoolGradientType.None)
                    CoolDraw.DrawGradientShape(CoolShape.Rectangle, BackGradTypeActive, gGradBackMem, BackGradColor1Active,
                                                                BackGradColor2Active, rectBounds, Color.Transparent, 0, fGradientSpan, ptGradOffset.X, ptGradOffset.Y);
                else
                {
                    Color? FillColor = null;
                    if (BackGradColor1Active != Color.Transparent)
                        FillColor = BackGradColor1Active;

                    CoolDraw.DrawShape(CoolShape.Rectangle, gGradBackMem, rectBounds, Color.Transparent, 0,
                                                    0, 0, FillColor);
                }//end if

                Rectangle rectDraw = new Rectangle(0, 0, this.Width, this.Height);
                //gCtl.CompositingMode = CompositingMode.SourceCopy;
                gCtl.DrawImage(bmpGradBackMem, rectDraw);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawBackGradientImage function of DotCoolPanel control.");
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

        #region Panel Enabling/Visibility/Focus Properties, Functions

        /// <summary>
        /// Gets or sets a value indicating whether the control can respond to user interaction.  
        /// </summary>
        [Browsable(true), Category("Behavior"),
          Description("Gets or sets a value indicating whether the control can respond to user interaction.")]
        public new bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
                this.Refresh();
            }
        }

        #endregion

        #region Control Mouse Interaction Functions, Event Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            try
            {                
                if (!m_blMouseOver)
                {
                    base.OnMouseEnter(new EventArgs());

                    m_blMouseOver = true;

                    if (BackGradientSettingsMouseOver.GradientColor1 != Color.Transparent)
                        this.Refresh();
                }//end if                
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnMouseEnter function of DotCoolPanel class.", "", true);
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
                if (m_blMouseOver)
                {
                    base.OnMouseLeave(new EventArgs());

                    m_blMouseOver = false;

                    if (BackGradientSettingsMouseOver.GradientColor1 != Color.Transparent)
                        this.Refresh();
                }//end if                
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnMouseLeave function of DotCoolPanel class.", "", true);
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
                base.OnMouseMove(e);
                
                if (!m_blMouseOver)
                {
                    m_blMouseOver = true;

                    if (BackGradientSettingsMouseOver.GradientColor1 != Color.Transparent)                    
                        this.Refresh();
                }//end if                
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnMouseMove function of DotCoolPanel class.", "", true);
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
                base.OnMouseDown(e);
                
                if (!m_blMouseDown)
                {
                    m_blMouseDown = true;

                    if (BackGradientSettingsMouseDown.GradientColor1 != Color.Transparent)                    
                        this.Refresh();
                }//end if                                                
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnMouseDown function of DotCoolPanel class.", "", true);
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
                ErrorHandler.ShowErrorMessage(err, "Error in OnMouseUp function of DotCoolPanel class.", "", true);
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
                base.OnDoubleClick(e);
            }//end if
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
                else if (m_blMouseDown && BackGradientSettingsMouseDown.GradientColor1 != Color.Transparent)
                {
                    if (iColorIndex == 1)
                        return BackGradientSettingsMouseDown.GradientColor1;
                    else
                        return BackGradientSettingsMouseDown.GradientColor2;
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
                ErrorHandler.ShowErrorMessage(err, "Error in GetBackGradientColor function of DotCoolPanel control.");
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
                else if (m_blMouseDown && BackGradientSettingsMouseDown.GradientColor1 != Color.Transparent)
                    return BackGradientSettingsMouseDown.GradientType;
                else if (m_blMouseOver && BackGradientSettingsMouseOver.GradientColor1 != Color.Transparent)
                    return BackGradientSettingsMouseOver.GradientType;
                else
                    return BackGradientSettings.GradientType;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetBackGradientType function of DotCoolPanel control.");
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
                if (m_blMouseDown && BackGradientSettingsMouseDown.GradientColor1 != Color.Transparent)
                {
                    if (BackGradientSettingsMouseDown.UseDefaultGradientSpan)
                        return CoolGradient.GetDefaultGradientSpan(BackGradientSettingsMouseDown.GradientType);
                    else
                        return BackGradientSettingsMouseDown.GradientSpan;
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
                ErrorHandler.ShowErrorMessage(err, "Error in GetBackGradientSpan function of DotCoolPanel control.");
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
                if (m_blMouseDown && BackGradientSettingsMouseDown.GradientColor1 != Color.Transparent)
                    return BackGradientSettingsMouseDown.GradientOffset;
                else if (m_blMouseOver && BackGradientSettingsMouseOver.GradientColor1 != Color.Transparent)
                    return BackGradientSettingsMouseOver.GradientOffset;
                else
                    return BackGradientSettings.GradientOffset;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetBackGradientOffset function of DotCoolPanel control.");
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
                ErrorHandler.ShowErrorMessage(err, "Error in GetBackGradientSettings function of DotCoolPanel class.");
                return null;
            }
        }

        /// <summary>
        /// Background gradient settings of the DotCoolPanel when the control is in its normal state.
        /// </summary>
        [Browsable(true), Category("CoolBackground"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Background gradient settings of the DotCoolPanel when the control is in its normal state.")]
        public virtual DotCoolCtlGradientSettings BackGradientSettings
        {
            get
            {

                return GetBackGradientSettings(VisualSettingEnum.Normal);
            }
        }

        /// <summary>
        /// Background gradient settings of the DotCoolPanel when the control is in its disabled state.
        /// </summary>
        [Browsable(true), Category("CoolBackground"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Background gradient settings of the DotCoolPanel when the control is in its disabled state.")]
        public virtual DotCoolCtlGradientSettings BackGradientSettingsDisabled
        {
            get
            {

                return GetBackGradientSettings(VisualSettingEnum.Disabled);
            }
        }

        /// <summary>
        /// Background gradient settings of the DotCoolPanel when the mouse cursor is over the control.
        /// </summary>
        [Browsable(true), Category("CoolBackground"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Background gradient settings of the DotCoolPanel when the mouse cursor is over the control.")]
        public virtual DotCoolCtlGradientSettings BackGradientSettingsMouseOver
        {
            get
            {

                return GetBackGradientSettings(VisualSettingEnum.MouseOver);
            }
        }       

        /// <summary>
        /// Background gradient settings of the DotCoolPanel when the mouse button is pushed down on the control.
        /// </summary>
        [Browsable(true), Category("CoolBackground"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Background gradient settings of the DotCoolPanel when the mouse button is pushed down on the control.")]
        public virtual DotCoolCtlGradientSettings BackGradientSettingsMouseDown
        {
            get
            {

                return GetBackGradientSettings(VisualSettingEnum.MouseDown);
            }
        }

        #endregion
    }
}

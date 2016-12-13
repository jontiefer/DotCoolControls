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

namespace DotCoolControls
{
    /// <summary>
    /// DotCoolCheckBase class supplys a prototype to all controls with checking type of capabilities like checkboxes and radio buttons.
    /// </summary>
    public abstract class DotCoolCheckBase : DotCoolControl
    {
        #region Enumerations

        #endregion

        #region Events

        /// <summary>
        /// The event that is raised when the check property of the check control is changed.
        /// </summary>
        public event EventHandler CheckChanged;

        #endregion

        #region DotCoolControl Box/Border Specific Variables

        protected RectangleF m_BoxBounds = RectangleF.Empty;

        #endregion

        #region Check Setting/Value Variables

        protected CheckState m_CheckState = CheckState.Unchecked;

        protected bool m_blThreeState = false;

        protected bool m_blSelectOnFocus = false;

        #endregion

        #region Gradient Check Symbol Variables

        protected DotCoolCtlGradientSettingsList m_GradCheckSettingsList = null;

        #endregion        

        #region Check Symbol Border Variables

        protected DotCoolCtlBorderSettingsList m_CheckBorderSettingsList = null;

        protected DotCoolCheckBaseBorderColorSettings m_CheckBorderColorSettings = null;

        #endregion        

        #region Construction/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public DotCoolCheckBase()
            : base()
        {
            try
            {
                m_GradCheckSettingsList = new DotCoolCtlGradientSettingsList(this);

                //m_CheckBorderSettingsList = new DotCoolCtlBorderSettingsList(this);
                //m_BorderColorSettings = new DotCoolControls.DotCoolCheckBaseBorderColorSettings(m_CheckBorderSettingsList);
                
                for (VisualSettingEnum setting = VisualSettingEnum.Normal; setting <= VisualSettingEnum.Indeterminate; setting++)
                {
                    m_BorderSettingsList[setting].BorderSize = new Size(14, 14);
                    m_BorderSettingsList[setting].BorderOffset = new Point(1, 1);

                    m_ImageSettingsList[setting].ImageAlign = ContentAlignment.TopLeft;

                    m_TextSettingsList[setting].TextAlign = ContentAlignment.TopLeft;
                    m_TextSettingsList[setting].TextOffset = new Point(16, 1);
                    
                    if (setting != VisualSettingEnum.Normal)
                    {
                        m_GradCheckSettingsList[setting].GradientColor1 = Color.Transparent;
                    }//end if                                        
                }//next i          

                m_GradBackSettingsList[VisualSettingEnum.Normal].GradientColor1 = Color.White;
                m_GradCheckSettingsList[VisualSettingEnum.Normal].GradientColor1 = Color.Black;

                m_BoxBounds = new RectangleF(1, 1, 14, 14);
                SelectBehavior = ControlSelectBehavior.Control;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Constructor function of DotCoolCheckBase control.");
            }
        }

        #endregion

        #region Overriden Window Procedure/Subclassing Functions, Event Handlers

        #endregion

        #region General Control Drawing/Paint/GDI+ Functions, Event Handlers

        /// <summary>
        /// Handles all custom GDI/drawing/painting operations of the DotCoolCheckBase control that extend past the custom painting that is performed 
        /// in the base DotCoolControl class.  The paint event of the DotCoolCheckBase class will including the drawing of the control's gradient check symbol.
        /// </summary>
        /// <param name="pevent"></param>
        protected override void OnPaint(PaintEventArgs pevent)
        {
            IntPtr hDC = IntPtr.Zero;

            try
            {
                base.OnPaint(pevent);

                GraphicsPath pathBox = new GraphicsPath();
                pathBox.AddPolygon(m_aryBorderPoints);
                pathBox.CloseFigure();
                m_BoxBounds = pathBox.GetBounds();
                pathBox.Dispose();

                hDC = pevent.Graphics.GetHdc();
                Graphics gCtlMem = Graphics.FromHdc(hDC);
                pevent.Graphics.ReleaseHdc();
                hDC = IntPtr.Zero;

                if (m_CheckState != CheckState.Unchecked)
                {
                    if (GetImage() == null)
                    {
                        DrawCheckSymbol(gCtlMem);
                    }//end if                    
                }//end if

                Bitmap bmpCtlMem = new Bitmap(this.Width, this.Height);
                gCtlMem.DrawImage(bmpCtlMem, 0, 0);
                gCtlMem.Dispose();

                pevent.Graphics.DrawImage(bmpCtlMem, 0, 0);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnPaint function of DotCoolCheckBase control.");
            }
            finally
            {
                if (hDC != IntPtr.Zero)
                    pevent.Graphics.ReleaseHdc();
            }
        }

        #endregion

        #region Check Symbol and Check Image Symbol Drawing/Painting/GDI+ Functions, Event Handlers

        /// <summary>
        /// Draws the appropriate gradient check symbol in the box portion of the check control when the control is checked, according to the current
        /// state of the control.
        /// </summary>
        protected virtual void DrawCheckSymbol(Graphics gCtl)
        {
            Bitmap bmpGradCheckMem = null;
            Graphics gGradCheckMem = null;
            GraphicsState origGraphState = null;

            try
            {
                // At the beginning of your drawing
                origGraphState = gCtl.Save();

                bmpGradCheckMem = new Bitmap(CheckSize.Width, CheckSize.Height);
                gGradCheckMem = Graphics.FromImage(bmpGradCheckMem);

                CoolGradientType CheckGradTypeActive = GetCheckGradientType();
                Color CheckGradColor1Active = GetCheckGradientColor(1);
                Color CheckGradColor2Active = GetCheckGradientColor(2);
                float fGradientSpan = GetCheckGradientSpan();
                Point ptGradOffset = GetCheckGradientOffset();

                Color CheckBorderColorActive = GetCheckBorderColor();
                int iBorderWidth = CheckBorderWidth;

                Rectangle rectBounds = new Rectangle(Convert.ToInt32(iBorderWidth / 2), Convert.ToInt32(iBorderWidth / 2),
                                                                CheckSize.Width - iBorderWidth - 1,
                                                                CheckSize.Height - iBorderWidth - 1);

                Point ptCheckLocation = new Point(Convert.ToInt32(m_BoxBounds.Left) + GetBorderWidth() + CheckOffset.X,
                                                                   Convert.ToInt32(m_BoxBounds.Top) + GetBorderWidth() + CheckOffset.Y);

                if (CheckGradColor1Active != Color.Transparent && CheckGradTypeActive != CoolGradientType.None)
                    CoolDraw.DrawGradientShape(CheckShape, CheckGradTypeActive, gGradCheckMem, CheckGradColor1Active,
                                                                CheckGradColor2Active, rectBounds, CheckBorderColorActive, iBorderWidth,
                                                                fGradientSpan, ptGradOffset.X, ptGradOffset.Y,
                                                                CheckRadius.X, CheckRadius.Y);
                else
                {
                    Color? FillColor = null;
                    if (CheckGradColor1Active != Color.Transparent)
                        FillColor = CheckGradColor1Active;

                    CoolDraw.DrawShape(CheckShape, gGradCheckMem, rectBounds, CheckBorderColorActive, iBorderWidth,
                                                    CheckRadius.X, CheckRadius.Y, FillColor);
                }//end if

                Rectangle rectDraw = new Rectangle(ptCheckLocation.X, ptCheckLocation.Y, CheckSize.Width, CheckSize.Height);

                gCtl.CompositingMode = CompositingMode.SourceOver;
                gCtl.DrawImage(bmpGradCheckMem, rectDraw);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawCheckSymbol function of DotCoolCheckBase control.");
            }
            finally
            {
                if (origGraphState != null)
                    gCtl.Restore(origGraphState);

                if (gGradCheckMem != null)
                    gGradCheckMem.Dispose();

                if (bmpGradCheckMem != null)
                    bmpGradCheckMem.Dispose();
            }
        }

        /// <summary>
        /// Draws the check symbol image for the control in the box portion of the check control.  The image will be drawn and rendered onto the 
        /// control based on the various check symbol image settings that are set in the control.  
        /// NOTE: When an image check symbol is used, it will override the gradient check symbol settings and be displayed instead of the gradient 
        /// check symbol.
        /// </summary>
        /// <param name="gCtl"></param>
        protected override void DrawControlImage(Graphics gCtl)
        {
            GraphicsState origGraphState = null;

            try
            {
                if (m_CheckState == CheckState.Unchecked)
                    return;

                origGraphState = gCtl.Save();
                gCtl.CompositingMode = CompositingMode.SourceOver;

                Image imgActive = GetImage();

                if (imgActive != null)
                {
                    Point ptCheckLocation = new Point(Convert.ToInt32(m_BoxBounds.Left) + GetBorderWidth() + GetImageOffset().X,
                                                                   Convert.ToInt32(m_BoxBounds.Top) + GetBorderWidth() + GetImageOffset().Y);

                    CoolDraw.DrawImage(GetImage(), gCtl, new Rectangle(0, 0, this.Width, this.Height), GetImageAlign(), ptCheckLocation,
                                                    true, GetImageTransColor());
                }//end if
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawControlImage function of DotCoolCheckBase class.");
            }
            finally
            {
                if (origGraphState != null)
                    gCtl.Restore(origGraphState);
            }
        }

        #endregion

        #region Control Mouse Interaction Functions, Event Handlers                

        #endregion
        
        #region Box Border (All States) Size and Position Properties, Functions

        /// <summary>
        /// Gets or sets the size of the box portion of the control.
        /// </summary>
        [Browsable(true), Category("CoolBorder"),
         Description("Gets or sets the size of the box portion of the control.")]
        public virtual Size BorderSize
        {
            get
            {
                return GetBorderSize();
            }
            set
            {
                SetBorderSize(value);
            }
        }

        /// <summary>
        /// Gets or sets the X and Y offset position of the box portion of the control.
        /// </summary>
        [Browsable(true), Category("CoolBorder"),
         Description("Gets or sets the X and Y offset position of the box portion of the control.")]
        public virtual Point BorderOffset
        {
            get
            {
                return GetBorderOffset();
            }
            set
            {
                SetBorderOffset(value);
            }
        }

        #endregion

        #region Box Border Width (All States) Properties, Functions

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
        /// Gets or sets the width (thickness) of the control's border for each associated control state.  PROPERTY IS READ ONLY.. Only one border 
        /// width setting will be used for check controls.
        /// </summary>
        [Browsable(false), Category("CoolBorder"), TypeConverter(typeof(ExpandableObjectConverter)),
         Description("Gets or sets the width (thickness) of the control's border for each associated control state.")]
        public new DotCoolCtlBorderWidthSettings BorderWidthSettings
        {
            get
            {
                return m_BorderWidthSettings;
            }
        }

        #endregion

        #region General Check Symbol Border Properties, Functions

        /// <summary>
        /// Gets the appropriate border color of the check symbol based on the control's current state.
        /// </summary>
        /// <returns></returns>
        /// <param name="delStateCheckFunc">Delegate to a function containing additional logic to add to the function.  The function will be usually 
        /// passed from children classes to the base DotCoolControl class to embed additional criteria to a visual setting state checking function.</param>
        protected virtual Color GetCheckBorderColor(Func<Color?> delStateCheckFunc)
        {
            try
            {
                if (!this.Enabled && GetCheckBorderColorSettings().BorderColorDisabled != Color.Transparent)
                {
                    return GetCheckBorderColorSettings().BorderColorDisabled;
                }
                if (m_blMouseOver && GetCheckBorderColorSettings().BorderColorMouseOver != Color.Transparent)
                {
                    return GetCheckBorderColorSettings().BorderColorMouseOver;
                }
                else if (delStateCheckFunc != null)
                {
                    Color? colorResult = delStateCheckFunc.Invoke();

                    if (colorResult != null)
                        return colorResult.Value;
                    else
                        return GetCheckBorderColorSettings().BorderColor;
                }
                else
                {
                    return GetCheckBorderColorSettings().BorderColor;
                }//end if
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetCheckBorderColor function of DotCoolCheckBase class.");
                return Color.Black;
            }
        }

        /// <summary>
        /// Gets the appropriate border color of the check symbol based on the control's current state.
        /// </summary>
        /// <returns></returns>
        protected virtual Color GetCheckBorderColor()
        {
            return GetCheckBorderColor(null);
        }

        #endregion

        #region Check Symbol Border (All States) Appearance Properties, Functions

        /// <summary>
        /// Gets or sets the shape of the check symbol.
        /// </summary>
        [Browsable(true), Category("CoolSymbol"),
         Description("Gets or sets the shape of the check symbol.")]
        public virtual CoolShape CheckShape
        {
            get
            {
                return m_CheckBorderSettingsList[VisualSettingEnum.Normal].BorderShape;
            }
            set
            {
                m_CheckBorderSettingsList[VisualSettingEnum.Normal].BorderShape = value;
                this.Refresh();
            }
        }

        #endregion

        #region Check Symbol Color (State Specific) Appearance Properties, Functions

        /// <summary>
        /// Gets or sets the color of the gradient check symbol's border for each associated control state.  The derived check controls of the DotCoolCheckBase 
        /// class will access this function to implement a BorderColorSettings property that can be accessible to the user.
        /// </summary>
        protected DotCoolCheckBaseBorderColorSettings GetCheckBorderColorSettings()
        {
            try
            {
                return m_CheckBorderColorSettings;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetCheckBorderColorSettings function of DotCoolCheckBase class.");
                return null;
            }
        }        

        #endregion

        #region Check Symbol (All States) Size and Position Properties, Functions

        /// <summary>
        /// Gets or sets the size of the check symbol displayed in the box portion of the control.
        /// </summary>
        [Browsable(true), Category("CoolSymbol"),
         Description("Gets or sets the size of the check symbol displayed in the box portion of the control.")]
        public virtual Size CheckSize
        {
            get
            {
                return m_CheckBorderSettingsList[VisualSettingEnum.Normal].BorderSize;
            }
            set
            {
                m_CheckBorderSettingsList[VisualSettingEnum.Normal].BorderSize = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the radius of the check symbol border's corners when drawing shapes that have curved corners or edges.
        /// </summary>
        [Browsable(true), Category("CoolSymbol"),
         Description("Gets or sets the radius of the check symbol border's corners when drawing shapes that have curved corners or edges.")]
        public virtual Point CheckRadius
        {
            get
            {
                return m_CheckBorderSettingsList[VisualSettingEnum.Normal].BorderRadius;
            }
            set
            {
                m_CheckBorderSettingsList[VisualSettingEnum.Normal].BorderRadius = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the x and y offset position of where the check symbol is displayed in relative to the position of the top-left corner of the box
        /// portion of the control.
        /// </summary>
        [Browsable(true), Category("CoolSymbol"),
         Description("Gets or sets the x and y offset position of where the check symbol is displayed in relative to the position of the top-left corner of " +
                          "the box portion of the control.")]
        public Point CheckOffset
        {
            get
            {
                return m_CheckBorderSettingsList[VisualSettingEnum.Normal].BorderOffset;
            }
            set
            {
                m_CheckBorderSettingsList[VisualSettingEnum.Normal].BorderOffset = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the width (thickness) of the check symol's border.
        /// </summary>
        [Browsable(true), Category("CoolSymbol"),
         Description("Gets or sets the width (thickness) of the check symbol's border.")]
        public virtual int CheckBorderWidth
        {
            get
            {
                return m_CheckBorderSettingsList[VisualSettingEnum.Normal].BorderWidth;
            }
            set
            {
                m_CheckBorderSettingsList[VisualSettingEnum.Normal].BorderWidth = value;
                this.Refresh();
            }
        }

        #endregion        

        #region General Check Symbol Color/Gradient Properties, Functions 

        /// <summary>
        /// Gets the appropriate gradient start or ending color of the check symbol based on the control's current state.
        /// </summary>
        /// <param name="iColorIndex"></param>
        /// <param name="delStateCheckFunc">Delegate to a function containing additional logic to add to the function.  The function will be usually 
        /// passed from children classes to the base DotCoolControl class to embed additional criteria to a visual setting state checking function.</param>
        /// <returns></returns>        
        protected virtual Color GetCheckGradientColor(int iColorIndex, Func<Color?> delStateCheckFunc)
        {
            try
            {
                if (!this.Enabled && CheckGradientSettingsDisabled.GradientColor1 != Color.Transparent)
                {
                    if (iColorIndex == 1)
                        return CheckGradientSettingsDisabled.GradientColor1;
                    else
                        return CheckGradientSettingsDisabled.GradientColor2;
                }
                else if (m_blMouseOver && CheckGradientSettingsMouseOver.GradientColor1 != Color.Transparent)
                {
                    if (iColorIndex == 1)
                        return CheckGradientSettingsMouseOver.GradientColor1;
                    else
                        return CheckGradientSettingsMouseOver.GradientColor2;
                }
                else if (delStateCheckFunc != null)
                {
                    Color? colorResult = delStateCheckFunc.Invoke();

                    if (colorResult != null)
                        return colorResult.Value;
                    else
                    {
                        if (iColorIndex == 1)
                            return CheckGradientSettings.GradientColor1;
                        else
                            return CheckGradientSettings.GradientColor2;
                    }//end if
                }
                else
                {
                    if (iColorIndex == 1)
                        return CheckGradientSettings.GradientColor1;
                    else
                        return CheckGradientSettings.GradientColor2;
                }//end if
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetCheckGradientColor function of DotCoolCheckBase control.");
                return Color.Black;
            }
        }

        /// <summary>
        /// Gets the appropriate gradient start or ending color of the check symbol based on the control's current state.
        /// </summary>
        /// <param name="iColorIndex"></param>
        /// <returns></returns>
        protected virtual Color GetCheckGradientColor(int iColorIndex)
        {
            return GetCheckGradientColor(iColorIndex, null);
        }

        /// <summary>
        /// Gets the appropriate gradient type of the check symbol based on the control's current state.
        /// </summary>
        /// <returns></returns>
        /// <param name="delStateCheckFunc">Delegate to a function containing additional logic to add to the function.  The function will be usually 
        /// passed from children classes to the base DotCoolControl class to embed additional criteria to a visual setting state checking function.</param>
        /// <returns></returns>        
        protected virtual CoolGradientType GetCheckGradientType(Func<CoolGradientType?> delStateCheckFunc)
        {
            try
            {
                if (!this.Enabled && CheckGradientSettingsDisabled.GradientColor1 != Color.Transparent)
                    return CheckGradientSettingsDisabled.GradientType;
                else if (m_blMouseOver && CheckGradientSettingsMouseOver.GradientColor1 != Color.Transparent)
                    return CheckGradientSettingsMouseOver.GradientType;
                else if (delStateCheckFunc != null)
                {
                    CoolGradientType? gradTypeResult = delStateCheckFunc.Invoke();

                    if (gradTypeResult != null)
                        return gradTypeResult.Value;
                    else
                    {
                        return CheckGradientSettings.GradientType;
                    }//end if
                }
                else
                    return CheckGradientSettings.GradientType;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetCheckGradientType function of DotCoolCheckBase control.");
                return CoolGradientType.None;
            }
        }

        /// <summary>
        /// Gets the appropriate gradient type of the check symbol based on the control's current state.
        /// </summary>
        /// <returns></returns>
        protected virtual CoolGradientType GetCheckGradientType()
        {
            return GetCheckGradientType(null);
        }

        /// <summary>
        /// Gets the appropriate gradient span value of the check symbol based on the control's current state.
        /// </summary>
        /// <returns></returns>
        /// <param name="delStateCheckFunc">Delegate to a function containing additional logic to add to the function.  The function will be usually 
        /// passed from children classes to the base DotCoolControl class to embed additional criteria to a visual setting state checking function.</param>
        /// <returns></returns>        
        protected virtual float GetCheckGradientSpan(Func<float> delStateCheckFunc)
        {
            try
            {
                if (!this.Enabled && CheckGradientSettings.GradientColor1 != Color.Transparent)
                {
                    if (CheckGradientSettingsDisabled.UseDefaultGradientSpan)
                        return CoolGradient.GetDefaultGradientSpan(CheckGradientSettingsDisabled.GradientType);
                    else
                        return CheckGradientSettingsDisabled.GradientSpan;
                }
                else if (m_blMouseOver && CheckGradientSettingsMouseOver.GradientColor1 != Color.Transparent)
                {
                    if (CheckGradientSettingsMouseOver.UseDefaultGradientSpan)
                        return CoolGradient.GetDefaultGradientSpan(CheckGradientSettingsMouseOver.GradientType);
                    else
                        return CheckGradientSettingsMouseOver.GradientSpan;
                }
                else if (delStateCheckFunc != null)
                {
                    float fResult = delStateCheckFunc.Invoke();

                    if (fResult != -1)
                        return fResult;                    
                }
                
                if (CheckGradientSettings.UseDefaultGradientSpan)
                    return CoolGradient.GetDefaultGradientSpan(CheckGradientSettings.GradientType);
                else
                    return CheckGradientSettings.GradientSpan;                
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetCheckGradientSpan function of DotCoolCheckBase control.");
                return 0f;
            }
        }

        /// <summary>
        /// Gets the appropriate gradient span value of the check symbol based on the control's current state.
        /// </summary>
        /// <returns></returns>
        protected virtual float GetCheckGradientSpan()
        {
            return GetCheckGradientSpan(null);
        }

        /// <summary>
        /// Gets the appropriate background gradient X and Y offset position of the check symbol based on the control's current state.
        /// </summary>
        /// <param name="delStateCheckFunc">Delegate to a function containing additional logic to add to the function.  The function will be usually 
        /// passed from children classes to the base DotCoolControl class to embed additional criteria to a visual setting state checking function.</param>
        /// <returns></returns>        
        protected virtual Point GetCheckGradientOffset(Func<Point?> delStateCheckFunc)
        {
            try
            {
                if (!this.Enabled && CheckGradientSettingsDisabled.GradientColor1 != Color.Transparent)
                    return CheckGradientSettingsDisabled.GradientOffset;
                else if (m_blMouseOver && CheckGradientSettingsMouseOver.GradientColor1 != Color.Transparent)
                    return CheckGradientSettingsMouseOver.GradientOffset;
                else if (delStateCheckFunc != null)
                {
                    Point? ptResult = delStateCheckFunc.Invoke();

                    if (ptResult != null)
                        return ptResult.Value;
                    else
                        return CheckGradientSettings.GradientOffset;
                }
                else
                    return CheckGradientSettings.GradientOffset;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetCheckGradientOffset function of DotCoolCheckBase control.");
                return new Point(0, 0);
            }
        }

        /// <summary>
        /// Gets the appropriate background gradient X and Y offset position of the check symbol based on the control's current state.
        /// </summary>
        /// <returns></returns>
        protected virtual Point GetCheckGradientOffset()
        {
            return GetCheckGradientOffset(null);
        }

        #endregion

        #region Check Symbol Color/Gradient Properties, Functions        

        /// <summary>
        /// Gets the set of gradient settings of the check symbol portion of the control for the control state specified in the function's setting parameter.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public DotCoolCtlGradientSettings GetCheckGradientSettings(VisualSettingEnum setting)
        {
            try
            {
                return m_GradCheckSettingsList[setting];
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetCheckGradientSettings function of DotCoolControl class.");
                return null;
            }
        }

        /// <summary>
        /// Gradient settings of the check symbol portion of the control when the control is in its normal state.
        /// </summary>
        [Browsable(true), Category("CoolSymbol"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Gradient settings of the check symbol portion of the control when the control is in its normal state.")]
        public DotCoolCtlGradientSettings CheckGradientSettings
        {
            get
            {

                return GetCheckGradientSettings(VisualSettingEnum.Normal);
            }
        }

        /// <summary>
        /// Gradient settings of the check symbol portion of the control when the control is in its disabled state.
        /// </summary>
        [Browsable(true), Category("CoolSymbol"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Gradient settings of the check symbol portion of the control when the control is in its disabled state.")]
        public DotCoolCtlGradientSettings CheckGradientSettingsDisabled
        {
            get
            {

                return GetCheckGradientSettings(VisualSettingEnum.Disabled);
            }
        }

        /// <summary>
        /// Gradient settings of the check symbol portion of the control when the mouse cursor is over the control.
        /// </summary>
        [Browsable(true), Category("CoolSymbol"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
          Description("Gradient settings of the check symbol portion of the control when the mouse cursor is over the control.")]
        public DotCoolCtlGradientSettings CheckGradientSettingsMouseOver
        {
            get
            {

                return GetCheckGradientSettings(VisualSettingEnum.MouseOver);
            }
        }

        #endregion
    
        #region Check Setting/Value/Behavior Properties, Functions        

        /// <summary>
        /// Gets/Sets a value indicating if the check control is checked or unchecked.
        /// </summary>
        [Browsable(true), Category("CoolCheck"),
         Description("Gets/Sets a value indicating if the check control is checked or unchecked.")]
        public virtual bool Checked
        {
            get
            {
                return (m_CheckState == CheckState.Checked);
            }
            set
            {
                if (value)
                    m_CheckState = CheckState.Checked;
                else
                    m_CheckState = CheckState.Unchecked;

                OnCheckChanged(new EventArgs());

                this.Refresh();
            }
        }

        /// <summary>
        /// Indicates if the check control is selected by clicking specifically on the box portion of the control or any part of the control.
        /// </summary>
        [Browsable(true), Category("Behavior"), DefaultValue(ControlSelectBehavior.Control),
         Description("Indicates if the check control is selected by clicking specifically on the box portion of the control or any part of the control.")]
        public virtual ControlSelectBehavior SelectBehavior
        {
            get
            {
                return GetSelectBehavior();
            }
            set
            {
                SetSelectBehavior(value);
            }
        }

        #endregion

        #region Check State Functions, Event Handlers

        /// <summary>
        /// Raises the CheckChanged event of the DotCoolCheckBase derived control.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCheckChanged(EventArgs e)
        {
            try
            {
                if (m_CancelEvents.CheckBoxCheckStateChanged)
                    return;

                if (this.CheckChanged != null)
                    CheckChanged(this, e);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnCheckChanged function of DotCoolCheckBase class.");
            }
        }

        #endregion

        #region Control Cloning and Serialization Properties, Functions

        /// <summary>
        /// Copies all of the properties and settings that are specific to the base DotCoolCheckBase class from one derived DotCoolCheckBase control to
        /// another derived DotCoolCheckBase control.
        /// </summary>
        /// <param name="chbaseClone">The DotCoolCheckBase dervied control that will be cloned from the current DotCoolCheckBase control.</param>
        protected virtual void CloneBase(DotCoolCheckBase chbaseClone)
        {
            try
            {
                base.CloneBase(chbaseClone);
                
                //Border Width
                chbaseClone.BorderWidth = BorderWidth;

                //Check Symbol Shape
                chbaseClone.CheckShape = CheckShape;

                //Check Symbol Border Color Settings
                chbaseClone.GetCheckBorderColorSettings().BorderColor = GetCheckBorderColorSettings().BorderColor;
                chbaseClone.GetCheckBorderColorSettings().BorderColorDisabled = GetCheckBorderColorSettings().BorderColorDisabled;
                chbaseClone.GetCheckBorderColorSettings().BorderColorMouseDown = GetCheckBorderColorSettings().BorderColorMouseDown;
                chbaseClone.GetCheckBorderColorSettings().BorderColorMouseOver = GetCheckBorderColorSettings().BorderColorMouseOver;

                //Check Symbol Size and Position
                chbaseClone.CheckSize = CheckSize;
                chbaseClone.CheckRadius = CheckRadius;
                chbaseClone.CheckOffset = CheckOffset;
                chbaseClone.CheckBorderWidth = CheckBorderWidth;

                //Check Symbol Color/Gradient Settings
                //Background Gradient Settings
                for (VisualSettingEnum setting = VisualSettingEnum.Normal; setting <= VisualSettingEnum.Indeterminate; setting++)
                {
                    DotCoolCtlGradientSettings gradChkSettingsClone = chbaseClone.GetCheckGradientSettings(setting);
                    DotCoolCtlGradientSettings gradChkSettings = GetCheckGradientSettings(setting);

                    gradChkSettingsClone.GradientColor1 = gradChkSettings.GradientColor1;
                    gradChkSettingsClone.GradientColor2 = gradChkSettings.GradientColor2;
                    gradChkSettingsClone.GradientOffset = gradChkSettings.GradientOffset;
                    gradChkSettingsClone.GradientSpan = gradChkSettings.GradientSpan;
                    gradChkSettingsClone.GradientType = gradChkSettings.GradientType;
                    gradChkSettingsClone.UseDefaultGradientSpan = gradChkSettings.UseDefaultGradientSpan;
                }//next setting           

                //Check Setting/Value/Behavior Settings
                chbaseClone.SelectBehavior = SelectBehavior;                
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in CloneBase function of DotCoolCheckBase class.");                
            }
        }

        #endregion

        #region DotCoolCheckBase Specific Control Settings Classes

        /// <summary>
        /// Contains the control border visual setting propeties for a specific control state associated with a check control.  Each control border visual 
        /// setting property set will be contained in a DotCoolCtlBorderSettingsList class where it can be accessed through its associated control state.
        /// </summary>
        public class DotCoolCheckBaseBorderColorSettings : DotCoolCtlBorderColorSettings
        {
            #region Construction/Initialization

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="BorderSettingsList"></param>
            public DotCoolCheckBaseBorderColorSettings(DotCoolCtlBorderSettingsList BorderSettingsList)
                : base(BorderSettingsList)
            {
                try
                {
                }
                catch (Exception err)
                {
                    ErrorHandler.ShowErrorMessage(err, "Error in Constructor function of DotCoolCheckBaseBorderColorSettings class.");
                }
            }

            #endregion

            #region State Specific Border Color Properties, Functions        

            /// <summary>
            /// Gets or sets the color of the control's border when the mouse button is pushed down on the control.  Control state NOT USED for check controls.
            /// </summary>
            [Browsable(false), Category("CoolBorder")]
            public new Color BorderColorMouseDown
            {
                get
                {
                    return Color.Transparent;
                }
                set
                {
                    base.BorderColorMouseDown = Color.Transparent;
                }
            }

            #endregion
        }

        #endregion
    }
}

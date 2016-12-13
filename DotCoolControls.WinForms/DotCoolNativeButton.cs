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

namespace DotCoolControls.WinForms
{
    /// <summary>
    /// An expansion of the native .NET button control that displays stunning visual effects.  Visual effects, include,  but are not limited to various types
    /// of customizable gradients and text.  Both the effects and functionality of the button can be made to be state-specific, such as producing various 
    /// effects for when the button is in a Normal or Disabled state, as well various types of mouse interactions, such as MouseDown and MouseOver events.
    /// </summary>
    public class DotCoolNativeButton : Button
    {
        #region Member Variables        

        #endregion

        #region Member Object Variables

        private Dictionary<string, object> m_dicSetCellTransOrigVals = new Dictionary<string, object>();

        #endregion

        #region Member Data Object Variables
        #endregion

        #region General Background Gradient Variables

        //private bool m_blDrawBackGrad = false;

        #endregion

        #region Background Gradient Variables

        /// <summary>
        /// Contains the various gradient settings for the background of the DotCoolNativeButton class.
        /// </summary>
        protected DotCoolCtlGradientSettingsList m_GradBackSettingsList = null;


        #endregion

        #region Caption Variables        

        /// <summary>
        /// Contains the various text and font settings for the caption displayed in the DotCoolNativeButton class.
        /// </summary>
        protected DotCoolCtlTextSettingsList m_TextSettingsList = null;

        #endregion        

        #region Mouse Position Variables

        private bool m_blMouseOver = false;

        private bool m_blMouseDown = false;

        #endregion

        #region Construction/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public DotCoolNativeButton()
            : base()
        {
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.DoubleBuffered = true;

            m_GradBackSettingsList = new DotCoolCtlGradientSettingsList(this, false);
            m_TextSettingsList = new DotCoolCtlTextSettingsList(this, false);

            m_GradBackSettingsList.SettingChanged += GradBackSettingsList_SettingChanged;
            m_TextSettingsList.SettingChanged += TextSettingsList_SettingChanged;
        }

        #endregion

        #region Background Gradient Drawing/Paint/GDI+ Functions, Event Handlers

        /// <summary>
        /// Draws the gradient background image of the DotCoolNativeButton according to the background gradient settings that are set for the control.
        /// NOTE: The DotCoolNativeButton will use the button's background image, rather than the paint event handlers to render the background gradient 
        /// image so that the button's native functionality can be maintained. 
        /// </summary>
        protected virtual void DrawGradientBackgroundImage()
        {
            Bitmap bmpGradBackMem = null;
            Graphics gGradBackMem = null;
            Graphics gGradBack = null;

            try
            {
                bmpGradBackMem = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
                gGradBackMem = Graphics.FromImage(bmpGradBackMem);

                CoolGradientType BackGradTypeActive = GetBackGradientType();
                Color BackGradColor1Active = GetBackGradientColor(1);
                Color BackGradColor2Active = GetBackGradientColor(2);
                float fGradSpan = GetBackGradientSpan();
                Point ptGradOffset = GetBackGradientOffset();

                if (BackGradTypeActive == CoolGradientType.None || BackGradColor1Active == Color.Transparent)
                {
                    ClearGradientBackgroundImage();
                    return;
                }
                else
                    CoolGradient.DrawGradient(BackGradTypeActive, gGradBackMem, BackGradColor1Active, BackGradColor2Active, ClientRectangle,
                                                            fGradSpan, ptGradOffset.X, ptGradOffset.Y);

                this.BackgroundImage = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
                gGradBack = Graphics.FromImage(this.BackgroundImage);
                gGradBack.DrawImage(bmpGradBackMem, ClientRectangle);

                this.Refresh();
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in DrawGradientBackgroundImage function of DotCoolNativeButton control.");
            }
            finally
            {
                if (gGradBackMem != null)
                    gGradBackMem.Dispose();

                if (gGradBack != null)
                    gGradBack.Dispose();

                if (bmpGradBackMem != null)
                    bmpGradBackMem.Dispose();
            }
        }

        /// <summary>
        /// Clears the current gradient background image set in the DotCoolNativeButton. 
        /// </summary>
        protected virtual void ClearGradientBackgroundImage()
        {
            try
            {
                this.BackgroundImage = null;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in ClearGradientBackgroundImage function of DotCoolNativeButton control.");
            }
        }

        #endregion

        #region Button Enabling/Visibility/Focus Properties, Functions

        /// <summary>
        /// Gets or sets a value indicating whether the control can respond to user interaction.  
        /// </summary>
        public new bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;

                if (DrawBackgroundGradient)
                    DrawGradientBackgroundImage();

                UpdateButtonText();
            }
        }
    
        #endregion

        #region General Background Color/Gradient Properties, Functions

        /// <summary>
        /// Indicates if the background will be drawn with a gradient, using the background gradient settings set in the control.
        /// </summary>
        [Browsable(true), DefaultValue(false), Category("CoolBackground"),
        Description("Indicates if the background will be drawn with a gradient, using the background gradient settings set in the control.")]
        public bool DrawBackgroundGradient
        {
            get
            {
                return m_GradBackSettingsList.GetVisualSetting(VisualSettingEnum.Normal).DrawGradient;
            }
            set
            {
                m_GradBackSettingsList.GetVisualSetting(VisualSettingEnum.Normal).DrawGradient = value;

                if (value)
                {
                    BackgroundImageLayout = ImageLayout.Stretch;
                    DrawGradientBackgroundImage();
                }
                else
                    ClearGradientBackgroundImage();
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
                ErrorHandler.ShowErrorMessage(err, "Error in GetBackGradientColor function of DotCoolNativeButton control.");
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
                ErrorHandler.ShowErrorMessage(err, "Error in GetBackGradientType function of DotCoolNativeButton control.");
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
                ErrorHandler.ShowErrorMessage(err, "Error in GetBackGradientSpan function of DotCoolNativeButton control.");
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
                ErrorHandler.ShowErrorMessage(err, "Error in GetBackGradientOffset function of DotCoolNativeButton control.");
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
        public DotCoolCtlGradientSettings GetBackGradientSettings(VisualSettingEnum setting)
        {
            try
            {
                return m_GradBackSettingsList[setting];
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetBackGradientSettings function of DotCoolNativeButton class.");
                return null;
            }
        }

        /// <summary>
        /// Background gradient settings of the DotCoolNativeButton when the control is in its normal state.
        /// </summary>
        [Browsable(true), Category("CoolBackground"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Background gradient settings of the DotCoolNativeButton when the control is in its normal state.")]
        public DotCoolCtlGradientSettings BackGradientSettings
        {
            get
            {

                return GetBackGradientSettings(VisualSettingEnum.Normal);
            }
        }

        /// <summary>
        /// Background gradient settings of the DotCoolNativeButton when the control is in its disabled state.
        /// </summary>
        [Browsable(true), Category("CoolBackground"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Background gradient settings of the DotCoolNativeButton when the control is in its disabled state.")]
        public DotCoolCtlGradientSettings BackGradientSettingsDisabled
        {
            get
            {

                return GetBackGradientSettings(VisualSettingEnum.Disabled);
            }
        }

        /// <summary>
        /// Background gradient settings of the DotCoolNativeButton when the mouse button is pushed down on the control.
        /// </summary>
        [Browsable(true), Category("CoolBackground"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Background gradient settings of the DotCoolNativeButton when the mouse button is pushed down on the control.")]
        public DotCoolCtlGradientSettings BackGradientSettingsMouseDown
        {
            get
            {

                return GetBackGradientSettings(VisualSettingEnum.MouseDown);
            }
        }

        /// <summary>
        /// Background gradient settings of the DotCoolNativeButton when the mouse cursor is over the control.
        /// </summary>
        [Browsable(true), Category("CoolBackground"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Background gradient settings of the DotCoolNativeButton when the mouse cursor is over the control.")]
        public DotCoolCtlGradientSettings BackGradientSettingsMouseOver
        {
            get
            {

                return GetBackGradientSettings(VisualSettingEnum.MouseOver);
            }
        }

        #endregion      

        #region Button Image/Layout Properties, Functions

        /// <summary>
        ///  Gets or sets the background image layout as defined in the System.Windows.Forms.ImageLayout
        ///   enumeration.                 
        /// </summary>           
        [Browsable(true), DefaultValue(ImageLayout.Stretch), Localizable(true), Category("Appearance"),
         Description("Gets or sets the background image layout as defined in the System.Windows.Forms.ImageLayout enumeration.")]
        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }

            set
            {
                if (DrawBackgroundGradient)
                    base.BackgroundImageLayout = ImageLayout.Stretch;
                else
                    base.BackgroundImageLayout = value;
            }
        }

        #endregion

        #region General Button Caption Properties, Functions

        /// <summary>
        /// Gets or sets the font of the text displayed in the button.
        /// </summary>
        [Browsable(true), Category("CoolText"),
         Description("Gets or sets the font of the text displayed in the button.")]
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                if (!(m_blMouseOver && TextSettingsMouseOver.EnableText) && !(m_blMouseDown && TextSettingsMouseDown.EnableText))
                {
                    base.Font = value;
                    m_TextSettingsList[VisualSettingEnum.Normal].Font = value;
                }//end if
            }
        }        
    
        /// <summary>
        ///  Gets or sets the foreground color of the button.
        /// </summary>
        [Browsable(true), Category("CoolText"),
         Description("Gets or sets the foreground color of the button.")]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                if (!(m_blMouseOver && TextSettingsMouseOver.EnableText) && !(m_blMouseDown && TextSettingsMouseDown.EnableText))
                {
                    base.ForeColor = value;
                    GetTextSettings(VisualSettingEnum.Normal).ForeColor = value;
                }//end if                                               
            }
        }       

        /// <summary>
        /// Updates the font, color and other related settings of the caption of the DotCoolNativeButton with respect to the current state of the 
        /// button.  The font, color and text display settings will depend on whether the button is in a normal state or if the mouse is 
        /// being moved over the button or the button is currently pushed down.
        /// </summary>
        private void UpdateButtonText()
        {
            try
            {         
                if(!this.Enabled && TextSettingsDisabled.EnableText)
                {
                    base.Font = m_TextSettingsList[VisualSettingEnum.Disabled].Font;
                    base.ForeColor = m_TextSettingsList[VisualSettingEnum.Disabled].ForeColor;
                }       
                else if (m_blMouseDown && TextSettingsMouseDown.EnableText)
                {

                    base.Font = m_TextSettingsList[VisualSettingEnum.MouseDown].Font;
                    base.ForeColor = m_TextSettingsList[VisualSettingEnum.MouseDown].ForeColor;
                }
                else if (m_blMouseOver && TextSettingsMouseOver.EnableText)
                {
                    base.Font = m_TextSettingsList[VisualSettingEnum.MouseOver].Font;
                    base.ForeColor = m_TextSettingsList[VisualSettingEnum.MouseOver].ForeColor;
                }                
                else
                {
                    base.Font = m_TextSettingsList[VisualSettingEnum.Normal].Font;
                    base.ForeColor = m_TextSettingsList[VisualSettingEnum.Normal].ForeColor;
                }//end if

                this.Refresh();
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in UpdateButtonText function of DotCoolNativeButton control.");
            }
        }

        #endregion

        #region Button Caption Properties, Functions

        /// <summary>
        /// Gets the set of control text/caption settings for the control state specified in the function's setting parameter.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public DotCoolCtlTextSettings GetTextSettings(VisualSettingEnum setting)
        {
            try
            {
                return m_TextSettingsList[setting];
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetTextSettings function of DotCoolNativeButton class.");
                return null;
            }
        }

        /// <summary>
        /// Text/Caption display settings of the DotCoolNativeButton when the control is in its normal state.
        /// </summary>
        [Browsable(true), Category("CoolText"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Text/Caption display settings of the DotCoolNativeButton when the control is in its normal state.")]
        public DotCoolCtlTextSettings TextSettings
        {
            get
            {

                return GetTextSettings(VisualSettingEnum.Normal);
            }
        }

        /// <summary>
        /// Text/Caption display settings of the DotCoolNativeButton when the control is in its disabled state.
        /// </summary>
        [Browsable(true), Category("CoolText"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Text/Caption display settings of the DotCoolNativeButton when the control is in its disabled state.")]
        public DotCoolCtlTextSettings TextSettingsDisabled
        {
            get
            {

                return GetTextSettings(VisualSettingEnum.Disabled);
            }
        }

        /// <summary>
        /// Text/Caption display settings of the DotCoolNativeButton when the mouse button is pushed down on the control.
        /// </summary>
        [Browsable(true), Category("CoolBackground"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Text/Caption display settings of the DotCoolNativeButton when the mouse button is pushed down on the control.")]
        public DotCoolCtlTextSettings TextSettingsMouseDown
        {
            get
            {

                return GetTextSettings(VisualSettingEnum.MouseDown);
            }
        }

        /// <summary>
        /// Text/Caption display settings of the DotCoolNativeButton when the mouse cursor is over the control.
        /// </summary>
        [Browsable(true), Category("CoolText"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Text/Caption display settings of the DotCoolNativeButton when the mouse cursor is over the control.")]
        public DotCoolCtlTextSettings TextSettingsMouseOver
        {
            get
            {

                return GetTextSettings(VisualSettingEnum.MouseOver);
            }
        }

        #endregion                

        #region Mouse Position and Mouse Press Related and Overridden Functions, Event Handlers

        /// <summary>
        /// Overrides the OnMouseEnter event to handle the drawing of gradients and the button text when the mouse cursor moves 
        /// over the button.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            try
            {
                base.OnMouseEnter(e);
                
                m_blMouseOver = true;

                if(DrawBackgroundGradient)
                    DrawGradientBackgroundImage();

                if (TextSettingsMouseOver.EnableText)
                    UpdateButtonText();
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnMouseEnter function of DotCoolNativeButton control.");
            }
        }

        /// <summary>
        /// Overrides the OnMouseLeave event to handle the drawing of gradients and the button text when the mouse cursor moves 
        /// outside the region of the button.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            try
            {                
                base.OnMouseLeave(e);
                
                m_blMouseOver = false;

                if (DrawBackgroundGradient)
                    DrawGradientBackgroundImage();
                
                if(TextSettingsMouseOver.EnableText)
                    UpdateButtonText();
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnMouseLeave function of DotCoolNativeButton control.");
            }
        }

        /// <summary>
        /// Overrides the OnMouseDown event to handle the drawing of gradients and the button text when the mouse's left button is 
        /// pushed down on the button.
        /// </summary>
        /// <param name="mevent"></param>
        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            try
            {
                base.OnMouseDown(mevent);

                if (mevent.Button == MouseButtons.Left)
                {
                    m_blMouseDown = true;

                    if (DrawBackgroundGradient)
                        DrawGradientBackgroundImage();

                    if (TextSettingsMouseDown.EnableText)
                        UpdateButtonText();
                }//end if
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnMouseDown function of DotCoolNativeButton control.");
            }
        }

        /// <summary>
        /// Overrides the OnMouseUp event to handle the drawing of gradients and the button text when the mouse's left button is 
        /// pushed up on the button.
        /// </summary>
        /// <param name="mevent"></param>
        protected override void OnMouseUp(MouseEventArgs mevent)
        {            
            try
            {
                base.OnMouseUp(mevent);

                if (mevent.Button == MouseButtons.Left)
                {
                    m_blMouseDown = false;

                    if (DrawBackgroundGradient)
                        DrawGradientBackgroundImage();

                    if (TextSettingsMouseDown.EnableText)
                        UpdateButtonText();
                }//end if
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnMouseUp function of DotCoolNativeButton control.");
            }
        }

        #endregion

        #region Control Setting Change Event Handlers

        /// <summary>
        /// When any property contained in the background gradient settings list is changed, the SettingChanged event will be raised to allow the 
        /// DotCoolNativeButton control to update the control's UI.  It is necessary to update the UI through the SettingChanged event since the 
        /// DotCoolNativeButton control will manipulate specific text related properties of the .NET button control to integrate the DotCool visual 
        /// text settings in the control.
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        private void GradBackSettingsList_SettingChanged(VisualSettingEnum arg1, string arg2, object arg3)
        {
            try
            {
                if (DrawBackgroundGradient)
                    DrawGradientBackgroundImage();
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GradBackSettingsList_SettingChanged function of DotCoolNativeButton control.");
            }
        }

        /// <summary>
        /// When any property contained in the text settings list is changed, the SettingChanged event will be raised to allow the 
        /// DotCoolNativeButton control to update the control's UI.  It is necessary to update the UI through the SettingChanged event since the 
        /// DotCoolNativeButton control is not owner-drawn and uses the derived .NET button control's background image property to draw its 
        /// gradient background.
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        private void TextSettingsList_SettingChanged(VisualSettingEnum arg1, string arg2, object arg3)
        {
            try
            {
                UpdateButtonText();
                this.Refresh();
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in TextSettingsList_SettingChanged function of DotCoolNativeButton control.");
            }
        }        

        #endregion
    }
}

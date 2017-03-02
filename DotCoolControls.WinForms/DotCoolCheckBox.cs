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
    /// The DotCoolCheckBox is a checkbox control that supports check boxes and check symbols of various shapes, as well as displays stunning visual
    /// effects.  Visual effects, include, but are not limited to various types customizable gradients, borders, images and text.  In addition, custom images 
    /// can be displayed for the check symbol.  Both the effects and functionality of the checkbox can be made to be state-specific, such as producing
    /// various effects for when the button is in a Normal or Disabled state, as well various types of mouse interactions, such as MouseOver events.    
    /// </summary>    
    public class DotCoolCheckBox : DotCoolCheckBase
    {
        #region Events

        /// <summary>
        /// The event that is raised when the check state property of the DotCoolCheckBox control is changed.
        /// </summary>
        public event EventHandler CheckStateChanged;

        #endregion

        #region Member Variables

        #endregion

        #region Member Object Variables
        #endregion

        #region Construction/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public DotCoolCheckBox()
            : base()
        {
            try
            {
                m_BorderColorSettings = new DotCoolCheckBoxBorderColorSettings(m_BorderSettingsList);

                VisualSettingProperties<BorderVisualSettings> BorderVisSettings =
                                                VisualSettingPropGenerator.CreateBorderVisualSettings(
                                                                                                        CoolShape.Diamond, Color.Transparent, new Size(10, 10),
                                                                                                        new Point(10, 10), new Point(2, 2), 1);

                m_CheckBorderSettingsList = new DotCoolCtlBorderSettingsList(this, true, BorderVisSettings);                
                m_CheckBorderColorSettings = new DotCoolCheckBoxBorderColorSettings(m_CheckBorderSettingsList);

                for (VisualSettingEnum setting = VisualSettingEnum.Normal; setting <= VisualSettingEnum.Indeterminate; setting++)
                {
                    m_BorderSettingsList[setting].BorderShape = CoolShape.Square;
                }//next iSetting

                m_CheckBorderSettingsList[VisualSettingEnum.Normal].BorderColor = Color.Black;                
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Constructor function of DotCoolCheckBox class.");
            }
        }

        #endregion

        #region Border Color (State Specific) Appearance Properties, Functions

        /// <summary>
        /// Gets or sets the color of the control's border for each associated control state.
        /// </summary>
        [Browsable(true), Category("CoolBorder"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Gets or sets the color of the control's border for each associated control state.")]
        public new DotCoolCheckBoxBorderColorSettings BorderColorSettings
        {
            get
            {
                return (DotCoolCheckBoxBorderColorSettings)m_BorderColorSettings;
            }
        }

        #endregion

        #region General Image Properties, Functions

        /// <summary>
        /// Gets the appropriate check symbol image that will be displayed in the box portion of the control based on the control's current state.
        /// </summary>
        /// <returns></returns>     
        protected override Image GetImage()
        {
            try
            {
                return base.GetImage(() =>
                                {
                                    if (m_CheckState == CheckState.Indeterminate && ImageSettingsIndeterminate.EnableImage && ImageSettingsIndeterminate.Image != null)
                                        return ImageSettingsIndeterminate.Image;
                                    else
                                        return null;
                                });
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetImage function of DotCoolCheckBox control.");
                return null;
            }
        }

        /// <summary>
        /// Gets the appropriate image alignment of the check symbol image displayed in the control based on the control's current state.
        /// </summary>
        /// <returns></returns>
        protected override ContentAlignment GetImageAlign()
        {
            try
            {
                return base.GetImageAlign(() =>
                {
                    if (m_CheckState == CheckState.Indeterminate && ImageSettingsIndeterminate.EnableImage && ImageSettingsIndeterminate.Image != null)
                        return ImageSettingsIndeterminate.ImageAlign;
                    else
                        return null;
                });
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetImageAlign function of DotCoolCheckBox control.");
                return ContentAlignment.TopLeft;
            }
        }

        /// <summary>
        /// Gets the appropriate X and Y offset positions of the check symbol image displayed in the control based on the control's current state.
        /// </summary>
        /// <returns></returns>
        protected override Point GetImageOffset()
        {
            try
            {
                return base.GetImageOffset(() =>
                {
                    if (m_CheckState == CheckState.Indeterminate && ImageSettingsIndeterminate.EnableImage && ImageSettingsIndeterminate.Image != null)
                        return ImageSettingsIndeterminate.ImageOffset;
                    else
                        return null;
                });
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetImageOffset function of DotCoolCheckBox control.");
                return new Point(0, 0);
            }
        }

        /// <summary>
        /// Gets the appropriate transparent color of the check symbol image displayed in the control based on the control's current state.
        /// </summary>
        /// <returns></returns>
        protected override Color GetImageTransColor()
        {
            try
            {
                return base.GetImageTransColor(() =>
                {
                    if (m_CheckState == CheckState.Indeterminate && ImageSettingsIndeterminate.EnableImage && ImageSettingsIndeterminate.Image != null)
                        return ImageSettingsIndeterminate.ImageTransColor;
                    else
                        return null;
                });
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetImageTransColor function of DotCoolCheckBox control.");
                return Color.Transparent;
            }
        }

        #endregion

        #region General Check Symbol Border Properties, Functions

        /// <summary>
        /// Gets the appropriate border color of the check symbol based on the control's current state.
        /// </summary>
        /// <returns></returns>
        protected override Color GetCheckBorderColor()
        {
            try
            {
                return base.GetCheckBorderColor(() =>
                {
                    if (m_CheckState == CheckState.Indeterminate && ImageSettingsIndeterminate.EnableImage && ImageSettingsIndeterminate.Image != null)
                        return CheckBorderColorSettings.BorderColorIndeterminate;
                    else
                        return null;
                });
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetCheckBorderColor function of DotCoolCheckBox class.");
                return Color.Black;
            }
        }

        #endregion

        #region Check Symbol Color (State Specific) Appearance Properties, Functions        

        /// <summary>
        /// Gets or sets the color of the gradient check symbol's border for each associated control state.
        /// </summary>
        [Browsable(true), Category("CoolBorder"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
         Description("Gets or sets the color of the gradient check symbol's border for each associated control state.")]
        public virtual DotCoolCheckBoxBorderColorSettings CheckBorderColorSettings
        {
            get
            {
                return (DotCoolCheckBoxBorderColorSettings)m_CheckBorderColorSettings;                
            }            
        }

        #endregion

        #region General Check Symbol Color/Gradient Properties, Functions     

        /// <summary>
        /// Gets the appropriate gradient start or ending color of the gradient check symbol based on the control's current state.
        /// </summary>
        /// <param name="iColorIndex">1 = Gradient Starting Color, 2 = Gradient Ending Color</param>
        /// <returns></returns>
        protected override Color GetCheckGradientColor(int iColorIndex)
        {
            try
            {
                return base.GetCheckGradientColor(iColorIndex, () =>
                    {
                        if (m_CheckState == CheckState.Indeterminate && CheckGradientSettingsIndeterminate.GradientColor1 != Color.Transparent)
                        {
                            if (iColorIndex == 1)
                                return CheckGradientSettingsIndeterminate.GradientColor1;
                            else
                                return CheckGradientSettingsIndeterminate.GradientColor2;
                        }
                        else
                            return null;
                    });
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetCheckGradientColor function of DotCoolControl control.");
                return Color.Black;
            }
        }

        /// <summary>
        /// Gets the appropriate gradient type of the gradient check symbol based on the control's current state.
        /// </summary>
        /// <returns></returns>
        protected override CoolGradientType GetCheckGradientType()
        {
            try
            {
                return base.GetCheckGradientType(() =>
                {
                    if (m_CheckState == CheckState.Indeterminate && CheckGradientSettingsIndeterminate.GradientColor1 != Color.Transparent)
                    {
                        return CheckGradientSettingsIndeterminate.GradientType;
                    }
                    else
                        return null;
                });
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetCheckGradientType function of DotCoolCheckBox control.");
                return CoolGradientType.None;
            }
        }

        /// <summary>
        /// Gets the appropriate gradient span value of the gradient check symbol based on the control's current state.
        /// </summary>
        /// <returns></returns>
        protected override float GetCheckGradientSpan()
        {
            try
            {
                return base.GetCheckGradientSpan(() =>
                {
                    if (m_CheckState == CheckState.Indeterminate && CheckGradientSettingsIndeterminate.GradientColor1 != Color.Transparent)
                    {
                        return CheckGradientSettingsIndeterminate.GradientSpan;
                    }
                    else
                        return -1;
                });
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetCheckGradientSpan function of DotCoolCheckBox control.");
                return 0f;
            }
        }

        /// <summary>
        /// Gets the appropriate gradient X and Y offset position of the gradient check symbol based on the control's current state.
        /// </summary>
        /// <returns></returns>
        protected override Point GetCheckGradientOffset()
        {
            try
            {
                return base.GetCheckGradientOffset(() =>
                {
                    if (m_CheckState == CheckState.Indeterminate && CheckGradientSettingsIndeterminate.GradientColor1 != Color.Transparent)
                    {
                        return CheckGradientSettingsIndeterminate.GradientOffset;
                    }
                    else
                        return null;
                });
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetCheckGradientOffset function of DotCoolCheckBox control.");
                return new Point(0, 0);
            }
        }

        #endregion

        #region Indeterminate Check Symbol Color/Gradient Properties, Functions

        /// <summary>
        /// Gradient settings of the check symbol portion of the control when the control is in an indeterminate state.
        /// </summary>
        [Browsable(true), Category("CoolSymbol"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Gradient settings of the check symbol portion of the control when the control is in an indeterminate state.")]
        public DotCoolCtlGradientSettings CheckGradientSettingsIndeterminate
        {
            get
            {

                return GetCheckGradientSettings(VisualSettingEnum.Indeterminate);
            }
        }

        #endregion

        #region Indeterminate Check Image Properties, Functions

        /// <summary>
        /// Image display settings of the DotCoolControl when the control is in an indeterminate state.
        /// </summary>
        [Browsable(true), Category("CoolImage"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Image display settings of the DotCoolControl when the control is in an indeterminate state.")]
        public DotCoolCtlImageSettings ImageSettingsIndeterminate
        {
            get
            {

                return GetImageSettings(VisualSettingEnum.Indeterminate);
            }
        }        

        #endregion

        #region Check Setting Properties, Functions        

        /// <summary>
        /// Gets/Sets a value indicating if the check control is checked, unchecked or in an indeterminate state.
        /// </summary>
        [Browsable(true), Category("CoolCheck"),
         Description("Gets/Sets a value indicating if the check control is checked, unchecked or in an indeterminate state.")]
        public virtual CheckState CheckState
        {
            get
            {
                return m_CheckState;
            }
            set
            {
                if (m_CheckState == value)
                    return;

                if (value == CheckState.Checked)
                    m_CheckState = CheckState.Checked;
                else if (value == CheckState.Indeterminate && ThreeState)
                    m_CheckState = CheckState.Indeterminate;
                else
                    m_CheckState = CheckState.Unchecked;

                OnCheckStateChanged(new EventArgs());
                OnCheckChanged(new EventArgs());

                this.Refresh();
            }
        }

        /// <summary>
        /// Gets/Sets a value indicating if the check control is checked or unchecked.
        /// </summary>
        [Browsable(true), Category("CoolCheck"),
         Description("Gets/Sets a value indicating if the check control is checked or unchecked.")]
        public override bool Checked
        {
            get
            {
                return base.Checked;
            }

            set
            {
                base.Checked = value;
                OnCheckStateChanged(new EventArgs());
            }
        }

        /// <summary>
        /// Gets/Sets a value indicating if the DotCoolCheckBox will allow three check states, rather than two.
        /// </summary>
        [Browsable(true), Category("CoolCheck"),
         Description("Gets/Sets a value indicating if the DotCoolCheckBox will allow three check states, rather than two.")]
        public virtual bool ThreeState
        {
            get
            {
                return m_blThreeState;
            }
            set
            {
                m_blThreeState = value;
            }
        }

        #endregion

        #region Check State Functions, Event Handlers

        /// <summary>
        /// Raises the CheckStateChanged event of the DotCoolCheckBox control.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCheckStateChanged(EventArgs e)
        {
            try
            {
                if (m_CancelEvents.CheckBoxCheckStateChanged)
                    return;

                if (this.CheckStateChanged != null)
                    CheckStateChanged(this, e);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnCheckStateChanged function of DotCoolCheckBox class.");
            }
        }

        #endregion

        #region Control Mouse Interaction Functions, Event Handlers               

        /// <summary>
        /// Raises the Click event of the control.  When the control is clicked, the state of the checkbox will be changed according to the current state 
        /// the checkbox was set before it was clicked.  
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClick(EventArgs e)
        {
            try
            {
                bool blChangeCheckStatus = false;

                if (m_blMouseDown)
                    blChangeCheckStatus = true;

                base.OnClick(e);

                if (blChangeCheckStatus)
                {
                    if (CheckState == CheckState.Checked)
                        CheckState = CheckState.Unchecked;
                    else if (CheckState == CheckState.Indeterminate)
                        CheckState = CheckState.Checked;
                    else
                    {
                        if (!ThreeState)
                            CheckState = CheckState.Checked;
                        else
                            CheckState = CheckState.Indeterminate;
                    }//end if           
                }//end if                
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnClick function of DotCoolCheckBox class.", "", true);
            }
        }

        /// <summary>
        /// The DotCoolCheckBox will not raise a DoubleClick event, so a double click on the control will result in the control raising a Click event, instead.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDoubleClick(EventArgs e)
        {
            OnClick(e);                        
        }

        #endregion

        #region Control Cloning and Serialization Properties, Functions

        /// <summary>
        /// Clones all properties and settings of the DotCoolCheckBox control to a new DotCoolCheckBox control.
        /// </summary>
        /// <returns></returns>
        public DotCoolCheckBox Clone()
        {
            try
            {
                DotCoolCheckBox chbClone = new DotCoolCheckBox();
                base.CloneBase(chbClone);

                chbClone.ThreeState = ThreeState;

                return chbClone;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Clone function of DotCoolCheckBox class.");
                return null;
            }
        }

        #endregion
    }

    #region DotCoolCheckBox Specific Control Settings Classes    

    /// <summary>
    /// Contains the control border color setting propeties for a specific control state associated with a DotCoolCheckBox.  The control border color 
    /// setting property set wil allow user's to access various border color settings from the property designer in an organized manner.  Each 
    /// border color property setting will be linked to a border visual setting contained in a DotCoolCtlBorderSetingsList class.
    /// </summary>
    public class DotCoolCheckBoxBorderColorSettings : DotCoolCheckBase.DotCoolCheckBaseBorderColorSettings 
    {
        #region Construction/Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="BorderSettingsList"></param>
        public DotCoolCheckBoxBorderColorSettings(DotCoolCtlBorderSettingsList BorderSettingsList)
            : base(BorderSettingsList)
        {
            try
            {
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Constructor function of DotCoolCheckBoxBorderColorSettings class.");
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

        /// <summary>
        /// Gets or sets the color of the control's border when the control is in an indeterminate state.
        /// </summary>
        [Browsable(true), Category("CoolBorder"),
         Description("Gets or sets the color of the control's border when the control is in an indeterminate state.")]
        public virtual Color BorderColorIndeterminate
        {
            get
            {
                return m_BorderSettingsList[VisualSettingEnum.Indeterminate].BorderColor;
            }
            set
            {
                m_BorderSettingsList[VisualSettingEnum.Indeterminate].BorderColor = value;
            }
        }

        #endregion
    }
    
    #endregion
}
